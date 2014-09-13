/********************************************************************
 * Author: Naveen Karamchetti  
 * Description: The class presents an UI for the Job Queue Controller, 
 * it looks job messages in the queue and processes them.
 *******************************************************************/

using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
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
    /// Looks out for messages in the Job Queue(JQ).
    /// </summary>
    public class FrmPoller : System.Windows.Forms.Form
    {
        private Thread pollerThread = null;
        private string strJQName, strDbConn = null;
        private Hashtable htConfig = null;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.Label lblJQ;
        private System.Windows.Forms.Label lblJQName;

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;

        /// <summary>
        /// The constructor of the class.
        /// </summary>
        public FrmPoller()
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();
    
            // Setting the Log file name.
            EASDebug.setLogFileName("JQController.log");
            EASDebug.LogException("FrmPoller::Application Started...");
            // Read the 'OQController.config' file.
            ReadConfigFile rcf = new ReadConfigFile("JQController.config");
                htConfig = rcf.GetConfigData();

            if ( htConfig == null)
            {
                MessageBox.Show(this,
                                "File 'JQController.config' was not found in the "+ Application.StartupPath ,
                                "JQueue Controller", 
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);

                Application.Exit();
            }
            else
            {
                this.strJQName = (string) htConfig["job_queue_name"];
                this.strDbConn = (string) htConfig["database_url"];
                lblJQName.Text = this.strJQName;
                EASDebug.LogException("FrmPoller::Database URL:"+ this.strDbConn);
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
            System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(FrmPoller));
            this.btnStop = new System.Windows.Forms.Button();
            this.btnExit = new System.Windows.Forms.Button();
            this.lblJQ = new System.Windows.Forms.Label();
            this.btnStart = new System.Windows.Forms.Button();
            this.lblJQName = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnStop
            // 
            this.btnStop.Enabled = false;
            this.btnStop.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.btnStop.Location = new System.Drawing.Point(144, 48);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(104, 24);
            this.btnStop.TabIndex = 6;
            this.btnStop.Text = "S&top";
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // btnExit
            // 
            this.btnExit.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.btnExit.Location = new System.Drawing.Point(272, 48);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(104, 24);
            this.btnExit.TabIndex = 9;
            this.btnExit.Text = "E&xit";
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // lblJQ
            // 
            this.lblJQ.AutoSize = true;
            this.lblJQ.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.lblJQ.Location = new System.Drawing.Point(16, 16);
            this.lblJQ.Name = "lblJQ";
            this.lblJQ.Size = new System.Drawing.Size(82, 16);
            this.lblJQ.TabIndex = 0;
            this.lblJQ.Text = "Job Queue:";
            // 
            // btnStart
            // 
            this.btnStart.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.btnStart.Location = new System.Drawing.Point(16, 48);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(104, 24);
            this.btnStart.TabIndex = 5;
            this.btnStart.Text = "&Start";
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // lblJQName
            // 
            this.lblJQName.AutoSize = true;
            this.lblJQName.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.lblJQName.Location = new System.Drawing.Point(152, 16);
            this.lblJQName.Name = "lblJQName";
            this.lblJQName.Size = new System.Drawing.Size(0, 16);
            this.lblJQName.TabIndex = 7;
            // 
            // FrmPoller
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(386, 79);
            this.Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                          this.btnExit,
                                                                          this.btnStop,
                                                                          this.btnStart,
                                                                          this.lblJQName,
                                                                          this.lblJQ});
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "FrmPoller";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Job Queue Controller";
            this.Closing += new System.ComponentModel.CancelEventHandler(this.FrmPoller_Cancel);
            this.ResumeLayout(false);

        }
        #endregion

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main() 
        {
            Application.Run(new FrmPoller());
        }

        /// <summary>
        /// Handles the exit button click event.
        /// </summary>
        /// <param name="sender">The Sender object</param>
        /// <param name="e">The event arguments</param>
        private void btnExit_Click(object sender, System.EventArgs e)
        {
            DialogResult dResult = 
                MessageBox.Show("Are you sure you want STOP the controller?", 
                                "Please Confirm...",
                                MessageBoxButtons.OKCancel, 
                                MessageBoxIcon.Question, 
                                MessageBoxDefaultButton.Button2);

            if ( dResult == DialogResult.OK)
            {
                // Stop the poller thread.
                if ( pollerThread != null)
                {
                    try
                    {
                        pollerThread.Abort();
                    }
                    catch
                    {
                        // Nothing to do.
                    }

                    pollerThread = null;
                }

                EASDebug.LogException("FrmPoller::Application Ended.");
                Application.Exit();         
            }
        }

        /// <summary>
        /// Called when start button is clicked.
        /// </summary>
        /// <param name="sender">The sender object</param>
        /// <param name="e">The event arguments</param>
        private void btnStart_Click(object sender, System.EventArgs e)
        {
            btnStart.Enabled = false;
            btnStop.Enabled = true;

            if ( strJQName.Equals("") )
            {
                MessageBox.Show(this,
                                "Specify the Job queue name.",
                                "JQueue Controller",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Exclamation);
            }
            else if ( !(MessageQueue.Exists(strJQName )) )
            {
                MessageBox.Show(this,
                                "The Job Queue does not exist.",
                                "JQueue Controller",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Exclamation);
            }
            else
            {
                pollerThread 
                    = new Thread(
                            new ThreadStart(this.lookOutForMessagesInQueue) );
                pollerThread.Name = "PollerThread";
                pollerThread.IsBackground = true;
                pollerThread.Start();
            }

        }

        /// <summary>
        /// This method looks for messages for in the queue.
        /// </summary>
        protected void lookOutForMessagesInQueue()
        {
            while(true)
            {
                // Handle message queue transactions.
                MessageQueueTransaction msgTx = null;
                
                try
                {
                    EASDebug.LogException("FrmPoller::"+ Thread.CurrentThread.Name +" thread started...");
                    EASDebug.LogException("FrmPoller::Waiting for messages in "+ strJQName + " queue...");
                    
                    msgTx = new MessageQueueTransaction();
                    MessageQueue mqReceive = new MessageQueue(this.strJQName);
                    msgTx.Begin();
                    mqReceive.Formatter = new BinaryMessageFormatter();
                    System.Messaging.Message messageX = mqReceive.Receive(msgTx);

                    if ( messageX.Label.Equals(EASConstants.RT_NWJ))
                    {
                        // Sprawn a thread for every message received from the queue.
                        QueueMessageProcessor qmp 
                        = new QueueMessageProcessor(this.strDbConn,(string) messageX.Body);
                    }
                    
                    mqReceive.Close();
                    msgTx.Commit();
                    EASDebug.LogException("FrmPoller::Transaction Status: " + msgTx.Status);
                }
                catch(MessageQueueException mqe)
                {
                    if ( msgTx != null)
                    {
                        msgTx.Abort();
                    }
                    
                    EASDebug.LogException("FrmPoller::LookOutForMessagesInQueue::" +
                                            mqe.StackTrace +
                                            "\r\n");
                }
            }

        } // end of the method LookOutForMessagesInQueue()

        /// <summary>
        /// Handles the Stop button click event.
        /// </summary>
        /// <param name="sender">The sender object.</param>
        /// <param name="e">The event arguments.</param>
        private void btnStop_Click(object sender, System.EventArgs e)
        {
            btnStart.Enabled = true;
            btnStop.Enabled = false;

            // Stop the poller thread.
            if ( pollerThread != null)
            {
                try
                {
                    pollerThread.Suspend();
                    pollerThread.Abort();
                }
                catch
                {
                    // Nothing to do.
                }
                pollerThread = null;
            }

            EASDebug.LogException("FrmPoller::Stopped...");
        }
        
        /// <summary>
        /// Handles the form closing event.
        /// </summary>
        /// <param name="sender">The sender object.</param>
        /// <param name="e">The cancel event arguments.</param>
        protected void FrmPoller_Cancel (Object sender, CancelEventArgs e) 
        {
            DialogResult dResult = 
                MessageBox.Show("Are you sure you want to stop the controller?", 
                                "Please Confirm...",
                                MessageBoxButtons.OKCancel, 
                                MessageBoxIcon.Question, 
                                MessageBoxDefaultButton.Button2);

            if ( dResult == DialogResult.OK)
            {
                e.Cancel = false;

                // Stop the poller thread.
                if ( pollerThread != null)
                {
                    try
                    {
                        pollerThread.Suspend();
                        pollerThread.Abort();
                    }
                    catch
                    {
                        // Nothing to do.
                    }
                    pollerThread = null;
                }

                EASDebug.LogException("FrmPoller::Application Ended.");
                Application.Exit();         
            }
            else
            {
                e.Cancel = true;
            }

        }
    

    }
}
