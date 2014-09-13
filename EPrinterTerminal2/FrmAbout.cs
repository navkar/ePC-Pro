/********************************************************************
 * Author: Naveen Karamchetti  
 * Description: This class displays the about information dialog.
 ********************************************************************/

using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Reflection;

namespace EPrinterTerminal
{
	/// <summary>
	/// Summary description for FrmAbout.
	/// </summary>
	public class FrmAbout : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Label lblAboutInfo;
		private System.Windows.Forms.GroupBox grpBox;
		private System.Windows.Forms.Button btnOK;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public FrmAbout()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			// Setting the data in the label.
			Assembly thisAssembly = Assembly.GetAssembly(this.GetType());
			string strVersion = thisAssembly.GetName().Version.ToString(); 

			lblAboutInfo.Text = "EPrinterTerminal Version " + strVersion;

		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(FrmAbout));
			this.lblAboutInfo = new System.Windows.Forms.Label();
			this.grpBox = new System.Windows.Forms.GroupBox();
			this.btnOK = new System.Windows.Forms.Button();
			this.grpBox.SuspendLayout();
			this.SuspendLayout();
			// 
			// lblAboutInfo
			// 
			this.lblAboutInfo.Location = new System.Drawing.Point(48, 40);
			this.lblAboutInfo.Name = "lblAboutInfo";
			this.lblAboutInfo.Size = new System.Drawing.Size(208, 16);
			this.lblAboutInfo.TabIndex = 0;
			this.lblAboutInfo.Text = "EPrinterTerminal";
			// 
			// grpBox
			// 
			this.grpBox.Controls.AddRange(new System.Windows.Forms.Control[] {
																				 this.btnOK,
																				 this.lblAboutInfo});
			this.grpBox.Name = "grpBox";
			this.grpBox.Size = new System.Drawing.Size(288, 136);
			this.grpBox.TabIndex = 1;
			this.grpBox.TabStop = false;
			// 
			// btnOK
			// 
			this.btnOK.Location = new System.Drawing.Point(104, 96);
			this.btnOK.Name = "btnOK";
			this.btnOK.TabIndex = 1;
			this.btnOK.Text = "OK";
			this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
			// 
			// FrmAbout
			// 
			this.AcceptButton = this.btnOK;
			this.AutoScaleBaseSize = new System.Drawing.Size(6, 14);
			this.ClientSize = new System.Drawing.Size(292, 141);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.grpBox});
			this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FrmAbout";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "About EPrinterTerminal";
			this.grpBox.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// OK button click event.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btnOK_Click(object sender, System.EventArgs e)
		{
			this.Close();
		}
	}
}
