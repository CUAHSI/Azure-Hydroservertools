using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HydroServerToolsUtilities
{
    //Db load counts: inserted, updated and rejected rows 
    public class DbLoadCounts
    {
        //Constructors...

        //Default 
        private DbLoadCounts() { }

        //Initializing 
        public DbLoadCounts(int inserted, int updated, int rejected, int duplicated)
        {
#if (DEBUG)
            //Validate/initialize input parameters...
            if (0 > inserted || 0 > updated || 0 > rejected || 0 > duplicated)
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

        //Update counts per input value changes
        //Assumption: The input positive values represent the counts of the 'listOf...' lists returned by 
        // a repository 'Add...' method call for a collection of previously rejected records.  For example,
        // For a given table:
        //  1.) Initial Record Counts: Inserted: 12, Updated: 3, Rejected: 9, Duplicated: 0  (total: 24)
        //  2.) User edits 7 rejected records, submits all edited records for upload
        //  3.) Repository 'Add...' method returns: Inserted: 3, Updated: 2, Rejected: 1, Duplicated: 1  (total: 7)
        //  4.) UpdateCounts(...) logic adjusts the Initial Record Counts:
        //      a.) Rejected: 9 - (Inserted: 3 + Updated: 2 + Rejected: 1 + Duplicated: 1) = 2
        //      b.) Inserted: 12 + Inserted: 3 = 15
        //      c.) Updated: 3 + Updated: 2 = 5
        //      d.) Rejected: 2 + Rejected: 1 = 3
        //      e.) Duplicated: 0 + Duplicated: 1 = 1
        //      
        //  5.) Resulting in Adjusted Record Counts: Inserted: 15, Updated: 5, Rejected: 3, Duplicated: 1  (total: 24)
        public void UpdateCounts(int insertedChange, int updatedChange, int rejectedChange, int duplicatedChange)
        {
#if (DEBUG)
            //Validate/initialize input parameters...
            if (0 > insertedChange || 0 > updatedChange || 0 > rejectedChange || 0 > duplicatedChange)
            {
                //Negative input parameter...
                throw new ArgumentOutOfRangeException("UpdateCounts: Negative parameter entered!!");
            }
#endif
            int totalChanged = insertedChange + updatedChange + rejectedChange + duplicatedChange;
            if (0 < totalChanged)
            {
                //Positive total changed records...
#if (DEBUG)
                if (Rejected < totalChanged)
                {
                    //Total changed exceeds Rejected - 
                    throw new ArgumentOutOfRangeException(String.Format("UpdateCounts: Rejected {0} less than total changed {1}", Rejected, totalChanged));
                }
#endif
                //Update current values...
                Rejected -= totalChanged;       //Decrement Rejected by total changed
                Inserted += insertedChange;     //Increment values by associated change...
                Updated += updatedChange;
                Rejected += rejectedChange;
                Duplicated += duplicatedChange;
            }
        }

        //Add input values to member counts...
        public void IncrementCounts(int insertedIncrement, int updatedIncrement, int rejectedIncrement, int duplicatedIncrement)
        {
#if (DEBUG)
            //Validate/initialize input parameters...
            if (0 > insertedIncrement || 0 > updatedIncrement || 0 > rejectedIncrement || 0 > duplicatedIncrement)
            {
                //Negative input parameter...
                throw new ArgumentOutOfRangeException("UpdateCounts: Negative parameter entered!!");
            }
#endif
            //Increment member counts...
            Inserted += insertedIncrement;
            Updated += updatedIncrement;
            Rejected += rejectedIncrement;
            Duplicated += duplicatedIncrement;
        }

        //Assign input values to member counts...
        public void SetCounts(int inserted, int updated, int rejected, int duplicated)
        {
#if (DEBUG)
            //Validate/initialize input parameters...
            if (0 > inserted || 0 > updated || 0 > rejected || 0 > duplicated)
            {
                //Negative input parameter...
                throw new ArgumentOutOfRangeException("UpdateCounts: Negative parameter entered!!");
            }
#endif
            //Increment member counts...
            Inserted = inserted;
            Updated = updated;
            Rejected = rejected;
            Duplicated = duplicated;
        }

        //Override ToString to produce 'human-readable' output...
        public override string ToString()
        {
            return String.Format("Inserted: {0}, Updated: {1}, Rejected: {2}, Duplicated: {3} ", Inserted, Updated, Rejected, Duplicated);
        }
    }
}
