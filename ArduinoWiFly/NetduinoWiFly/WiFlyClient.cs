using System;

namespace NetduinoWiFly
{
	public class WiFlyClient
	{
		#region Fields
		// TODO: Unfuck the WiFlyServer class so these can be made private!
		internal WiFlyDevice _wiFly;
		internal byte[] _ip;
		internal ushort _port;
		internal string _hostname;

		internal bool _open;

		private ParsedStream _stream;
		#endregion
		#region Properties
		public int AvailableBytes
		{
			get
			{
				if (!_open)
					return 0;

				return _stream.Available();
			}
		}
		public bool Connected
		{
			get { return _open && !_stream.Closed; }
		}

		public bool Valid { get { return (_ip != null || _hostname != null) && _port != 0; } }
		#endregion

		#region Constructors
		public WiFlyClient(byte[] ip, ushort port)
			: this(ip, null, port) {}

		public WiFlyClient(string domain, ushort port)
			: this(null, domain, port) {}

		private WiFlyClient(byte[] ip, string hostname, ushort port)
		{
			_ip = ip;
			_port = port;
			_hostname = hostname;

			_open = false;

			SpiUartDevice uart = new SpiUartDevice();
			_wiFly = new WiFlyDevice(uart);
			_stream = new ParsedStream(uart);
		}
		#endregion

		#region Methods
		public bool Connect()
		{
			if (!Valid)
				throw new InvalidOperationException("Must have either an IP or a hostname and a valid port.");

			_stream.Reset();

			_wiFly.EnterCommandMode();
			_wiFly.SendCommand("open ", true, String.Empty);

			if (_ip != null)
			{
				string ip = String.Empty;
				foreach (byte b in _ip)
				{
					ip += b;
					ip += ".";
				}
				ip.Trim('.');
				_wiFly.SendCommand(ip, true);
			}
			else if (_hostname != null)
			{
				_wiFly.SendCommand(_hostname, true);
			}
			else
				throw new InvalidOperationException("Must have either and IP or a hostname.");

			_wiFly.SendCommand(" ", true);
			_wiFly.SendCommand(_port.ToString(), true);
			_wiFly.SendCommand(String.Empty, false, "*OPEN*");


			_open = true;
			return true;
		}

		public void Write(byte value)
		{
			_wiFly._uart.Write(value);
		}

		public void Write(string value)
		{
			_wiFly._uart.Write(value);
		}

		public void Write(byte[] buffer)
		{
			_wiFly._uart.Write(buffer);
		}

		public int Read()
		{
			if (!_open)
				return -1;

			return _stream.Read();
		}

		public void Flush()
		{
			if (!_open)
				return;

			while (_stream.Available() > 0)
				_stream.Read();
		}

		public void Stop()
		{
			_wiFly.EnterCommandMode();
			_wiFly._uart.WriteLine("close");

			_wiFly._uart.WriteLine("exit");
			_wiFly.WaitForResponse("EXIT");
			_wiFly.SkipRemainderOfResponse();

			_stream.Reset();

			_open = false;

			_wiFly._serverConnectionActive = false;
		}
		#endregion
	}
}
