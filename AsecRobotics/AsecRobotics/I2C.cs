using System;
using System.IO;
using Microsoft.SPOT.Hardware;

namespace Netduino
{
	public class I2C : IDisposable
	{
		private int ClockRate;
		private I2CDevice Device;

		/// <summary>
		/// Instantiate the I2C bus for use.
		/// </summary>
		/// <param name="clockRate">I2C clock rate as kHz.</param>
		public I2C(int clockRate)
		{
			ClockRate = clockRate;
			Device = new I2CDevice(null);
		}

		~I2C()
		{
			Device.Dispose();
		}

		/// <summary>
		/// Write to a I2C slave device.
		/// </summary>
		/// <param name="device">I2C slave device.</param>
		/// <param name="writeBuffer">Bytes to be written to the slave.</param>
		/// <param name="transactionTimeout">Time in mS the system will allow for a transaction.</param>
		public void Write(I2CSlave device, byte[] writeBuffer, int transactionTimeout)
		{
			Device.Config = new I2CDevice.Configuration(device.Address, ClockRate);
			I2CDevice.I2CTransaction[] writeTransaction = new I2CDevice.I2CTransaction[] { I2CDevice.CreateWriteTransaction(writeBuffer) };
			lock(Device)
				if(Device.Execute(writeTransaction, transactionTimeout) != writeBuffer.Length)
					throw new IOException("Could not write to slave.");
		}

		/// <summary>
		/// Read from a I2C slave device.
		/// </summary>
		/// <param name="device">I2C slave device.</param>
		/// <param name="readBuffer">Bytes to be read from the slave.</param>
		/// <param name="transactionTimeout">The amount of time the system will wait before resuming execution of the transaction.</param>
		public void Read(I2CSlave device, byte[] readBuffer, int transactionTimeout)
		{
			Device.Config = new I2CDevice.Configuration(device.Address, ClockRate);
			I2CDevice.I2CTransaction[] readTransaction = new I2CDevice.I2CTransaction[] { I2CDevice.CreateReadTransaction(readBuffer) };
			lock(Device)
				if(Device.Execute(readTransaction, transactionTimeout) != readBuffer.Length)
					throw new IOException("Could not read from slave.");
		}

		/// <summary>
		/// Read from a register on a I2C slave device.
		/// </summary>
		/// <param name="device">I2C slave device.</param>
		/// <param name="register">Register to read from.</param>
		/// <param name="transactionTimeout">Time in mS the system will allow for a transaction.</param>
		public byte ReadRegister(I2CSlave device, byte register, int transactionTimeout)
		{
			byte[] buffer = { register };
			Write(device, buffer, transactionTimeout);
			Read(device, buffer, transactionTimeout);
			return buffer[0];
		}

		/// <summary>
		/// Write to a register on a I2C slave device.
		/// </summary>
		/// <param name="device">I2C slave device.</param>
		/// <param name="register">Register to write to.</param>
		/// <param name="transactionTimeout">Time in mS the system will allow for a transaction.</param>
		public void WriteRegister(I2CSlave device, byte register, byte value, int transactionTimeout)
		{
			byte[] buffer = { register, value };
			Write(device, buffer, transactionTimeout);
		}

		/// <summary>
		/// Dispose of the I2C bus.
		/// </summary>
		public void Dispose()
		{
			Device.Dispose();
		}
	}

	public class I2CSlave
	{
		public readonly byte Address;

		/// <summary>
		/// Instantiate a new I2C slave device.
		/// </summary>
		/// <param name="address">The slave's 7bit address.</param>
		public I2CSlave(byte address)
		{
			if(address < 0x80)
				Address = address;
			else
				throw new ArgumentException("The address provided was invalid.", "address");
		}
	}
}