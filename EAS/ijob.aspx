<%@ Page Language="C#" %>
<% @Import Namespace="System.Data" %>
<% @Import Namespace="System.IO" %>
<% @Import Namespace="System.Data.SqlClient" %>
<% @Import Namespace="EAppService" %>

<html>
  <META HTTP-EQUIV=Refresh CONTENT="2; URL=http://localhost/eas/ijob.aspx"> 
	<head>
		<title>EPSON iPrint Service</title>
	</head>
	<body>
		<table border=0 bgcolor="#ccccff" CellPadding="2" CellSpacing="0" Width="100%">
			<tr><td align="center">
			<asp:label HorizontalAlign="Center" runat="server" id="lblTitle"
				Font-Name="Verdana" Font-Italic="false" Font-Size="Smaller" />
			</td></tr>
			<tr bgcolor="white">
				<td>&nbsp;
				</td>
			</tr>
			<tr bgcolor="white">
			<td align="center">
			<asp:label HorizontalAlign="Center" runat="server" id="lblMessage"
				Font-Name="Verdana" Font-Italic="false" Font-Size="Smaller" />
			</td></tr>
			</table>


	<form runat="server">
	<div align="center">
	<table border="0" bgcolor="#ccffee" CellPadding="2" CellSpacing="0" Width="90%" >
	<tr><td align="center">
	<asp:label HorizontalAlign="Left" runat="server" id="lblTitle2"
			Font-Name="Verdana" Font-Italic="false" Font-Size="Smaller" />
	</td></tr>
	</table>

	<asp:DataGrid id="jobsGrid" runat="server"
    	AutoGenerateColumns="false"
		BackColor="White"
	    BorderWidth="1px" BorderStyle="Solid" BorderColor="Tan"
    	CellPadding="2" CellSpacing="0"	Width="90%"
	    Font-Name="Verdana" Font-Size="9pt"
		HorizontalAlign="Center"
		AllowSorting="false"
		AllowPaging="true" PageSize="7"
		OnItemCommand="CancelPrintCommand"
	    OnPageIndexChanged="OnPageIndexChangedPrintersGrid">
	    
    <SelectedItemStyle BackColor="PaleGoldenRod" Font-Bold="true"/>
  	<HeaderStyle BackColor="DarkRed" ForeColor="White" Font-Bold="true"/>
    <AlternatingItemStyle BackColor="Beige"/>
    <ItemStyle ForeColor="DarkSlateBlue"/>
   
	<PagerStyle Mode="NextPrev" HorizontalAlign="Left"
				ForeColor="White" BackColor="Tan" 
				NextPageText="Next Page >>" PrevPageText="<< Prev. Page">
   	</PagerStyle> 

   	<Columns>

		<asp:BoundColumn HeaderText="Job File Name" DataField="job_filename"/> 
		<asp:BoundColumn DataFormatString="{0:dd-MMM-yy H:mm:ss }" 
		HeaderText="Created at" DataField="job_date"/> 
	    <asp:BoundColumn HeaderText="Copies" DataField="copies"/> 	
	    <asp:BoundColumn HeaderText="Job Id" DataField="job_id"/> 	
		<asp:BoundColumn HeaderText="Status" DataField="job_status" /> 
	    <asp:BoundColumn HeaderText="Location" DataField="area_code"/> 
	    <asp:BoundColumn HeaderText="Printer Id" DataField="printer_id"/> 
	    <asp:ButtonColumn CommandName="CancelPrintJob" ButtonType="LinkButton"
           HeaderText="Cancel Job" Text="Cancel" Visible="True"/>

  
   	</Columns>

	</asp:DataGrid>

	<table border=0 CellPadding="2" CellSpacing="0" Width="90%">
	<tr><td>
	<asp:label HorizontalAlign="Left" runat="server" id="lblNavigation"
			Font-Name="Verdana" Font-Italic="false" Font-Size="Smaller" />
	</td></tr>
	<tr><td align="Center">
	<asp:button Text="Refresh" runat="server" id="btnRefresh"
			Font-Name="Verdana" Font-Italic="false" Font-Size="Smaller" />
	</td></tr>
	</table>
	<BR><BR>
	<table border="0" CellPadding="2" CellSpacing="0" Width="100%">
	<tr>
		<td align="Left" bgcolor="#ccccff">
			<asp:label HorizontalAlign="Center" runat="server" id="lblSubTitle"
			Font-Name="Verdana" Font-Italic="false" Font-Size="Smaller" />
		</td>
	</tr>
	<tr>
		<td>
			<asp:HyperLink HorizontalAlign="Center" runat="server" id="docLink"
			Font-Name="Verdana" Font-Italic="false" Font-Size="Smaller" 
		    NavigateUrl="iprint.aspx" Text="iPrint Document Page"/>
		</td>
   	</tr>
	</table>
	</div>	
	
	</form>
	</body>
	</html>

<script language="C#" runat="server">

	protected void OnPageIndexChangedPrintersGrid(object sender, DataGridPageChangedEventArgs e)
	{
		jobsGrid.CurrentPageIndex = e.NewPageIndex;
		bindData();
	}

	void CancelPrintCommand(Object sender, DataGridCommandEventArgs e) 
	{
		try
		{
				TableCell jobIdCell = e.Item.Cells[3];

				string strDBUrl =
				ConfigurationSettings.AppSettings["database_url"];

				// Change the Job status value to '3', If necessary, to support cancellation.
				string strSQL 
				= "update jobinfo set job_status = '5' where job_id = '" +
				  jobIdCell.Text +"' and job_status in ('2', '4')";

				SqlConnection sqlConn = new SqlConnection(strDBUrl);
				sqlConn.Open();

				SqlCommand sqlCmd = new SqlCommand(strSQL, sqlConn);
				sqlCmd.ExecuteReader();

				sqlConn.Close();
		}
		catch
		{
				// bindata will handle the errors.
		}

		bindData();
	}

	public void Page_Load( Object source, EventArgs e)
	{
		lblTitle.Text = "EPSON iPrint Service";
		lblSubTitle.Text = "Quick Links";
		lblTitle2.Text = "iPrint Job Box";
		lblMessage.Text = "";

		string strPrinterGuid = Request.QueryString["printer_guid"];

		try
		{

				if ( !Page.IsPostBack && strPrinterGuid != null )
				{
					string strDirectory = ConfigurationSettings.AppSettings["shared_directory"];
					string strDbURL = ConfigurationSettings.AppSettings["database_url"];
					string strJobQueueName = ConfigurationSettings.AppSettings["job_queue_name"];
					string strFileName = (string) Session["printFile"];
					int iCostPerCopy = Int32.Parse( (string) ConfigurationSettings.AppSettings["cost_per_copy"] );
					string strTotalCost = (Int32.Parse( (string) Session["NoOfCopies"]) * iCostPerCopy) + "";

					FileInfo fileInfo = new FileInfo(strDirectory + "\\" + strFileName );
					string strFileSize = fileInfo.Length + "";

					// Create a 'long' Random number Job Id.
					Random rnd = new Random();
					string strJobId = rnd.Next() + "";

					// Get the printer_id from the printer table using printer_guid.
					SqlConnection sqlConn = new SqlConnection(strDbURL);
					sqlConn.Open();

					string strSQL = "select area_code, printer_id, ept_host_ip, ept_host_portno from printer where printer_guid='" +
					strPrinterGuid + "'";

					SqlCommand sqlCmd = new SqlCommand(strSQL,sqlConn);
					SqlDataReader sqlDataReader = sqlCmd.ExecuteReader();

					string strAreaCode = null;
					string strPrinterId = null;
					string strEptHostIP = null;
					string strEptPortNo = null;

					while(sqlDataReader.Read() )
					{
						strAreaCode = sqlDataReader.GetString(0);
						strPrinterId = sqlDataReader.GetString(1);
						strEptHostIP = sqlDataReader.GetString(2);
						strEptPortNo = sqlDataReader.GetString(3);
					}
							
					sqlDataReader.Close();
					sqlConn.Close();

					// Creating a new job and placing it in the database.
					DBAccess dbAccess = new DBAccess(strDbURL);
					dbAccess.insertPrintJob(strAreaCode + "_" + strPrinterId, strJobId,
								 (string) Session["NoOfCopies"],
								 strTotalCost, 
								 strFileName, 
								 strFileSize);
							
					JobInfo jobInfo = new JobInfo(EASConstants.RT_NWJ,
							   strAreaCode + "_" + strPrinterId,
							   strJobId,
							   strTotalCost,
							   (string) Session["NoOfCopies"],
							   strFileName,
							   strFileSize,
							   strEptHostIP,
							   strEptPortNo);							  

					string strNewJobMsg = ResponseHandlers.GenerateNWJRequest(jobInfo); 
					// Placing the new job message in the queue.
					QueueManager qMgr = new QueueManager(strJobQueueName);
					EASDebug.LogException(strNewJobMsg);
					qMgr.sendMessage(strNewJobMsg,"NWJ");
				}

		}
		catch
		{
				// bindata() will handle the errors.
		}

		bindData();

	}

	private void bindData()
	{
		try
		{
				string strDBUrl = ConfigurationSettings.AppSettings["database_url"];

				SqlConnection conn = new SqlConnection(strDBUrl);
				SqlDataAdapter dataAdapt 
				= new SqlDataAdapter("select * from jobinfo where job_status not in ( '1','5') order by job_date", conn);

				DataSet ds = new DataSet();
				dataAdapt.Fill(ds, "jobinfo");

				jobsGrid.DataSource = ds.Tables["jobinfo"].DefaultView;
				jobsGrid.DataBind();

				lblNavigation.Text = "Viewing Page " + (jobsGrid.CurrentPageIndex + 1) +
									  " of " + jobsGrid.PageCount;
		  }
		  catch
		  {
				lblNavigation.Text = "This service is currently unavailable. Please try again after some time.";

		  }
	}

</script>


