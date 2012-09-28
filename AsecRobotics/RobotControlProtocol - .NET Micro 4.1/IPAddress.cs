using System;

namespace Asec.Robotics.Net
{
	// Summary:
	//     Provides an Internet Protocol (IP) address.
	[Serializable]
	public class IPAddress
	{
		// Summary:
		//     Provides an IP address that indicates that the server must listen for client
		//     activity on all network interfaces. This field is read-only.
		public static readonly IPAddress Any;
		//
		// Summary:
		//     Provides the IP broadcast address. This field is read-only.
		public static readonly IPAddress Broadcast;
		//
		// Summary:
		//     The System.Net.Sockets.Socket.Bind(System.Net.EndPoint) method uses the System.Net.IPAddress.IPv6Any
		//     field to indicate that a System.Net.Sockets.Socket must listen for client
		//     activity on all network interfaces.
		public static readonly IPAddress IPv6Any;
		//
		// Summary:
		//     Provides the IP loopback address. This property is read-only.
		public static readonly IPAddress IPv6Loopback;
		//
		// Summary:
		//     Provides an IP address that indicates that no network interface should be
		//     used. This property is read-only.
		public static readonly IPAddress IPv6None;
		//
		// Summary:
		//     Provides the IP loopback address. This field is read-only.
		public static readonly IPAddress Loopback;
		//
		// Summary:
		//     Provides an IP address that indicates that no network interface should be
		//     used. This field is read-only.
		public static readonly IPAddress None;

		// Summary:
		//     Initializes a new instance of the System.Net.IPAddress class with the address
		//     specified as a System.Byte array.
		//
		// Parameters:
		//   address:
		//     The byte array value of the IP address.
		//
		// Exceptions:
		//   System.ArgumentNullException:
		//     address is null.
		public IPAddress(byte[] address);
		//
		// Summary:
		//     Initializes a new instance of the System.Net.IPAddress class with the address
		//     specified as an System.Int64.
		//
		// Parameters:
		//   newAddress:
		//     The long value of the IP address. For example, the value 0x2414188f in big-endian
		//     format would be the IP address "143.24.20.36".
		public IPAddress(long newAddress);
		//
		// Summary:
		//     Initializes a new instance of the System.Net.IPAddress class with the address
		//     specified as a System.Byte array and the specified scope identifier.
		//
		// Parameters:
		//   address:
		//     The byte array value of the IP address.
		//
		//   scopeid:
		//     The long value of the scope identifier.
		//
		// Exceptions:
		//   System.ArgumentNullException:
		//     address is null.
		//
		//   System.ArgumentOutOfRangeException:
		//     scopeid < 0 or scopeid > 0x00000000FFFFFFFF
		public IPAddress(byte[] address, long scopeid);

		// Summary:
		//     An Internet Protocol (IP) address.
		//
		// Returns:
		//     The long value of the IP address.
		[Obsolete("This property has been deprecated. It is address family dependent. Please use IPAddress.Equals method to perform comparisons. http://go.microsoft.com/fwlink/?linkid=14202")]
		public long Address { get; set; }
		//
		// Summary:
		//     Gets the address family of the IP address.
		//
		// Returns:
		//     Returns System.Net.Sockets.AddressFamily.InterNetwork for IPv4 or System.Net.Sockets.AddressFamily.InterNetworkV6
		//     for IPv6.
		public AddressFamily AddressFamily { get; }
		//
		// Summary:
		//     Gets whether the address is an IPv6 link local address.
		//
		// Returns:
		//     true if the IP address is an IPv6 link local address; otherwise, false.
		public bool IsIPv6LinkLocal { get; }
		//
		// Summary:
		//     Gets whether the address is an IPv6 multicast global address.
		//
		// Returns:
		//     true if the IP address is an IPv6 multicast global address; otherwise, false.
		public bool IsIPv6Multicast { get; }
		//
		// Summary:
		//     Gets whether the address is an IPv6 site local address.
		//
		// Returns:
		//     true if the IP address is an IPv6 site local address; otherwise, false.
		public bool IsIPv6SiteLocal { get; }
		//
		// Summary:
		//     Gets whether the address is an IPv6 Teredo address.
		//
		// Returns:
		//     true if the IP address is an IPv6 Teredo address; otherwise, false.
		public bool IsIPv6Teredo { get; }
		//
		// Summary:
		//     Gets or sets the IPv6 address scope identifier.
		//
		// Returns:
		//     A long integer that specifies the scope of the address.
		//
		// Exceptions:
		//   System.Net.Sockets.SocketException:
		//     AddressFamily = InterNetwork.
		//
		//   System.ArgumentOutOfRangeException:
		//     scopeId < 0- or -scopeId > 0x00000000FFFFFFFF
		public long ScopeId { get; set; }

		// Summary:
		//     Compares two IP addresses.
		//
		// Parameters:
		//   comparand:
		//     An System.Net.IPAddress instance to compare to the current instance.
		//
		// Returns:
		//     true if the two addresses are equal; otherwise, false.
		[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
		public override bool Equals(object comparand);
		//
		// Summary:
		//     Provides a copy of the System.Net.IPAddress as an array of bytes.
		//
		// Returns:
		//     A System.Byte array.
		public byte[] GetAddressBytes();
		//
		// Summary:
		//     Returns a hash value for an IP address.
		//
		// Returns:
		//     An integer hash value.
		public override int GetHashCode();
		//
		// Summary:
		//     Converts an integer value from host byte order to network byte order.
		//
		// Parameters:
		//   host:
		//     The number to convert, expressed in host byte order.
		//
		// Returns:
		//     An integer value, expressed in network byte order.
		public static int HostToNetworkOrder(int host);
		//
		// Summary:
		//     Converts a long value from host byte order to network byte order.
		//
		// Parameters:
		//   host:
		//     The number to convert, expressed in host byte order.
		//
		// Returns:
		//     A long value, expressed in network byte order.
		public static long HostToNetworkOrder(long host);
		//
		// Summary:
		//     Converts a short value from host byte order to network byte order.
		//
		// Parameters:
		//   host:
		//     The number to convert, expressed in host byte order.
		//
		// Returns:
		//     A short value, expressed in network byte order.
		public static short HostToNetworkOrder(short host);
		//
		// Summary:
		//     Indicates whether the specified IP address is the loopback address.
		//
		// Parameters:
		//   address:
		//     An IP address.
		//
		// Returns:
		//     true if address is the loopback address; otherwise, false.
		public static bool IsLoopback(IPAddress address);
		//
		// Summary:
		//     Converts an integer value from network byte order to host byte order.
		//
		// Parameters:
		//   network:
		//     The number to convert, expressed in network byte order.
		//
		// Returns:
		//     An integer value, expressed in host byte order.
		[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
		public static int NetworkToHostOrder(int network);
		//
		// Summary:
		//     Converts a long value from network byte order to host byte order.
		//
		// Parameters:
		//   network:
		//     The number to convert, expressed in network byte order.
		//
		// Returns:
		//     A long value, expressed in host byte order.
		[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
		public static long NetworkToHostOrder(long network);
		//
		// Summary:
		//     Converts a short value from network byte order to host byte order.
		//
		// Parameters:
		//   network:
		//     The number to convert, expressed in network byte order.
		//
		// Returns:
		//     A short value, expressed in host byte order.
		[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
		public static short NetworkToHostOrder(short network);
		//
		// Summary:
		//     Converts an IP address string to an System.Net.IPAddress instance.
		//
		// Parameters:
		//   ipString:
		//     A string that contains an IP address in dotted-quad notation for IPv4 and
		//     in colon-hexadecimal notation for IPv6.
		//
		// Returns:
		//     An System.Net.IPAddress instance.
		//
		// Exceptions:
		//   System.ArgumentNullException:
		//     ipString is null.
		//
		//   System.FormatException:
		//     ipString is not a valid IP address.
		[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
		public static IPAddress Parse(string ipString);
		//
		// Summary:
		//     Converts an Internet address to its standard notation.
		//
		// Returns:
		//     A string that contains the IP address in either IPv4 dotted-quad or in IPv6
		//     colon-hexadecimal notation.
		public override string ToString();
		//
		// Summary:
		//     Determines whether a string is a valid IP address.
		//
		// Parameters:
		//   ipString:
		//     The string to validate.
		//
		//   address:
		//     The System.Net.IPAddress version of the string.
		//
		// Returns:
		//     true if ipString is a valid IP address; otherwise, false.
		public static bool TryParse(string ipString, out IPAddress address);
	}
}