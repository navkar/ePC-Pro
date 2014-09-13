using System;
using System.IO;
using System.Drawing.Printing;
using System.Drawing;

namespace PrintConsoleApp
{
	/// <summary>
	/// Summary description for Class1.
	/// </summary>
	class Class1
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main(string[] args)
		{
			StringReader streamToPrint = new StringReader ("This is a sample test");
			TextPrintDocument pd = new TextPrintDocument(streamToPrint, new Font("Verdana",9));
			pd.Print();
		}
	}
}
