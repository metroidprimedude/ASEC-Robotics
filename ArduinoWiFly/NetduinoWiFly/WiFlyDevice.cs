using System;

namespace NetduinoWiFly
{
	using System.Threading;

	public class WiFlyDevice
	{
		#region Static fields
		private static WiFlyDevice _WiFly = new WiFlyDevice(new SpiUartDevice());
		#endregion
		#region Static Properties
		public static WiFlyDevice WiFly { get { return _WiFly; } };
		#endregion

		#region Constants
		private const int COMMAND_MODE_ENTER_RETRY_ATTEMPTS = 5;
		private const int COMMAND_MODE_GUARD_TIME = 250;
		private const int DEFAULT_SERVER_PORT = 42000;
		private const int SOFTWARE_REBOOT_RETRY_ATTEMPTS = 5;
		private const bool USE_HARDWARE_REBOOT = true;
		private const int IP_ADDRESS_BUFFER_SIZE = 16; // "255.255.255.255\0"
		#endregion
		#region Fields
		// TODO: Unfuck the Client and server classes so these can be made private again!
		internal SpiUartDevice _uart;
		internal ushort _serverPort;
		internal bool _serverConnectionActive;
		#endregion
		#region Properties
		public string IP
		{
			get
			{
				char[] ip = new char[IP_ADDRESS_BUFFER_SIZE];

				EnterCommandMode();

				SendCommand("get ip", false, "IP=");

				int nextChar;
				byte offset = 0;
				while (offset < IP_ADDRESS_BUFFER_SIZE)
				{
					nextChar = _uart.Read();
					if(nextChar < 0)
						continue;
					if (nextChar == ':')
					{
						ip[offset] = '\0';
						break;
					}

					ip[offset] = (char)nextChar;
					offset++;
				}

				ip[IP_ADDRESS_BUFFER_SIZE - 1] = '\0';

				WaitForResponse("<");
				while (_uart.Read() == ' ')
					continue; // Skip the prompt

				_uart.WriteLine("exit");

				return new string(ip);
			}
		}
		#endregion

		#region Constructors
		internal WiFlyDevice(SpiUartDevice uart)
		{
			_uart = uart;

			_serverPort = DEFAULT_SERVER_PORT;
			_serverConnectionActive = false;
		}
		#endregion

		#region Implementation Methods
		//internal bool TrySwitchToCommandMode()
		//{
			
		//}

		//internal void SwitchToCommandMode()
		//{
			
		//}

		internal void Reboot()
		{
			bool result;
			if (USE_HARDWARE_REBOOT)
				result = HardwareReboot();
			else
				result = SoftwareReboot();

			if (!result)
				throw new InvalidOperationException("Failed to Reboot."); // TODO: Use a more appropriate exception type
		}

		internal void RequireFlowControl()
		{
			EnterCommandMode();
			SendCommand("get uart", false, "Flow=0x");

			while (_uart.AvailableBytes == 0)
				continue; // Wait for response

			byte flowControlState = (byte)_uart.Read();

			_uart.Flush();

			if (flowControlState == '1')
				return;

			SendCommand("set uart flow 1");
			SendCommand("save", false, "Storing in config");
			SendCommand("get uart", false, "Flow=0x1");

			Reboot();
		}

		internal void SetConfiguration()
		{
			EnterCommandMode();

			SendCommand("set wlan join 0");
			SendCommand("set ip localport ", true);
			_uart.Write(_serverPort);
			SendCommand(String.Empty);

			SendCommand("set comm remote 0");
		}

		internal bool SendCommand(string command, bool isMultipart = false, string expectedResponse = null)
		{
			_uart.Write(command);

			if(!isMultipart)
			{
				_uart.Flush();
				_uart.WriteLine();

				WaitForResponse(expectedResponse);
			}

			return true;
		}

		internal void WaitForResponse(string expectedResponse)
		{
			if (expectedResponse == null || expectedResponse == String.Empty)
				return;

			while (!ResponsMatched(expectedResponse))
				continue;
		}

		internal void SkipRemainderOfResponse()
		{
			// This doesn't make sense... should it be || instead of &&?
			while (_uart.AvailableBytes == 0 && (_uart.Read() == '\n'))
				continue;
		}

		internal bool ResponsMatched(string toMatch)
		{
			bool matchFound = true;

			for (int i = 0; i < toMatch.Length; i++)
			{
				while (_uart.AvailableBytes == 0)
				{
					continue;
				}
				if(_uart.Read() != toMatch[i])
				{
					matchFound = false;
					break;
				}
			}

			return matchFound;
		}

		internal bool FindInRespons(string toMatch, uint timeout)
		{
			int byteRead;
			long timeOutTarget;

			for (int i = 0; i < toMatch.Length; i++)
			{
				timeOutTarget = DateTime.Now.Ticks + timeout * 10;
				while (_uart.AvailableBytes == 0)
				{
					if (timeout > 0 && DateTime.Now.Ticks > timeOutTarget)
						return false;
				}

				byteRead = _uart.Read();

				if(byteRead != toMatch[i])
				{
					i = 0;
					if (byteRead != toMatch[i])
						i = -1;
				}
			}

			return true;
		}

		internal bool EnterCommandMode(bool afterReboot = false)
		{
			for (int retryCount = 0; retryCount < COMMAND_MODE_ENTER_RETRY_ATTEMPTS; retryCount++)
			{
				if (afterReboot)
					Thread.Sleep(1000);

				Thread.Sleep(COMMAND_MODE_GUARD_TIME);
				_uart.Write("$$$");
				Thread.Sleep(COMMAND_MODE_GUARD_TIME);

				_uart.Write("\r\n\r\n");
				_uart.Write("ver");

				if (FindInRespons("\r\nWiFly Ver", 1000))
					return true;
			}
			return false;
		}

		internal bool SoftwareReboot(bool afterReboot = true)
		{
			for(int retryCount = 0; retryCount < SOFTWARE_REBOOT_RETRY_ATTEMPTS; retryCount++)
			{
				if (!EnterCommandMode(afterReboot))
					break;

				_uart.WriteLine("reboot");

				if(FindInRespons("*READY*", 2000))
					return true;
			}

			return false;
		}

		internal bool HardwareReboot()
		{
			_uart.SetIODirection(0x02);
			_uart.SetIOState(0x00);
			Thread.Sleep(1);
			_uart.SetIOState(0x02);

			return FindInRespons("*READY*", 2000);
		}
		#endregion
		#region Methods
		public void Beging()
		{
			_uart.Begin();
			Reboot(); // To ensure that the device is in a known state
			RequireFlowControl();
			SetConfiguration();
		}

		public bool Join(string ssid)
		{
			SendCommand("join ", true);
			if(SendCommand(ssid, false, "Associated!"))
			{
				WaitForResponse("Listen on ");
				SkipRemainderOfResponse();
				return true;
			}

			return false;
		}

		public bool Join(string ssid, string passphrase, bool wpa)
		{
			SendCommand("set wlan ", true);

			if (wpa)
				SendCommand("passphrase ", true);
			else
				SendCommand("key ", true);

			SendCommand(passphrase);

			return Join(ssid);
		}
		#endregion
	}
}
