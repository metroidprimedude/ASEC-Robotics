using System;
using System.Threading;
using System.Text;
using System.Net.Sockets;
using System.Net;
using Microsoft.SPOT.Hardware;
using Microsoft.SPOT;
using System.IO.Ports;

using SecretLabs.NETMF.Hardware.NetduinoPlus;

using Asec.Robotics;

namespace Asec.Robotics.Components
{
	public class WiFlyShield
	{
		private enum Registers
		{
			THR = 0x00 << 3,
			RHR = 0x00 << 3,
			IER = 0x01 << 3,
			FCR = 0x02 << 3,
			IIR = 0x02 << 3,
			LCR = 0x03 << 3,
			MCR = 0x04 << 3,
			LSR = 0x05 << 3,
			MSR = 0x06 << 3,
			SPR = 0x07 << 3,
			DLL = 0x00 << 3,
			DLM = 0x01 << 3,
			EFR = 0x02 << 3
		}

		private SPI m_uart;
		private SPI.SPI_module m_spiModule = SPI.SPI_module.SPI1;
		private Cpu.Pin m_chipSelect = Pins.GPIO_PIN_D10;
		private SerialPort m_serialPort;

		public WiFlyShield(SPI bus)
		{
			// Block start: SPIBus class like I2CBus class!!! Require an initialized bus.
			if (bus == null)
				m_uart = new SPI(new SPI.Configuration(m_chipSelect, false, 10, 10, false, true, 2000, m_spiModule));
			else
				m_uart.Config = new SPI.Configuration(m_chipSelect, false, 10, 10, false, true, 2000, m_spiModule);
			// Block end.

			// Block start: Initialize the SPI<->UART chip.
			WriteRegister(WiFlyShield.Registers.LCR, 0x80); // 0x80 to program baudrate
			WriteRegister(WiFlyShield.Registers.DLL, 48);   // 4800=191, 9600=96, 19200=48, 38400=24
			WriteRegister(WiFlyShield.Registers.DLM, 0);
			WriteRegister(WiFlyShield.Registers.LCR, 0xbf); // access EFR register
			WriteRegister(WiFlyShield.Registers.EFR, 0xd0); // enable enhanced registers and enable RTC/CTS on the SPI UART
			WriteRegister(WiFlyShield.Registers.LCR, 3);    // 8 data bit, 1 stop bit, no parity
			WriteRegister(WiFlyShield.Registers.FCR, 0x06); // reset TXFIFO, reset RXFIFO, non FIFO mode
			WriteRegister(WiFlyShield.Registers.FCR, 0x01); // enable FIFO mode
			WriteRegister(WiFlyShield.Registers.SPR, 0x55);
			if (ReadRegister(WiFlyShield.Registers.SPR) != 0x55)
				throw new Exception("Failed to init SPI<->UART chip");
			// Block end.

			// Block start: Initialize the WiFi settings and connect to network.
			Thread.Sleep(200); // ??? copied from HoneyPot thing.
			enterCommandMode(); // Enter comand mode.
			SendCommand("set wlan auth 0");              // Is default (open network). 4 would be WPA2.
			//SendCommand("set wlan phrase PASS_PHRASE");  // If using auth above, this should be used with WPA2 to set password.
			SendCommand("set wlan rate 0");              // Set the wlan rate to 1Mb/s becuase this is UART max and will increase range.
			//SendCommand("set wlan ssid SSID_NAME");      // Set the ssid name to whatever we are using. Move to bottom of wlan settings if connect command is later.
			SendCommand("set ip dhcp 1");                // Enable DHCP mode. 3 turns on cacheing.
			SendCommand("set ip localport 42000");       // Set the local port, 42000 for now.
			SendCommand("set opt password PASS_STRING"); // Clients connecting must send PASS_STRING or WiFly closes connection. Up to 32chars.

			SendCommand("join SSID"); // Self explanitory...

			leaveCommandMode();
			// End block.

			// Start block: Listen for RobotControlProtocol init data.
			while ((ReadRegister(WiFlyShield.Registers.LSR) & 0x01) > 0)
			{
				ReadRegister(WiFlyShield.Registers.RHR);
			}
			// End block.
		}

		private void WriteRegister(WiFlyShield.Registers register, byte data)
		{
			m_uart.Write(new byte[] { (byte)register, data });
		}
		private byte ReadRegister(WiFlyShield.Registers register)
		{
			byte[] buffer = new byte[] { (byte)((byte)register | 0x80), 0 };
			m_uart.WriteRead(buffer, buffer);
			return buffer[1];
		}
		protected void enterCommandMode()
		{
			Thread.Sleep(250);
			WriteArray(Encoding.UTF8.GetBytes("$$$"));
			Thread.Sleep(250);
		}
		protected void leaveCommandMode()
		{
			WriteArray(Encoding.UTF8.GetBytes("exit\r"));
		}
		private void WriteArray(byte[] bytes)
		{
			for (int i = 0; i < bytes.Length; i++)
				WriteRegister(WiFlyShield.Registers.THR, bytes[i]);
		}
		private void SendCommand(string command)
		{
			WriteArray(Encoding.UTF8.GetBytes(command + '\r'));
		}



		public String SendCommand(String command, String key1, String key2, int nLines, int delay)
		{
			StringBuilder buffer = new StringBuilder();
			String result = "";
			Boolean countLines = nLines > 0;

			WriteArray(Encoding.UTF8.GetBytes(command + '\r'));
			while (--delay > 0)
			{
				while ((ReadRegister(WiFlyShield.Registers.LSR) & 0x01) > 0)
				{
					char c = (char)ReadRegister(WiFlyShield.Registers.RHR);
					if (c == '\r')
					{
						String line = buffer.ToString();
						Debug.Print("> " + line);
						if (line.IndexOf(key1) != -1 || line.IndexOf(key2) != -1)
							result = line;
						if (countLines && nLines-- <= 0)
							return result;
						buffer.Clear();
					}
					else if (c != '\r' && c != '\n')
						buffer.Append(c);
				}
				Thread.Sleep(4);
			}
			return result;
		}



	}











	public class HttpWiflyImpl : HttpImplementation
	{
		public enum DeviceType
		{
			crystal_12_288_MHz,
			crystal_14_MHz,
		}

		private enum WiflyRegister
		{
			THR = 0x00 << 3,
			RHR = 0x00 << 3,
			IER = 0x01 << 3,
			FCR = 0x02 << 3,
			IIR = 0x02 << 3,
			LCR = 0x03 << 3,
			MCR = 0x04 << 3,
			LSR = 0x05 << 3,
			MSR = 0x06 << 3,
			SPR = 0x07 << 3,
			DLL = 0x00 << 3,
			DLM = 0x01 << 3,
			EFR = 0x02 << 3,
		}
		public DeviceType m_deviceType { get; set; }
		public int LocalPort { get; set; }
		private SPI m_uart;
		private SPI.SPI_module m_spiModule;
		private Cpu.Pin m_chipSelect;
		private Boolean m_initialized = false;
		private Boolean m_opened = false;
		private SerialPort m_serialPort;
		private HttpImplementationClient.RequestReceivedDelegate m_requestReceived = null;

		public HttpWiflyImpl(HttpImplementationClient.RequestReceivedDelegate requestReceived, int localPort, DeviceType deviceType, SPI.SPI_module spiModule, Cpu.Pin chipSelect)
		{
			m_requestReceived = requestReceived;
			LocalPort = localPort;
			this.m_spiModule = spiModule;
			this.m_chipSelect = chipSelect;
		}

		public void EnableGateway(String port = "COM1", int rate = 38400)
		{
			String hello = "WiFly-GSX ready\n\r";
			m_serialPort = new SerialPort(port, rate, Parity.None, 8, StopBits.One);
			m_serialPort.ReadTimeout = 0;
			m_serialPort.Open();
			m_serialPort.Write(getBytes(hello), 0, hello.Length);
		}

		private void Init()
		{
			if (!m_initialized)
			{
				m_uart = new SPI(new SPI.Configuration(m_chipSelect, false, 10, 10, false, true, 2000, m_spiModule));
				WriteRegister(WiflyRegister.LCR, 0x80); // 0x80 to program baudrate

				if (m_deviceType == DeviceType.crystal_12_288_MHz)
					// value = (12.288*1024*1024) / (baudrate*16)
					WriteRegister(WiflyRegister.DLL, 42);       // 4800=167, 9600=83, 19200=42, 38400=21
				else
					// value = (14*1024*1024) / (baudrate*16)
					WriteRegister(WiflyRegister.DLL, 48);     // 4800=191, 9600=96, 19200=48, 38400=24
				WriteRegister(WiflyRegister.DLM, 0);
				WriteRegister(WiflyRegister.LCR, 0xbf); // access EFR register
				WriteRegister(WiflyRegister.EFR, 0xd0); // enable enhanced registers and enable RTC/CTS on the SPI UART
				WriteRegister(WiflyRegister.LCR, 3);    // 8 data bit, 1 stop bit, no parity
				WriteRegister(WiflyRegister.FCR, 0x06); // reset TXFIFO, reset RXFIFO, non FIFO mode
				WriteRegister(WiflyRegister.FCR, 0x01); // enable FIFO mode
				WriteRegister(WiflyRegister.SPR, 0x55);

				if (ReadRegister(WiflyRegister.SPR) != 0x55)
					throw new Exception("Failed to init SPI<->UART chip");
				m_initialized = true;
			}
		}

		public void Open()
		{
			if (!m_opened)
			{
				Init();
				Thread.Sleep(200);
				enterCommandMode();
				SendCommand("reboot", "Listen on", "ERR:", 0, 500);
				m_opened = true;
			}
		}

		public void Open(String ssid, String phrase)
		{
			if (!m_opened)
			{
				Init();
				Thread.Sleep(200);
				enterCommandMode();
				Send("\r");
				SendCommand("factory RESET");
				SendCommand("set uart baud 19200");
				SendCommand("set wlan ssid " + ssid);
				if (phrase != null && phrase.Length != 0)
					SendCommand("set wlan phrase " + phrase);
				SendCommand("set wlan rate 14");
				SendCommand("set wlan hide 1");
				SendCommand("set ip localport " + LocalPort);
				SendCommand("set uart flow 1");
				SendCommand("set uart mode 0");
				SendCommand("set comm remote 0");
				SendCommand("save", "Storing", "ERR:", 0, 100);
				SendCommand("reboot", "Listen on", "ERR:", 0, 500);
				enterCommandMode();
				m_opened = true;
			}
		}

		private void WriteArray(byte[] ba)
		{
			for (int i = 0; i < ba.Length; i++)
				WriteRegister(WiflyRegister.THR, ba[i]);
		}

		private void WriteRegister(WiflyRegister reg, byte b)
		{
			m_uart.Write(new byte[] { (byte)reg, b });
		}

		private byte ReadRegister(WiflyRegister reg)
		{
			byte[] buffer = new byte[] { (byte)((byte)reg | 0x80), 0 };
			m_uart.WriteRead(buffer, buffer);
			return buffer[1];
		}

		public void Send(string str)
		{
			WriteArray(getBytes(str));
			Debug.Print(str);
		}

		byte[] getBytes(String str)
		{
			return Encoding.UTF8.GetBytes(str);
		}

		/*
		 * Sends the command <command> to the module, with default timeout
		 * 
		 * Returns "AOK" or "ERR:xxx" or else the last line that the command has returned
		 */
		public String SendCommand(String command)
		{
			return SendCommand(command, "AOK", "ERR:", 0, 100);
		}

		/*
		 * Sends the command <command> to the module, with lookup key and with default timeout
		 * 
		 * Returns the first line containing the lookup <key> or "ERR:xxx" or else the last line that the command has returned
		 */
		public String SendCommand(String command, String key)
		{
			return SendCommand(command, key, "ERR:", 0, 100);
		}

		/*
		 * Sends the command <command> to the module, and returns a response starting with <key>; wait nLines to be consumed, or delay to ellapse
		 * 
		 * if <key> is non null and <nLines> is 0 => returns at the first received line that contains <key>
		 * if <key> is non null and <nLines> is not 0 => wait to receive <nLines> lines, and returns at the first received line that contains <key>
		 */
		public String SendCommand(String command, String key1, String key2, int nLines, int delay)
		{
			StringBuilder buffer = new StringBuilder();
			String result = "";
			Boolean countLines = nLines > 0;

			Send(command + '\r');
			while (--delay > 0)
			{
				while ((ReadRegister(WiflyRegister.LSR) & 0x01) > 0)
				{
					char c = (char)ReadRegister(WiflyRegister.RHR);
					if (c == '\r')
					{
						String line = buffer.ToString();
						Debug.Print("> " + line);
						if (line.IndexOf(key1) != -1 || line.IndexOf(key2) != -1)
							result = line;
						if (countLines && nLines-- <= 0)
							return result;
						buffer.Clear();
					}
					else if (c != '\r' && c != '\n')
						buffer.Append(c);
				}
				Thread.Sleep(4);
			}
			return result;
		}

		protected void enterCommandMode()
		{
			// Need to frame a 250ms window around the $$$ sequence (we use 300 below, to stay safe), as stated in the WiFly documentation:
			// "Characters are PASSED until this exact sequence is seen. If any bytes are seen before these chars, or after these chars, in a
			//  250ms window, command mode will not be entered and these bytes will be passed on to other side"
			Thread.Sleep(300);
			Send("$$$");
			Thread.Sleep(300);
		}

		protected void leaveCommandMode()
		{
			Send("exit\r");
		}

		String receiveLine()
		{
			StringBuilder line = new StringBuilder();
			for (int i = 0; i < 200; i++)
			{
				while ((ReadRegister(WiflyRegister.LSR) & 0x01) > 0)
				{
					char c = (char)ReadRegister(WiflyRegister.RHR);
					if (c == '\n')
						return line.ToString();
					line.Append(c);
				}
			}
			return line.ToString();
		}

		public void Write(String response)
		{
			Send(response);
		}
		public void BinaryWrite(byte[] response)
		{
			WriteArray(response);
		}
		public void Close()
		{
			enterCommandMode();
			Send("close\r");
		}

		public String getIP()
		{
			String key = "IP=";
			leaveCommandMode();
			enterCommandMode();
			String result = SendCommand("get ip", key);
			int i = result.IndexOf(key);
			if (i != -1)
				return result.Substring(i + key.Length);
			else
				return null;
		}

		public void Listen()
		{
			StringBuilder line = new StringBuilder();
			Open();
			while (true)
			{
				while ((ReadRegister(WiflyRegister.LSR) & 0x01) > 0)
				{
					char c = (char)ReadRegister(WiflyRegister.RHR);
					line.Append(c);
				}
				String contents = line.ToString();
				if (contents.Length != 0)
				{
					String[] lines = contents.Split('\n');
					HttpRequest request = new HttpRequest();
					new HttpRequestParser().parse(request, new HttpRequestLines(lines));
					line.Clear();
					if (request.RawUrl != null && request.RawUrl.Length != 0 && m_requestReceived != null)
						m_requestReceived(new HttpContext(request, new HttpResponse(this)));
				}
			}
		}
	}
}