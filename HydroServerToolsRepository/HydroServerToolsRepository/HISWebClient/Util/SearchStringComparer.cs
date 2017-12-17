using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HISWebClient.Util
{
	public class SearchStringComparer : IEqualityComparer<String>
	{
		private StringComparison _stringComparison;

		//Constructors...
		public SearchStringComparer() : this(StringComparison.CurrentCultureIgnoreCase) { }

		public SearchStringComparer(StringComparison stringComparison)
		{
			_stringComparison = stringComparison;
		}

		public bool Equals(String strLHS, String strRHS)
		{
			bool result = true;	//Assume equals...
			string[] splitStrings = strRHS.Split(' ');

			foreach (string str in splitStrings)
			{
				if (!strLHS.Contains(str, _stringComparison))
				{
					result = false;
					break;
				}
			}

			return result;
		}

		public int GetHashCode(String str)
		{
			return str.GetHashCode();
		}

	}
}