using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HydroserverToolsBusinessObjects
{
    public class UpdateFieldsModel
    {
        public string TableName;
        public string ColumnName;
        public string CurrentValue;
        public string UpdatedValue;

        public UpdateFieldsModel( string tableName, string columnName, string currentValue, string updatedValue)
        {
            TableName = tableName;
            ColumnName = columnName;
            CurrentValue = currentValue;
            UpdatedValue = updatedValue; ;
        }
    }
}
