/*
 * Created by SharpDevelop.
 * User: Fabian
 * Date: 15.07.2014
 * Time: 19:19
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Text;
using System.Linq;
using System.IO;
using Ini;
using System.Xml;
using System.Security.Cryptography;
using System.Threading;

namespace UServer
{
	/// <summary>
	/// Description of MainForm.
	/// </summary>
	public partial class MainForm : Form
	{
		public static Dictionary<string, string> Ini = new Dictionary<string, string>();
		public static bool startUp = false;
		public UpdateManager updateManager;
		public MultiSocket Server;
		public static MainForm Instance = null;
		private int DebugCount = 0;
		
		public MainForm()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			Instance = this;
			
			//
			// TODO: Add constructor code after the InitializeComponent() call.
			//
			
			this.FormBorderStyle = FormBorderStyle.FixedSingle;
			Print("Initializing...");
			
			if (LoadIni()) {
				Print("Loading updates...");
				updateManager = new UpdateManager();
				updateManager.SetIni(Ini);
				if (!updateManager.ReloadUpdates()) {
					Print("Failed to load XML Updates.");
				}
			} else {
				Print("Failed to load ini file. Writing './config.ini'...");
				WriteDefaultIni();
				Print("Done...if no errors appeared.");
			}
		}
		
		
		#region RandomFunctions
		public string GetTimestamp() {
			return DateTime.Now.ToString("HH:mm:ss");
		}
		
		public void Print(string str) {
			richTextBox1.Text += this.GetTimestamp() + " - " + str + "\n";
		}
		
		public void Debug(string str) {
			if (Convert.ToInt32(Ini["log_level"]) == 1) {
				if (DebugCount > 100) {
					DebugCount = 0;
					richTextBox2.Text = "";
				}
				DebugCount++;
				richTextBox2.Text += this.GetTimestamp() + " - " + str + "\n";
			}
		}
		
		private bool LoadIni() {
			try {
				IniFile File = new IniFile("./config.ini");
				Ini.Add("default_port", File.IniReadValue("server", "default_port"));
				Ini.Add("current_version", File.IniReadValue("server", "current_version"));
				
				Ini.Add("auto_zip", File.IniReadValue("behavior", "auto_zip"));
				Ini.Add("max_diff", File.IniReadValue("behavior", "max_diff"));
				Ini.Add("log_level", File.IniReadValue("behavior", "log_level"));
			} catch (Exception e) {
				return false;
			}
			
			return
				!String.IsNullOrEmpty(Ini["default_port"]) &&
				!String.IsNullOrEmpty(Ini["auto_zip"]) &&
				!String.IsNullOrEmpty(Ini["max_diff"]) &&
				!String.IsNullOrEmpty(Ini["current_version"]) &&
				!String.IsNullOrEmpty(Ini["log_level"]);
		}
		
		private void WriteDefaultIni() {
			try {
				IniFile File = new IniFile("./config.ini");
				
				File.IniWriteValue("server", "default_port", "15101");
				File.IniWriteValue("server", "current_version", "1298");
				
				File.IniWriteValue("behavior", "auto_zip", "1");
				File.IniWriteValue("behavior", "max_diff", "0");
				File.IniWriteValue("behavior", "log_level", "0");
			} catch (Exception e) {
				Print("Failed writing example ini file.");
			}
		}
		
		private void Enable() {
			textBox1.Enabled = true;
			textBox2.Enabled = true;
			button1.Enabled = true;
			label8.SendToBack();
		}
		
		private void Loading() {
			textBox1.Visible = false;
			textBox2.Visible = false;
			button1.Visible = false;
			label3.Visible = false;
			label4.Visible = false;
			label5.Visible = false;
			label8.BringToFront();
			label8.Text = "Processing...";
		}
		
		private void Loaded() {
			textBox1.Visible = true;
			textBox2.Visible = true;
			button1.Visible = true;
			label3.Visible = true;
			label4.Visible = true;
			label5.Visible = true;
			label8.SendToBack();
		}
		
		private string GetMD5FromFile(string filename) {
			using (var md5 = MD5.Create())
			{
			    using (var stream = File.OpenRead(filename))
			    {
			        return BitConverter.ToString(md5.ComputeHash(stream)).Replace("-","").ToLower();
			    }
			}
		}
		#endregion
		
		#region Interactions
		void TextBox1Click(object sender, EventArgs e)
		{
			DialogResult Folder = folderBrowserDialog1.ShowDialog();
			
			if (Folder == DialogResult.OK) {
				textBox1.Text = folderBrowserDialog1.SelectedPath;
				Environment.SpecialFolder root = folderBrowserDialog1.RootFolder;
			}
		}
		
		void Button1Click(object sender, EventArgs e)
		{
			string SourcePath = textBox1.Text;
			string DestinationPath = "./updates";
			Loading();
			
			if (!String.IsNullOrEmpty(textBox1.Text) && !String.IsNullOrWhiteSpace(textBox1.Text) && Directory.Exists(textBox1.Text)) {
				try {
					foreach (string dirPath in Directory.GetDirectories(SourcePath, "*",
					                                                    SearchOption.AllDirectories))
				    	Directory.CreateDirectory(dirPath.Replace(SourcePath, DestinationPath));
					
					int ver = Convert.ToInt16(Ini["current_version"]);
					ver++;
					Ini["current_version"] = ver.ToString();
					XmlDocument Updates = new XmlDocument();
					Updates.Load("./updates.xml");
					XmlNode Update = Updates.CreateElement("update");
					XmlNode NewVersion = Updates.CreateElement("new_version");
					XmlNode ReleasedFiles = Updates.CreateElement("released_files");
					XmlNode RCD = Updates.CreateElement("release_date");
					NewVersion.InnerText = Ini["current_version"];
					RCD.InnerText = DateTime.Now.ToString();
					Update.AppendChild(NewVersion);
					Update.AppendChild(ReleasedFiles);
					Update.AppendChild(RCD);
					
					foreach (string newPath in Directory.GetFiles(SourcePath, "*.*", 
					                                              SearchOption.AllDirectories)) {
						string abP = newPath.Replace(SourcePath, DestinationPath).Replace("\\", "/");
					    File.Copy(newPath, abP, true);
					    XmlNode RCF = Updates.CreateElement("file");
					    XmlNode FPath = Updates.CreateElement("path");
					    XmlNode FSum = Updates.CreateElement("checksum");
					    ReleasedFiles.AppendChild(RCF);
					    RCF.AppendChild(FPath);
					    RCF.AppendChild(FSum);
					    FPath.InnerText = abP;
					    FSum.InnerText = GetMD5FromFile(abP);
					}
					
					Updates.DocumentElement.AppendChild(Update);
					Updates.Save("./updates.xml");
					
					Print("Added Update " + Ini["current_version"] + " from " + SourcePath + ".");
				} catch (Exception ex) {
					Print("Patch could not be released: " + ex.Message);
				} finally {
					textBox1.Text = "";
					textBox2.Text = "";
				}
			}
			
			Loaded();
		}
		#endregion
		
		#region Socket
		public void StartSocket()
		{
			Server = new MultiSocket(Convert.ToInt32(Ini["default_port"]));
			AppDomain.CurrentDomain.ProcessExit += new EventHandler(onExit);
			Server.Listen();
		}
		
		public void onExit(object sender, EventArgs e) {
			Server.StopListen();
			Application.Exit();
		}
		
		void MainFormShown(object sender, EventArgs e)
		{
			bool success = true;
			Print("Creating threaded socket...");
			try {
				Thread sThread = new Thread(StartSocket);
				sThread.IsBackground = true;
				sThread.Start();
			} catch (Exception Ex) {
				success = false;
				Print("Could not listen. Please, check your settings and/or environment; " + Ex.Message);
			} finally {
				if (success) {
					Print("Now listening on port " + Ini["default_port"] + "...");
					Print("Success! :)");
					Loaded();
					Enable();
					if (Convert.ToInt32(Ini["log_level"]) == 0) {
						richTextBox2.Text = "Debugging is deactivated.\nDo not enable it unless you absolutely have to (and aren't running a live-server).";
					} else {
						Debug("Debugging activated...");
					}
				}
			}
		}
		#endregion
	}
}
