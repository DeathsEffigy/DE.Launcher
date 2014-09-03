/*
 * Created by SharpDevelop.
 * User: Fabian
 * Date: 24.08.2014
 * Time: 16:44
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

#define DEBUG
 
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Threading;
using Ini;

namespace DELauncher
{
	
	
	/// <summary>
	/// Interaction logic for Window1.xaml
	/// </summary>
	public partial class Window1 : Window
	{
		private bool MouseHold = false;
		private bool isConnected = false;
		public static Dictionary<string, string> Ini = new Dictionary<string, string>();
		public static string Version = "0.01";
		public static USocket uSocket;
		public const int SocketPort = 15101;
		public const byte REQ_VERSION = 0x01,
						  REQ_INFO = 0x02;
		private Thread sThread;
		
		public Window1()
		{
			InitializeComponent();
			SetTitle("Knight OnLine Launcher");
		}
		
		private void AfterRender(object sender, EventArgs e)
		{
			try
			{
				LoadIni();
				
				sThread = new Thread(InThread);
				sThread.IsBackground = true;
				sThread.Start();
			} catch (Exception ex)
			{
				#if (DEBUG)
				throw ex;
				#endif
				Environment.Exit(0);
			}
		}
		
		private void InThread()
		{
			DoConnect();
			DoUpdate();
		}
		
		#region Misc
		private void Exit(object sender, RoutedEventArgs e)
		{
			Environment.Exit(0);
		}
		
		private void MouseDown(object sender, RoutedEventArgs e)
		{
			MouseHold = true;
		}
		
		private void MouseUp(object sender, RoutedEventArgs e)
		{
			MouseHold = false;
		}
		
		private void MouseMove(object sender, RoutedEventArgs e)
		{
			if (MouseHold)
			{
				try
				{
					this.DragMove();
				}
				catch (Exception ex)
				{
					
				}
			}
		}
		
		public void SetTitle(string Title)
		{
			LauncherTitle.Content = Title + " - v" + Version + " (C) DeathsEffigy - www.snoxd.net";
		}
		
		public void SetAction(string Action)
		{
			this.Dispatcher.Invoke((Action)(() =>
    			{
        			PatchActivity.Content = Action;
    			}));
		}
		
		private void WriteIni()
		{
			IniFile File = new IniFile("./server.example.ini");
			File.IniWriteValue("Server", "Name", "My KO Server");
			File.IniWriteValue("Server", "IP", "127.0.0.1");
			File.IniWriteValue("Update", "IP", "127.0.0.1");
			File.IniWriteValue("Version", "Client", "1298");
		}
		#endregion
		
		#region Worker
		private void LoadIni()
		{
			SetAction("Initializing...");
			IniFile File = new IniFile("./server.ini");
			Ini["Server_Name"] = File.IniReadValue("Server", "Name");
			Ini["Server_IP"] = File.IniReadValue("Server", "IP");
			Ini["Update_IP"] = File.IniReadValue("Update", "IP");
			Ini["Version_Client"] = File.IniReadValue("Version", "Client");
			
			if (String.IsNullOrEmpty(Ini["Server_Name"]) || 
			    String.IsNullOrEmpty(Ini["Server_IP"]) || 
				String.IsNullOrEmpty(Ini["Update_IP"]) ||
				String.IsNullOrEmpty(Ini["Version_Client"]))
			{
				WriteIni();
				System.Windows.MessageBox.Show("Corrupted server.ini file.", "Oops");
				Environment.Exit(0);
			}
			
			SetTitle(Ini["Server_Name"]);
		}
		
		private void DoConnect()
		{
			SetAction("Connecting...");
			
			try
			{
				isConnected = true;
				uSocket = new USocket(Ini["Update_IP"], SocketPort);
				uSocket.Connect();
			}
			catch (Exception ex)
			{
				isConnected = false;
				SetAction("Connection failed; " + ex.Message);
			}
		}
		
		private void DoUpdate()
		{
			if (isConnected)
			{
				SetAction("Checking for updates...");
			}
		}
		#endregion
	}
}