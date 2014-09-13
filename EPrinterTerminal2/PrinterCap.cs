/********************************************************************
 * Author: Naveen Karamchetti  
 * Description: Printer Capabilities Data object.
 ********************************************************************/

using System;

namespace EPrinterTerminal
{
	/// <summary>
	/// Summary description for PrinterCap.
	/// </summary>
	public class PrinterCap
	{
		private string strAreaCode = null;
		private string strEptHostIp = null;
		private string strEptPortNo = null;
		private string strManufacturer = null;
		private string strModel = null;
		private string strType = null;
		private string strColor = null;
		private string strDuplex = null;
		private PaperInfo[] paperInfo = null;

		/// <summary>
		/// Returns the area code.
		/// </summary>
		/// <returns>AreaCode</returns>
		public string getAreaCode()
		{
			return this.strAreaCode;
		}

		/// <summary>
		/// returns the Host IP Address.
		/// </summary>
		/// <returns>IP Address</returns>
		public string getEptHostIp()
		{
			return this.strEptHostIp;
		}

		/// <summary>
		/// Returns the EPT Port No.
		/// </summary>
		/// <returns>Port No</returns>
		public string getEptPortNo()
		{
			return this.strEptPortNo;
		}

		/// <summary>
		/// Returns the manufacturer name.
		/// </summary>
		/// <returns>Manufacturer</returns>
		public string getManufacturer()
		{
				return this.strManufacturer;
		}

		/// <summary>
		/// Returns the printer name.
		/// </summary>
		/// <returns>Printer Name</returns>
		public string getModel()
		{
				return this.strModel;
		}

		/// <summary>
		/// Return the printer type.
		/// </summary>
		/// <returns>Printer type</returns>
		public string getType()
		{
				return this.strType;
		}

		/// <summary>
		/// Return the printer color code.
		/// </summary>
		/// <returns>Color code</returns>
		public string getColor()
		{
			return this.strColor;
		}

		/// <summary>
		/// Returns the printer duplex code.
		/// </summary>
		/// <returns>Duplex code.</returns>
		public string getDuplex()
		{
			return this.strDuplex;
		}
		
		/// <summary>
		/// Returns the paperinfo data object array.
		/// </summary>
		/// <returns>Array of PaperInfo objects.</returns>
		public PaperInfo[] getPaperInfo()
		{
			return this.paperInfo;
		}

		/// <summary>
		/// Constructor of the data class.
		/// </summary>
		/// <param name="strAreaCode">AreaCode</param>
		/// <param name="strEptHostIp">IP Address.</param>
		/// <param name="strEptPortNo">Port No</param>
		/// <param name="strManufacturer">Manufacturer</param>
		/// <param name="strModel">Printer Name</param>
		/// <param name="strType">Printer Type</param>
		/// <param name="strColor">Printer Color</param>
		/// <param name="strDuplex">Printer Duplex</param>
		/// <param name="paperInfo">Paper Information array.</param>
		public PrinterCap(string strAreaCode,
							string strEptHostIp,
							string strEptPortNo,
							string strManufacturer,
							string strModel,
							string strType,
							string strColor,
							string strDuplex,
							PaperInfo[] paperInfo)
			
		{
			this.strAreaCode = strAreaCode;
			this.strEptHostIp = strEptHostIp;
			this.strEptPortNo = strEptPortNo; 
			this.strManufacturer = strManufacturer;
			this.strModel = strModel;
			this.strType = strType;
			this.strColor = strColor;
			this.strDuplex = strDuplex;
			this.paperInfo = paperInfo;
		}
	}
}
