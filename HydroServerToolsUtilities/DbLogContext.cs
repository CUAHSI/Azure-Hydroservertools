using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Configuration;
using System.Globalization;
using System.Net;

using log4net;
using log4net.Appender;
using log4net.Core;

using System.Runtime.Remoting.Messaging;

namespace HydroServerToolsUtilities
{
	/// <summary>
	/// A derived class for use with the AdoNetAppender...
	/// </summary>
	public class DbLogContext : DbBaseContext
	{
		//members...
		//private Dictionary<string, string> m_dictReturns = new Dictionary<string,string>();
        private static Dictionary<int, Dictionary<string, string>> m_dictReturns = new Dictionary<int, Dictionary<string, string>>();

        private static Object lockObjectReturns = new object();

        //Singleton instance for 'sharing' of current instance among different projects in the application...
        private static DbLogContext _instance;

        //Initializing Constructor
        public DbLogContext(string loggerName, string adoNetAppenderName, string localConnectionStringKey, string deployConnectionStringKey) 
					: base(loggerName, adoNetAppenderName, localConnectionStringKey, deployConnectionStringKey)
        {
            _instance = this;
        }

        //Access singleton instance...
        public static DbLogContext Instance
        {
            get
            {
                return _instance;
            }
        }

        //interface...
        public void clearReturns()
		{
            lock (lockObjectReturns)
            {
                //Check call context for uniqueId...
                if (null != CallContext.LogicalGetData("uniqueId"))
                {
                    int uniqueId = (int)CallContext.LogicalGetData("uniqueId");
                    if (m_dictReturns.Keys.Contains(uniqueId))
                    {
                        //Associated returns dictionary found - clear...
                        var dictReturns = m_dictReturns[uniqueId];
                        dictReturns.Clear();
                    }
                }
            }
		}

		public void addReturn<Type>(string name, Type returnIn)
		{
            //Validate/initialize input parameters...
            if (!String.IsNullOrWhiteSpace(name))
			{
                //Input parameters valid - check call context for uniqueId...
                lock (lockObjectReturns)
                {
                    if (null != CallContext.LogicalGetData("uniqueId"))
                    {
                        int uniqueId = (int)CallContext.LogicalGetData("uniqueId");
                        Dictionary<string, string> dictReturns = null;

                        if (m_dictReturns.Keys.Contains(uniqueId))
                        {
                            //Associated parameters dictionary found
                            dictReturns = m_dictReturns[uniqueId];
                        }
                        else
                        {
                            //Parameters dictionary not found - add
                            dictReturns = new Dictionary<string, string>();
                            m_dictReturns.Add(uniqueId, dictReturns);
                        }

                        //Add input value to returns dictionary...
                        dictReturns.Add(name, ((null != returnIn) ? returnIn.ToString() : "null value..."));
                    }
                }
			}
		}


		//Write and entry to the log table (for use when an HttpContext is not available...)
		public void createLogEntry(string sessionId, string userIpAddress, string domainName, string networkApiKey, string googleEmailAddress,  
                                                     DateTime startDtUtc, DateTime endDtUtc, string methodName, string message, Level logLevel)
		{
			//Validate/initialize input parameters...
			if ( String.IsNullOrWhiteSpace(sessionId) ||
				 String.IsNullOrWhiteSpace(userIpAddress) ||
				 String.IsNullOrWhiteSpace(domainName) ||
                 String.IsNullOrWhiteSpace(networkApiKey) ||
				 String.IsNullOrWhiteSpace(googleEmailAddress) ||
				 null == startDtUtc ||
				 null == endDtUtc ||
				 String.IsNullOrWhiteSpace(methodName) ||
				 String.IsNullOrWhiteSpace(message) ||
				 null == logLevel)
			{
				return;		//Invalid parameter - return early...
			}

			//Write derived and input values to MDC...
			MDC.Clear();

			MDC.Set("SessionId", sessionId);
			MDC.Set("IPAddress", userIpAddress);
			MDC.Set("Domain", domainName);

            MDC.Set("NetworkApiKey", networkApiKey);
			MDC.Set("GoogleEmailAddress", googleEmailAddress);
			string dtFormat = "dd-MMM-yyyy HH:mm:ss.fff";
			MDC.Set("StartDateTime", startDtUtc.ToString(dtFormat, CultureInfo.CreateSpecificCulture("en-US")));
			MDC.Set("EndDateTime", endDtUtc.ToString(dtFormat, CultureInfo.CreateSpecificCulture("en-US")));
			MDC.Set("MethodName", methodName);
			MDC.Set("Message", message);
			MDC.Set("LogLevel", logLevel.DisplayName);

            //Convert parameters to JSON and write to MDC...
            string json = getParametersAsJson();
            MDC.Set("Parameters", json);

            //Convert returns to JSON and write to MDC...
            json = "{}";
            lock (lockObjectReturns)
            {
                //Check call context for uniqueId...
                if (null != CallContext.LogicalGetData("uniqueId"))
                {
                    int uniqueId = (int)CallContext.LogicalGetData("uniqueId");
                    if (m_dictReturns.Keys.Contains(uniqueId))
                    {
                        //Associated returns dictionary found - retrieve returns...
                        var dictReturns = m_dictReturns[uniqueId];
                        if (0 < dictReturns.Count)
                        {
                            var kvs = dictReturns.Select(kvp => string.Format("\"{0}\":\"{1}\"", kvp.Key, string.Join(",", kvp.Value)));
                            json = string.Concat("{", string.Join(",", kvs), "}");
                        }
                    }
                }
            }

            MDC.Set("Returns", json);

            //Write to the log per the input level...
            string logMessage = "log message";	//NOTE: Due to MDC usage and AdoNetAppender usage, this message is not logged..
			if (Level.Debug == logLevel)
			{
				m_loggerDB.Debug(logMessage);
			}
			else if (Level.Error == logLevel)
			{
				m_loggerDB.Error(logMessage);
			}
			else if (Level.Fatal == logLevel)
			{
				m_loggerDB.Fatal(logMessage);
			}
			else if (Level.Info == logLevel)
			{
				m_loggerDB.Info(logMessage);
			}
			else if (Level.Warn == logLevel)
			{
				m_loggerDB.Warn(logMessage);
			}

			//Processing complete - return
			return;
		}

		//Write an entry to the log table
		public void createLogEntry(HttpContext httpcontextCurrent, DateTime startDtUtc, DateTime endDtUtc, string methodName, string message, Level logLevel)
		{
			//Validate/initialize input parameters...
			//NOTE: If running under a Task - httpcontextCurrent may legitimately be null!!
			if (null == startDtUtc ||
				 null == endDtUtc ||
				 String.IsNullOrWhiteSpace(methodName) ||
				 String.IsNullOrWhiteSpace(message) ||
				 null == logLevel )
			{
				return;		//Invalid parameter - return early...
			}

			//Retrieve session id, IP address and domain name...
			string sessionId = String.Empty;
			string userIpAddress = String.Empty;
			string domainName = String.Empty;
            string networkApiKey = String.Empty;
            string googleEmailAddress = String.Empty;

			getIds(httpcontextCurrent, ref sessionId, ref userIpAddress, ref domainName);
            getBulkUploadIds(ref networkApiKey, ref googleEmailAddress);

			createLogEntry(sessionId, userIpAddress, domainName, networkApiKey, googleEmailAddress, startDtUtc, endDtUtc, methodName, message, logLevel);

			return;
		}

	}
}