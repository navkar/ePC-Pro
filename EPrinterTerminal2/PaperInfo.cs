/********************************************************************
 * Author: Naveen Karamchetti  
 * Description: Paper Information Data Class.
 ********************************************************************/

using System;

namespace EPrinterTerminal
{
	/// <summary>
	/// Summary description for PaperInfo.
	/// </summary>
	public class PaperInfo
	{
		/// <summary>
		/// Paper size
		/// </summary>
		private string strSize = null;

		/// <summary>
		/// Paper width
		/// </summary>
		private string strWidth = null;

		/// <summary>
		/// Paper Height
		/// </summary>
		private string strHeight = null;

		/// <summary>
		/// Returns the paper size.
		/// </summary>
		/// <returns></returns>
		public string getSize()
		{
			return this.strSize;
		}
	
		/// <summary>
		/// Returns the paper width
		/// </summary>
		/// <returns></returns>
		public string getWidth()
		{
			return this.strWidth;
		}

		/// <summary>
		/// Returns the paper height
		/// </summary>
		/// <returns></returns>
		public string getHeight()
		{
			return this.strHeight;
		}

		/// <summary>
		/// Paper Information data class constructor.
		/// </summary>
		/// <param name="strSize">Size</param>
		/// <param name="strWidth">Width</param>
		/// <param name="strHeight">Height</param>
		public PaperInfo(string strSize, string strWidth, string strHeight)
		{
			this.strSize = strSize;
			this.strWidth = strWidth;
			this.strHeight = strHeight;
		}

		/// <summary>
		/// String representation of the object.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return "strSize:" + strSize + "\nstrWidth:"+ strWidth + "\nstrHeight: " + strHeight;
		}

	}
}
