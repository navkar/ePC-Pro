/********************************************************************
 * Author: Naveen Karamchetti  
 * Description: Job Information Manager. This is a singleton class that holds
 * the jobs for the printer.
 ********************************************************************/

using System;
using System.Collections;

namespace EPrinterTerminal
{
	/// <summary>
	/// Maintains a Queue of Print Jobs, this is a singleton class.
	/// </summary>
	public sealed class JobInfoManager
	{
		/// <summary>
		/// Creating a Singleton Class.
		/// </summary>
		public static readonly JobInfoManager Instance = new JobInfoManager();	

		/// <summary>
		/// A Delegate for the event. 
		/// </summary>
		public delegate void StartProcessingJobs();
	
		/// <summary>
		/// An Event which indicates the PrintJobsArrival. 
		/// </summary>
		public event StartProcessingJobs PrintJobsArrival;
		
		/// <summary>
		/// The Queue data structure.
		/// </summary>
		private Queue jobsQueue = null;

		/// <summary>
		/// The constructor of the class, this is instantiated only once.
		/// </summary>
		private JobInfoManager()
		{
			jobsQueue = new Queue();
			EPTDebug.LogException("JobInfoManager::Created the Job Information Manager.");
		}

		/// <summary>
		/// Places the Print Job data object into the queue.
		/// </summary>
		/// <param name="jobInfo">The Job Info data object.</param>
		public void putJobInfo(JobInfo jobInfo)
		{
			if ( jobsQueue != null && jobInfo != null)
			{
				jobsQueue.Enqueue(jobInfo);
				EPTDebug.LogException("JobInfoManager:: new job placed into the queue.");
				PrintJobsArrival();
			}
		}

		/// <summary>
		/// Gets the Job Information.
		/// </summary>
		/// <returns>The Job Info data object.</returns>
		public JobInfo getJobInfo()
		{
			if ( jobsQueue != null && jobsQueue.Count > 0 )
			{
				return (JobInfo) jobsQueue.Dequeue();
			}
			else
			{
				return null;
			}
		}

		/// <summary>
		/// Get the no of jobs in the queue.
		/// </summary>
		/// <returns>the count of no of jobs in the queue.</returns>
		public int getJobCount()
		{
			if ( jobsQueue != null)
			{
				return jobsQueue.Count;
			}
			else
			{
				return 0;
			}
		}

	}
}
