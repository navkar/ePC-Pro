/********************************************************************
 * Author: Naveen Karamchetti  
 * Description: Reads the config file.
 ********************************************************************/

using System;
using System.IO;
using System.Collections;

namespace EAppService
{
	/// <summary>
	/// Summary description for ReadConfigFile.
	/// </summary>
	public class ReadConfigFile
	{
		/// <summary>
		/// The config file name.
		/// </summary>
		private string strFileName = null;
		
		/// <summary>
		/// The constructor of the class.
		/// </summary>
		/// <param name="strFileName">The file name</param>
		public ReadConfigFile(string strFileName)
		{
			this.strFileName = strFileName;
		}

		/// <summary>
		/// Get the hashtable config data.
		/// </summary>
		/// <returns>The Hashtable containing the key-value pairs.</returns>
		public Hashtable GetConfigData()
		{
			Hashtable htConfig = new Hashtable();

			try
			{
				FileStream fs = new FileStream(strFileName, FileMode.Open, FileAccess.Read);
				// Create a Char reader.
				StreamReader sr = new StreamReader(fs);        
				// Set the file pointer to the beginning.
				sr.BaseStream.Seek(0, SeekOrigin.Begin);   
				while (sr.Peek() > -1) 
				{
					string strLine = sr.ReadLine();
					int equalIndex = strLine.IndexOf("=");

					if ( equalIndex != -1)
					{
						string strName = strLine.Substring(0,equalIndex);
						string strValue = strLine.Substring(equalIndex + 1);

						if ( strName!= null && strValue != null)
						{
							htConfig.Add(strName.Trim() ,strValue.Trim() );
						}
					}

				}
				return htConfig;
			}
			catch(Exception e)
			{
				EASDebug.LogException("ReadConfigFile:"	+ e.Message);
				return null;
			}	
	
		}
	}
}
