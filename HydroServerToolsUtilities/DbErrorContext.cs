using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using log4net;

namespace HydroServerToolsUtilities
{
	/// <summary>
	/// A derived class for use with the AdoNetAppender...
	/// </summary>
//	public class DbErrorContext : DbBaseContext
	public class DbErrorContext : DbBaseContext
	{
        //Singleton instance for 'sharing' of current instance among different projects in the application...
        private static DbErrorContext _instance;

        //Initializing Constructor
        public DbErrorContext(string loggerName, string adoNetAppenderName, string localConnectionStringKey, string deployConnectionStringKey) 
					: base(loggerName, adoNetAppenderName, localConnectionStringKey, deployConnectionStringKey)
        {
            //Set singleton instance during construction...
            _instance = this;
        }

        //Access singleton instance...
        public static DbErrorContext Instance
        {
            get
            {
                return _instance;
            }
        }

		//Write an entry to the log table
		public void createLogEntry(HttpContext httpcontextCurrent, DateTime occurrenceDtUtc, string methodName, Exception exception, string exceptionMessage)
		{
			//Validate/initialize input parameters...
			//NOTE: If running under a Task - httpcontextCurrent may legitimately be null!!
			if ( null == occurrenceDtUtc ||
				 String.IsNullOrWhiteSpace(methodName) ||
				 null == exception ||
				 String.IsNullOrWhiteSpace(exceptionMessage))
			{
				return;		//Invalid parameter - return early...
			}

			//Retrieve session id, IP address and domain name...
			string sessionId = String.Empty;
			string userIpAddress = String.Empty;
			string domainName = String.Empty;

			getIds(httpcontextCurrent, ref sessionId, ref userIpAddress, ref domainName);

			createLogEntry(sessionId, userIpAddress, domainName, occurrenceDtUtc, methodName, exception, exceptionMessage);

			return;
		}

		//Write and entry to the log table (for use when an HttpContext is not available...)
		public void createLogEntry(string sessionId, string userIpAddress, string domainName, DateTime occurrenceDtUtc, string methodName, Exception exception, string exceptionMessage)
		{
			//Validate/initialize input parameters...
			if (String.IsNullOrWhiteSpace(sessionId) ||
				 String.IsNullOrWhiteSpace(userIpAddress) ||
				 String.IsNullOrWhiteSpace(domainName) ||
				 null == occurrenceDtUtc ||
				 String.IsNullOrWhiteSpace(methodName) ||
				 null == exception ||
				 String.IsNullOrWhiteSpace(exceptionMessage) )
			{
				return;		//Invalid parameter - return early...
			}

			//Write derived and input values to MDC...
			MDC.Clear();

			MDC.Set("SessionId", sessionId);
			MDC.Set("IPAddress", userIpAddress);
			MDC.Set("Domain", domainName);
			MDC.Set("OccurrenceDateTime", occurrenceDtUtc.ToString());
			MDC.Set("MethodName", methodName);
			MDC.Set("ExceptionType", exception.GetType().ToString());
			MDC.Set("ExceptionMessage", exceptionMessage);

            //Convert parameters to JSON and write to MDC...
            string json = getParametersAsJson();
            MDC.Set("Parameters", json);

			//Write to the log...
			string logMessage = "log message";	//NOTE: Due to MDC usage and AdoNetAppender usage, this message is not logged..

			m_loggerDB.Error(logMessage);

			//Processing complete - return
			return;
		}

	}
}