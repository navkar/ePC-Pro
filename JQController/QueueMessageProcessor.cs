/********************************************************************
 * Author: Naveen Karamchetti  
 * Description: The class is responsible for processing the job queue messages.
 * It runs as a separate background thread.
 *******************************************************************/

using System;
using System.Data;
using System.Xml;
using System.Resources;
using System.IO;
using System.Messaging;
using System.Threading;	
using EAppService;

namespace JobQueueController
{
	/// <summary>
	/// Summary description for QueueMessageProcessor.
	/// </summary>
	public class QueueMessageProcessor
	{
		/// <summary>
		/// Holds the new job message.
		/// </summary>
		private string strNewJobMessage = null;

		/// <summary>
		/// Database connection string.
		/// </summary>
		private string strDbConn = null;

		/// <summary>
		/// The constructor of the class.
		/// </summary>
		/// <param name="strDbConn">The DB connection string.</param>
		/// <param name="strNewJobMessage">The New Job Message.</param>
		public QueueMessageProcessor(string strDbConn, string strNewJobMessage)
		{
			this.strDbConn = strDbConn;
			this.strNewJobMessage = strNewJobMessage;
			Thread thrdStartProcessing = new Thread(new ThreadStart(this.process));
			thrdStartProcessing.Name = "QueueMessageProcessor";
			// NOTE: This runs as a background thread.
			thrdStartProcessing.IsBackground = true;
			thrdStartProcessing.Start();
		}

		/// <summary>
		/// Processes the queue messages.
		/// </summary>
		private void process()
		{
			try
			{
				EASDebug.LogException("QMP::New thread started...");
				StringReader sr = new StringReader(this.strNewJobMessage);
				XmlTextReader xtr = new XmlTextReader(sr);
				// Parse the XML data.
				EASXmlReader easXmlReader = new EASXmlReader(xtr);		

				string strEPTHostIp = easXmlReader.getEPTHostIp();
				string strEPTPortNo = easXmlReader.getEPTPortNo();
				string strAreaCode_PrinterId = easXmlReader.getPrinterId();

				EASDebug.LogException("QMP::strEPTHostIp: " + strEPTHostIp );
				EASDebug.LogException("QMP::strEPTPortNo: " + strEPTPortNo);
				EASDebug.LogException("QMP::strAreaCode_PrinterId: " + strAreaCode_PrinterId);
	
				try
				{	
					TCPClient client = new TCPClient(strEPTHostIp, Int32.Parse(strEPTPortNo));
					string strResponse = client.sendMessageToEPT(this.strNewJobMessage);
					EASDebug.LogException("QMP::Response from EPT: " + strResponse);
				}
				catch(Exception e)
				{
					EASDebug.LogException("QMP::TCP Client exception: " +  e.ToString() );
					// Update the job status to 4.
					DBAccess dba = new DBAccess(this.strDbConn);
					dba.updatePrinterStatus(strAreaCode_PrinterId, EASConstants.PRINTER_STATUS_NOTREADY, strEPTHostIp, strEPTPortNo);
					dba.updateJobStatus(strAreaCode_PrinterId, easXmlReader.getJobId(), EASConstants.JOB_STATUS_FAILED );
				}
				
				EASDebug.LogException("QMP:: " + Thread.CurrentThread.Name + " thread execution complete.");
			}
			catch(Exception allExceptions)
			{
				EASDebug.LogException("QMP exception: " +  allExceptions.ToString() );
			}
		}

	}
}
