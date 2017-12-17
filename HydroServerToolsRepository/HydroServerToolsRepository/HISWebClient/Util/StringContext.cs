using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HISWebClient.Util
{
	//Utility methods for strings...
	public static class StringContext
	{

		//Extension Contains method which accepts a StringComparison parameter...
		public static bool Contains(this string stringLHS, string stringRHS, StringComparison stringComparison)
		{
			//Validate/initialize input parameters...
			// NOTE: StringComparison parameter can never be null!!
			if ( null == stringLHS || null == stringRHS || //null == stringComparison ||
				 String.IsNullOrWhiteSpace(stringLHS) ||
				 String.IsNullOrWhiteSpace(stringRHS))
			{
				return false;
			}

			return stringLHS.IndexOf(stringRHS, stringComparison) >= 0;
		}

		//Extension Contains method which accepts a StringComparison parameter...
		public static bool Contains(this string stringLHS, string stringRHS, StringComparison stringComparison, SearchStringComparer ssc)
		{
			//Validate/initialize input parameters...
			// NOTE: StringComparison parameter can never be null!!
			if (null == stringLHS || null == stringRHS || //null == stringComparison ||
				 String.IsNullOrWhiteSpace(stringLHS) ||
				 String.IsNullOrWhiteSpace(stringRHS))
			{
				return false;
			}

			return ssc.Equals(stringLHS, stringRHS);
		}

	}
}