using System.Collections.Generic;

namespace jQuery.DataTables.Mvc
{
    /// <summary>
    /// Represents the required data for a response from a request by DataTables.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class JQueryDataTablesResponse<T>
    {
        public JQueryDataTablesResponse(IEnumerable<T> items,
            int totalRecords,
            int totalDisplayRecords,
            int sEcho)
        {
            aaData = items;
            iTotalRecords = totalRecords;
            iTotalDisplayRecords = totalDisplayRecords;
            this.sEcho = sEcho;
        }

        /// <summary>
        /// Sets the Total records, before filtering (i.e. the total number of records in the database)
        /// </summary>
        public int iTotalRecords { get; private set; }

        /// <summary>
        /// Sets the Total records, after filtering 
        /// (i.e. the total number of records after filtering has been applied - 
        /// not just the number of records being returned in this result set)
        /// </summary>
        public int iTotalDisplayRecords { get; private set; }

        /// <summary>
        /// Sets an unaltered copy of sEcho sent from the client side. This parameter will change with each 
        /// draw (it is basically a draw count) - so it is important that this is implemented. 
        /// Note that it strongly recommended for security reasons that you 'cast' this parameter to an 
        /// integer in order to prevent Cross Site Scripting (XSS) attacks.
        /// </summary>
        public int sEcho { get; private set; }

        /// <summary>
        /// Sets the data in a 2D array (Array of JSON objects). Note that you can change the name of this 
        /// parameter with sAjaxDataProp.
        /// </summary>
        public IEnumerable<T> aaData { get; private set; }
    }
}