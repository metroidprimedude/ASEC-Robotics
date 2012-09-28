namespace NetduinoWiFly
{
	using System;
	using System.Text;
	using System.Threading;
	using Microsoft.SPOT;
	using Microsoft.SPOT.Hardware;
	using SecretLabs.NETMF.Hardware;
	using SecretLabs.NETMF.Hardware.NetduinoPlus;

	internal class SpiUartDevice
	{
		#region Constants
		public const byte THR = 0x00 << 3;
		public const byte RHR = 0x00 << 3;
		public const byte IER = 0x01 << 3;
		public const byte FCR = 0x02 << 3;
		public const byte IIR = 0x02 << 3;
		public const byte LCR = 0x03 << 3;
		public const byte MCR = 0x04 << 3;
		public const byte LSR = 0x05 << 3;
		public const byte MSR = 0x06 << 3;
		public const byte SPR = 0x07 << 3;
		public const byte TXLVL = 0x08 << 3;
		public const byte RXLVL = 0x09 << 3;
		public const short DLAB = 0x80 << 3;
		public const byte IODIR = 0x0A << 3;
		public const byte IOSTATE = 0x0B << 3;
		public const byte IOINTMSK = 0x0C << 3;
		public const byte IOCTRL = 0x0E << 3;
		public const byte EFCR = 0x0F << 3;

		public const byte DLL = 0x00 << 3;
		public const byte DLM = 0x01 << 3;
		public const byte EFR = 0x02 << 3;
		public const byte XON1 = 0x04 << 3;
		public const byte XON2 = 0x05 << 3;
		public const byte XOFF1 = 0x06 << 3;
		public const byte XOFF2 = 0x07 << 3;


		public const byte SPI_READ_MODE_FLAG = 0x80;
		public const uint BAUD_RATE_DEFAULT = 9600;
		#endregion
		#region Additional Constants
		private const byte EFR_ENABLE_CTS = 1 << 7;
		private const byte EFR_ENABLE_RTS = 1 << 6;
		private const byte EFR_ENABLE_ENHANCED_FUNCTIONS = 1 << 4;

		private const byte LCR_ENABLE_DIVISOR_LATCH = 1 << 7;

		private const uint XTAL_FREQUENCY = 14745600;
		#endregion
		#region Static Fields
		private static SPI_UART_cfg SPI_Uart_config = new SPI_UART_cfg
		{
			DataFormat = 0x03,
			Flow = EFR_ENABLE_CTS | EFR_ENABLE_RTS | EFR_ENABLE_ENHANCED_FUNCTIONS
		};
		#endregion
		#region Properties
		public byte AvailableBytes
		{
			get
			{
				return ReadRegister(RXLVL);
			}
		}

		private SpiDevice Device { get; set; }

		private bool UartConnected
		{
			get
			{
				const byte TEST_VALUE = 0x04;
				WriteRegister(SPR, TEST_VALUE);
				return ReadRegister(SPR) == TEST_VALUE;
			}
		}
		#endregion

		#region Constructors
		public SpiUartDevice()
		{
			Device = new SpiDevice();
		}
		#endregion

		#region Internal Implementation Methods
		private ushort BaudRateDivisor(uint baud)
		{
			uint value = XTAL_FREQUENCY / (baud * 16);
			if (value > ushort.MaxValue)
				throw new InvalidOperationException("Divisor is greater than UInt16.MaxValue");
			return (ushort)value;
		}

		public void ConfigureUart(uint baudrate)
		{
			SetBaudRate(baudrate);

			WriteRegister(LCR, 0xBF); // access EFR register
			WriteRegister(EFR, SPI_Uart_config.Flow); // enable enhanced registers
			WriteRegister(LCR, SPI_Uart_config.DataFormat); // 8 data bit, 1 stop bit, no parity
			WriteRegister(FCR, 0x06); // reset TXFIFO, reset RXFIFO, non FIFO mode
			WriteRegister(FCR, 0x01); // enable FIFO mode     
		}

		private void InitUart(uint baudrate)
		{
			ConfigureUart(baudrate);
			if (!UartConnected)
				throw new WiFlyException("Failed to initialize UART device.");
		}

		private byte ReadRegister(byte register)
		{
			const byte SPI_DUMMY_BYTE = 0xFF;

			Device.Select();
			Device.Transfer((byte)(SPI_READ_MODE_FLAG | register));
			byte result = Device.Transfer(SPI_DUMMY_BYTE);
			Device.Deselect();
			return result;
		}

		private void SetBaudRate(uint baudRate)
		{
			ushort divisor = BaudRateDivisor(baudRate);
			WriteRegister(LCR, LCR_ENABLE_DIVISOR_LATCH);
			WriteRegister(DLL, (byte)divisor);
			WriteRegister(DLM, (byte)(divisor >> 8));
		}

		private void WriteRegister(byte register, byte data)
		{
			Device.Select();
			Device.Transfer(register);
			Device.Transfer(data);
			Device.Deselect();
		}
		#endregion
		#region Methods
		public void Begin() { Begin(BAUD_RATE_DEFAULT); }

		public void Begin(uint baudrate)
		{
			Device.Begin();
			InitUart(baudrate);
		}

		public void Flush()
		{
			while (AvailableBytes > 0)
			{
				Read();
			}
		}

		public int Read()
		{
			if (AvailableBytes == 0)
				return -1;
			return ReadRegister(RHR);
		}

		public void SetIODirection(byte bits)
		{
			WriteRegister(IODIR, bits);
		}

		public void SetIOState(byte bits)
		{
			WriteRegister(IOSTATE, bits);
		}

		public void Write(byte value)
		{
			while (ReadRegister(TXLVL) == 0)
			{
				// Wait for space in TX buffer
				Thread.Sleep(5);
			}
			WriteRegister(THR, value);
		}

		public void Write(short value)
		{
			byte high = (byte)(value >> 8);
			byte low = (byte)value;

			Write(new[] { high, low });
		}

		public void Write(ushort value) { Write((short)value); }

		public void Write(byte[] data)
		{
			Device.Select();
			Device.Transfer(THR);

			int size = data.Length;
			int offset = 0;
			byte[] bytes = new byte[16];
			while (size > 16)
			{
				Array.Copy(data, offset, bytes, 0, 16);
				Device.TransferBulk(bytes);
				size -= 16;
				offset += 16;
			}
			bytes = new byte[size];
			Array.Copy(data, offset, bytes, 0, size);
			Device.TransferBulk(bytes);

			Device.Deselect();
		}

		public void Write(string value)
		{
			Write(Encoding.UTF8.GetBytes(value));
		}

		public void WriteLine() { WriteLine(String.Empty); }

		public void WriteLine(string value) { Write(value + "\r\n"); }
		#endregion
	}

	#region Related Types
	internal struct SPI_UART_cfg
	{
		public byte DataFormat;
		public byte Flow;
	}
	#endregion
}
