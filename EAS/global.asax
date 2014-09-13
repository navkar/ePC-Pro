<% @Import Namespace="System" %>
<% @Import Namespace="EAppService" %>

<script language="C#" runat="server">

     public void Application_OnStart() 
     {
	   string strLogFileName = ConfigurationSettings.AppSettings["log_file_name"];
	   string strLogFilePath = ConfigurationSettings.AppSettings["log_file_directory"];

		EASDebug.setLogFileName(strLogFilePath + "\\" + strLogFileName);
		EASDebug.LogException("global.asax:Application_OnStart() "); 
	 }          

     public void Application_BeginRequest() 
     {
         // Application code for each request could go here.
     }

     public void Application_OnEnd() 
     {
         // Application clean-up code goes here.
     }

</script>
