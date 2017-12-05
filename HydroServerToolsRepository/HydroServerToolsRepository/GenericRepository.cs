using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AutoMapper;
using ODM_1_1_1EFModel;

using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Data.Linq.SqlClient;
using System.Linq.Expressions;
using System.Reflection;

using jQuery.DataTables.Mvc;

using HISWebClient.Util;
using HydroserverToolsBusinessObjects;

//using HydroserverToolsBusinessObjects.Interfaces;
using HydroserverToolsBusinessObjects.Models;

using HydroServerToolsEFDerivedObjects.Interfaces;

//Generic class implementing the generic repository interface...
namespace HydroServerToolsRepository.Repository
{
    public class GenericRepository<repositoryType, odmType> : IGenericRepository<repositoryType> where repositoryType : class, ICuahsiUpload<odmType>, new()
                                                                                                 where odmType: class, new()
    {
        //Properties...
        private string ConnectionString { get; set; }

        private ODM_1_1_1Entities OdmEntities { get; set; }

        //Contains method info declaration...
        //Source: https://www.codeproject.com/Articles/493917/Dynamic-Querying-with-LINQ-to-Entities-and-Express
        private static readonly MethodInfo StringContainsMethod =
          typeof(string).GetMethod(@"Contains", BindingFlags.Instance |
          BindingFlags.Public, null, new[] { typeof(string) }, null);

        //Constructors 

        //Default constructor - private to prevent use...
        private GenericRepository() { } 
        
        //Initializing - throw exception on empty connection string
        //TO DO - RegEx check for connection string format?
        public GenericRepository( string connectionString )
        {
            //Validate/initialize input parameters...
            if ( String.IsNullOrWhiteSpace(connectionString))
            {
                throw new ArgumentNullException("connectionString", "empty/null input value...");
            }

            //Assign input value to member...
            ConnectionString = connectionString;

            //Instantiate entities member...
            OdmEntities = new ODM_1_1_1Entities(connectionString);
        }

        //Methods

        //Find the target DbSet in the member db context...
        private DbSet<odmType> GetDbSet() 
        {
            DbSet<odmType> result = null;

            //For each property...
            var properties = OdmEntities.GetType().GetProperties();

            foreach (var property in properties)
            {
                var type = property.PropertyType;

                if (type.IsGenericType && (type == typeof (DbSet<odmType>)))
                {
                    result = property.GetValue(OdmEntities) as DbSet<odmType>;
                    break;
                }
            }

            //Processing complete - return result
            return result;
        }

        //Return all instances...
        public List<repositoryType> GetAll()
        {
            //Create result list...
            var list = new List<repositoryType>();

            //Get repository DbSet...
            var dbSet = GetDbSet();

            if (null != dbSet)
            {
                //Select all items...
                var items = from item in dbSet
                            select item;

                //Map items to list...
                foreach (var item in items)
                {
                    var model = Mapper.Map<odmType, repositoryType>(item);

                    list.Add(model);
                }
            }

            //Return the list...
            return list;
        }

        //Return matching instances...
        public List<repositoryType> GetInstances(int startIndex,
                                                 int pageSize,
                                                 ReadOnlyCollection<SortedColumn> sortedColumns,
                                                 out int totalRecordCount,
                                                 out int searchRecordCount,
                                                 string searchString)
        {
            //Initialize out parameters, result list...
            totalRecordCount = 0;
            searchRecordCount = 0;

            var repositoryT = new repositoryType();
            var list = new List<repositoryType>();

            //Get repository DbSet
            var dbSet = GetDbSet();

            if (null != dbSet)
            {
                //Update totals...
                totalRecordCount = dbSet.Count();
                searchRecordCount = totalRecordCount;

                if ((!String.IsNullOrWhiteSpace(searchString)) && (0 < totalRecordCount))
                {
                    //Non-empty search string.  Non-empty dbset.  Check items for match via LINQ...
                    System.Linq.Expressions.Expression<Func<odmType, bool>> predicate;
                    //NOTE: Initializing the predicate to always return false and then adding ORs 
                    //		 will select only those timeseries with values matching the search criteria
                    predicate = HISWebClient.Util.LinqPredicateBuilder.Create<odmType>(item => false);

                    //Add search terms...
                    var searchFields = repositoryT.GetFields(FieldType.ftSearching);
                    foreach (var searchField in searchFields)
                    {
                        //predicate = predicate.Or(item => (item.GetType().GetProperty(searchField).GetValue(item, null) ?? String.Empty).ToString().Contains(searchString, StringComparison.CurrentCultureIgnoreCase, new HISWebClient.Util.SearchStringComparer()));

                        //Build lambda expression dynamically (as explained elsewhere in this class)

                        //Parameter...
                        var odmT = typeof(odmType);
                        var parameterExpression = Expression.Parameter(odmT, "item");

                        //Property name to compare to...
                        var property = Expression.Property(parameterExpression, odmT.GetProperty(searchField).Name);

                        //Constant (value to compare to)
                        var constant = Expression.Constant(searchString);

                        //Comparison expression (contains)...
                        var expression = Expression.Equal(property, constant);

                        //var expression = Expression.Equal(SqlMethods.Like(property.ToString(), constant.ToString()), true);

                        //Create an expression call for Contains method...
                        //Source: https://www.codeproject.com/Articles/493917/Dynamic-Querying-with-LINQ-to-Entities-and-Express
                        //var containsCall = Expression.Call(dbFieldMember, StringContainsMethod, criterionConstant);
                        var containsCall = Expression.Call(property, StringContainsMethod, constant);

                        //Create the lambda expression...
                        //var lambdaExpression = Expression.Lambda<Func<odmType, bool>>(expression, parameterExpression);
                        var lambdaExpression = Expression.Lambda(containsCall, parameterExpression) as Expression<Func<odmType, bool>>;

                        //Add newly created lambda expression to predicate...
                        predicate = predicate.Or(lambdaExpression);
                    }

                    //Apply search criteria...
                    //var items = dbSet.ToList().AsQueryable();
                    List<odmType> results = null;
                    try
                    {
                        //results = items.Where(predicate).ToList();
                        results = dbSet.Where(predicate).ToList();
                    }
                    catch (Exception ex)
                    {
                        //Error - return empty results
                        string msg = ex.Message;

                        return list;
                    }

                    if (null == results || 0 >= results.Count)
                    {
                        //No matches - return empty results
                        return list;
                    }

                    //Non-empty results - apply page length...
                    searchRecordCount = results.Count();

                    var finalResults = results.Take(pageSize).ToList();

                    foreach (var finalResult in finalResults)
                    {
                        var model = Mapper.Map<odmType, repositoryType>(finalResult);
                        list.Add(model);
                    }
                }
                else if (String.IsNullOrWhiteSpace(searchString) && (0 < totalRecordCount))
                {
                    //Empty search string.  Non-empty dbset.  Sort dbSet contents...
                    List<odmType> sortedItems = null;
                    Dictionary<int, string> fieldsForSorting = repositoryT.GetFieldsForSorting();

                    foreach (var sortedColumn in sortedColumns)
                    {
                        SortingDirection direction = sortedColumn.Direction;
                        int fieldIndex;
                        if (Int32.TryParse(sortedColumn.PropertyName, out fieldIndex))
                        {
                            if (fieldIndex < fieldsForSorting.Count)
                            {
                                //Build lambda expression dynamically (as explained elsewhere in this class)
                                string fieldName = fieldsForSorting[fieldIndex];

                                //Parameter...
                                var odmT = typeof(odmType);
                                var parameterExpression = Expression.Parameter(odmT, "item");

                                //Property name...
                                var property = Expression.Property(parameterExpression, odmT.GetProperty(fieldName).Name);

                                //var expression = Expression.Field(property, fieldName);

                                //var lambdaExpression = Expression.Lambda<Func<odmType, string>>(expression, parameterExpression);
                                var lambdaExpression = Expression.Lambda<Func<odmType, string>>(property, parameterExpression);

                                //sortedItems = (direction == SortingDirection.Ascending) ? dbSet.OrderBy(a => a.GetType().GetProperty(fieldName).GetValue(a, null) ?? String.Empty).Skip(startIndex).Take(pageSize).ToList()
                                //                                                        : dbSet.OrderByDescending(a => a.GetType().GetProperty(fieldName).GetValue(a, null) ?? String.Empty).Skip(startIndex).Take(pageSize).ToList();
                                sortedItems = (direction == SortingDirection.Ascending) ? dbSet.OrderBy(lambdaExpression).Skip(startIndex).Take(pageSize).ToList()
                                                                                        : dbSet.OrderByDescending(lambdaExpression).Skip(startIndex).Take(pageSize).ToList();
                            }
                        }
                    }

                    if (null == sortedItems)
                    {
                        //No sort applied - sort on '0' index, descending...
                        int fieldIndex = 0; ;
                        if (fieldIndex < fieldsForSorting.Count)
                        {
                            string fieldName = fieldsForSorting[fieldIndex];

                            sortedItems = dbSet.OrderByDescending(a => a.GetType().GetProperty(fieldName).GetValue(a, null) ?? String.Empty).Skip(startIndex).Take(pageSize).ToList();
                        }
                    }

                    //Map sorted items...
                    foreach (var sortedItem in sortedItems)
                    {
                        var model = Mapper.Map<odmType, repositoryType>(sortedItem);
                        list.Add(model);
                    }
                }
            }

            return list;
        }

        //Add input instances...
        public void AddInstances(List<repositoryType> lstInstances,
                                 string instanceIdentifier,
                                 out List<repositoryType> lstIncorrectInstances,
                                 out List<repositoryType> lstCorrectInstances,
                                 out List<repositoryType> lstDuplicateInstances,
                                 out List<repositoryType> lstEditedInstances)
        {
            //For now just return list(s)...
            lstIncorrectInstances = new List<repositoryType>();
            lstCorrectInstances = new List<repositoryType>();
            lstDuplicateInstances = new List<repositoryType>();
            lstEditedInstances = new List<repositoryType>();

            var maxCount = lstInstances.Count;
            var count = 0;

            //Get repository DbSet
            var dbSet = GetDbSet();

            if (null != dbSet)
            {
                //For each input instance...
                foreach (var instance in lstInstances)
                {
                    try
                    {
                        //Attempt to copy...
                        var model = new odmType();
                        var listOfErrors = new List<ErrorModel>();
                        var listOfUpdates = new List<UpdateFieldsModel>();
                        bool result = true;    //Assume success
                        StringBuilder sbError = new StringBuilder();

                        ++count;
                        result = instance.CopyTo(model, sbError);
                        if (!result)
                        {
                            //Error - update error list
                            var error = new ErrorModel("GenericRepository<" + instance.GetType().Name + ">", sbError.ToString());
                            listOfErrors.Add(error);

                            //Update instance (via reflection), update records list...
                            instance.GetType().GetProperty("Errors").SetValue(instance, sbError.ToString());
                            lstIncorrectInstances.Add(instance);

                            //To next instance...
                            continue;
                        }

                        //Check for duplicate entry via LINQ...
                        System.Linq.Expressions.Expression<Func<odmType, bool>> predicate;
                        //NOTE: Initializing the predicate to always return true and then adding ANDs 
                        //		 will select only those timeseries with values matching all the search criteria
                        predicate = HISWebClient.Util.LinqPredicateBuilder.Create<odmType>(item => true);

                        var dupFields = instance.GetFields(FieldType.ftDuplicatesCheck);
                        var sbDupFields = new StringBuilder();
                        foreach (var dupField in dupFields)
                        {
                            //predicate = predicate.And(item => (item.GetType().GetProperty(dupField).GetValue(item, null) ?? String.Empty).ToString().ToLowerInvariant() ==
                            //                                  (model.GetType().GetProperty(dupField).GetValue(model, null) ?? String.Empty).ToString().ToLowerInvariant());

                            //Build lambda expression dynamically - necessary since 'LINQ to Entities' does not accommodate such Reflection calls as GetProperty(), GetValue etc...
                            //Source: http://www.codemag.com/article/1607041
                            //NOTE: The dynamically built expression is assumed to be of this form (for example - checking for a method code value in the Methods table)
                            //     (some db context).Methods.Where(a => a.MethodCode == model.MethodCode).FirstOrDefault();

                            //Parameter...
                            var odmT = typeof (odmType);
                            var parameterExpression = Expression.Parameter(odmT, "item");

                            //Property name to compare to...
                            var property = Expression.Property(parameterExpression, odmT.GetProperty(dupField).Name);

                            //Constant (model value to compare to)
                            var constant = Expression.Constant((model.GetType().GetProperty(dupField).GetValue(model, null) ?? String.Empty).ToString());

                            //Comparison expression (equality)...
                            var expression = Expression.Equal(property, constant);

                            //Create the lambda expression...
                            var lambdaExpression = Expression.Lambda<Func<odmType, bool>>(expression, parameterExpression);

                            //Add newly created lambda expression to predicate...
                            predicate = predicate.And(lambdaExpression);

                            sbDupFields.AppendFormat("|{0}| ", dupField);
                        }

                        //Apply duplicates check...
                        //NOTE: Calling dbSet.Where(...) runs the query on SQL Server
                        //      Calling dbSet.AsEnumerable() (before calling .Where(...) on a 'queryable' collection) 
                        //       downloads all values from SQL Server first!!  
                        //      Thus running the query on SQL Server should always be the more efficient alternative...
                        var dupInstance = dbSet.Where(predicate).FirstOrDefault();

                        if (null == dupInstance)
                        {
                            //Not a duplicate - check for duplicate in previously added records
                            //NOTE: Since no 'LINQ to Entities' calls occur herein, the lambda expression can include Reflection calls...
                            var predicate2 = HISWebClient.Util.LinqPredicateBuilder.Create<repositoryType>(item => true);
                            foreach (var dupField in dupFields)
                            {
                                predicate2 = predicate2.And(item => (item.GetType().GetProperty(dupField).GetValue(item, null) ?? String.Empty)
                                                                    .Equals((instance.GetType().GetProperty(dupField).GetValue(instance, null) ?? String.Empty)));
                            }

                            var qCorrectInstances = lstCorrectInstances.AsQueryable();
                            var existsInUpload = qCorrectInstances.Where(predicate2).FirstOrDefault();
                            
                            if (null == existsInUpload)
                            {
                                //Not a duplicate - add to context...
                                dbSet.Add(model);
                                lstCorrectInstances.Add(instance);
                            }
                            else
                            {
                                //Duplicate - update incorrect records...
                                StringBuilder sb = new StringBuilder();

                                sb.AppendFormat(Resources.IMPORT_VALUE_ISDUPLICATE, sbDupFields.ToString());
                                var error = new ErrorModel("GenericRepository<" + instance.GetType().Name + ">", sb.ToString());

                                instance.GetType().GetProperty("Errors").SetValue(instance, error.ErrorMessage);
                                lstIncorrectInstances.Add(instance);
                            }
                        }
                        else
                        {
                            //Duplicate - check for update...
                            predicate = HISWebClient.Util.LinqPredicateBuilder.Create<odmType>(item => false);

                            var updateFields = instance.GetFields(FieldType.ftUpdatesCheck);
                            foreach (var updateField in updateFields)
                            {
                                //Build lambda expression dynamically (as explained above)

                                //Parameter...
                                var odmT = typeof(odmType);
                                var parameterExpression = Expression.Parameter(odmT, "item");

                                //Property name to compare to...
                                var property = Expression.Property(parameterExpression, odmT.GetProperty(updateField).Name);

                                //Constant (model value to compare to)
                                var constant = Expression.Constant((model.GetType().GetProperty(updateField).GetValue(model, null) ?? String.Empty).ToString());

                                //Comparison expression (equality)...
                                var expression = Expression.Equal(property, constant);

                                //Create the lambda expression...
                                var lambdaExpression = Expression.Lambda<Func<odmType, bool>>(expression, parameterExpression);

                                //Add newly created lambda expression to predicate...
                                predicate = predicate.Or(lambdaExpression);
                            }

                            //Apply update check (as explained above)
                            var updatedInstance = dbSet.Where(predicate).FirstOrDefault();
                            if ( null != updatedInstance)
                            {
                                //Success - treat current instance as an update...
                                lstEditedInstances.Add(instance);

                                //Scan instance properties for differences...
                                var sb = new StringBuilder();
                                foreach (var updateField in updateFields)
                                {
                                    var currentValue = (updatedInstance.GetType().GetProperty(updateField).GetValue(updatedInstance, null) ?? String.Empty).ToString().ToLowerInvariant();
                                    var updatedValue = (instance.GetType().GetProperty(updateField).GetValue(instance, null) ?? String.Empty).ToString().ToLowerInvariant();
                                    if ( currentValue != updatedValue )
                                    {
                                        sb.Append(string.Format(Resources.IMPORT_VALUE_UPDATED, updateField, currentValue, updatedValue + ";"));
                                    }
                                }

                                instance.GetType().GetProperty("Errors").SetValue(instance, sb.ToString());

                                continue;
                            }
                            else
                            {
                                //Not an update - add to duplicates list...
                                lstDuplicateInstances.Add(instance);
                            }
                        }
                    }
                    //catch (Exception ex)
                    catch (Exception)
                    {
                        lstIncorrectInstances.Add(instance);
                    }
                }
            }

            //Processing complete - return
            return;
        }

        //Delete all instances...
        public void DeleteAll()
        {
            //Get repository DbSet
            var dbSet = GetDbSet();

            if (null != dbSet)
            {
                //Success - call repository type method to queue for delete...
                var repositoryT = new repositoryType();
                if (repositoryT.QueueDeleteAll(dbSet))
                {
                    //Success - save changes...
                    OdmEntities.SaveChanges();
                }
            }

            //Processing complete - return 
            return;
        }
    }
}
