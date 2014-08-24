/*
 * Created by SharpDevelop.
 * User: Fabian
 * Date: 16.07.2014
 * Time: 16:39
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Xml;
using System.Text;

namespace UServer
{
	/// <summary>
	/// Description of UpdateManager.
	/// </summary>
	public class UpdateManager
	{
		public static Dictionary<string, string> Ini = new Dictionary<string, string>();
		private static XmlReader Reader;
		
		public UpdateManager()
		{
			
		}
		
		public void SetIni(Dictionary<string, string> newIni) {
			Ini = newIni;
		}
		
		public bool ReloadUpdates() {
			try {
				Reader = XmlReader.Create("./updates.xml");
				while (Reader.Read()) {}
			} catch (Exception e) {
				return false;
			} finally {
				Reader.Close();
			}
			return true;
		}
	}
}
