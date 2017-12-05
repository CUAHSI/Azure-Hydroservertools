using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Threading;

namespace HydroServerTools.Utilities
{
    //A simple class for the association of prefixed file names with an access semaphore...
    public class FileContext
    {
        //Constructors...

        //Default...
        private FileContext()
        {
            FileNames = new List<string>();
            FileSemaphore = new SemaphoreSlim(1, 1);
        }

        //Initializing constructor...
        public FileContext( string fileNamePrefix, List<string> fileNames) : this()
        {
#if (DEBUG)
            //Validate/initialize input parameters...
            if (String.IsNullOrWhiteSpace(fileNamePrefix) || null == fileNames || 0 >= fileNames.Count)
            {
                string paramName = String.IsNullOrWhiteSpace(fileNamePrefix) ? "fileNamePrefix" : "fileNames";
                throw new ArgumentException("Invalid input parameter...", paramName);
            }
#endif
            //Update instance member(s)...
            FileNamePrefix = fileNamePrefix;

            foreach (var fileName in fileNames)
            {
                FileNames.Add(fileName);
            }
        }

        //Properties...
        private string FileNamePrefix { get; set; }

        public List<string> FileNames { get; private set; }

        public SemaphoreSlim FileSemaphore { get; private set; }

        //Methods...

        //Return a prefixed file name (success) or the empty string (failure)
        public string PrefixedFileName(string fileName)
        {
            string result = String.Empty;
            if (! String.IsNullOrWhiteSpace(fileName))
            {
                if ( -1 != FileNames.IndexOf(fileName))
                {
                    result = FileNamePrefix + "-" + fileName;
                }
            }

            //Processing complete - return result
            return result;
        }
    }
}