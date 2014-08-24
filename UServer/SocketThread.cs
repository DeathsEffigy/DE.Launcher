/*
 * Created by SharpDevelop.
 * User: Fabian
 * Date: 11.08.2014
 * Time: 17:31
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Threading;
using System.Net.Sockets;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace UServer
{
	/// <summary>
	/// Description of SocketThread.
	/// </summary>
	public class SocketThread : Form
	{
		TcpClient clientSocket;
		string clNo;
		public bool activated = false;
		Form App;
		Thread cThread;
		
		public SocketThread()
		{
			App = Application.OpenForms[0];
		}
		
		public void startClient(TcpClient inClientSocket, string clientNo)
		{
			this.clientSocket = inClientSocket;
			this.clNo = clientNo;
			activated = true;
			cThread = new Thread(HandleRequest);
			cThread.IsBackground = true;
			cThread.Start();
		}
		
		private void HandleRequest()
		{
			byte[] packet = new byte[32768];
			NetworkStream networkStream = clientSocket.GetStream();
			
			while (activated == true)
			{
				if (networkStream.CanRead && networkStream.DataAvailable)
				{
					try
					{
						do
						{
							int read = networkStream.Read(packet, 0, packet.Length);
						} while (networkStream.DataAvailable);
						VerifyRequest(packet);
						
						packet = new byte[packet.Length];
					}
					catch (Exception e)
					{
						MessageBox.Show(e.Message);
					}
				}
			}
		}
			
		private void VerifyRequest(byte[] packet)
		{
			switch (packet[0])
			{
					case 0x02: {
						//MessageBox.Show("Patch this shit!");
						//MainForm.Instance.Debug("Patch");
					} break;
					case 0x01: {
						MainForm.Instance.Debug("0x01 - ");
					} break;
			}
		}
	}
}
