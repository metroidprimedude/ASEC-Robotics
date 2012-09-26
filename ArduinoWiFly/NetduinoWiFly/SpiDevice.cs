namespace NetduinoWiFly
{
	using Microsoft.SPOT.Hardware;
	using SecretLabs.NETMF.Hardware.NetduinoPlus;

	internal sealed class SpiDevice
	{
		#region Fields
		private SPI _device;
		private SPI.Configuration _currentConfig;
		#endregion
		#region Properties
		private SPI.Configuration CurrentConfig
		{
			get { return _device.Config; }
			set { _device.Config = value; }
		}
		private Cpu.Pin SelectPin
		{
			get { return CurrentConfig.ChipSelect_Port; }
			set
			{
				CurrentConfig = new SPI.Configuration(value,
				                                       CurrentConfig.ChipSelect_ActiveState,
				                                       CurrentConfig.ChipSelect_SetupTime,
				                                       CurrentConfig.ChipSelect_HoldTime,
				                                       CurrentConfig.Clock_IdleState,
				                                       CurrentConfig.Clock_Edge,
				                                       CurrentConfig.Clock_RateKHz,
				                                       CurrentConfig.SPI_mod);
			}
		}
		#endregion

		#region Constructors
		public SpiDevice()
		{
			SPI.Configuration config = new SPI.Configuration(Pins.GPIO_NONE,
			                                                 // Verify these values!
			                                                 false,
			                                                 0,
			                                                 0,
			                                                 false,
			                                                 false,
			                                                 48000,
			                                                 SPI_Devices.SPI1);
			_device = new SPI(config);
		}
		#endregion

		#region Methods
		public void Begin() { Begin(Pins.GPIO_NONE); } // This method should probably be removed

		public void Begin(Cpu.Pin selectPin)
		{
			SelectPin = selectPin;
			InitPins();
			InitSpi();
		}

		private void InitPins()
		{
			// This method doesn't apply to C#
		}

		private void InitSpi()
		{
			// Moved to constructor
		}

		public void Deselect()
		{
			CurrentConfig = new SPI.Configuration(CurrentConfig.ChipSelect_Port,
			                                      false,
			                                      CurrentConfig.ChipSelect_SetupTime,
			                                      CurrentConfig.ChipSelect_HoldTime,
			                                      CurrentConfig.Clock_IdleState,
			                                      CurrentConfig.Clock_Edge,
			                                      CurrentConfig.Clock_RateKHz,
			                                      CurrentConfig.SPI_mod);
		}

		public void Select()
		{
			CurrentConfig = new SPI.Configuration(CurrentConfig.ChipSelect_Port,
												  true,
												  CurrentConfig.ChipSelect_SetupTime,
												  CurrentConfig.ChipSelect_HoldTime,
												  CurrentConfig.Clock_IdleState,
												  CurrentConfig.Clock_Edge,
												  CurrentConfig.Clock_RateKHz,
												  CurrentConfig.SPI_mod);
		}

		public byte Transfer(byte data)
		{
			byte[] readBuffer = new byte[1];
			_device.WriteRead(new[] {data}, 0, 1, readBuffer, 0, 1, 0);
			return readBuffer[0];
		}

		public void TransferBulk(byte[] data)
		{
			_device.Write(data);
		}
		#endregion
	}
}
