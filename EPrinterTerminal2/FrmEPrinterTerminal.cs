/********************************************************************
 * Author: Naveen Karamchetti  
 * Description: This class displays the main UI for the printer terminal.
 ********************************************************************/

using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Threading;
using System.Xml;
using System.IO;
using System.Resources;
using System.Net;

namespace EPrinterTerminal
{
	/// <summary>
	/// The main class that displays the EPrinterTerminal form.
	/// </summary>
	public class FrmEPrinterTerminal : System.Windows.Forms.Form
	{
		/// <summary>
		/// This hashtable holds the EPT configuration data.
		/// </summary>
		private Hashtable htConfigData = null;
		private Hashtable htReturnCodes = null;
		private Thread thrdRegisterPrinter = null;
		private string strRprXmlRequest = null;
		private Icon tmpPrinterIcon = null;
		private EPTTCPServer tcpServer = null;
		private PrintJobManager printJobMgr  = null;
		//
		private System.Windows.Forms.ListView listVJobs;
		private System.Windows.Forms.MainMenu mainMnu;
		private System.Windows.Forms.MenuItem mICtrl;
		private System.Windows.Forms.MenuItem mICtrlRPR;
		private System.Windows.Forms.MenuItem mICtrlDPR;
		private System.Windows.Forms.MenuItem menuItem4;
		private System.Windows.Forms.MenuItem mICtrlExit;
		private System.Windows.Forms.MenuItem mIEdit;
		private System.Windows.Forms.MenuItem mIProp;
		private System.Windows.Forms.MenuItem mIDelJobs;
		private System.Windows.Forms.MenuItem mIHelp;
		private System.Windows.Forms.MenuItem mIHelpAbout;
		private System.Windows.Forms.ContextMenu mnuCtxList;
		private System.Windows.Forms.MenuItem mnuIDelJob;
		private System.Windows.Forms.MenuItem mnuIDispGridLines;
		private System.Windows.Forms.StatusBar statusBar;
		private System.Windows.Forms.StatusBarPanel statusBarPanelReq;
		private System.Windows.Forms.MenuItem mISepCtx;
		private System.Windows.Forms.StatusBarPanel statusBarPanelDummy;
		private System.Windows.Forms.MenuItem mnuDelAllJobs;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		/// <summary>
		/// constructor of the class.
		/// </summary>
		public FrmEPrinterTerminal()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			// Log Flag. - For Release Build Version only
			// true - messages are not logged.
			// false - messages are logged.
			//EPTDebug.setLogFlag(false);

			//--- Reading Configuration Files
			XmlFileReaderWriter xfrw 
			= new XmlFileReaderWriter(EPTConstants.APP_CONFIG_PATH + EPTConstants.APP_CONFIG_NAME);
			this.htConfigData = xfrw.ReadXmlFile();
			this.htConfigData.Remove("EPTHostIp");

			// -- Add Local IP Address to the htConfigData.
			IPHostEntry ipHost = Dns.Resolve(Dns.GetHostName() );
			IPAddress[] ipAddr = ipHost.AddressList;
			this.htConfigData.Add("EPTHostIp", ipAddr[0].ToString() );
			
			// Reading the return code resource files.
			IResourceReader reader = new ResourceReader(EPTConstants.APP_CONFIG_PATH + "returncodes.resources");
			IDictionaryEnumerator en = reader.GetEnumerator();
			this.htReturnCodes = new Hashtable();
			
			//Go through the enumerator, adding out the key and value pairs.
			while (en.MoveNext()) 
			{
				this.htReturnCodes.Add(en.Key,en.Value);
			}
			reader.Close();
			// closing the reader.
		
			// Creating the columns in the list view.
			ColumnHeader[] colHeader = new ColumnHeader[4];

						colHeader[0] = new ColumnHeader();
						colHeader[0].Text = "Date";
						colHeader[0].Width = listVJobs.Width/3;
						
						colHeader[1] = new ColumnHeader();
						colHeader[1].Text = "Print Jobs";
						colHeader[1].Width = listVJobs.Width/4;

						colHeader[2] = new ColumnHeader();
						colHeader[2].Text = "Cost (Yen)";
						colHeader[2].Width = listVJobs.Width/5;

						colHeader[3] = new ColumnHeader();
						colHeader[3].Text = "Copies";
						colHeader[3].Width = listVJobs.Width/5;

			ListView.ColumnHeaderCollection lvColHeadColl 
						= new ListView.ColumnHeaderCollection(listVJobs);

			lvColHeadColl.AddRange(colHeader);

			// Check for the items in the job list view.
			if ( listVJobs.Items.Count <= 0)
			{
				mIDelJobs.Enabled = false;
				mnuDelAllJobs.Enabled = false;
				mnuIDelJob.Enabled = false;
			}

			string strPrinterId = (string) this.htConfigData["PrinterId"];
			
			// Check for printer id, if the printer id exists! 
			if (strPrinterId != null)
			{
				this.mICtrlRPR.Enabled = false;	
				this.SetStatusBarText("Ready.");
				// Sending NPS request.
				//sendNPSRequest();
				startEPTServers();
			}
			else // Printer is not registered.
			{
				// Copy the icon before setting it to null
				this.tmpPrinterIcon = this.statusBarPanelReq.Icon;
				this.statusBarPanelReq.Icon = null;
				this.mICtrlDPR.Enabled = false;
				this.SetStatusBarText("EPrinterTerminal is not registered.");
			}
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(FrmEPrinterTerminal));
            this.listVJobs = new System.Windows.Forms.ListView();
            this.mnuCtxList = new System.Windows.Forms.ContextMenu();
            this.mnuIDelJob = new System.Windows.Forms.MenuItem();
            this.mnuDelAllJobs = new System.Windows.Forms.MenuItem();
            this.mISepCtx = new System.Windows.Forms.MenuItem();
            this.mnuIDispGridLines = new System.Windows.Forms.MenuItem();
            this.mainMnu = new System.Windows.Forms.MainMenu();
            this.mICtrl = new System.Windows.Forms.MenuItem();
            this.mICtrlRPR = new System.Windows.Forms.MenuItem();
            this.mICtrlDPR = new System.Windows.Forms.MenuItem();
            this.menuItem4 = new System.Windows.Forms.MenuItem();
            this.mICtrlExit = new System.Windows.Forms.MenuItem();
            this.mIEdit = new System.Windows.Forms.MenuItem();
            this.mIDelJobs = new System.Windows.Forms.MenuItem();
            this.mIProp = new System.Windows.Forms.MenuItem();
            this.mIHelp = new System.Windows.Forms.MenuItem();
            this.mIHelpAbout = new System.Windows.Forms.MenuItem();
            this.statusBarPanelDummy = new System.Windows.Forms.StatusBarPanel();
            this.statusBarPanelReq = new System.Windows.Forms.StatusBarPanel();
            this.statusBar = new System.Windows.Forms.StatusBar();
            ((System.ComponentModel.ISupportInitialize)(this.statusBarPanelDummy)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.statusBarPanelReq)).BeginInit();
            this.SuspendLayout();
            // 
            // listVJobs
            // 
            this.listVJobs.Anchor = (((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
                | System.Windows.Forms.AnchorStyles.Left) 
                | System.Windows.Forms.AnchorStyles.Right);
            this.listVJobs.ContextMenu = this.mnuCtxList;
            this.listVJobs.FullRowSelect = true;
            this.listVJobs.GridLines = true;
            this.listVJobs.HoverSelection = true;
            this.listVJobs.Location = new System.Drawing.Point(0, 2);
            this.listVJobs.MultiSelect = false;
            this.listVJobs.Name = "listVJobs";
            this.listVJobs.Size = new System.Drawing.Size(472, 200);
            this.listVJobs.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.listVJobs.TabIndex = 2;
            this.listVJobs.View = System.Windows.Forms.View.Details;
            // 
            // mnuCtxList
            // 
            this.mnuCtxList.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
                                                                                       this.mnuIDelJob,
                                                                                       this.mnuDelAllJobs,
                                                                                       this.mISepCtx,
                                                                                       this.mnuIDispGridLines});
            // 
            // mnuIDelJob
            // 
            this.mnuIDelJob.Index = 0;
            this.mnuIDelJob.Text = "Delete Selected Job";
            this.mnuIDelJob.Click += new System.EventHandler(this.mnuIDelJob_Click);
            // 
            // mnuDelAllJobs
            // 
            this.mnuDelAllJobs.Index = 1;
            this.mnuDelAllJobs.Text = "Delete All Jobs";
            this.mnuDelAllJobs.Click += new System.EventHandler(this.mnuDelAllJobs_Click);
            // 
            // mISepCtx
            // 
            this.mISepCtx.Index = 2;
            this.mISepCtx.Text = "-";
            // 
            // mnuIDispGridLines
            // 
            this.mnuIDispGridLines.Checked = true;
            this.mnuIDispGridLines.Index = 3;
            this.mnuIDispGridLines.Text = "Display Grid lines";
            this.mnuIDispGridLines.Click += new System.EventHandler(this.mnuIDispGridLines_Click);
            // 
            // mainMnu
            // 
            this.mainMnu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
                                                                                    this.mICtrl,
                                                                                    this.mIEdit,
                                                                                    this.mIHelp});
            // 
            // mICtrl
            // 
            this.mICtrl.Index = 0;
            this.mICtrl.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
                                                                                   this.mICtrlRPR,
                                                                                   this.mICtrlDPR,
                                                                                   this.menuItem4,
                                                                                   this.mICtrlExit});
            this.mICtrl.Text = "&Control";
            // 
            // mICtrlRPR
            // 
            this.mICtrlRPR.Index = 0;
            this.mICtrlRPR.Text = "&Register Printer";
            this.mICtrlRPR.Click += new System.EventHandler(this.mICtrlRPR_Click);
            // 
            // mICtrlDPR
            // 
            this.mICtrlDPR.Index = 1;
            this.mICtrlDPR.Text = "&DeRegister Printer";
            this.mICtrlDPR.Click += new System.EventHandler(this.mICtrlDPR_Click);
            // 
            // menuItem4
            // 
            this.menuItem4.Index = 2;
            this.menuItem4.Text = "-";
            // 
            // mICtrlExit
            // 
            this.mICtrlExit.Index = 3;
            this.mICtrlExit.Text = "&Exit";
            this.mICtrlExit.Click += new System.EventHandler(this.mICtrlExit_Click);
            // 
            // mIEdit
            // 
            this.mIEdit.Index = 1;
            this.mIEdit.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
                                                                                   this.mIDelJobs,
                                                                                   this.mIProp});
            this.mIEdit.Text = "&Edit";
            // 
            // mIDelJobs
            // 
            this.mIDelJobs.Index = 0;
            this.mIDelJobs.Text = "Delete Jobs";
            this.mIDelJobs.Click += new System.EventHandler(this.mIDelJobs_Click);
            // 
            // mIProp
            // 
            this.mIProp.Index = 1;
            this.mIProp.Text = "Properties";
            this.mIProp.Click += new System.EventHandler(this.mIProp_Click);
            // 
            // mIHelp
            // 
            this.mIHelp.Index = 2;
            this.mIHelp.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
                                                                                   this.mIHelpAbout});
            this.mIHelp.Text = "&Help";
            // 
            // mIHelpAbout
            // 
            this.mIHelpAbout.Index = 0;
            this.mIHelpAbout.Text = "About EPrinterTerminal...";
            this.mIHelpAbout.Click += new System.EventHandler(this.mIHelpAbout_Click);
            // 
            // statusBarPanelDummy
            // 
            this.statusBarPanelDummy.MinWidth = 3;
            this.statusBarPanelDummy.Width = 3;
            // 
            // statusBarPanelReq
            // 
            this.statusBarPanelReq.AutoSize = System.Windows.Forms.StatusBarPanelAutoSize.Spring;
            this.statusBarPanelReq.Icon = ((System.Drawing.Icon)(resources.GetObject("statusBarPanelReq.Icon")));
            this.statusBarPanelReq.MinWidth = 100;
            this.statusBarPanelReq.ToolTipText = "Displays the current status";
            this.statusBarPanelReq.Width = 453;
            // 
            // statusBar
            // 
            this.statusBar.Anchor = ((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
                | System.Windows.Forms.AnchorStyles.Right);
            this.statusBar.Dock = System.Windows.Forms.DockStyle.None;
            this.statusBar.Location = new System.Drawing.Point(0, 203);
            this.statusBar.Name = "statusBar";
            this.statusBar.Panels.AddRange(new System.Windows.Forms.StatusBarPanel[] {
                                                                                         this.statusBarPanelReq,
                                                                                         this.statusBarPanelDummy});
            this.statusBar.ShowPanels = true;
            this.statusBar.Size = new System.Drawing.Size(472, 22);
            this.statusBar.TabIndex = 1;
            // 
            // FrmEPrinterTerminal
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(6, 14);
            this.ClientSize = new System.Drawing.Size(472, 225);
            this.Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                          this.listVJobs,
                                                                          this.statusBar});
            this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Menu = this.mainMnu;
            this.Name = "FrmEPrinterTerminal";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "EPrinterTerminal";
            this.Closing += new System.ComponentModel.CancelEventHandler(this.frmEPrinterTerminal_Closing);
            ((System.ComponentModel.ISupportInitialize)(this.statusBarPanelDummy)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.statusBarPanelReq)).EndInit();
            this.ResumeLayout(false);

        }
		#endregion

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() 
		{
			Application.Run(new FrmEPrinterTerminal());
		}

		/// <summary>
		/// Handles the Register Printer Request.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void mICtrlRPR_Click(object sender, System.EventArgs e)
		{
			FrmRegisterPrinter x = new FrmRegisterPrinter(this);
			x.ShowDialog();
		}

		/// <summary>
		/// Deletes the Print Jobs from the list.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void mIDelJobs_Click(object sender, System.EventArgs e)
		{
			ListView.SelectedListViewItemCollection lstVSelColl 
				= new ListView.SelectedListViewItemCollection(listVJobs);

			lstVSelColl = listVJobs.SelectedItems;
	
			if (lstVSelColl.Count > 0 )
			{
				listVJobs.Items.Remove(lstVSelColl[0]);
			}

			if ( listVJobs.Items.Count <= 0)
			{
				mIDelJobs.Enabled = false;
				mnuDelAllJobs.Enabled = false;
				mnuIDelJob.Enabled = false;
			}
		}

		/// <summary>
		/// Deletes a job entry
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void mnuIDelJob_Click(object sender, System.EventArgs e)
		{
			mIDelJobs_Click(sender,e);
		}

		/// <summary>
		/// Deletes a job entry
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void mnuIDispGridLines_Click(object sender, System.EventArgs e)
		{
			if ( mnuIDispGridLines.Checked == true)
			{
				mnuIDispGridLines.Checked = false;	
				listVJobs.GridLines = false;
			}
			else
			{
				mnuIDispGridLines.Checked = true;
				listVJobs.GridLines = true;
			}
		}
	
		/// <summary>
		/// This function is called by the (FrmRegisterPrinter) form.
		/// </summary>
		/// <param name="strRprXmlRequest">The XML RPR request.</param>
		public void sendRprRequest(string strRprXmlRequest)
		{
			this.strRprXmlRequest = strRprXmlRequest;
			// Write the RPR xml request into a file.
			XmlFileReaderWriter xfrw 
				= new XmlFileReaderWriter(EPTConstants.APP_CONFIG_PATH + "RegPrinter.xml");
			xfrw.WriteFile(strRprXmlRequest);

			// Sends a RPR request to the EAS on a separate thread.
			thrdRegisterPrinter 
				= new Thread(new ThreadStart( this.RegisterPrinterHandler ));	

			thrdRegisterPrinter.Name = "RPR Thread";
			thrdRegisterPrinter.Start();
		}

		/// <summary>
		/// This method runs as a separate thread, this is handler for the RPR thread.
		/// </summary>
		private void RegisterPrinterHandler()
		{
			ConnectionManager cm = new ConnectionManager
				((string) htConfigData["HostName"], // Host Name
				System.Int32.Parse( (string) htConfigData["PortNo"]) // Port No
				);
			
			// update the status bar.
			SetStatusBarText("Waiting for RPR response...");

			string strResponse 
				= cm.sendPostRequest(this.strRprXmlRequest);
			
			// Now parse the XML response and do something.
			if ( strResponse != String.Empty)
			{
				StringReader sr = new StringReader(strResponse);
				XmlTextReader xtr = new XmlTextReader(sr);
				XmlResponseParser xrp = new XmlResponseParser(xtr);

				int iReturnCode = System.Int32.Parse(xrp.getReturnCode());
				
				switch (iReturnCode)
				{
					// OK
					case 200:
						// Save the Printer Id in the Hashtable.
						this.htConfigData.Remove("PrinterId");
						this.htConfigData.Add("PrinterId",xrp.getPrinterId());
						// Writing the new printer ID in the configuration file.
						XmlFileReaderWriter xfrw 
							= new XmlFileReaderWriter(EPTConstants.APP_CONFIG_PATH + EPTConstants.APP_CONFIG_NAME);

						xfrw.WriteXmlFile(htConfigData,"Configuration");
						
						this.statusBarPanelReq.Icon = this.tmpPrinterIcon;
						this.mICtrlRPR.Enabled = false;							
						this.mICtrlDPR.Enabled = true;
						SetStatusBarText("EPrinterTerminal registration complete.");
						
						sendNPSRequest();
						startEPTServers();

						break;

					default:
						
						string strReturn = (string) this.htReturnCodes[iReturnCode+""];

						DialogResult dr = 
							MessageBox.Show("Printer registration problem, reason : "+ strReturn ,
							"System Information!",
							MessageBoxButtons.RetryCancel,
							MessageBoxIcon.Question,
							MessageBoxDefaultButton.Button1);

						if ( dr == DialogResult.Retry)
						{
							this.sendRprRequest(this.strRprXmlRequest);		
						}
						else if ( dr == DialogResult.Cancel)
						{
							this.SetStatusBarText("EPrinterTerminal is not registered.");
						}

						break;
				}

			}
			else
			{
				// This condition never occurs.
			}
		}
		
		/// <summary>
		/// Adds an entry in the job list box for display.
		/// </summary>
		/// <param name="strFileName">File Name</param>
		/// <param name="strCost">Cost</param>
		/// <param name="strCopies">No of copies</param>
		public void addJobDataToListView(string strFileName, string strCost, string strCopies)
		{
			string[] strListData = { System.DateTime.Now.ToString(), strFileName, strCost ,strCopies};
			ListViewItem lvi = new ListViewItem(strListData);
			listVJobs.Items.Add(lvi);

			if (!mIDelJobs.Enabled)
			{
				mIDelJobs.Enabled = true;
				mnuDelAllJobs.Enabled = true;
				mnuIDelJob.Enabled = true;
			}

			SetStatusBarText("Printing Jobs...");
		}

		/// <summary>
		/// Event handler for Edit > Properties.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void mIProp_Click(object sender, System.EventArgs e)
		{
			FrmEptConfig frmConf = new FrmEptConfig(this, this.htConfigData);
			frmConf.ShowDialog();
		}

		/// <summary>
		/// Set the hashtable after changing 
		/// </summary>
		/// <param name="htConfigData"></param>
		public void SetConfigHashtable(Hashtable htConfigData)
		{
			this.htConfigData = htConfigData;

			// update the EAS connection value for the print job manager.
			if ( printJobMgr != null)
			{
				printJobMgr.setHostIp((string) htConfigData["HostName"]);
				printJobMgr.setHostPortNo( Int32.Parse( (string) htConfigData["PortNo"] ));
			}
		}

		/// <summary>
		/// Set the text on the status bar.
		/// </summary>
		/// <param name="strText">The status bar text.</param>
		public void SetStatusBarText(string strText)
		{
				statusBarPanelReq.Text = strText;
		}

		/// <summary>
		/// Raises an exit event.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void mICtrlExit_Click(object sender, System.EventArgs e)
		{
				Application.Exit();		
		}

		/// <summary>
		/// Close the application
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void frmEPrinterTerminal_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
				Application.Exit();		
		}

		/// <summary>
		/// Notifies the Printer Status
		/// </summary>
		private void sendNPSRequest()
		{
			string strNPSRequest = EPTXmlRequest.GetNPSRequest(
								(string) htConfigData["PrinterId"],
								EPTConstants.PRINTERSTATUS_READY,
								(string) htConfigData["EPTHostIp"],
								(string) htConfigData["EPTPortNo"]);

			ConnectionManager cm = new ConnectionManager
				((string) htConfigData["HostName"], // Host Name
				System.Int32.Parse( (string) htConfigData["PortNo"]) // Port No
				);

			string strResponse = cm.sendPostRequest(strNPSRequest);
			
			// Now parse the XML response and do something.
			if ( strResponse != String.Empty)
			{
				try
				{
					StringReader sr = new StringReader(strResponse);
					XmlTextReader xtr = new XmlTextReader(sr);
					XmlResponseParser xrp = new XmlResponseParser(xtr);

					int iReturnCode = System.Int32.Parse(xrp.getReturnCode());
					EPTDebug.LogException("NPS Return code: "+ 	iReturnCode);

					switch (iReturnCode)
					{
							// OK
						case 200:
							break;

							// Something went wrong. Report Error.
						default:
	
							string strReturn = (string) this.htReturnCodes[iReturnCode+""];

							DialogResult dr = 
								MessageBox.Show("Unable to notify status : "+ strReturn + "\r\n Go to Edit > Properties to change the settings." ,
								"System Information!",
								MessageBoxButtons.OK,
								MessageBoxIcon.Information);

							break;
					}
				}
				catch(Exception e)
				{
					EPTDebug.LogException("NPS Exception : "+ e.ToString() );
				}
			}
		}

		/// <summary>
		/// Get the Return Code(RC) description from the *.resources file.
		/// </summary>
		/// <param name="strKey">The Return Code (For eg: 200,911, ...) </param>
		/// <returns></returns>
		public string getRCDescription(string strKey)
		{
			return (string) this.htReturnCodes[strKey];
		}

		/// <summary>
		/// [0] - AreaCode
		/// [1] - EPTHostIp
		/// [2] - EPTPortNo
		/// </summary>
		/// <returns> </returns>
		public string[] getEPTDetails()
		{
			return new string[] 
			{ 
			(string) this.htConfigData["AreaCode"],
			(string) this.htConfigData["EPTHostIp"],
			(string) this.htConfigData["EPTPortNo"]
			};
		}

		/// <summary>
		/// Returns the Current Printer Id.
		/// </summary>
		/// <returns>AreaCode_PrinterId</returns>
		public string GetCurrentPrinterId()
		{
			return (string) this.htConfigData["PrinterId"];
		}


		/// <summary>
		/// Start all the servers.
		/// </summary>
		private void startEPTServers()
		{
			int iPortNo = Int32.Parse((string) this.htConfigData["EPTPortNo"]);

			// If the TCP server is not null, then resume it.
			if ( tcpServer != null)
			{
				tcpServer.resumeTCPServer();	
			}
			else // restart the TCP server.
			{
				tcpServer = new EPTTCPServer(iPortNo);
			}

			printJobMgr 
				= new PrintJobManager(
							this, // current form pointer.
							(string) this.htConfigData["HostName"],	// EAS Host IP			
							Int32.Parse((string) this.htConfigData["PortNo"]),	// EAS Port No			
							Application.StartupPath,	
							(string) this.htConfigData["PrinterId"],				
							(string) this.htConfigData["EPTHostIp"],
							(string) this.htConfigData["EPTPortNo"]);
			
			// create a new delegate instance and bind it to the
			// observer's startprocessing jobs method.
			JobInfoManager.StartProcessingJobs jobDelegate
			= new JobInfoManager.StartProcessingJobs(printJobMgr.startProcessingJobs);
			// Add the event to the delegate.
			JobInfoManager.Instance.PrintJobsArrival += jobDelegate;

		}

		/// <summary>
		/// Shut down the TCP server and Print Job Manager.
		/// </summary>
		private void stopEPTServers()
		{
			if ( printJobMgr != null)
			{
			//	printJobMgr.stopPrintJobManager();
			//	printJobMgr = null;
			}

			if ( tcpServer != null)
			{
			//	tcpServer.suspendTCPServer();
			}

			EPTDebug.LogException("Stopped all EPT Servers.");
		}

		/// <summary>
		/// The event de-registers the printer. This is not threaded.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void mICtrlDPR_Click(object sender, System.EventArgs e)
		{
			try
			{
				string strDPRRequest 
					= EPTXmlRequest.GetDPRRequest( (string) this.htConfigData["PrinterId"]);

				ConnectionManager cm = new ConnectionManager
						((string) htConfigData["HostName"],					 // Host Name
						System.Int32.Parse( (string) htConfigData["PortNo"]) // Port No
					);

				SetStatusBarText("Waiting for DPR response...");
				string strResponse = cm.sendPostRequest(strDPRRequest);
			
				// Now parse the XML response.
				if ( strResponse != String.Empty)
				{
					StringReader sr = new StringReader(strResponse);
					XmlTextReader xtr = new XmlTextReader(sr);
					XmlResponseParser xrp = new XmlResponseParser(xtr);

					int iReturnCode = System.Int32.Parse(xrp.getReturnCode());
					EPTDebug.LogException("DPR Return code: "+ 	iReturnCode);

					if ( iReturnCode != 200)
					{
						string strReturn = (string) this.htReturnCodes[iReturnCode+""];

						MessageBox.Show("Printer De-registration problem, reason : "+ strReturn ,
							"System Information!",
							MessageBoxButtons.OK,
							MessageBoxIcon.Asterisk);
						this.SetStatusBarText("Ready.");
					}
					else // De-register successfull.
					{
                        // Remove the PrinterID from the hashtable and save the config file.
			            this.htConfigData.Remove("PrinterId");
                        // Writing the configuration file.
                        XmlFileReaderWriter xfrw 
                            = new XmlFileReaderWriter(EPTConstants.APP_CONFIG_PATH + EPTConstants.APP_CONFIG_NAME);
                        xfrw.WriteXmlFile(htConfigData,"Configuration");

                        this.tmpPrinterIcon = this.statusBarPanelReq.Icon;
                        this.statusBarPanelReq.Icon = null;
                        this.SetStatusBarText("EPrinterTerminal is not registered.");
                        mICtrlDPR.Enabled = false;
                        mICtrlRPR.Enabled = true;
                        stopEPTServers();
					}

				}

			}
			catch(Exception dprException)
			{
				EPTDebug.LogException("FrmEPrinterTerminal::mICtrlDPR_Click " +  dprException.ToString() );
			}
			
		}

		/// <summary>
		/// Displays the Version Information.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void mIHelpAbout_Click(object sender, System.EventArgs e)
		{
			FrmAbout aboutInfo = new FrmAbout();
			aboutInfo.ShowDialog();				
			SetStatusBarText("Ready.");
		}

		/// <summary>
		/// Removes all the jobs from the list view.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void mnuDelAllJobs_Click(object sender, System.EventArgs e)
		{
			listVJobs.Items.Clear();		

			if ( listVJobs.Items.Count <= 0)
			{
				mIDelJobs.Enabled = false;
				mnuDelAllJobs.Enabled = false;
				mnuIDelJob.Enabled = false;
			}
		}
	}
}
