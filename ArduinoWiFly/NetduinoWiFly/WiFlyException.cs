namespace NetduinoWiFly
{
	using System;

	public class WiFlyException : Exception
	{
		public WiFlyException() {}

		public WiFlyException(string message) : base(message) {}

		public WiFlyException(string message, Exception inner) : base(message, inner) {}
	}
}
