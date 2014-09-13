/********************************************************************
 * Author: Naveen Karamchetti  
 * Description: This class generates EPT XML responses.
 ********************************************************************/

using System;
using System.Text;

namespace EPrinterTerminal
{
	/// <summary>
	/// Generates Generic responses for error handling.
	/// </summary>
	public class EPTXmlResponse
	{
		public EPTXmlResponse()
		{
		}

		/// <summary>
		/// Get the generic response used for error handling.
		/// </summary>
		/// <param name="strReturnCode">The Return Code (RC)</param>
		/// <param name="strRequest">The Request type.</param>
		/// <param name="strPrinterId">The Printer Id.</param>
		/// <returns></returns>
		public static string GetResponse(string strReturnCode, 
											string strRequest, 
											string strPrinterId)
		{
			StringBuilder sb = new StringBuilder();

			sb.Append("<?xml version=\"1.0\" encoding=\"ISO-8859-1\" standalone=\"yes\"?>");
			sb.Append(EPTConstants.CRLF);
	
			sb.Append("<" + EPTConstants.XT_PRINTERTERMINAL + ">");
			sb.Append(EPTConstants.CRLF);

			sb.Append("<" + EPTConstants.XT_RETURNCODE + ">");
			sb.Append(strReturnCode);
			sb.Append("</" + EPTConstants.XT_RETURNCODE + ">");
			sb.Append(EPTConstants.CRLF);

			sb.Append("<" + EPTConstants.XT_REQUEST + ">");
			sb.Append(strRequest);
			sb.Append("</" + EPTConstants.XT_REQUEST + ">");
			sb.Append(EPTConstants.CRLF);

			sb.Append("<" + EPTConstants.XT_PRINTERID + ">");
			sb.Append(strPrinterId);
			sb.Append("</" + EPTConstants.XT_PRINTERID + ">");
			sb.Append(EPTConstants.CRLF);

			sb.Append("</" + EPTConstants.XT_PRINTERTERMINAL + ">");
			sb.Append(EPTConstants.CRLF);

			return sb.ToString();
		}

	}
}
