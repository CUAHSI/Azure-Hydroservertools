using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HydroServerTools.Extensions
{
    //A simple class implementing HtmlHelper extensions...
    public static class HtmlHelperExtensions
    {
        //Indicate build type: true - DEBUG, false - release
        //Source: https://stackoverflow.com/questions/4696175/razor-view-engine-how-to-enter-preprocessorif-debug/7135343#7135343 
        public static bool IsDebugBuild(this HtmlHelper htmlHelper)
        {
#if (DEBUG)
            return true;
#else
            return false;
#endif
        }
    }
}