/********************************************************************
 * Author: Naveen Karamchetti  
 * Description: This class is a helper class that is used for debugging 
 * this application. It is used to log information flowing across 
 * various modules.
 ********************************************************************/

using System;
using System.IO;

namespace EPrinterTerminal
{
	/// <summary>
	/// Summary description for EPCDebug.
	/// </summary>
	public class EPTDebug
	{
		/// <summary>
		/// The EPC Log File Name.
		/// </summary>
		private static string strLogFileName = "ept.log";

		/// <summary>
		/// Log Flag - false by default.
		/// </summary>
		public static bool bLogFlag = false;
		
		/// <summary>
		/// Constructor.
		/// </summary>
		public EPTDebug()
		{
			// Nothing to do.
		}

		/// <summary>
		/// If this flag is set to true, then messages are not logged.
		/// </summary>
		/// <param name="bFlag">The Log Flag.</param>
		public static void setLogFlag(bool bFlag)
		{
			bLogFlag = bFlag;
		}

		/// <summary>
		/// Sets the log file name.
		/// </summary>
		/// <param name="strFileName"></param>
		public static void setLogFileName(string strFileName)
		{
			strLogFileName = strFileName;
		}

		/// <summary>
		/// Logs the exceptions into a log file.
		/// </summary>
		/// <param name="strData">The message to be logged.</param>
		public static void LogException(string strData)
		{
			if (!bLogFlag)
			{
				lock( typeof(string))
				{
					FileStream fs = null;
					try
					{
						fs = new FileStream(strLogFileName, FileMode.Append,FileAccess.Write);
						StreamWriter sw = new StreamWriter(fs);
						sw.WriteLine(System.DateTime.Now + " @ " + strData);
						sw.Close();
					}
					catch(IOException e)
					{
						Console.WriteLine(e.ToString() );
					}
					finally
					{
						if ( fs != null)
						{
							fs.Close();
						}
					}
				}
			}
		}


	}
}
