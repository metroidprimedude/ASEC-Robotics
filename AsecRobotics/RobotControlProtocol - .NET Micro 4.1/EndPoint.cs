using System;

namespace Asec.Robotics.Net
{
	// Summary:
	//     Identifies a network address. This is an abstract class.
	[Serializable]
	public abstract class EndPoint
	{
		// Summary:
		//     Initializes a new instance of the System.Net.EndPoint class.
		[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
		protected EndPoint();

		// Summary:
		//     Gets the address family to which the endpoint belongs.
		//
		// Returns:
		//     One of the System.Net.Sockets.AddressFamily values.
		//
		// Exceptions:
		//   System.NotImplementedException:
		//     Any attempt is made to get or set the property when the property is not overridden
		//     in a descendant class.
		public virtual AddressFamily AddressFamily { get; }

		// Summary:
		//     Creates an System.Net.EndPoint instance from a System.Net.SocketAddress instance.
		//
		// Parameters:
		//   socketAddress:
		//     The socket address that serves as the endpoint for a connection.
		//
		// Returns:
		//     A new System.Net.EndPoint instance that is initialized from the specified
		//     System.Net.SocketAddress instance.
		//
		// Exceptions:
		//   System.NotImplementedException:
		//     Any attempt is made to access the method when the method is not overridden
		//     in a descendant class.
		public virtual EndPoint Create(SocketAddress socketAddress);
		//
		// Summary:
		//     Serializes endpoint information into a System.Net.SocketAddress instance.
		//
		// Returns:
		//     A System.Net.SocketAddress instance that contains the endpoint information.
		//
		// Exceptions:
		//   System.NotImplementedException:
		//     Any attempt is made to access the method when the method is not overridden
		//     in a descendant class.
		public virtual SocketAddress Serialize();
	}
}