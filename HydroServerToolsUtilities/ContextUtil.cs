using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HydroServerToolsUtilities
{
    public static class ContextUtil
    {
        //Utility methods...
        public static string GetIPAddress(System.Web.HttpContext contextIn)
        {
            //Validate/initialize input parameters...
            if ( null == contextIn )
            {
                return (ulConstants.Unknown);  //Invalid parameter - return early...
            }

            string ipAddress = contextIn.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

            if (!string.IsNullOrEmpty(ipAddress))
            {
                string[] addresses = ipAddress.Split(',');
                if (addresses.Length != 0)
                {
					if ("::1" != addresses[0])
					{
						return addresses[0].Split(':')[0];	//Remove trailing port number, if any
					}

					return addresses[0];
                }
            }

            ipAddress = contextIn.Request.ServerVariables["REMOTE_ADDR"];
		
			if (!string.IsNullOrEmpty(ipAddress))
			{
				string[] addresses = ipAddress.Split(',');
				if (addresses.Length != 0)
				{
					if ("::1" != addresses[0])
					{
						return addresses[0].Split(':')[0];	//Remove trailing port number, if any
					}

					return addresses[0];
				}
			}

			return ulConstants.Unknown;
		}
    }
}