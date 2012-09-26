using System;
using Microsoft.SPOT;

namespace NetduinoWiFly
{
	using System.Threading;

	public class WiFlyDevice
	{
		#region Constants
		private const int COMMAND_MODE_ENTER_RETRY_ATTEMPTS = 5;
		private const int COMMAND_MODE_GUARD_TIME = 250;
		#endregion
		#region Fields
		private SpiUartDevice _uart;
		private ushort _serverPort;
		#endregion
		#region Properties
		public string IP
		{
			get
			{
				
			}
		}
		#endregion

		#region Constructors
		public WiFlyDevice(SpiUartDevice uart)
		{
			
		}
		#endregion

		#region Implementation Methods
		internal bool TrySwitchToCommandMode()
		{
			
		}

		internal void SwitchToCommandMode()
		{
			
		}

		internal void Reboot()
		{

		}

		internal void RequireFlowControl()
		{
			
		}

		internal void SetConfiguration()
		{
			
		}

		internal bool SendCommand(string command, bool isMultipart, string expectedRespons)
		{
			
		}

		internal void WaitForResponse(string expectedRespons)
		{
			
		}

		internal void SkipRemainderOfRespons()
		{
			
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

		internal bool EnterCommandMode() { return EnterCommandMode(false); }
		internal bool EnterCommandMode(bool afterReboot)
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

		internal bool SoftwareReboot(bool afterReboot)
		{
			
		}

		internal bool HardwareReboot()
		{
			
		}
		#endregion
		#region Methods
		public void Beging()
		{
			
		}

		public bool Join(string ssid)
		{
			
		}

		public bool Join(string ssid, string passphrase)
		{
			
		}


		#endregion
	}
}
