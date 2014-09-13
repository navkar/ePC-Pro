/********************************************************************
 * Author: Naveen Karamchetti  
 * Description: This class is used to establish connections to the server.
 ********************************************************************/

using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.IO;
using System.Xml;

namespace EPrinterTerminal
{
	/// <summary>
	/// This class is responsible for establishing the connection with the server
	/// and sending the requests and receiving the responses.
	/// </summary>
	public class ConnectionManager
	{
		/// <summary>
		/// The Boundary string.
		/// </summary>
		private static string BOUNDARY = EPTConstants.BOUNDARY;

		/// <summary>
		/// The Host Name.
		/// </summary>
		private string strHostName = null;

		/// <summary>
		/// The Port No.
		/// </summary>
		private int iPortNo = 80;
		
		/// <summary>
		/// Sets the '.aspx' page where the client's data gets posted.
		/// </summary>
		/// <param name="strEntryPointPath"></param>
		public static void setEntryPointPath(string strEntryPointPath)
		{
			EPTConstants.ENTRY_POINT = strEntryPointPath;
		}

		/// <summary>
		/// The constructor of the class.
		/// </summary>
		/// <param name="strHostName">The host name.</param>
		/// <param name="strPortNo">The port no.</param>
		public ConnectionManager(string strHostName, int iPortNo)
		{
			this.strHostName = strHostName;
			this.iPortNo = iPortNo;
		}

		/// <summary>
		/// Adds starting and ending boundarty to the request.
		/// </summary>
		/// <param name="strRequestData">The XML request data.</param>
		/// <returns>Returns the request data with boundary appended.</returns>
		private string appendBoundaryToRequest(string strRequestData)
		{
			return "--" + BOUNDARY + "\r\n" + strRequestData + "--" + BOUNDARY + "--" + "\r\n";
		}

		/// <summary>
		/// Sends a Xml 'POST' request to the server.
		/// </summary>
		/// <param name="strXmlFormData">The XML POST form data.</param>
		/// <returns>The XML data received from the server.</returns>
		public string sendPostRequest(string strXmlFormData)
		{
			try
			{
				strXmlFormData = appendBoundaryToRequest(strXmlFormData);
				
				//Set up the variables and string to write to the server
				Encoding ASCII = Encoding.ASCII;
				string Post = 

					"POST "+ EPTConstants.ENTRY_POINT +" HTTP/1.0\r\n" +
					"Host: " + this.strHostName + "\r\n" +
					"Content-type: multipart/form-data; charset=ISO-8859-1; boundary=" + BOUNDARY + "\r\n" +
					"Content-length: " + strXmlFormData.Length + "\r\n" +  
					"Connection: close\r\n\r\n" +
					strXmlFormData;

				Byte[] ByteGet = ASCII.GetBytes(Post);
 
				// IPAddress and IPEndPoint represent the endpoint that will
				// receive the request.
				// Get first IPAddress in list return by DNS
				IPAddress hostadd = Dns.Resolve(this.strHostName).AddressList[0];
				IPEndPoint EPhost = new IPEndPoint(hostadd, this.iPortNo);
 
				//Create the Socket for sending data over TCP
				Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Stream,
					ProtocolType.Tcp );
 
				// Connect to host using IPEndPoint
				s.Connect(EPhost);
				if (!s.Connected)
				{
					return EPTXmlResponse.GetResponse("911","","");
				}
 
				// Sent the POST text to the host
				s.Send(ByteGet, ByteGet.Length, 0);
 
				Byte[] RecvBytes = new Byte[1024];
				Int32 bytes = 0;
				Int32 totalBytes = 0;
 
				do
				{
					bytes = s.Receive(RecvBytes, RecvBytes.Length, 0);
					totalBytes += bytes;
				}
				while (bytes > 0);

				// Removing the header part of the response.
				bool bNewLineFlag = false;
				int i = 0;
				for ( i = 0 ; i < totalBytes; i++)
				{
					if ( RecvBytes[i] == '\r' )
					{

					}
					else if ( RecvBytes[i] == '\n' )
					{
						if ( bNewLineFlag)
							break;

						bNewLineFlag = true;		
					}
					else
					{
						bNewLineFlag = false;
					}

				}

				i++;

				// The body part of the response is returned.
				return ASCII.GetString(RecvBytes, i, totalBytes - i);
			}
			catch (System.Net.Sockets.SocketException se)
			{
				EPTDebug.LogException("ConnectionManager:"+ se.ToString() );
				return EPTXmlResponse.GetResponse("911","","");
			}
			catch (Exception e)
			{
				EPTDebug.LogException("ConnectionManager:"+ e.ToString() );
				return EPTXmlResponse.GetResponse("911","","");
			}

		}

		/// <summary>
		/// Sends the GJF request and downloads the file.
		/// </summary>
		/// <param name="iFileDownloadSize">The file size.</param>
		/// <param name="strPath">The local path to save the file.</param>
		/// <param name="strSaveFileName">The file name to be saved.</param>
		/// <param name="strXmlFormData">The GJF request data.</param>
		public void sendGetJobFileRequest(int iFileDownloadSize, 
							string strPath,
							string strSaveFileName,  string strXmlFormData)
		{
			try
			{
				strXmlFormData = appendBoundaryToRequest(strXmlFormData);
				// Set up the variables and string to write to the server.
				Encoding ASCII = Encoding.ASCII;
				string Post = 

					"POST "+ EPTConstants.ENTRY_POINT +" HTTP/1.0\r\n" +
					"Host: " + this.strHostName + "\r\n" +
					"Content-type: multipart/form-data; charset=ISO-8859-1; boundary=" + BOUNDARY + "\r\n" +
					"Content-length: " + strXmlFormData.Length + "\r\n" +  
					"Connection: close\r\n\r\n" +
					strXmlFormData;

				Byte[] ByteGet = ASCII.GetBytes(Post);
				//String strRetPage = null;
 
				// IPAddress and IPEndPoint represent the endpoint that will
				// receive the request.
				// Get first IPAddress in list return by DNS
				IPAddress hostadd = Dns.Resolve(this.strHostName).AddressList[0];
				IPEndPoint EPhost = new IPEndPoint(hostadd, this.iPortNo);
 
				//Create the Socket for sending data over TCP
				Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Stream,
					ProtocolType.Tcp );
 
				// Connect to host using IPEndPoint
				s.Connect(EPhost);
				if (!s.Connected)
				{
					EPTDebug.LogException("ConnectionManager::sendGetJobFileRequest::Not connected");
					throw new Exception("NOT_CONNECTED");
				}
 
				// Sent the POST text to the host
				s.Send(ByteGet, ByteGet.Length, 0);
 
				// Receive the page, loop until all bytes are received.
				// The server must tell the client in advance the size of the
				// file before the file download. It will enable the client
				// to reserve the buffer space for the file storage.
				Byte[] RecvBytes = new Byte[iFileDownloadSize + 500];
				Int32 bytes = 0;
				Int32 totalBytes = 0;
 
				do
				{
					bytes = s.Receive(RecvBytes, RecvBytes.Length, 0);
					totalBytes += bytes;
				}
				while (bytes > 0);

				EPTDebug.LogException("ConnectionManager:sendGetJobFileRequest: No of bytes downloaded in "+ strSaveFileName + " =" + totalBytes);

				// Separating out the headers in the code.
				bool bNewLineFlag = false;
				int i = 0;
				for ( i = 0 ; i < totalBytes; i++)
				{
					if ( RecvBytes[i] == '\r' )
					{

					}
					else if ( RecvBytes[i] == '\n' )
					{
						if ( bNewLineFlag)
							break;

						bNewLineFlag = true;		
					}
					else
					{
						bNewLineFlag = false;
					}

				}

				i++;
				
				EPTDebug.LogException("ConnectionManager:sendGetJobFileRequest: Bytes Downloaded :" + (totalBytes - i) );
				
				//--------------------------------------------------------------
				if ( (totalBytes - i) < 500 )
				{
					//Check for Job Cancellation.
					string strData = ASCII.GetString(RecvBytes, i, totalBytes - i);
					StringReader sr = new StringReader(strData);
					XmlTextReader xtr = new XmlTextReader(sr);
					XmlResponseParser xrp = new XmlResponseParser(xtr);

					// JOB is CANCELLED
					if (xrp.getErrorFlag() == false)
					{
						EPTDebug.LogException("ConnectionManager:: Return Code: " + xrp.getReturnCode());
						throw new Exception("JOB_CANCELLED");
					}
				}
				//--------------------------------------------------------------

				// Create the directory before saving the file.
				DirectoryInfo dirInfo = new DirectoryInfo(strPath);

				if (dirInfo.Exists == false)
				{
					dirInfo.Create();
				}
	
				// The file stream is written to the file.
				FileStream fs = new FileStream(strPath + "\\" + strSaveFileName, FileMode.Create, 
					FileAccess.Write);

				// Saving the actual file on the disk.
				BinaryWriter bw = new BinaryWriter(fs);
				bw.Write(RecvBytes, i, totalBytes - i);
				bw.Flush();
				bw.Close();

			}
			catch (System.Net.Sockets.SocketException se)
			{
				EPTDebug.LogException("ConnectionManager:sendGetJobFileRequest:" + se.ToString() );
				throw new Exception("JOB_DOWNLOAD_ERROR");
			}

		}


	}
}
