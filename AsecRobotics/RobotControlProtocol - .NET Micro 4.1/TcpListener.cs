using System;

namespace Asec.Robotics.Net
{
	// Summary:
	//     Listens for connections from TCP network clients.
	public class TcpListener
	{
		//
		// Summary:
		//     Initializes a new instance of the System.Net.Sockets.TcpListener class with
		//     the specified local endpoint.
		//
		// Parameters:
		//   localEP:
		//     An System.Net.IPEndPoint that represents the local endpoint to which to bind
		//     the listener System.Net.Sockets.Socket.
		//
		// Exceptions:
		//   System.ArgumentNullException:
		//     localEP is null.
		public TcpListener(IPEndPoint localEP);
		//
		// Summary:
		//     Initializes a new instance of the System.Net.Sockets.TcpListener class that
		//     listens for incoming connection attempts on the specified local IP address
		//     and port number.
		//
		// Parameters:
		//   localaddr:
		//     An System.Net.IPAddress that represents the local IP address.
		//
		//   port:
		//     The port on which to listen for incoming connection attempts.
		//
		// Exceptions:
		//   System.ArgumentNullException:
		//     localaddr is null.
		//
		//   System.ArgumentOutOfRangeException:
		//     port is not between System.Net.IPEndPoint.MinPort and System.Net.IPEndPoint.MaxPort.
		public TcpListener(IPAddress localaddr, int port);

		// Summary:
		//     Gets a value that indicates whether System.Net.Sockets.TcpListener is actively
		//     listening for client connections.
		//
		// Returns:
		//     true if System.Net.Sockets.TcpListener is actively listening; otherwise,
		//     false.
		protected bool Active { get; }
		//
		// Summary:
		//     Gets or sets a System.Boolean value that specifies whether the System.Net.Sockets.TcpListener
		//     allows only one underlying socket to listen to a specific port.
		//
		// Returns:
		//     true if the System.Net.Sockets.TcpListener allows only one System.Net.Sockets.TcpListener
		//     to listen to a specific port; otherwise, false. . The default is true for
		//     Windows Server 2003 and Windows XP Service Pack 2 and later, and false for
		//     all other versions.
		//
		// Exceptions:
		//   System.InvalidOperationException:
		//     The System.Net.Sockets.TcpListener has been started. Call the System.Net.Sockets.TcpListener.Stop()
		//     method and then set the System.Net.Sockets.Socket.ExclusiveAddressUse property.
		//
		//   System.Net.Sockets.SocketException:
		//     An error occurred when attempting to access the underlying socket.
		//
		//   System.ObjectDisposedException:
		//     The underlying System.Net.Sockets.Socket has been closed.
		public bool ExclusiveAddressUse { get; set; }
		//
		// Summary:
		//     Gets the underlying System.Net.EndPoint of the current System.Net.Sockets.TcpListener.
		//
		// Returns:
		//     The System.Net.EndPoint to which the System.Net.Sockets.Socket is bound.
		public EndPoint LocalEndpoint { get; }
		//
		// Summary:
		//     Accepts a pending connection request
		//
		// Returns:
		//     A System.Net.Sockets.TcpClient used to send and receive data.
		//
		// Exceptions:
		//   System.InvalidOperationException:
		//     The listener has not been started with a call to System.Net.Sockets.TcpListener.Start().
		//
		//   System.Net.Sockets.SocketException:
		//     Use the System.Net.Sockets.SocketException.ErrorCode property to obtain the
		//     specific error code. When you have obtained this code, you can refer to the
		//     Windows Sockets version 2 API error code documentation in MSDN for a detailed
		//     description of the error.
		public TcpClient AcceptTcpClient();
		//
		// Summary:
		//     Begins an asynchronous operation to accept an incoming connection attempt.
		//
		// Parameters:
		//   callback:
		//     An System.AsyncCallback delegate that references the method to invoke when
		//     the operation is complete.
		//
		//   state:
		//     A user-defined object containing information about the accept operation.
		//     This object is passed to the callback delegate when the operation is complete.
		//
		// Returns:
		//     An System.IAsyncResult that references the asynchronous creation of the System.Net.Sockets.Socket.
		//
		// Exceptions:
		//   System.Net.Sockets.SocketException:
		//     An error occurred while attempting to access the socket. See the Remarks
		//     section for more information.
		//
		//   System.ObjectDisposedException:
		//     The System.Net.Sockets.Socket has been closed.
		public IAsyncResult BeginAcceptSocket(AsyncCallback callback, object state);
		//
		// Summary:
		//     Begins an asynchronous operation to accept an incoming connection attempt.
		//
		// Parameters:
		//   callback:
		//     An System.AsyncCallback delegate that references the method to invoke when
		//     the operation is complete.
		//
		//   state:
		//     A user-defined object containing information about the accept operation.
		//     This object is passed to the callback delegate when the operation is complete.
		//
		// Returns:
		//     An System.IAsyncResult that references the asynchronous creation of the System.Net.Sockets.TcpClient.
		//
		// Exceptions:
		//   System.Net.Sockets.SocketException:
		//     An error occurred while attempting to access the socket. See the Remarks
		//     section for more information.
		//
		//   System.ObjectDisposedException:
		//     The System.Net.Sockets.Socket has been closed.
		public IAsyncResult BeginAcceptTcpClient(AsyncCallback callback, object state);
		//
		// Summary:
		//     Asynchronously accepts an incoming connection attempt and creates a new System.Net.Sockets.Socket
		//     to handle remote host communication.
		//
		// Parameters:
		//   asyncResult:
		//     An System.IAsyncResult returned by a call to the System.Net.Sockets.TcpListener.BeginAcceptSocket(System.AsyncCallback,System.Object)
		//     method.
		//
		// Returns:
		//     A System.Net.Sockets.Socket.
		//
		// Exceptions:
		//   System.ObjectDisposedException:
		//     The underlying System.Net.Sockets.Socket has been closed.
		//
		//   System.ArgumentNullException:
		//     The asyncResult parameter is null.
		//
		//   System.ArgumentException:
		//     The asyncResult parameter was not created by a call to the System.Net.Sockets.TcpListener.BeginAcceptSocket(System.AsyncCallback,System.Object)
		//     method.
		//
		//   System.InvalidOperationException:
		//     The System.Net.Sockets.TcpListener.EndAcceptSocket(System.IAsyncResult) method
		//     was previously called.
		//
		//   System.Net.Sockets.SocketException:
		//     An error occurred while attempting to access the System.Net.Sockets.Socket.
		//     See the Remarks section for more information.
		public Socket EndAcceptSocket(IAsyncResult asyncResult);
		//
		// Summary:
		//     Asynchronously accepts an incoming connection attempt and creates a new System.Net.Sockets.TcpClient
		//     to handle remote host communication.
		//
		// Parameters:
		//   asyncResult:
		//     An System.IAsyncResult returned by a call to the System.Net.Sockets.TcpListener.BeginAcceptTcpClient(System.AsyncCallback,System.Object)
		//     method.
		//
		// Returns:
		//     A System.Net.Sockets.TcpClient.
		public TcpClient EndAcceptTcpClient(IAsyncResult asyncResult);
		//
		// Summary:
		//     Determines if there are pending connection requests.
		//
		// Returns:
		//     true if connections are pending; otherwise, false.
		//
		// Exceptions:
		//   System.InvalidOperationException:
		//     The listener has not been started with a call to System.Net.Sockets.TcpListener.Start().
		public bool Pending();
		//
		// Summary:
		//     Starts listening for incoming connection requests.
		//
		// Exceptions:
		//   System.Net.Sockets.SocketException:
		//     Use the System.Net.Sockets.SocketException.ErrorCode property to obtain the
		//     specific error code. When you have obtained this code, you can refer to the
		//     Windows Sockets version 2 API error code documentation in MSDN for a detailed
		//     description of the error.
		[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
		public void Start();
		//
		// Summary:
		//     Closes the listener.
		//
		// Exceptions:
		//   System.Net.Sockets.SocketException:
		//     Use the System.Net.Sockets.SocketException.ErrorCode property to obtain the
		//     specific error code. When you have obtained this code, you can refer to the
		//     Windows Sockets version 2 API error code documentation in MSDN for a detailed
		//     description of the error.
		public void Stop();
	}
}