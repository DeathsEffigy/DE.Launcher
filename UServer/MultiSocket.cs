/*
 * Created by SharpDevelop.
 * User: Fabian
 * Date: 11.08.2014
 * Time: 17:25
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Threading;
using System.Net.Sockets;
using System.Text;

namespace UServer
{
	/// <summary>
	/// Description of MultiSocket.
	/// </summary>
	public class MultiSocket
	{
		int iPort;
		private bool Listening = false;
		TcpListener serverSocket;
		TcpClient clientSocket;
		
		public MultiSocket(int port)
		{
			iPort = port;
		}
		
		public void Listen()
		{
			serverSocket = new TcpListener(iPort);
			clientSocket = default(TcpClient);
			int counter = 0;
			
			serverSocket.Start();
			Listening = true;
			
			while (Listening == true) {
				counter += 1;
				clientSocket = serverSocket.AcceptTcpClient();
				SocketThread client = new SocketThread();
				client.startClient(clientSocket, Convert.ToString(counter));
			}
			
			clientSocket.Close();
			serverSocket.Stop();
		}
		
		public void StopListen()
		{
			Listening = false;
		}
	}
}
