using System;
using Microsoft.SPOT;
using SecretLabs.NETMF.Hardware.NetduinoPlus;

namespace Asec.Robotics.Net
{
	// Summary:
	//     Provides client connections for TCP network services.
	public class TcpClient : IDisposable
	{
		// Summary:
		//     Initializes a new instance of the System.Net.Sockets.TcpClient class.
		public TcpClient();
		//
		// Summary:
		//     Initializes a new instance of the System.Net.Sockets.TcpClient class and
		//     connects to the specified port on the specified host.
		//
		// Parameters:
		//   hostname:
		//     The DNS name of the remote host to which you intend to connect.
		//
		//   port:
		//     The port number of the remote host to which you intend to connect.
		//
		// Exceptions:
		//   System.ArgumentNullException:
		//     The hostname parameter is null.
		//
		//   System.ArgumentOutOfRangeException:
		//     The port parameter is not between System.Net.IPEndPoint.MinPort and System.Net.IPEndPoint.MaxPort.
		//
		//   System.Net.Sockets.SocketException:
		//     An error occurred when accessing the socket. See the Remarks section for
		//     more information.
		public TcpClient(string hostname, int port);

		// Summary:
		//     Gets or set a value that indicates whether a connection has been made.
		//
		// Returns:
		//     true if the connection has been made; otherwise, false.
		protected bool Active { get; set; }
		//
		// Summary:
		//     Gets the amount of data that has been received from the network and is available
		//     to be read.
		//
		// Returns:
		//     The number of bytes of data received from the network and available to be
		//     read.
		//
		// Exceptions:
		//   System.Net.Sockets.SocketException:
		//     An error occurred when attempting to access the socket. See the Remarks section
		//     for more information.
		//
		//   System.ObjectDisposedException:
		//     The System.Net.Sockets.Socket has been closed.
		public int Available { get; }
		//
		// Summary:
		//     Gets a value indicating whether the underlying System.Net.Sockets.Socket
		//     for a System.Net.Sockets.TcpClient is connected to a remote host.
		//
		// Returns:
		//     true if the System.Net.Sockets.TcpClient.Client socket was connected to a
		//     remote resource as of the most recent operation; otherwise, false.
		public bool Connected { get; }
		//
		// Summary:
		//     Gets or sets the size of the receive buffer.
		//
		// Returns:
		//     The size of the receive buffer, in bytes. The default value is 8192 bytes.
		//
		// Exceptions:
		//   System.Net.Sockets.SocketException:
		//     An error occurred when setting the buffer size.-or-In .NET Compact Framework
		//     applications, you cannot set this property. For a workaround, see the Platform
		//     Note in Remarks.
		public int ReceiveBufferSize { get; set; }
		//
		// Summary:
		//     Gets or sets the amount of time a System.Net.Sockets.TcpClient will wait
		//     to receive data once a read operation is initiated.
		//
		// Returns:
		//     The time-out value of the connection in milliseconds. The default value is
		//     0.
		public int ReceiveTimeout { get; set; }
		//
		// Summary:
		//     Gets or sets the size of the send buffer.
		//
		// Returns:
		//     The size of the send buffer, in bytes. The default value is 8192 bytes.
		public int SendBufferSize { get; set; }
		//
		// Summary:
		//     Gets or sets the amount of time a System.Net.Sockets.TcpClient will wait
		//     for a send operation to complete successfully.
		//
		// Returns:
		//     The send time-out value, in milliseconds. The default is 0.
		public int SendTimeout { get; set; }

		// Summary:
		//     Begins an asynchronous request for a remote host connection. The remote host
		//     is specified by an System.Net.IPAddress and a port number (System.Int32).
		//
		// Parameters:
		//   address:
		//     The System.Net.IPAddress of the remote host.
		//
		//   port:
		//     The port number of the remote host.
		//
		//   requestCallback:
		//     An System.AsyncCallback delegate that references the method to invoke when
		//     the operation is complete.
		//
		//   state:
		//     A user-defined object that contains information about the connect operation.
		//     This object is passed to the requestCallback delegate when the operation
		//     is complete.
		//
		// Returns:
		//     An System.IAsyncResult object that references the asynchronous connection.
		//
		// Exceptions:
		//   System.ArgumentNullException:
		//     The address parameter is null.
		//
		//   System.Net.Sockets.SocketException:
		//     An error occurred when attempting to access the socket. See the Remarks section
		//     for more information.
		//
		//   System.ObjectDisposedException:
		//     The System.Net.Sockets.Socket has been closed.
		//
		//   System.Security.SecurityException:
		//     A caller higher in the call stack does not have permission for the requested
		//     operation.
		//
		//   System.ArgumentOutOfRangeException:
		//     The port number is not valid.
		public IAsyncResult BeginConnect(IPAddress address, int port, AsyncCallback requestCallback, object state);
		//
		// Summary:
		//     Begins an asynchronous request for a remote host connection. The remote host
		//     is specified by an System.Net.IPAddress array and a port number (System.Int32).
		//
		// Parameters:
		//   addresses:
		//     At least one System.Net.IPAddress that designates the remote hosts.
		//
		//   port:
		//     The port number of the remote hosts.
		//
		//   requestCallback:
		//     An System.AsyncCallback delegate that references the method to invoke when
		//     the operation is complete.
		//
		//   state:
		//     A user-defined object that contains information about the connect operation.
		//     This object is passed to the requestCallback delegate when the operation
		//     is complete.
		//
		// Returns:
		//     An System.IAsyncResult object that references the asynchronous connection.
		//
		// Exceptions:
		//   System.ArgumentNullException:
		//     The addresses parameter is null.
		//
		//   System.Net.Sockets.SocketException:
		//     An error occurred when attempting to access the socket. See the Remarks section
		//     for more information.
		//
		//   System.ObjectDisposedException:
		//     The System.Net.Sockets.Socket has been closed.
		//
		//   System.Security.SecurityException:
		//     A caller higher in the call stack does not have permission for the requested
		//     operation.
		//
		//   System.ArgumentOutOfRangeException:
		//     The port number is not valid.
		public IAsyncResult BeginConnect(IPAddress[] addresses, int port, AsyncCallback requestCallback, object state);
		//
		// Summary:
		//     Begins an asynchronous request for a remote host connection. The remote host
		//     is specified by a host name (System.String) and a port number (System.Int32).
		//
		// Parameters:
		//   host:
		//     The name of the remote host.
		//
		//   port:
		//     The port number of the remote host.
		//
		//   requestCallback:
		//     An System.AsyncCallback delegate that references the method to invoke when
		//     the operation is complete.
		//
		//   state:
		//     A user-defined object that contains information about the connect operation.
		//     This object is passed to the requestCallback delegate when the operation
		//     is complete.
		//
		// Returns:
		//     An System.IAsyncResult object that references the asynchronous connection.
		//
		// Exceptions:
		//   System.ArgumentNullException:
		//     The host parameter is null.
		//
		//   System.Net.Sockets.SocketException:
		//     An error occurred when attempting to access the socket. See the Remarks section
		//     for more information.
		//
		//   System.ObjectDisposedException:
		//     The System.Net.Sockets.Socket has been closed.
		//
		//   System.Security.SecurityException:
		//     A caller higher in the call stack does not have permission for the requested
		//     operation.
		//
		//   System.ArgumentOutOfRangeException:
		//     The port number is not valid.
		public IAsyncResult BeginConnect(string host, int port, AsyncCallback requestCallback, object state);
		//
		// Summary:
		//     Disposes this System.Net.Sockets.TcpClient instance and requests that the
		//     underlying TCP connection be closed.
		public void Close();
		//
		// Summary:
		//     Connects the client to a remote TCP host using the specified IP address and
		//     port number.
		//
		// Parameters:
		//   address:
		//     The System.Net.IPAddress of the host to which you intend to connect.
		//
		//   port:
		//     The port number to which you intend to connect.
		//
		// Exceptions:
		//   System.ArgumentNullException:
		//     The address parameter is null.
		//
		//   System.ArgumentOutOfRangeException:
		//     The port is not between System.Net.IPEndPoint.MinPort and System.Net.IPEndPoint.MaxPort.
		//
		//   System.Net.Sockets.SocketException:
		//     An error occurred when accessing the socket. See the Remarks section for
		//     more information.
		//
		//   System.ObjectDisposedException:
		//     System.Net.Sockets.TcpClient is closed.
		public void Connect(IPAddress address, int port);
		//
		// Summary:
		//     Connects the client to the specified port on the specified host.
		//
		// Parameters:
		//   hostname:
		//     The DNS name of the remote host to which you intend to connect.
		//
		//   port:
		//     The port number of the remote host to which you intend to connect.
		//
		// Exceptions:
		//   System.ArgumentNullException:
		//     The hostname parameter is null.
		//
		//   System.ArgumentOutOfRangeException:
		//     The port parameter is not between System.Net.IPEndPoint.MinPort and System.Net.IPEndPoint.MaxPort.
		//
		//   System.Net.Sockets.SocketException:
		//     An error occurred when accessing the socket. See the Remarks section for
		//     more information.
		//
		//   System.ObjectDisposedException:
		//     System.Net.Sockets.TcpClient is closed.
		public void Connect(string hostname, int port);
		//
		// Summary:
		//     Releases the unmanaged resources used by the System.Net.Sockets.TcpClient
		//     and optionally releases the managed resources.
		//
		// Parameters:
		//   disposing:
		//     Set to true to release both managed and unmanaged resources; false to release
		//     only unmanaged resources.
		protected virtual void Dispose(bool disposing);
		//
		// Summary:
		//     Asynchronously accepts an incoming connection attempt.
		//
		// Parameters:
		//   asyncResult:
		//     An System.IAsyncResult object returned by a call to Overload:System.Net.Sockets.TcpClient.BeginConnect.
		//
		// Exceptions:
		//   System.ArgumentNullException:
		//     The asyncResult parameter is null.
		//
		//   System.ArgumentException:
		//     The asyncResult parameter was not returned by a call to a Overload:System.Net.Sockets.TcpClient.BeginConnect
		//     method.
		//
		//   System.InvalidOperationException:
		//     The System.Net.Sockets.TcpClient.EndConnect(System.IAsyncResult) method was
		//     previously called for the asynchronous connection.
		//
		//   System.Net.Sockets.SocketException:
		//     An error occurred when attempting to access the System.Net.Sockets.Socket.
		//     See the Remarks section for more information.
		//
		//   System.ObjectDisposedException:
		//     The underlying System.Net.Sockets.Socket has been closed.
		public void EndConnect(IAsyncResult asyncResult);
		//
		// Summary:
		//     Returns the System.Net.Sockets.NetworkStream used to send and receive data.
		//
		// Returns:
		//     The underlying System.Net.Sockets.NetworkStream.
		//
		// Exceptions:
		//   System.InvalidOperationException:
		//     The System.Net.Sockets.TcpClient is not connected to a remote host.
		//
		//   System.ObjectDisposedException:
		//     The System.Net.Sockets.TcpClient has been closed.
		public NetworkStream GetStream();
	}
}
