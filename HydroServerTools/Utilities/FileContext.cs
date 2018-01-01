using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Threading;

namespace HydroServerTools.Utilities
{
    //A simple class for the association of a file name and a file (MIME) type
    public class FileNameAndType
    {
        //Constructors...

        //Default...
        private FileNameAndType() { }

        //Initializing
        public FileNameAndType( string fileName, string fileType)
        {
#if (DEBUG)
            if ( String.IsNullOrWhiteSpace(fileName) || String.IsNullOrWhiteSpace(fileType))
            {
                var paramName = String.IsNullOrWhiteSpace(fileName) ? "fileName" : "fileType";
                throw new ArgumentException("Invalid input parameter...", paramName);
            }
#endif
            this.fileName = fileName;
            this.fileType = fileType;
        }

        //Properties...
        public string fileName { get; set; }

        public string fileType { get; set; }
    }


    //A simple class for the association of prefixed file names with an access semaphore...
    public class FileContext
    {
        //Constructors...

        //Default...
        private FileContext()
        {
            FileNamesAndTypes = new List<FileNameAndType>();
            FileSemaphore = new SemaphoreSlim(1, 1);
        }

        //Initializing constructor...
        public FileContext( string fileNamePrefix, List<FileNameAndType> fileNamesAndTypes) : this()
        {
#if (DEBUG)
            //Validate/initialize input parameters...
            if (String.IsNullOrWhiteSpace(fileNamePrefix) || null == fileNamesAndTypes || 0 >= fileNamesAndTypes.Count)
            {
                string paramName = String.IsNullOrWhiteSpace(fileNamePrefix) ? "fileNamePrefix" : "fileNamesAndTypes";
                throw new ArgumentException("Invalid input parameter...", paramName);
            }
#endif
            //Update instance member(s)...
            FileNamePrefix = fileNamePrefix;

            foreach (var fileNameAndType in fileNamesAndTypes)
            {
                FileNamesAndTypes.Add( new FileNameAndType(fileNameAndType.fileName, fileNameAndType.fileType));
            }
        }

        //Properties...
        private string FileNamePrefix { get; set; }

        public List<FileNameAndType> FileNamesAndTypes { get; private set; }

        public SemaphoreSlim FileSemaphore { get; private set; }

        //Methods...

        //Return a prefixed file name (success) or the empty string (failure)
        public string PrefixedFileName(string fileName)
        {
            string result = String.Empty;
            if (! String.IsNullOrWhiteSpace(fileName))
            {
                foreach (var fileNameAndType in FileNamesAndTypes)
                {
                    if (fileNameAndType.fileName == fileName)
                    {
                        result = FileNamePrefix + "-" + fileName;
                        break;
                    }
                }
            }

            //Processing complete - return result
            return result;
        }
    }
}