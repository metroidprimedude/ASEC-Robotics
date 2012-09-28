using System;
using Microsoft.SPOT;

namespace NetduinoWiFly
{
	public class WiFlyServer
	{
		#region Constants
		private readonly WiFlyClient NO_CLIENT = new WiFlyClient((byte[])null, 0);
		#endregion
		#region Fields
		private ushort _port;
		private WiFlyClient _activeClient;
		#endregion

		#region Constuctors
		public WiFlyServer(ushort port)
		{
			_port = port;
			WiFlyDevice.WiFly._serverPort = 0;
			_activeClient = NO_CLIENT;
		}
		#endregion

		#region Methods
		public void Begin()
		{
			
		}
		
		public WiFlyClient Available()
		{
			if (!WiFlyDevice.WiFly._serverConnectionActive)
				_activeClient._port = 0;

			const string TOKEN_MATCH_OPEN = "*OPEN*";

			if(!_activeClient.Valid)
			{
				if(WiFlyDevice.WiFly._uart.AvailableBytes >= TOKEN_MATCH_OPEN.Length)
				{
					if (WiFlyDevice.WiFly.ResponsMatched(TOKEN_MATCH_OPEN))
					{
						_activeClient._port = _port;
						_activeClient._hostname = null;
						_activeClient._ip = null;

						_activeClient._open = true;
						WiFlyDevice.WiFly._serverConnectionActive = true;
					}
					else
						WiFlyDevice.WiFly._uart.Flush();
				}
			}

			return _activeClient;
		}
		#endregion
	}
}
