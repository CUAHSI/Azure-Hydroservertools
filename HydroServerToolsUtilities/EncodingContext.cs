using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.IO;
using System.Text;

namespace HydroServerToolsUtilities
{
    //A simple class for miscellaneous file encoding methods...
    public static class EncodingContext
    {
        //Retieve the encoding per the input content type --OR-- for the input file path and name, per the byte order mark...
        //Source: https://weblog.west-wind.com/posts/2007/Nov/28/Detecting-Text-Encoding-for-StreamReader
        public static Encoding GetFileEncoding(string contentType, string filePathAndName)
        {
            Encoding result = Encoding.Default; //Ansi codepage...

            //For now a simple check against the ms-excel type...
            //TO DO - More research needed to handle other content types!!
            //         Add logic to parse charset values if available...
            //Sources: https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/Content-Type
            //         https://developer.mozilla.org/en-US/docs/Web/HTTP/Basics_of_HTTP/MIME_types 
            //         https://developer.mozilla.org/en-US/docs/Web/HTTP/Basics_of_HTTP/MIME_types/Complete_list_of_MIME_types
            //         https://www.w3.org/International/questions/qa-choosing-encodings
            //NOTE: The content type can provide the data type and the encoding.  Some examples:
            //       Content-Type: text/html; charset=utf-8
            if (!String.IsNullOrWhiteSpace(contentType))
            {
                //Check content type...
                var contentTypeLower = contentType.ToLowerInvariant();
                if (-1 != contentTypeLower.IndexOf("application/vnd.ms-excel"))
                {
                    //return Encoding.Default;    //Default encoding for the .NET implementation
                    return Encoding.GetEncoding("iso-8859-1");      //Martin's encoding selection
                }
            }

            //Validate/initialize input parameters
            if (!String.IsNullOrWhiteSpace(filePathAndName))
            {
                //Attempt to retrieve byte order mark...
                byte[] buffer = new byte[5];
                try
                {
                    using (var fileStream = new FileStream(filePathAndName, FileMode.Open, FileAccess.Read, FileShare.None))
                    {
                        fileStream.Read(buffer, 0, 5);
                        fileStream.Close();
                    }
                }
                catch (Exception)
                {
                    //Error - return early
                    return result;
                }

                //Check byte order mark...
                if (buffer[0] == 0xef && buffer[1] == 0xbb && buffer[2] == 0xbf)
                {
                    result = Encoding.UTF8;
                }
                else if (buffer[0] == 0xfe && buffer[1] == 0xff)
                {
                    result = Encoding.Unicode;
                }
                else if (buffer[0] == 0 && buffer[1] == 0 && buffer[2] == 0xfe && buffer[3] == 0xff)
                {
                    result = Encoding.UTF32;
                }
                else if (buffer[0] == 0x2b && buffer[1] == 0x2f && buffer[2] == 0x76)
                {
                    result = Encoding.UTF7;
                }
            }

            //Processing complete - return result...
            return result;
        }

    }
}