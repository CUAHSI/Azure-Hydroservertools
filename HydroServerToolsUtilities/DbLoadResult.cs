using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HydroServerToolsUtilities
{
    public class DbLoadResult
    {
        //Constructors...

        //Default
        private DbLoadResult()
        {
            Final = false;
            RecordCount = 0;
        }

        //Initializing
        public DbLoadResult(string tableName, int inserted, int updated, int rejected, int duplicated) :this()
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

        //Used for record count reporting...
        public bool Final { get; set; }

        public int RecordCount { get; set; }

        //Override ToString to produce 'human-readable' output...
        public override string ToString()
        {
            return String.Format("Table: {0} - Load Counts: {1}", TableName, LoadCounts.ToString() );
        }
    }
}
