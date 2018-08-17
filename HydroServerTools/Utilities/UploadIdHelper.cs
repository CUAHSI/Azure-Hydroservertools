using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Concurrent;

using HydroServerToolsUtilities;
using HydroServerToolsUtilities.Validators;

namespace HydroServerTools.Utilities
{
    public class UploadIdHelper
    {

        //Constructors...
        private UploadIdHelper()
        {
            UploadSemaphore = new SemaphoreSlim(1, 1);
            UploadId = String.Empty;
        }

        public UploadIdHelper(string uploadId) : this()
        {
#if (DEBUG)
            //Validate/initialize input parameters...
            if (String.IsNullOrWhiteSpace(uploadId))
            {
                string paramName = "uploadId";
                throw new ArgumentException("Invalid input parameter...", paramName);
            }
#endif
            UploadId = uploadId;
        }

        //Properties
        public SemaphoreSlim UploadSemaphore { get; private set; }

        public String UploadId { get; private set; }

        //Methods

        //Delete the member uploadId from the input contexts....
        public async Task DeleteFromCollections(ConcurrentDictionary<string, FileContext> fileContexts,
                                                string pathUploads,
                                                ConcurrentDictionary<string, ValidationContext<CsvValidator>> validationContexts,
                                                string pathValidated,
                                                ConcurrentDictionary<string, DbLoadContext> dbLoadContexts,
                                                string pathProcessed)
        {
            //Validate/initialize input parameters/member variables...
            if ( null != fileContexts && 
                 (!String.IsNullOrWhiteSpace(pathUploads)) &&
                 null != validationContexts &&
                 (!String.IsNullOrWhiteSpace(pathValidated)) &&
                 null != dbLoadContexts && 
                 (!String.IsNullOrWhiteSpace(pathProcessed)) &&
                 (!String.IsNullOrWhiteSpace(UploadId)))
            {
                //Input parameters/member variables valid - for member uploadId, remove associated file context and uploaded files, if any...
                var myUploadId = UploadId;
                var wildCard = myUploadId + "*.*";

                FileContext fileContext = null;
                if (fileContexts.TryRemove(myUploadId, out fileContext))
                {
                    using (await fileContext.FileSemaphore.UseWaitAsync())
                    {
                        //Source: https://forums.asp.net/t/1899755.aspx?How+to+delete+files+with+wildcard+
                        DirectoryInfo directoryInfo = new DirectoryInfo(pathUploads);
                        foreach (FileInfo fI in directoryInfo.GetFiles(wildCard))
                        {
                            fI.Delete();
                        }
                    }
                }

                //For member uploadId, remove associated validation context and validated files, if any...
                ValidationContext<CsvValidator> validationContext = null;
                if (validationContexts.TryRemove(myUploadId, out validationContext))
                {
                    using (await validationContext.ValidationResultSemaphore.UseWaitAsync())
                    {
                        //Source: https://forums.asp.net/t/1899755.aspx?How+to+delete+files+with+wildcard+
                        DirectoryInfo directoryInfo = new DirectoryInfo(pathValidated);
                        foreach (FileInfo fI in directoryInfo.GetFiles(wildCard))
                        {
                            fI.Delete();
                        }
                    }
                }

                //For member uploadId, remove associated db load context and rsults files, if any...
                DbLoadContext dbLoadContext = null;
                if (dbLoadContexts.TryRemove(myUploadId, out dbLoadContext))
                {
                    using (await dbLoadContext.DbLoadSemaphore.UseWaitAsync())
                    {
                        //Source: https://forums.asp.net/t/1899755.aspx?How+to+delete+files+with+wildcard+
                        DirectoryInfo directoryInfo = new DirectoryInfo(pathProcessed);
                        foreach (FileInfo fI in directoryInfo.GetFiles(wildCard))
                        {
                            fI.Delete();
                        }
                    }
                }
            }
        }


    }
}