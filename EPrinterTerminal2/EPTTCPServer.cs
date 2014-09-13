/********************************************************************
 * Author: Naveen Karamchetti  
 * Description: This class is a TCP Server that listens on a specified
 * port.
 ********************************************************************/

using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.IO;
using System.Xml;

namespace EPrinterTerminal
{
	/// <summary>
	/// TCP server
	/// </summary>
	public class EPTTCPServer
	{
		/// <summary>
		/// Holds the port no.
		/// </summary>
		private int iPortNo = 55555;

		/// <summary>
		/// Holds the TCP thread reference.
		/// </summary>
		private Thread thrdTCP = null;
	
		/// <summary>
		/// The constructor of the class.
		/// </summary>
		/// <param name="iPort">The port no</param>
		public EPTTCPServer(int iPort)
		{
			try
			{
				this.iPortNo = iPort;
				thrdTCP = new Thread( new ThreadStart(this.run));
				// Background threads do not prevent a process from terminating.
				// Set this to TRUE in production.
				thrdTCP.IsBackground = true;
				thrdTCP.Start();
				EPTDebug.LogException("EPTTCPServer::TCP Server Thread started");
			}
			catch(Exception e)
			{
				EPTDebug.LogException("EPTTCPServer::"+ e.ToString() );
			}
		}

		/// <summary>
		/// Stops the TCP server.
		/// </summary>
		public void stopTCPServer()
		{
			if ( thrdTCP != null)
			{
				try
				{
					EPTDebug.LogException("EPTTCPServer::stopTCPServer");
					thrdTCP.Abort();
					thrdTCP = null;
				}
				catch(Exception thrdException)
				{
					EPTDebug.LogException("EPTTCPServer::"+ thrdException.ToString() );
				}
			}
		}

		/// <summary>
		/// Suspends the TCP server thread.
		/// </summary>
		public void suspendTCPServer()
		{
			if ( thrdTCP != null)
			{
				try
				{
					EPTDebug.LogException("EPTTCPServer::suspendTCPServer");
					thrdTCP.Suspend();
				}
				catch(Exception thrdException)
				{
					EPTDebug.LogException("EPTTCPServer:suspendTCPServer:"+ thrdException.ToString() );
				}
			}
		}

		/// <summary>
		/// Resumes the TCP Server thread.
		/// </summary>
		public void resumeTCPServer()
		{
			if ( thrdTCP != null)
			{
				try
				{
					EPTDebug.LogException("EPTTCPServer::resumeTCPServer");
					if ( thrdTCP.ThreadState.Equals(ThreadState.Suspended))
					{
						thrdTCP.Resume();
					}
				}
				catch(Exception thrdException)
				{
					EPTDebug.LogException("EPTTCPServer:resumeTCPServer:"+ thrdException.ToString() );
				}
			}
		}


		/// <summary>
		/// Runs as a separate thread.
		/// </summary>
		private void run()
		{
			while(true)
			{
				try
				{
					this.executeTCPServer();
				}
				catch(Exception e)
				{
					EPTDebug.LogException("EPTTCPServer::" + e.ToString() );					
				}
			}
		}

		/// <summary>
		/// Starts the TCP Server which listens on a port...
		/// </summary>
		private void executeTCPServer()
		{
			TcpListener tcpListener = null;

			try
			{
				IPHostEntry ipHost = Dns.Resolve(Dns.GetHostName() );
				IPAddress[] ipAddr = ipHost.AddressList;
				//Creates an instance of the TcpListener class by providing a local IP address and port number.
				tcpListener = new TcpListener(ipAddr[0], iPortNo);    
				EPTDebug.LogException("ExecuteTCPServer::bound on ip:" + ipAddr[0].ToString() + " :" + iPortNo );
				tcpListener.Start();

				EPTDebug.LogException("ExecuteTCPServer::Waiting for requests from the client...");

				// Accepts the pending client connection and returns a socket for communciation.
				Socket socket = null;

				while( (socket = tcpListener.AcceptSocket()) != null )
				{
					EPTDebug.LogException("ExecuteTCPServer::Client connected...");
		
					// NOTE: The server will not accept more than 1MB of data.
					byte[] bData = new byte[1024];
					int iBytes = 0;
					string strClientRequest = null;
					string strPrinterId = null;
					string strResponse = null;
										
					iBytes = socket.Receive(bData, bData.Length, 0);

					strClientRequest = Encoding.ASCII.GetString(bData, 0, iBytes);
					EPTDebug.LogException("ExecuteTCPServer::client data :" + strClientRequest);
					
					// Handle NWJ requests.
					if ( strClientRequest != null)
					{
						try
						{
							StringReader sr = new StringReader(strClientRequest);
							XmlTextReader xtr = new XmlTextReader(sr);
							XmlResponseParser xrp = new XmlResponseParser(xtr);
							strPrinterId = xrp.getPrinterId();
							JobInfo jobInfo = new JobInfo(xrp.getRequest(),
															strPrinterId,
															xrp.getJobId(),
															xrp.getJobCost(),	
															xrp.getJobCopies(),
															xrp.getFileName(),
															xrp.getFileSize() );
							
							// This job information must be transmitted to the main pgm.
							JobInfoManager.Instance.putJobInfo(jobInfo);

							strResponse = EPTXmlResponse.GetResponse("200",EPTConstants.RT_NWJ,strPrinterId);
						}
						catch(Exception xmlParseException)
						{
							EPTDebug.LogException("ExecuteTCPServer::" + xmlParseException.ToString() );
							strResponse = EPTXmlResponse.GetResponse("403",EPTConstants.RT_NWJ,"");
						}
					}
					else
					{
						strResponse = EPTXmlResponse.GetResponse("403",EPTConstants.RT_NWJ,"");
					}
					
					//Forms and sends a response string to the connected client.
					Byte[] sendBytes = Encoding.ASCII.GetBytes(strResponse);
					int i = socket.Send(sendBytes);
					socket.Close();
					EPTDebug.LogException("ExecuteTCPServer::Reply sent to the client: " + strResponse);
					EPTDebug.LogException("ExecuteTCPServer::Waiting for requests...");
				}
	
			}
			catch (Exception err)
			{
				if ( tcpListener != null)
				{
					tcpListener.Stop();
					EPTDebug.LogException("EPTTCPServer::Stopping...");
					tcpListener = null;
				}

				EPTDebug.LogException("EPTTCPServer::"+ err.ToString() );
				throw err;
			}

		}

	}
}
