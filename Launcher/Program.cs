/*
 * Created by SharpDevelop.
 * User: Fabian
 * Date: 26.03.2014
 * Time: 23:47
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
 
using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using Ini;

namespace Launcher
{
	public static class Program
	{
		public static Dictionary<string, string> Ini = new Dictionary<string, string>();
		public static Dictionary<int, ConsoleColor> EventType = new Dictionary<int, ConsoleColor>();
		public static bool Startup = false;
		public static string VersionKey = "000127032014";
		public static string Version = "v0.01";
		//public static APISocket hSocket;
		public static USocket uSocket;
		public const int Success = 0, Failure = 1, Warning = 2, Casual = 3;
		public const int SocketPort = 15100;
		public const int Timeout = 5000; // ms
		public const byte REQ_VERSION = 0x01,
						  REQ_INFO = 0x02;
		
		public static void Main(string[] args)
		{
			SetTitle("Knight OnLine Launcher");
			SetColorRange();
			
			Console.WriteLine(" > Welcome to Knight OnLine Launcher v" + Version + ".");
			SetColor(Casual);
			
			if (LoadIni()) 
			{
				// Ini loaded successfully
				// Print("Successfully loaded 'Server.ini'.");
				SetTitle(Ini["Server_Name"] + " Launcher");
				if (DoConnect())
				{
					if (DoUpdate())
					{
						DoLaunch();
					}
				}
			} 
			else 
			{
				// Ini failed to load
				Print("Corrupted 'Server.ini'.", Failure);
				WriteExampleIni();
				Print("'ServerExample.ini' written. Please, see it for a clear understanding of its structure.");
			}
			
			// Done
			if (!Startup)
			{
				Console.ForegroundColor = ConsoleColor.Gray;
				Console.WriteLine(" > Press any key to exit.");
				Console.ReadKey(true);
			}
		}
		
		public static void SetTitle(string Title)
		{
			Console.Title = Title + " " + Version + " (C) DeathsEffigy - www.snoxd.net";
		}
		
		public static void SetColorRange()
		{
			EventType[Success] = ConsoleColor.Cyan;
			EventType[Failure] = ConsoleColor.Red;
			EventType[Warning] = ConsoleColor.Yellow;
			EventType[Casual] = ConsoleColor.White;
		}
		
		public static void SetColor(int Type)
		{
			if (Type > (EventType.Count - 1)) Type = 0;
			Console.ForegroundColor = EventType[Type];
		}
		
		public static string GetTimestamp()
		{
			return DateTime.Now.ToString("HH:mm:ss");
		}
		
		public static void Print(string Event, int Type = Success)
		{
			Console.Write(GetTimestamp() + " ");
			SetColor(Type);
			Console.Write(Event + "\n");
			SetColor(Casual);
		}
		
		public static bool LoadIni()
		{
			try 
			{
				IniFile File = new IniFile("./server.ini");
				Ini["Server_Name"] = File.IniReadValue("Server", "Name");
				Ini["Server_IP"] = File.IniReadValue("Server", "IP");
				Ini["Update_IP"] = File.IniReadValue("Update", "IP");
				Ini["Version_Client"] = File.IniReadValue("Version", "Client");
			} 
			catch (Exception Ex) 
			{
				throw Ex;
			}
			
			return !String.IsNullOrEmpty(Ini["Server_Name"]) && 
				   !String.IsNullOrEmpty(Ini["Server_IP"]) && 
				   !String.IsNullOrEmpty(Ini["Update_IP"]) &&
				   !String.IsNullOrEmpty(Ini["Version_Client"]);
		}
		
		public static void WriteExampleIni()
		{
			try 
			{
				IniFile File = new IniFile("./ServerExample.ini");
				File.IniWriteValue("Server", "Name", "My KO Server");
				File.IniWriteValue("Server", "IP", "127.0.0.1");
				File.IniWriteValue("Update", "IP", "127.0.0.1");
				File.IniWriteValue("Version", "Client", "1298");
			} 
			catch (Exception Ex) 
			{
				throw Ex;
			}
		}
		
		public static bool DoConnect()
		{

			Print("Connecting, please wait.");
			
			try {
				uSocket = new USocket(Ini["Update_IP"], SocketPort);
				uSocket.Connect();
			} catch (Exception e) {
				Print("Connection timeout. Error: " + e.Message, Failure);
				return false;
			}
			
			return true;
		}
		
		public static bool DoUpdate()
		{
			Print("Checking for updates.");
			
			int offset = 0;
			byte[] packet = new byte[3];
			uSocket.AddBytes(ref packet, ref offset, new byte[]{REQ_VERSION});
			byte[] version = new byte[2];
			//uSocket.AddBytes(ref packet, ref offset, );
			uSocket.Send(packet);
			
			return true;
			
			/*try
			{
				Print("Checking for updates.");
				
				int offset = 0;
				byte[] packet = new byte[3];
				hSocket.AddByte(ref packet, REQ_VERSION, ref offset);
				hSocket.AddShort(ref packet, Convert.ToInt16(Ini["Version_Client"]), ref offset);
				hSocket.Send(packet, 0, packet.Length, Timeout);
				byte[] buffer = new byte[7];
				hSocket.Receive(buffer, 0, buffer.Length, 500);
			}
			catch (Exception Ex)
			{
				Print(Ex.Message);
			}
			
			return false;
			*/
		}
		
		public static bool DoLaunch()
		{
			return false;
		}
	}
}