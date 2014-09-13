/********************************************************************
 * Author: Naveen Karamchetti  
 * Description: This class is responsible for printing the jobs.
 ********************************************************************/

using System;
using System.Threading;
using System.IO;
using System.Drawing;
using System.Text;
using System.Diagnostics;

namespace EPrinterTerminal
{
	/// <summary>
	/// Manages and Prints the Jobs.
	/// Printing a Job is the core functionality.
	/// Runs on a separate thread.
	/// </summary>
	public class PrintJobManager
	{
		/// <summary>
		/// The EAS Host IP address.
		/// </summary>
		private string strHostIp = null;

		/// <summary>
		/// The EAS Port No.
		/// </summary>
		private int iPort = 0;

		/// <summary>
		/// The local path where the EPT application runs.
		/// </summary>
		private string strAppPath = null;

		/// <summary>
		/// The Printer ID = AreaCode + Unique number
		/// </summary>
		private string strAreaCode_PrinterId = null;

		/// <summary>
		/// The EPT Host IP Adress.
		/// </summary>
		private string strEPTHostIp = null;

		/// <summary>
		/// The EPT Port No.
		/// </summary>
		private string strEPTPortNo = null;

		/// <summary>
		/// The Print Job Manager Thread.
		/// </summary>
		private Thread thrdPJobMgr = null;

		/// <summary>
		/// The FrmEPrinterTerminal form.
		/// </summary>
		private FrmEPrinterTerminal parentForm = null;
		
		/// <summary>
		/// The Constructor of the class Printer Job Manager.
		/// </summary>
		/// <param name="parentForm">The FrmEPrinterTerminal form.</param>
		/// <param name="strHostIp">The Host IP address.</param>
		/// <param name="iPort">The Port No.</param>
		/// <param name="strAppPath">The local path where the EPT application runs.</param>
		/// <param name="strAreaCode_PrinterId">The printer id.</param>
		/// <param name="strEPTHostIp">The EPT Host IP Address.</param>
		/// <param name="strEPTPortNo">The EPT Port No.</param>
		public PrintJobManager(
								FrmEPrinterTerminal parentForm,
								string strHostIp, 
								int iPort, 
								string strAppPath,
								string strAreaCode_PrinterId,
								string strEPTHostIp,
								string strEPTPortNo)
		{
			try
			{
				this.parentForm = parentForm;
				this.strHostIp = strHostIp;
				this.iPort = iPort;
				this.strAppPath = strAppPath;
				this.strAreaCode_PrinterId = strAreaCode_PrinterId;
				this.strEPTHostIp = strEPTHostIp;
				this.strEPTPortNo = strEPTPortNo;

				thrdPJobMgr = new Thread( new ThreadStart(this.run));
				// Background threads do not prevent a process from terminating.
				// Set this to TRUE in production.
				thrdPJobMgr.IsBackground = true;
				thrdPJobMgr.Start();
				EPTDebug.LogException("Print Job Manager thread started");
			}
			catch(Exception e)
			{
				EPTDebug.LogException("PrintJobManager::"+ e.ToString() );
			}		
		}

		/// <summary>
		/// Sets the EAS Host IP Address.
		/// </summary>
		/// <param name="strHostIp">IP address.</param>
		public void setHostIp(string strHostIp)
		{
			this.strHostIp = strHostIp;
		}

		/// <summary>
		/// Sets the EAS Host Port No.
		/// </summary>
		/// <param name="iPort">Port No.</param>
		public void setHostPortNo(int iPort)
		{
			this.iPort = iPort;
		}

		/// <summary>
		/// The local path where the EPT application runs.
		/// </summary>
		/// <param name="strAppPath">The path url</param>
		public void setAppPath(string strAppPath)
		{
			this.strAppPath = strAppPath;
		}

		/// <summary>
		/// Called by the Job Info Manager when there are jobs to be printed.
		/// </summary>
		public void startProcessingJobs()
		{
			EPTDebug.LogException("PrintJobManager::startProcessingJobs> Thread state:" + thrdPJobMgr.ThreadState);
			if ( thrdPJobMgr != null )
			{
				try
				{
						EPTDebug.LogException("PrintJobManager::startProcessingJobs: Resuming thread...");
						thrdPJobMgr.Resume();
				}
				catch(Exception thrdException)
				{
					EPTDebug.LogException("PrintJobManager::"+ thrdException.ToString() );
				}
			}
		}

		/// <summary>
		/// Stops the Print Job Manager by stopping the thread.
		/// </summary>
		public void stopPrintJobManager()
		{
			if ( thrdPJobMgr != null)
			{
				try
				{
					EPTDebug.LogException("PrintJobManager::stopPrintJobManager: Aborting thread...");
					// Terminates the thread. => Thread dies.
					thrdPJobMgr.Abort();
				}
				catch(Exception thrdException)
				{
					EPTDebug.LogException("PrintJobManager::"+ thrdException.ToString() );
				}
			}
		}

		/// <summary>
		/// Loops forever looking for Print Jobs, if there are no jobs it suspends itself.
		/// </summary>
		private void run()
		{
			while(true)
			{
				try
				{
					ConnectionManager cm = new ConnectionManager(this.strHostIp, this.iPort);

					if ( JobInfoManager.Instance.getJobCount() > 0)
					{
						// Getting the Job Information from the JobInfoManager.
						JobInfo jobInfo = (JobInfo) JobInfoManager.Instance.getJobInfo();
						
						string strPrinterId = jobInfo.getAreaCode_PrinterId();
						string strJobId = jobInfo.getJobId();
						string strFileName = jobInfo.getFileName();
						int iFileSize = Int32.Parse(jobInfo.getFileSize());
						int iCopies = Int32.Parse( jobInfo.getJobCopies() );
						
						// Job File Storage Path is formed here...
						string strPath = strAppPath + "\\" + EPTConstants.JOB_FILES_DIR + "\\" + strJobId;
						
						// Form the Get Job File request.
						string strGJFRequest 
							= EPTXmlRequest.GetGJFRequest(strPrinterId, strJobId, strFileName);

						try
						{
							EPTDebug.LogException("PrintJobManager::Sending GJF request...");
							// Throws JOB_CANCELLED, NOT_CONNECTED, JOB_DOWNLOAD_ERROR
							cm.sendGetJobFileRequest(iFileSize, strPath, strFileName, strGJFRequest);

							// Add the Job Information to the List box.
							parentForm.addJobDataToListView(strFileName, jobInfo.getJobCost(), jobInfo.getJobCopies() );
						
							EPTDebug.LogException("File Print Path: " + strPath + "\\" + strFileName);
							// 1. Try to read the file, if it is a XML file then do not print.
							// 2. If not print it.
							FileInfo jobFileInfo = new FileInfo(strFileName);
							
							if ( jobFileInfo.Extension.Equals(".txt") )
							{
								StreamReader streamToPrint 
									= new StreamReader(strPath + "\\" + strFileName, Encoding.ASCII);

								StringReader sr = new StringReader(streamToPrint.ReadToEnd());

								TextPrintDocument tpd = new TextPrintDocument(sr, new Font("Verdana",9));

								tpd.PrinterSettings.Copies = (short) iCopies;
								tpd.Print();
							}
							else if ( jobFileInfo.Extension.Equals(".pdf") )
							{
								Process.Start("PrintPDF.exe", strPath + "\\" + strFileName);
							}
			
							EPTDebug.LogException("PrintJobManager::Sending [NJS:1] request...");
							// Sending NJS:1 Request.
							string strNJSRequest = 
								EPTXmlRequest.GetNJSRequest(strPrinterId,
								strJobId, EPTConstants.JOBSTATUS_SUCCESS);
							cm.sendPostRequest(strNJSRequest);
							
						}
						catch(Exception gjfException)
						{
							string strGJFMsg = gjfException.Message;
							EPTDebug.LogException("PrintJobManager::"+ gjfException.ToString() );

							if ( strGJFMsg.Equals("JOB_CANCELLED") )
							{
								EPTDebug.LogException("PrintJobManager::JOB CANCELLED: jobId:"+ strJobId );
							}
							else
							{
								EPTDebug.LogException("PrintJobManager::Sending [NJS:4] request...");
								// Sending [NJS:4] Request.
								string strNJSRequest = 
									EPTXmlRequest.GetNJSRequest(strPrinterId,
									strJobId, EPTConstants.JOBSTATUS_FAILED);

								cm.sendPostRequest(strNJSRequest);
							}
						}

					
					}
					else // There are no jobs to print.
					{
						// Send [NPS:1] request and suspend the thread.
						string strNPSRequest = 
							EPTXmlRequest.GetNPSRequest(this.parentForm.GetCurrentPrinterId(),
							EPTConstants.PRINTERSTATUS_READY,
							this.strEPTHostIp,
							this.strEPTPortNo);
						cm.sendPostRequest(strNPSRequest);
						
						EPTDebug.LogException("PrintJobManager::Suspending thread...");
						this.parentForm.SetStatusBarText("Ready.");
						thrdPJobMgr.Suspend();
					}
	
				}
				catch(Exception e)
				{
						EPTDebug.LogException("PrintJobManager::"+ e.ToString() );
				}
			}
		}

	}
}
