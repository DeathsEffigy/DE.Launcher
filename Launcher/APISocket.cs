/*
 * Created by SharpDevelop.
 * User: Fabian
 * Date: 27.03.2014
 * Time: 16:11
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Linq;

namespace Launcher
{
	/// <summary>
	/// Description of APISocket.
	/// </summary>
	public class APISocket
	{
		public Socket Socket;
		private ManualResetEvent TimeoutObject = new ManualResetEvent(false);
		private Exception SocketException;
		private bool IsConnected = false;
		public const ushort PACKET_HEADER = 0xAA55,
					        PACKET_TAIL = 0x55AA;
		
		public APISocket()
		{
			
		}
		
		public bool Connect(string ServerIP, int ServerPort, int Timeout)
		{
			SocketException = null;
			
			try
			{
				Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
				IPEndPoint ipendpoint = new IPEndPoint(IPAddress.Parse(ServerIP), ServerPort);
				Socket.Connect(ipendpoint);
			}
			catch (SocketException Ex)
			{
				SocketException = Ex;
				return IsConnected = false;
			}
			
			return IsConnected = true;
		}
		
		/*public bool Connect(string ServerIP, int ServerPort, int Timeout)
		{
			TimeoutObject.Reset();
			SocketException = null;
			try
			{
				Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
				IPEndPoint IPEndPoint = new IPEndPoint(IPAddress.Parse(ServerIP), ServerPort);
				object state = new object();
				Socket.BeginConnect(IPEndPoint, new AsyncCallback(CallBackMethod), state);
				
				if (TimeoutObject.WaitOne(Timeout, false))
				{
					if (IsConnected)
						return true;
					else
						return false;
				}
				else
				{
					return false;
				}
					
			}
			catch (Exception Ex)
			{
				SocketException = Ex;
			}
			
			return false;
		}
		
		private void CallBackMethod(IAsyncResult AsyncResult)
		{
			try
			{
				IsConnected = false;
				
				if (Socket.Connected)
				{
					IsConnected = true;
				}
			}
			catch (Exception Ex)
			{
				IsConnected = false;
				SocketException = Ex;
			}
			finally
			{
				TimeoutObject.Set();
			}
		}
		*/
		
		public Exception GetException()
		{
			return SocketException;
		}
		
		public void AddByte(ref byte[] structure, byte nbyte, ref int offset)
		{
			structure[offset] = nbyte;
			offset++;
		}
		
		public void AddBytes(ref byte[] structure, byte[] nbytes, ref int offset)
		{
			for (int i = 0; i < nbytes.Length; i++)
			{
				structure[offset] = nbytes[i];
				offset++;
			}
		}
		
		public void AddShort(ref byte[] structure, short nshort, ref int offset)
		{
			byte[] hshort = BitConverter.GetBytes(nshort);
			structure[offset] = hshort[0];
			structure[offset+1] = hshort[1];
			offset += 2;
		}
		
		public void AddUShort(ref byte[] structure, ushort nushort, ref int offset)
		{
			byte[] hushort = BitConverter.GetBytes(nushort);
			structure[offset] = hushort[0];
			structure[offset+1] = hushort[1];
			offset += 2;
		}
		
		public void Send(byte[] packet, int offset, int size, int timeout)
		{
			
			int hoffset = 0;
			byte[] buffer = new byte[packet.Length + 4];
			AddUShort(ref buffer, PACKET_HEADER, ref hoffset);
			AddBytes(ref buffer, packet, ref hoffset);
			AddUShort(ref buffer, PACKET_TAIL, ref hoffset);
			
			SocketException = null;
			int startTick = Environment.TickCount;
			int sent = 0;
			
			do
			{
				if (Environment.TickCount > startTick + timeout)
				{
					SocketException = new Exception("Timeout.");
					return;
				}
				
				try
				{
					sent += Socket.Send(buffer, offset + sent, size - sent, SocketFlags.None);
				}
				catch (SocketException Ex)
				{
					if (Ex.SocketErrorCode == SocketError.WouldBlock ||
					    Ex.SocketErrorCode == SocketError.IOPending ||
					    Ex.SocketErrorCode == SocketError.NoBufferSpaceAvailable)
					{
						Thread.Sleep(30);
					}
					else
					{
						SocketException = Ex;
						return;
					}
				}
			} while (sent < size);
		}
		
		public void Receive(byte[] buffer, int offset, int size, int timeout)
		{
			SocketException = null;
			int startTick = Environment.TickCount;
			int received = 0;
			
			do
			{
				if (Environment.TickCount > startTick + timeout)
				{
					SocketException = new Exception("Timeout.");
					return;
				}
				
				try
				{
					received += Socket.Receive(buffer, offset + received, size - received, SocketFlags.None);
				}
				catch (SocketException Ex)
				{
					if (Ex.SocketErrorCode == SocketError.WouldBlock ||
					    Ex.SocketErrorCode == SocketError.IOPending ||
					    Ex.SocketErrorCode == SocketError.NoBufferSpaceAvailable)
					{
						Thread.Sleep(30);
					}
					else
					{
						SocketException = Ex;
						return;
					}
				}
			} while (received < size);
		}
	}
}
