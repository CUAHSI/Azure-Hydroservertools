using HydroserverToolsBusinessObjects;
using HydroServerToolsRepository.Repository;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity.Core;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HydroServerToolsRepository
{
    public class Utils
    {
        public static EntityKey GetEntityKey(EntitySet entitySet, dynamic d)
        {
            //check if entry with this key exists
            var entityKeyValues = new List<KeyValuePair<string, object>>();
            foreach (var member in entitySet.ElementType.KeyMembers)
            {
                var info = d.GetType().GetProperty(member.Name);
                var tempValue = info.GetValue(d, null);
                var pair = new KeyValuePair<string, object>(member.Name, tempValue);
                entityKeyValues.Add(pair);
            }
            var key = new EntityKey(entitySet.EntityContainer.Name + "." + entitySet.Name, entityKeyValues);
            return key;
        }

        public static DataTable ListToDataTable<T>(IList<T> list)
        {
            var table = new DataTable();

            return table;
        }

        public static void BulkInsert<T>(string connection, string tableName, IList<T> list)
        {

            //connection = "Data Source=tcp:bhi5g2ajst.database.windows.net,1433;Database=hydroservertest2;User ID=HisCentralAdmin@bhi5g2ajst;Password=f3deratedResearch;Integrated Security=false;Trusted_Connection=False;Encrypt=True;Connection Timeout=30;Persist Security Info = true";


            //bulkCopy.BatchSize = list.Count;
            // bulkCopy.DestinationTableName = tableName;


            var table = new DataTable();
            var props = TypeDescriptor.GetProperties(typeof(T))
                //Dirty hack to make sure we only have system data types 
                //i.e. filter out the relationships/collections
                                       .Cast<PropertyDescriptor>()
                                       .Where(propertyInfo => propertyInfo.PropertyType.Namespace.Equals("System"))
                                       .ToArray();
            var sortedProps = props.OrderBy(x => x.Name).ToArray();

            foreach (var propertyInfo in props)
            {
                if (propertyInfo.Name != "ValueID")
                {
                    //bulkCopy.ColumnMappings.Add(propertyInfo.Name, propertyInfo.Name);
                }
                table.Columns.Add(propertyInfo.Name, Nullable.GetUnderlyingType(propertyInfo.PropertyType) ?? propertyInfo.PropertyType);

            }

            var values = new object[props.Length];
            foreach (var item in list)
            {

                for (var i = 0; i < values.Length; i++)
                {
                    values[i] = props[i].GetValue(item);
                }

                table.Rows.Add(values);
            }



            // open the destination data
            using (SqlConnection destinationConnection =
                            new SqlConnection(connection))
            {
                // open the connection
                destinationConnection.Open();

                using (SqlBulkCopy bulkCopy =
                            new SqlBulkCopy(destinationConnection.ConnectionString, SqlBulkCopyOptions.KeepNulls | SqlBulkCopyOptions.CheckConstraints))
                {
                    //bulkCopy.SqlRowsCopied += new SqlRowsCopiedEventHandler(OnSqlRowsTransfer);
                    //bulkCopy.NotifyAfter = 100;
                    bulkCopy.BatchSize = 50;


                    // bulkCopy.ColumnMappings.Add("OrderID", "NewOrderID");     
                    bulkCopy.DestinationTableName = tableName;
                    bulkCopy.WriteToServer(table);
                }
            }

            //bulkCopy.WriteToServer(table);

        }

       
    }
}
