using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HydroServerToolsUtilities
{
	//A simple class representing the currently authenticated user (via Google login)
	public class CurrentUser
	{
		public CurrentUser( String userName, String eMail, bool authenticated)
		{
			Authenticated = authenticated;
			UserEmail = eMail;
			UserName = userName;
		}

		//Properties
		public bool Authenticated { get; set; }
		
		public string UserName { get; set; }

		public string UserEmail { get; set; }
	}
}