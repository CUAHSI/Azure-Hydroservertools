using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Configuration;
using System.Net;
using System.Threading.Tasks;

using System.Runtime.Remoting.Messaging;

using log4net;
using log4net.Config;

using log4net.Appender;
using log4net.Repository.Hierarchy;

namespace HydroServerToolsUtilities
{

	/// <summary>
	/// A base class for use with the log4net AdoNetAppender...
	/// </summary>
	public abstract class DbBaseContext
	{
		protected class ids
		{
			public string sessionId { get; set; }
			public string userIpAddress { get; set; }
			public string domainName { get; set; }
		}

        protected class bulkUploadIds
        {
            public string googleEmailAddress { get; set; }
            public string networkApiKey { get; set; }
        }

		//members...
        //Unique Ids to collections of parameter names and values
        protected static Dictionary<int, Dictionary<string, string>> m_dictParams = new Dictionary<int, Dictionary<string,string>>();

        protected ILog m_loggerDB;

		protected AdoNetAppender m_appender;

		protected string m_localConnectionString;

		protected string m_deployConnectionString;

		protected static Object lockObject = new Object();

        protected static Dictionary<int, ids> m_dictUniqueIdsToIds = new Dictionary<int, ids>();

        protected static Dictionary<int, bulkUploadIds> m_dictUniqueIdsToBulkUploadIds = new Dictionary<int, bulkUploadIds>();

        //Initializing constructor
        protected DbBaseContext(string loggerName, string adoNetAppenderName, string localConnectionStringKey, string deployConnectionStringKey)
		{
			//Validate/initialize input parameters...
			if ( String.IsNullOrWhiteSpace(loggerName) || 
				 String.IsNullOrWhiteSpace(adoNetAppenderName) ||
				 String.IsNullOrWhiteSpace(localConnectionStringKey) ||
				 String.IsNullOrWhiteSpace(deployConnectionStringKey) )
			{
				throw new ArgumentNullException("Empty parameter!!");
			}

			//Retrieve the logger instance...
			m_loggerDB = LogManager.GetLogger(loggerName);
			if (null == m_loggerDB)
			{
				throw new KeyNotFoundException(String.Format("Log4net logger: {0} NOT found!!", loggerName)); //Logger not found!!
			}

			//Retrieve the appender instance...
			m_appender = findAppender(adoNetAppenderName);
			if (null == m_appender)
			{
				throw new KeyNotFoundException(String.Format("Log4net AdoNetAppender: {0} NOT found!!", adoNetAppenderName)); //Appender not found!!
			}

			//Retrieve the local connection string...
			ConnectionStringSettings css = ConfigurationManager.ConnectionStrings[localConnectionStringKey];
			if (null != css)
			{
				m_localConnectionString = css.ConnectionString;
			}
			else
			{
				throw new KeyNotFoundException(String.Format("Connection String Key: {0} NOT found!!", localConnectionStringKey)); //Local connection string not found!!
			}

			//Retrieve the deploy connection string...
			css = ConfigurationManager.ConnectionStrings[deployConnectionStringKey];
			if (null != css)
			{
				m_deployConnectionString = css.ConnectionString;
			}
			else
			{
				throw new KeyNotFoundException(String.Format("Connection String Key: {0} NOT found!!", deployConnectionStringKey)); //Deploy connection string not found!!
			}

			//Set the appender's connection string...
			m_appender.ConnectionString = GetConnectionString();
			m_appender.ActivateOptions();

            //Assign newly constructed 
		}

		//methods

		//Source: http://mylifeandcode.blogspot.com/2012/12/setting-log4net-adonetappender.html
		private AdoNetAppender findAppender(string appenderName)
		{
			//Validate/initialize input parameters...
			if (String.IsNullOrWhiteSpace(appenderName))
			{
				return null;
			}

			Hierarchy hierarchy = LogManager.GetRepository() as Hierarchy;
			if (null != hierarchy)
			{

                var appenders = hierarchy.GetAppenders();

                AdoNetAppender appender = (AdoNetAppender)hierarchy.GetAppenders()
											.Where(x => x.GetType() == typeof(AdoNetAppender) && appenderName == x.Name)
											.FirstOrDefault();
				return appender;
			}

			return null;
		}

		//Retrieve connection string per current run-time environment...
		//source: http://cloudmonix.com/blog/how-to-check-if-code-is-running-on-azure-webapps/
		//		  Other Azure environment variables of possible interest:
		//			WEBSITE_HOSTNAME
		//			WEBSITE_IIS_SITE_NAME
		//			WEBSITE_OWNER_NAME
		private string GetConnectionString()
		{
			if (String.IsNullOrEmpty(Environment.GetEnvironmentVariable("WEBSITE_SITE_NAME")))
			{
				//Local environment...
				return m_localConnectionString;
			}
			else
			{
				//Deployed environment...
				return m_deployConnectionString;
			}
		}

		public void getIds(HttpContext httpcontextCurrent, ref string sessionId, ref string userIpAddress, ref string domainName)
		{
            sessionId = String.Empty;
            userIpAddress = String.Empty;
            domainName = String.Empty;

			if ( null == httpcontextCurrent)
			{
				//If no http context (running in an async task) check the dictionary for the 'Call Context' unique id...
				if ( null != CallContext.LogicalGetData("uniqueId"))
				{
					//'Call Context' unique id found - retrieve associated values...
					int uniqueId = (int) CallContext.LogicalGetData("uniqueId");
                  
					lock (lockObject)
					{
						if (m_dictUniqueIdsToIds.ContainsKey(uniqueId))
						{
							ids myIds = m_dictUniqueIdsToIds[uniqueId];

							sessionId = myIds.sessionId;
							userIpAddress = myIds.userIpAddress;
							domainName = myIds.domainName;
						}
					}
				}

				return;	//Retun early...
			}

			//Retrieve Session Id
			var httpContext = new HttpContextWrapper(httpcontextCurrent);
			sessionId = (null != httpContext.Session) ? httpContext.Session.SessionID : ulConstants.Unknown;

			//Retrieve user's IP address and domain name...
			//userIpAddress = ContextUtil.GetIPAddress(System.Web.HttpContext.Current);
            userIpAddress = ContextUtil.GetIPAddress(httpcontextCurrent);

            try
            {
				IPHostEntry host = Dns.GetHostEntry(userIpAddress);
				domainName = host.HostName;
			}
			catch (Exception exceptionDNS)
			{
				domainName =  exceptionDNS.Message;
			}

			//Processing complete - return
			return;
		}

        public void getBulkUploadIds(ref string networkApiKey, ref string googleEmailAddress)
        {
            googleEmailAddress = String.Empty;
            networkApiKey = String.Empty;

            //Check for the 'Call Context' unique id...
            if (null != CallContext.LogicalGetData("uniqueId"))
            {
                //'Call Context' unique id found - retrieve associated values...
                int uniqueId = (int)CallContext.LogicalGetData("uniqueId");

                lock (lockObject)
                {
                    if (m_dictUniqueIdsToBulkUploadIds.ContainsKey(uniqueId))
                    {
                        bulkUploadIds myIds = m_dictUniqueIdsToBulkUploadIds[uniqueId];

                        networkApiKey = myIds.networkApiKey;
                        googleEmailAddress = myIds.googleEmailAddress;
                    }
                }
            }
        }

		//Save input ids (used in logging) under a unique Id...
        public void saveIds( int uniqueId, string sessionId, string userIpAddress, string domainName,
                                            string networkApiKey = null, string googleEmailAddress = null)
		{
			ids myIds = new ids();
			myIds.sessionId = sessionId;
			myIds.userIpAddress = userIpAddress;
			myIds.domainName = domainName;

			lock (lockObject)
			{
				if (!m_dictUniqueIdsToIds.ContainsKey(uniqueId))
				{
					m_dictUniqueIdsToIds.Add(uniqueId, myIds);

                    if (!(String.IsNullOrWhiteSpace(googleEmailAddress) || String.IsNullOrWhiteSpace(networkApiKey)))
                    {
                        var myBulkUploadIds = new bulkUploadIds();
                        myBulkUploadIds.googleEmailAddress = googleEmailAddress;
                        myBulkUploadIds.networkApiKey = networkApiKey;

                        m_dictUniqueIdsToBulkUploadIds.Add(uniqueId, myBulkUploadIds);
                    }
				}
			}
		}

		//Remove ids previously associated with a unique Id...
		public void removeIds(int uniqueId)
		{
			lock (lockObject)
			{
				if (m_dictUniqueIdsToIds.ContainsKey(uniqueId))
				{
					m_dictUniqueIdsToIds.Remove(uniqueId);

                    m_dictUniqueIdsToBulkUploadIds.Remove(uniqueId);
				}
			}
		}


		//interface...

        //Clear parameters per the call context...
		public void clearParameters()
		{
            lock (lockObject)
            {
                //Check call context for uniqueId...
                if (null != CallContext.LogicalGetData("uniqueId"))
                {
                    int uniqueId = (int)CallContext.LogicalGetData("uniqueId");
                    if (m_dictParams.Keys.Contains(uniqueId))
                    {
                        //Associated parameters dictionary found - clear...
                        var dictParams = m_dictParams[uniqueId];
                        dictParams.Clear();
                    }
                }
            }
        }

        //Add parameter per the call context...
		public void addParameter<Type>(string name, Type parameter)
		{
            //Validate/initialize input parameters...
			if (!String.IsNullOrWhiteSpace(name))
			{
                //Input parameters valid - check call context for uniqueId...
                lock (lockObject)
                {
                    if (null != CallContext.LogicalGetData("uniqueId"))
                    {
                        int uniqueId = (int)CallContext.LogicalGetData("uniqueId");
                        Dictionary<string, string> dictParams = null;

                        if (m_dictParams.Keys.Contains(uniqueId))
                        {
                            //Associated parameters dictionary found
                            dictParams = m_dictParams[uniqueId];
                        }
                        else
                        {
                            //Parameters dictionary not found - add
                            dictParams = new Dictionary<string, string>();
                            m_dictParams.Add(uniqueId, dictParams);
                        }

                        //Add input value to parameters dictionary...
                        dictParams.Add(name, ((null != parameter) ? parameter.ToString() : "null value..."));
                    }
                }
            }
		}

        //Retrieve parameter(s), if any, in JSON format...
        //Source: http://stackoverflow.com/questions/23729477/converting-dictionary-string-string-to-json-string
        public string getParametersAsJson()
        {
            string json = "{}";
            lock (lockObject)
            {
                //Check call context for uniqueId...
                if (null != CallContext.LogicalGetData("uniqueId"))
                {
                    int uniqueId = (int)CallContext.LogicalGetData("uniqueId");
                    if (m_dictParams.Keys.Contains(uniqueId))
                    {
                        //Associated parameters dictionary found - retrieve parameters...
                        var dictParams = m_dictParams[uniqueId];
                        if (0 < dictParams.Count)
                        {
                            var kvs = dictParams.Select(kvp => string.Format("\"{0}\":\"{1}\"", kvp.Key, string.Join(",", kvp.Value)));
                            json = string.Concat("{", string.Join(",", kvs), "}");
                        }
                    }
                }
            }

            return json;
        }

    }
}