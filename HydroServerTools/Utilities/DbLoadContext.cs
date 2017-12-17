using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Threading;

namespace HydroServerTools.Utilities
{
    //Db load counts: inserted, updated and rejected rows 
    public class DbLoadCounts
    {
        //Constructors...

        //Default 
        private DbLoadCounts() { }

        //Initializing 
        public DbLoadCounts( int inserted, int updated, int rejected, int duplicated)
        {
#if (DEBUG)
            //Validate/initialize input parameters...
            if ( 0 > inserted || 0 > updated || 0 > rejected || 0 > duplicated)
            {
                throw new ArgumentOutOfRangeException("Negative value entered!!");
            }
#endif
            Inserted = inserted;
            Updated = updated;
            Rejected = rejected;
            Duplicated = duplicated;
        }

        //Properties...

        //Inserted records count
        public int Inserted { get; private set; }

        //Updated records count
        public int Updated { get; private set; }

        //Rejected records count
        public int Rejected { get; private set; }

        //Duplicated records count
        public int Duplicated { get; private set; }
    }

    public class DbLoadResult
    {
        //Constructors...

        //Default
        private DbLoadResult() { }

        //Initializing
        public DbLoadResult(string tableName, int inserted, int updated, int rejected, int duplicated)
        {
#if (DEBUG)
            //Validate/initialize input parameters...
            if (String.IsNullOrWhiteSpace(tableName) || (0 > inserted || 0 > updated || 0 > rejected || 0 > duplicated))
            {
                if (String.IsNullOrWhiteSpace(tableName))
                {
                    throw new ArgumentNullException("Table name cannot be empty!!");
                }
                else
                {
                    throw new ArgumentOutOfRangeException("Negative value entered!!");
                }
            }
#endif
            TableName = tableName;
            LoadCounts = new DbLoadCounts(inserted, updated, rejected, duplicated);
        }

        //Properties...
        public string TableName { get; private set; }

        public DbLoadCounts LoadCounts { get; private set; }
    }


    //A simple class for the association of db load results with an access semaphore...
    public class DbLoadContext
    {
        //Constructors...

        //Default
        public DbLoadContext()
        {
            DbLoadResults = new List<DbLoadResult>();
            DbLoadSemaphore = new SemaphoreSlim(1, 1);
        }

        //Initializing...
        public DbLoadContext( List<DbLoadResult> dbLoadResults) : this()
        {
#if (DEBUG)
            //Validate/initialize input parameters...
            if (null == dbLoadResults || 0 >= dbLoadResults.Count)
            {
                string paramName = "dbLoadResults";
                throw new ArgumentException("Invalid input parameter...", paramName);
            }
#endif
            //Set instance member(s)...
            DbLoadResults = dbLoadResults;
        }

        //Properties...
        public List<DbLoadResult> DbLoadResults { get; private set; }

        public SemaphoreSlim DbLoadSemaphore { get; private set; }
    }
}