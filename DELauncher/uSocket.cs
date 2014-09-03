/*
 * Created by SharpDevelop.
 * User: Fabian
 * Date: 11.08.2014
 * Time: 18:51
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Net.Sockets;

namespace DELauncher
{
	/// <summary>
	/// Description of USocket.
	/// </summary>
	public class USocket
	{
		System.Net.Sockets.TcpClient clientSocket;
		NetworkStream serverStream;
		string IP;
		int Port;
		
		public USocket(string ServerIP, int ServerPort)
		{
			IP = ServerIP;
			Port = ServerPort;
			clientSocket = new System.Net.Sockets.TcpClient();
		}
		
		public void Connect()
		{
			clientSocket.Connect(IP, Port);
			serverStream = clientSocket.GetStream();
		}
		
		public void Send(byte[] packet)
		{
			serverStream.Write(packet, 0, packet.Length);
		}
		
		public byte[] Receive(int bytes)
		{
			byte[] packet = new byte[bytes];
			serverStream.Read(packet, 0, bytes);
			return packet;
		}
		
		public void AddBytes(ref byte[] packet, ref int offset, byte[] nbytes)
		{
			for (int i = 0; i < nbytes.Length; i++)
			{
				packet[offset] = nbytes[i];
				offset++;
			}
		}
	}
}
