using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Collections.Concurrent;
using System.Threading;

namespace HydroServerToolsUtilities
{
    public class StatusMessage
    {
        //Constructors

        //Default...
        private StatusMessage()
        {
            //Assume: current date/time
            When = DateTime.Now;

            //Assume: NOT reported, NOT an error message...
            Reported = false;
            IsError = false;

            //Assume: NOT associated with any record list
            //RecordIndex = -1;
            ItemId = -1;
        }

        //Initializing...
        //public StatusMessage(string message, int recordIndex = -1,  bool reported = false ) : this()
        public StatusMessage(string message, int itemId = -1, bool reported = false) : this()
        {
#if (DEBUG)
            if (String.IsNullOrWhiteSpace(message))
            {
                ArgumentNullException exception = new ArgumentNullException("Status Message - message parameter cannot be null!!");
                throw exception;
            }
#endif
            Message = message;
            ItemId = itemId;
            Reported = reported;
        }

        //Copy...
        public StatusMessage(StatusMessage statusMessage) : this()
        {
#if (DEBUG)
            if (null == statusMessage)
            {
                ArgumentNullException exception = new ArgumentNullException("Status Message - statusMessage parameter cannot be null!!");
                throw exception;
            }
#endif
            //Assign input values to current instance...
            Message = statusMessage.Message;
            When = statusMessage.When;
            Reported = statusMessage.Reported;
            IsError = statusMessage.IsError;
            //RecordId = statusMessage.RecordId;
            ItemId = statusMessage.ItemId;
        }

        //Properties...
        public string Message { get; private set; }

        public DateTime When { get; private set; }

        public bool Reported { get; set; }

        public bool IsError { get; set; }

        //ItemId in the associated rejected items list
        //NOTE: A value of -1 indicates the message is NOT associated with any list
        //public int RecordIndex { get; set; }
        public int ItemId { get; set; }
    }


    public class RecordCountMessage
    {
        //Constructors

        //Default...
        private RecordCountMessage()
        {
            //Assume: current date/time
            When = DateTime.Now;

            //Assume: NOT final...
            Final = false;

            //Initialize all counts to zero..
            Inserted = 0;
            Updated = 0;
            Rejected = 0;
            Duplicated = 0;

            RecordCount = 0;
        }

        //Initializing...
        public RecordCountMessage(int recordCount, int inserted, int updated, int rejected, int duplicated, bool final = false) : this()
        {
#if (DEBUG)
            if (0 >= recordCount || 0 > inserted || 0 > updated || 0 > rejected || 0 > duplicated)
            {
                if (0 >= recordCount)
                {
                    throw new ArgumentOutOfRangeException("RecordCountMessage - input record count must be > zero!!");
                }
                else
                {
                    throw new ArgumentOutOfRangeException("RecordCountMessage - input inserted, updated, rejected, duplicated counts must be >= zero!!");
                }
            }
#endif
            RecordCount = recordCount;

            Inserted = inserted;
            Updated = updated;
            Rejected = rejected;
            Duplicated = duplicated;

            Final = final;
        }

        //Copy 
        public RecordCountMessage(RecordCountMessage recordCountMessage) : this()
        {
#if (DEBUG)
            if (null == recordCountMessage)
            {
                ArgumentNullException exception = new ArgumentNullException("RecordCountMessage - recordCountMessage parameter cannot be null!!");
                throw exception;
            }
#endif
            RecordCount = recordCountMessage.RecordCount;

            Inserted = recordCountMessage.Inserted;
            Updated = recordCountMessage.Updated;
            Rejected = recordCountMessage.Rejected;
            Duplicated = recordCountMessage.Duplicated;

            Final = recordCountMessage.Final;
            When = recordCountMessage.When;
        }

        //Properties...
        public DateTime When { get; private set; }

        public bool Final { get; set; }

        //Current inserted record count - subject to change...
        public int Inserted { get; set; }

        //Current updated record count - subject to change...
        public int Updated { get; set; }

        //Current rejected record count - subject to change...
        public int Rejected { get; set; }

        //Current duplicated record count - subject to change...
        public int Duplicated { get; set; }

        //Total record count - initialized at construction - not subject to change
        public int RecordCount { get; private set; }
    }


    //A simple class for the association of a dictionary of status message stacks and an access semaphore...
    public class StatusContext
    {

        private SemaphoreSlim _DbProcessSemaphore;
        private SemaphoreSlim _DbLoadSemaphore;

        [Flags]
        public enum enumCountType
        {
            ct_DbProcess = 0x0001,   //NOT OR-able
            ct_DbLoad = 0x0002
        }

        //Validate input value represents a single defined enum value...
        private bool ValidCountType(enumCountType eCountType)
        {
            return (((enumCountType.ct_DbProcess | enumCountType.ct_DbLoad) & eCountType) == eCountType);
        }

        //Constructors...

        //Default...
        public StatusContext()
        {
            StatusMessages = new ConcurrentDictionary<string, ConcurrentQueue<StatusMessage>>();

            StatusMessagesSemaphore = new SemaphoreSlim(1, 1);

            SubstituteKeys = new ConcurrentDictionary<string, string>();

            SubstituteKeysSemaphore = new SemaphoreSlim(1, 1);

            DbProcessCounts = new List<DbLoadResult>();
            _DbProcessSemaphore = new SemaphoreSlim(1, 1);

            DbLoadCounts = new List<DbLoadResult>();
            _DbLoadSemaphore = new SemaphoreSlim(1, 1);
        }

        //Initializing...
        public StatusContext(string key, ConcurrentQueue<StatusMessage> statusMessages) : this()
        {
#if (DEBUG)
            //Validate/initialize input parameters...
            if (String.IsNullOrWhiteSpace(key) || null == statusMessages || 0 >= statusMessages.Count)
            {
                string paramName = String.IsNullOrWhiteSpace(key) ? "key" : "statusMessages";
                throw new ArgumentException("Invalid input parameter...", paramName);
            }
#endif
            //Update instance member(s)...
            StatusMessages[key] = statusMessages;
        }

        //Properties...
        public ConcurrentDictionary< string, ConcurrentQueue<StatusMessage>> StatusMessages { get; private set; }

        public SemaphoreSlim StatusMessagesSemaphore { get; private set; }

        //Substitute key values used in AddStatusMessage calls... 
        private ConcurrentDictionary< string, string> SubstituteKeys { get; set; }

        private SemaphoreSlim SubstituteKeysSemaphore { get; set; }

        private List<DbLoadResult> DbProcessCounts { get; set; }

        private List<DbLoadResult> DbLoadCounts { get; set; }

        //Methods...
        public async Task AddSubstituteKey( string key, string substituteKey)
        {
            //Validate/initialize input parameters....
            if ( (!String.IsNullOrWhiteSpace(key)) && (!String.IsNullOrWhiteSpace(substituteKey)))
            {
                //Input parameters valid - add dictionary entry...
                using (await SubstituteKeysSemaphore.UseWaitAsync())
                {
                    SubstituteKeys[key] = substituteKey;
                }
            }
        }

        public async Task AddStatusMessage(string key, string statusMessage)
        {
            //Validate/initialize input parameters...
            if ((!String.IsNullOrWhiteSpace(key)) && (!String.IsNullOrWhiteSpace(statusMessage)))
            {
                //Parameters valid - access member semaphore...
                using (await StatusMessagesSemaphore.UseWaitAsync())
                {
                    //Check for substitute key...
                    string myKey = key;
                    using (await SubstituteKeysSemaphore.UseWaitAsync())
                    {
                        if (SubstituteKeys.Keys.Contains(key))
                        {
                            myKey = SubstituteKeys[key];
                        }
                    }

                    //Find/create status message dictionary per input key...
                    if (!StatusMessages.Keys.Contains(myKey))
                    {
                        StatusMessages[myKey] = new ConcurrentQueue<StatusMessage>();
                    }

                    ConcurrentQueue<StatusMessage> cq = StatusMessages[myKey];

                    //Create new status message instance, add to member queue
                    StatusMessage sM = new StatusMessage(statusMessage);
                    cq.Enqueue(sM);
                }
            }
        }

        public async Task AddStatusMessage(string key, StatusMessage statusMessage)
        {
            //Validate/initialize input parameters...
            if ((!String.IsNullOrWhiteSpace(key)) && (null != statusMessage))
            {
                //Parameters valid - access member semaphore...
                using (await StatusMessagesSemaphore.UseWaitAsync())
                {
                    //Check for substitute key...
                    string myKey = key;
                    using (await SubstituteKeysSemaphore.UseWaitAsync())
                    {
                        if (SubstituteKeys.Keys.Contains(key))
                        {
                            myKey = SubstituteKeys[key];
                        }
                    }

                    //Find/create status message dictionary per input key...
                    if (!StatusMessages.Keys.Contains(myKey))
                    {
                        StatusMessages[myKey] = new ConcurrentQueue<StatusMessage>();
                    }

                    ConcurrentQueue<StatusMessage> cq = StatusMessages[myKey];

                    //Create new status message instance, add to member queue
                    StatusMessage sM = new StatusMessage(statusMessage);
                    cq.Enqueue(sM);
                }
            }
        }

        //For the input count type and key, add the input values to the associated counts...  
        public async Task AddToCounts(enumCountType eCountType, string key, int inserted, int updated = 0, int rejected = 0, int duplicated = 0)
        {
            //Validate/initialize input parameters...
            if (ValidCountType(eCountType) && (!String.IsNullOrWhiteSpace(key)) && (0 <= inserted && 0 <= updated && 0 <= rejected && 0 <= duplicated))
            {
                //Input parameters valid - check for substitute key...
                string myKey = key;
                using (await SubstituteKeysSemaphore.UseWaitAsync())
                {
                    if (SubstituteKeys.Keys.Contains(key))
                    {
                        myKey = SubstituteKeys[key];
                    }
                }

                //Retrieve associated results list...
                List<DbLoadResult>  resultsList = null;
                SemaphoreSlim semSlim = null;

                if (enumCountType.ct_DbProcess == eCountType)
                {
                    //Db Process...
                    resultsList = DbProcessCounts;
                    semSlim = _DbProcessSemaphore;
                }
                else if (enumCountType.ct_DbLoad == eCountType)
                {
                    //Db Load...
                    resultsList = DbLoadCounts;
                    semSlim = _DbLoadSemaphore;
                }

                if (null != resultsList && null != semSlim)
                {
                    //Results list found - check for results...
                    using (await semSlim.UseWaitAsync())
                    {
                        var results = resultsList.FirstOrDefault(res => res.TableName == myKey);

                        if ( null != results)
                        {
                            //Results found - increment values...
                            DbLoadCounts dbLoadCounts = results.LoadCounts;

                            dbLoadCounts.IncrementCounts(inserted, updated, rejected, duplicated);
                        }
                        else
                        {
                            //Results not found - add new instance to list...
                            resultsList.Add(new DbLoadResult(myKey, inserted, updated, rejected, duplicated));
                        }
                    }
                }

            }

        }

        //For the input count type and key, set the input record count...
        public async Task SetRecordCount(enumCountType eCountType, string key, int recordCount)
        {
            //Validate/initialize input parameters...
            if (ValidCountType(eCountType) && (!String.IsNullOrWhiteSpace(key)) && 0 < recordCount)
            {
                //Input parameters valid - check for substitute key...
                string myKey = key;
                using (await SubstituteKeysSemaphore.UseWaitAsync())
                {
                    if (SubstituteKeys.Keys.Contains(key))
                    {
                        myKey = SubstituteKeys[key];
                    }
                }

                //Retrieve associated results list...
                List<DbLoadResult> resultsList = null;
                SemaphoreSlim semSlim = null;

                if (enumCountType.ct_DbProcess == eCountType)
                {
                    //Db Process...
                    resultsList = DbProcessCounts;
                    semSlim = _DbProcessSemaphore;
                }
                else if (enumCountType.ct_DbLoad == eCountType)
                {
                    //Db Load...
                    resultsList = DbLoadCounts;
                    semSlim = _DbLoadSemaphore;
                }

                if (null != resultsList && null != semSlim)
                {
                    //Results list found - check for results...
                    using (await semSlim.UseWaitAsync())
                    {
                        var results = resultsList.FirstOrDefault(res => res.TableName == myKey);

                        if (null != results)
                        {
                            //Results found - set record count...
                            results.RecordCount = recordCount;
                        }
                        else
                        {
                            //Results not found - add new instance to list...
                            results = new DbLoadResult(myKey, 0, 0, 0, 0);
                            results.RecordCount = recordCount;
                            resultsList.Add(results);
                        }
                    }
                }
            }
        }

        //For the input count type and key, set the final property...
        //Assumption: The referenced DBLoadResult instance exists
        public async Task Finalize(enumCountType eCountType, string key)
        {
            //Validate/initialize input parameters...
            if (ValidCountType(eCountType) && (!String.IsNullOrWhiteSpace(key)))
            {
                //Input parameters valid - check for substitute key...
                string myKey = key;
                using (await SubstituteKeysSemaphore.UseWaitAsync())
                {
                    if (SubstituteKeys.Keys.Contains(key))
                    {
                        myKey = SubstituteKeys[key];
                    }
                }

                //Retrieve associated results list...
                List<DbLoadResult> resultsList = null;
                SemaphoreSlim semSlim = null;

                if (enumCountType.ct_DbProcess == eCountType)
                {
                    //Db Process...
                    resultsList = DbProcessCounts;
                    semSlim = _DbProcessSemaphore;
                }
                else if (enumCountType.ct_DbLoad == eCountType)
                {
                    //Db Load...
                    resultsList = DbLoadCounts;
                    semSlim = _DbLoadSemaphore;
                }

                if (null != resultsList && null != semSlim)
                {
                    //Results list found - check for results...
                    using (await semSlim.UseWaitAsync())
                    {
                        var results = resultsList.FirstOrDefault(res => res.TableName == myKey);

                        if (null != results)
                        {
                            //Results found - set record count...
                            results.Final = true;
                        }
                    }
                }
            }
        }

        public async Task<RecordCountMessage> GetCountsMessage(enumCountType eCountType, string key)
        {
            RecordCountMessage result = null;

            //Validate/initialize input parameters...
            if (ValidCountType(eCountType) && (!String.IsNullOrWhiteSpace(key)))
            {
                //Input parameters valid - check for substitute key...
                string myKey = key;
                try
                {
                    using (await SubstituteKeysSemaphore.UseWaitAsync())
                    {
                        if (SubstituteKeys.Keys.Contains(key))
                        {
                            myKey = SubstituteKeys[key];
                        }
                    }
                }
                catch (Exception ex)
                {
                    string message = ex.Message;
                    int n = 5;

                    ++n;
                }
                //Retrieve associated results list...
                List<DbLoadResult> resultsList = null;
                SemaphoreSlim semSlim = null;

                if (enumCountType.ct_DbProcess == eCountType)
                {
                    //Db Process...
                    resultsList = DbProcessCounts;
                    semSlim = _DbProcessSemaphore;
                }
                else if (enumCountType.ct_DbLoad == eCountType)
                {
                    //Db Load...
                    resultsList = DbLoadCounts;
                    semSlim = _DbLoadSemaphore;
                }

                if (null != resultsList && null != semSlim)
                {
                    //Results list found - check for results...
                    using (await semSlim.UseWaitAsync())
                    {
                        DbLoadResult dbLoadResult = resultsList.FirstOrDefault(res => res.TableName == myKey);

                        if (null != dbLoadResult)
                        {
                            //Results found - set return value...
                            DbLoadCounts loadCounts = dbLoadResult.LoadCounts;
                            result = new RecordCountMessage(dbLoadResult.RecordCount, loadCounts.Inserted, loadCounts.Updated, loadCounts.Rejected, loadCounts.Duplicated, dbLoadResult.Final);
                        }
                    }
                }
            }

            //Processing complete - return results;
            return result;
        }

    }
}
