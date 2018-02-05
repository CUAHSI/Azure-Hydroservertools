using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Collections;

namespace HydroServerToolsUtilities
{
    //A simple class which associates a table name with various update categories of item Id lists...
    [Serializable()]
    public class TableUpdateResult
    {
        //Default constructor...
        private TableUpdateResult()
        {
            TableName = String.Empty;

            CorrectItemIds = new List<int>();
            DuplicateItemIds = new List<int>();
            EditedItemIds = new List<int>();
            IncorrectItemIds = new List<int>();

            ErrorMessages = new List<StatusMessage>();
        }

        //Initializing constructor...
        public TableUpdateResult(string tableName) : this()
        {
#if (DEBUG)
            if (String.IsNullOrWhiteSpace(tableName))
            {
                throw new ArgumentNullException("tableName");
            }
#endif
            TableName = tableName;
        }

        //Properties...
        public String TableName { get; private set; }

        public List<int> CorrectItemIds { get; private set; }
        public List<int> DuplicateItemIds { get; private set; }
        public List<int> EditedItemIds { get; private set; }
        public List<int> IncorrectItemIds { get; private set; }

        public List<StatusMessage> ErrorMessages { get; private set; }
    }

    //A simple class which associates an upload id with one or more table update result instances...
    [Serializable()]
    public class UpdateResults
    {
        //Default constructor...
        private UpdateResults()
        {
            UploadId = String.Empty;

            TableUpdateResults = new List<TableUpdateResult>();
        }

        //Initializing constructor...
        public UpdateResults(string uploadId) : this()
        {
#if (DEBUG)
            if (String.IsNullOrWhiteSpace(uploadId))
            {
                throw new ArgumentNullException("uploadId");
            }
#endif
            UploadId = uploadId;
        }

        //Properties 
        public String UploadId { get; private set; }

        public List<TableUpdateResult> TableUpdateResults { get; private set; }
    }


    //A simple generic class which associates an updateable item with one (or more) identifiers/indices...
    [Serializable()]
    public class UpdateableItem<tModelType>
    {
        //Default constructor...
        private UpdateableItem() 
        {
            Item = default(tModelType);
            ItemId = -1;                //Represents an uninitialized instance...
        }

        //Initializing constructor...
        public UpdateableItem( tModelType item, int itemId) : this()
        {
#if (DEBUG)
            if (null == item || 0 > itemId)
            {
                Exception ex = (null == item) ? new ArgumentNullException("item") : ((Exception) new ArgumentOutOfRangeException("itemId"));
                throw ex;
            }
#endif
            //Assign input variables...
            Item = item;
            ItemId = itemId;
        }

        //Properties...
        public tModelType Item { get; private set; }
        public int ItemId { get; private set; }
    }

    //A simple interface for use by the UpdatedItemsData class...
    public interface IUpdateableItemsData<tModelType>
    {
        String UploadId { get; set; }

        String TableName { get; set; }

        List<UpdateableItem<tModelType>> UpdateableItems { get; set; }
    }

    //A simple class encapsulating context-related and updated values received from the rejected items dialog...
    public class UpdateableItemsData<tModelType> : IUpdateableItemsData<tModelType>
    {
        public String UploadId { get; set; }

        public String TableName { get; set; }

        public List<UpdateableItem<tModelType>> UpdateableItems { get; set; }
    }
}
