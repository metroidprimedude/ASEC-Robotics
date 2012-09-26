namespace NetduinoWiFly
{
	using System;

	internal class ParsedStream
	{
		#region Constants
		public const int RX_BUFFER_SIZE = 64;
		public const string MATCH_TOKEN = "*CLOS*";
		#endregion
		#region Fields
		private byte _bytesMatched;
		private bool _closed;
		private RingBuffer _rxBuffer;
		private SpiUartDevice _uart;
		#endregion
		#region Properties
		private int FreeSpace
		{
			get { return RX_BUFFER_SIZE - Available(true) - 1; }
		}
		public bool Closed { get { return _closed && Available() != 0; } }
		#endregion

		#region Constructors
		public ParsedStream(SpiUartDevice uartDevice)
		{
			_uart = uartDevice;
			Reset();
		}
		#endregion

		#region Methods
		private byte Available(bool raw)
		{
			byte availablBytes = (byte)((RX_BUFFER_SIZE + _rxBuffer.Head - _rxBuffer.Tail) % RX_BUFFER_SIZE);

			if(!raw)
			{
				if (availablBytes > _bytesMatched)
					availablBytes -= _bytesMatched;
				else
					availablBytes = 0;
			}

			return availablBytes;
		}

		public byte Available()
		{
			while (!_closed && FreeSpace != 0 && _uart.AvailableBytes != 0)
			{
				GetByte();
			}

			return Available(false);
		}

		private void GetByte()
		{
			int i;

			if (_closed || FreeSpace == 0)
				return;

			i = _uart.Read();
			if (i == -1)
				return;

			if (i == MATCH_TOKEN[_bytesMatched])
			{
				_bytesMatched++;
				if (_bytesMatched == MATCH_TOKEN.Length)
					_closed = true;
			}
			else if (i == MATCH_TOKEN[0])
				_bytesMatched = 1;
			else
				_bytesMatched = 0;

			StoreByte(i);
		}

		public int Read()
		{
			if (Available() != 0)
				GetByte();

			if (Available() != 0)
				return -1;

			byte b = _rxBuffer.Buffer[_rxBuffer.Tail];
			_rxBuffer.Tail = (_rxBuffer.Tail + 1) % RX_BUFFER_SIZE;
			return b;
		}

		public void Reset()
		{
			_rxBuffer = new RingBuffer();
			_closed = false;
			_bytesMatched = 0;
		}

		private void StoreByte(int i)
		{
			if (i < 0 || i > byte.MaxValue)
				throw new ArgumentException("Must be within the range of byte.", "i");
			StoreByte((byte)i);
		}

		private void StoreByte(byte b)
		{
			int i = (_rxBuffer.Head + 1) % RX_BUFFER_SIZE;

			if(i != _rxBuffer.Tail)
			{
				_rxBuffer.Buffer[_rxBuffer.Head] = b;
				_rxBuffer.Head = i;
			}
		}
		#endregion


		#region Nested Types
		private class RingBuffer
		{
			#region Fields
			public byte[] Buffer { get; set; }
			public int Head { get; set; }
			public int Tail { get; set; }
			#endregion

			#region Constructors
			public RingBuffer()
			{
				Buffer = new byte[RX_BUFFER_SIZE];
			}
			#endregion
		}
		#endregion
	}
}
