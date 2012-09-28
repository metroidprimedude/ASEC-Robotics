using System;

namespace Asec.Robotics.Net
{
	// Summary:
	//     Represents a network endpoint as an IP address and a port number.
	[Serializable]
	public class IPEndPoint : EndPoint
	{
		// Summary:
		//     Specifies the maximum value that can be assigned to the System.Net.IPEndPoint.Port
		//     property. The MaxPort value is set to 0x0000FFFF. This field is read-only.
		public const int MaxPort = 65535;
		//
		// Summary:
		//     Specifies the minimum value that can be assigned to the System.Net.IPEndPoint.Port
		//     property. This field is read-only.
		public const int MinPort = 0;

		// Summary:
		//     Initializes a new instance of the System.Net.IPEndPoint class with the specified
		//     address and port number.
		//
		// Parameters:
		//   address:
		//     An System.Net.IPAddress.
		//
		//   port:
		//     The port number associated with the address, or 0 to specify any available
		//     port. port is in host order.
		//
		// Exceptions:
		//   System.ArgumentOutOfRangeException:
		//     port is less than System.Net.IPEndPoint.MinPort.-or- port is greater than
		//     System.Net.IPEndPoint.MaxPort.-or- address is less than 0 or greater than
		//     0x00000000FFFFFFFF.
		public IPEndPoint(IPAddress address, int port);
		//
		// Summary:
		//     Initializes a new instance of the System.Net.IPEndPoint class with the specified
		//     address and port number.
		//
		// Parameters:
		//   address:
		//     The IP address of the Internet host.
		//
		//   port:
		//     The port number associated with the address, or 0 to specify any available
		//     port. port is in host order.
		//
		// Exceptions:
		//   System.ArgumentOutOfRangeException:
		//     port is less than System.Net.IPEndPoint.MinPort.-or- port is greater than
		//     System.Net.IPEndPoint.MaxPort.-or- address is less than 0 or greater than
		//     0x00000000FFFFFFFF.
		public IPEndPoint(long address, int port);

		// Summary:
		//     Gets or sets the IP address of the endpoint.
		//
		// Returns:
		//     An System.Net.IPAddress instance containing the IP address of the endpoint.
		public IPAddress Address { get; set; }
		//
		// Summary:
		//     Gets the Internet Protocol (IP) address family.
		//
		// Returns:
		//     Returns System.Net.Sockets.AddressFamily.InterNetwork.
		public override AddressFamily AddressFamily { get; }
		//
		// Summary:
		//     Gets or sets the port number of the endpoint.
		//
		// Returns:
		//     An integer value in the range System.Net.IPEndPoint.MinPort to System.Net.IPEndPoint.MaxPort
		//     indicating the port number of the endpoint.
		//
		// Exceptions:
		//   System.ArgumentOutOfRangeException:
		//     The value that was specified for a set operation is less than System.Net.IPEndPoint.MinPort
		//     or greater than System.Net.IPEndPoint.MaxPort.
		public int Port { get; set; }

		// Summary:
		//     Creates an endpoint from a socket address.
		//
		// Parameters:
		//   socketAddress:
		//     The System.Net.SocketAddress to use for the endpoint.
		//
		// Returns:
		//     An System.Net.EndPoint instance using the specified socket address.
		//
		// Exceptions:
		//   System.ArgumentException:
		//     The AddressFamily of socketAddress is not equal to the AddressFamily of the
		//     current instance.-or- socketAddress.Size < 8.
		public override EndPoint Create(SocketAddress socketAddress);
		//
		// Summary:
		//     Determines whether the specified System.Object is equal to the current System.Net.IPEndPoint
		//     instance.
		//
		// Parameters:
		//   comparand:
		//     The specified System.Object to compare with the current System.Net.IPEndPoint
		//     instance.
		//
		// Returns:
		//     true if the specified System.Object is equal to the current System.Object;
		//     otherwise, false.
		public override bool Equals(object comparand);
		//
		// Summary:
		//     Returns a hash value for a System.Net.IPEndPoint instance.
		//
		// Returns:
		//     An integer hash value.
		public override int GetHashCode();
		//
		// Summary:
		//     Serializes endpoint information into a System.Net.SocketAddress instance.
		//
		// Returns:
		//     A System.Net.SocketAddress instance containing the socket address for the
		//     endpoint.
		public override SocketAddress Serialize();
		//
		// Summary:
		//     Returns the IP address and port number of the specified endpoint.
		//
		// Returns:
		//     A string containing the IP address and the port number of the specified endpoint
		//     (for example, 192.168.1.2:80).
		public override string ToString();
	}
}