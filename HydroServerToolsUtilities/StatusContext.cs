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
            RecordIndex = -1;
        }

        //Initializing...
        public StatusMessage(string message, int recordIndex = -1,  bool reported = false ) : this()
        {
#if (DEBUG)
            if (String.IsNullOrWhiteSpace(message))
            {
                ArgumentNullException exception = new ArgumentNullException("Status Message - message parameter cannot be null!!");
                throw exception;
            }
#endif
            Message = message;
            RecordIndex = recordIndex;
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
            RecordIndex = statusMessage.RecordIndex;
        }

        //Properties...
        public string Message { get; private set; }

        public DateTime When { get; private set; }

        public bool Reported { get; set; }

        public bool IsError { get; set; }

        //Index in the associated records list
        //NOTE: A value of -1 indicates the message is NOT associated with any records list
        public int RecordIndex { get; set; }
    }

    //A simple class for the association of a dictionary of status message stacks and an access semaphore...
    public class StatusContext
    {
        //Constructors...

        //Default...
        public StatusContext()
        {
            StatusMessages = new ConcurrentDictionary<string, ConcurrentQueue<StatusMessage>>();

            StatusMessagesSemaphore = new SemaphoreSlim(1, 1);
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

        //Methods...
        public async Task AddStatusMessage(string key, string statusMessage)
        {
            //Validate/initialize input parameters...
            if ((!String.IsNullOrWhiteSpace(key)) && (!String.IsNullOrWhiteSpace(statusMessage)))
            {
                //Parameters valid - access member semaphore...
                using (await StatusMessagesSemaphore.UseWaitAsync())
                {
                    //Find/create status message dictionary per input key...
                    if (!StatusMessages.Keys.Contains(key))
                    {
                        StatusMessages[key] = new ConcurrentQueue<StatusMessage>();
                    }

                    ConcurrentQueue<StatusMessage> cq = StatusMessages[key];

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
                    //Find/create status message dictionary per input key...
                    if (!StatusMessages.Keys.Contains(key))
                    {
                        StatusMessages[key] = new ConcurrentQueue<StatusMessage>();
                    }

                    ConcurrentQueue<StatusMessage> cq = StatusMessages[key];

                    //Create new status message instance, add to member queue
                    StatusMessage sM = new StatusMessage(statusMessage);
                    cq.Enqueue(sM);
                }
            }
        }

    }
}
