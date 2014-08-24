/*
 * Created by SharpDevelop.
 * User: Fabian
 * Date: 25.07.2014
 * Time: 16:28
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace DEShield
{
	class Program
	{
		[DllImport("kernel32.dll")]
		static extern IntPtr GetConsoleWindow();
		
		[DllImport("user32.dll")]
		static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
		
		const int SW_HIDE = 0;
		const int SW_SHOW = 5;
		
		public static void Main(string[] args)
		{
			ConsoleHide();
			try {
				Process.Start("./KnightOnLine.exe");
			} catch (Exception e){
				ConsoleShow();
				Console.WriteLine("Could not start Knight OnLine.");
				Console.WriteLine(e.Message);
				Console.ReadKey();
				Environment.Exit(0);
			}
		}
		
		public static void ConsoleHide(){
			var handle = GetConsoleWindow();
			ShowWindow(handle, SW_HIDE);
		}
		
		public static void ConsoleShow(){
			var handle = GetConsoleWindow();
			ShowWindow(handle, SW_SHOW);
		}
	}
}