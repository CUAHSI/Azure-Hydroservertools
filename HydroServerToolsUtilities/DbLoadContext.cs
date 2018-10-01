using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Threading;
using HydroServerToolsUtilities;

namespace HydroServerToolsUtilities
{

    //A simple class for the association of db load results with an access semaphore...
    public class DbLoadContext
    {
        [Flags]
        public enum enumDbLoadState
        {
            dbls_NotStarted = 0x0001,   //NOT OR-able
            dbls_Started = 0x0002,
            dbls_Complete = 0x0003,
            dbls_Error = 0x0004
        }

        private SemaphoreSlim _DbStateSemaphore;
        private enumDbLoadState _enumDbLoadState;

        //Constructors...

        //Default
        public DbLoadContext()
        {
            DbLoadResults = new List<DbLoadResult>();
            DbLoadSemaphore = new SemaphoreSlim(1, 1);

            _DbStateSemaphore = new SemaphoreSlim(1, 1);
            _enumDbLoadState = enumDbLoadState.dbls_NotStarted;
        }

        //        //Initializing...
        //        public DbLoadContext( List<DbLoadResult> dbLoadResults) : this()
        //        {
        //#if (DEBUG)
        //            //Validate/initialize input parameters...
        //            if (null == dbLoadResults || 0 >= dbLoadResults.Count)
        //            {
        //                string paramName = "dbLoadResults";
        //                throw new ArgumentException("Invalid input parameter...", paramName);
        //            }
        //#endif
        //            //Set instance member(s)...
        //            DbLoadResults = dbLoadResults;
        //        }

        //Properties...
        public List<DbLoadResult> DbLoadResults { get; private set; }

        public SemaphoreSlim DbLoadSemaphore { get; private set; }

        public enumDbLoadState DbLoadState
        {
            get
            {
                enumDbLoadState result = enumDbLoadState.dbls_NotStarted;

                using (_DbStateSemaphore.UseWaitAsync())
            {
                    result = _enumDbLoadState;
            }

                return result;
        }

            set
            {
                if (((enumDbLoadState.dbls_Complete | 
                      enumDbLoadState.dbls_Error | 
                      enumDbLoadState.dbls_NotStarted |
                      enumDbLoadState.dbls_Started) & value) == value)
                {
                    using (_DbStateSemaphore.UseWaitAsync())
                    {
                        _enumDbLoadState = value;
                    }
                }
            }
        } 
    }
}