using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Threading.Tasks;

using System.Data.Entity;
using System.Reflection;

using EntityFramework.Metadata.Extensions;

using HydroserverToolsBusinessObjects.Models;
using HydroServerToolsRepository.Repository;
using HydroServerToolsUtilities;

using ODM_1_1_1EFModel;

namespace HydroServerTools.Utilities
{
    public class RepositoryContext
    {
        //Dictionary of model types to repository types...
        //Listed in table load order!!!
        private static Dictionary<Type, Type> modelTypesToRepositoryTypes = new Dictionary<Type, Type>
                                                                                { {typeof(SiteModel), typeof(SitesRepository)},
                                                                                  {typeof(VariablesModel), typeof(VariablesRepository)},
                                                                                  {typeof(OffsetTypesModel), typeof(OffsetTypesRepository)},
                                                                                  {typeof(SourcesModel), typeof(SourcesRepository)},
                                                                                  {typeof(MethodModel), typeof(MethodsRepository)},
                                                                                  {typeof(LabMethodModel), typeof(LabMethodsRepository)},
                                                                                  {typeof(SampleModel), typeof(SamplesRepository)},
                                                                                  {typeof(QualifiersModel), typeof(QualifiersRepository)},
                                                                                  {typeof(QualityControlLevelModel), typeof(QualityControlLevelsRepository)},
                                                                                  {typeof(DataValuesModel), typeof(DataValuesRepository)},
                                                                                  {typeof(GroupDescriptionModel), typeof(GroupDescriptionsRepository)},
                                                                                  {typeof(GroupsModel), typeof(GroupsRepository)},
                                                                                  {typeof(DerivedFromModel), typeof(DerivedFromRepository)},
                                                                                  {typeof(CategoriesModel), typeof(CategoriesRepository)}
                                                                                };
        //Dictionary of model types to Entity Framework types...
        private static Dictionary<Type, Type> modelTypesToEntityFrameworkTypes = new Dictionary<Type, Type>
                                                                                { {typeof(SiteModel), typeof(Site)},
                                                                                  {typeof(VariablesModel), typeof(Variable)},
                                                                                  {typeof(OffsetTypesModel), typeof(OffsetType)},
                                                                                  {typeof(SourcesModel), typeof(Source)},
                                                                                  {typeof(MethodModel), typeof(Method)},
                                                                                  {typeof(LabMethodModel), typeof(LabMethod)},
                                                                                  {typeof(SampleModel), typeof(Sample)},
                                                                                  {typeof(QualifiersModel), typeof(Qualifier)},
                                                                                  {typeof(QualityControlLevelModel), typeof(QualityControlLevel)},
                                                                                  {typeof(DataValuesModel), typeof(DataValue)},
                                                                                  {typeof(GroupDescriptionModel), typeof(GroupDescription)},
                                                                                  {typeof(GroupsModel), typeof(Group)},
                                                                                  {typeof(DerivedFromModel), typeof(DerivedFrom)},
                                                                                  {typeof(CategoriesModel), typeof(Category)}
                                                                                };

        //Dictionary of repository types to repository instances...
        private Dictionary<Type, object> repositoryInstances = new Dictionary<Type, object>();

        private string entityConnectionString;

        //Utilities...

        //Associate parameter names with IList instances...
        private Dictionary<string, System.Collections.IList> MakeParamNamesToLists()
        {
            return new Dictionary<string, System.Collections.IList>
                                { {"itemList", null},
                                  {"listOfIncorrectRecords", null},
                                  {"listOfCorrectRecords", null},
                                  {"listOfDuplicateRecords", null},
                                  {"listOfEditedRecords", null} };
        }

        //Associate parameter names with record types...
        private Dictionary<string, string> MakeParamNamesToRecordTypes()
        {
            return new Dictionary<string, string>
                                { {"listOfIncorrectRecords", "IncorrectRecords" },
                                  {"listOfCorrectRecords", "CorrectRecords" },
                                  {"listOfDuplicateRecords", "DuplicateRecords" },
                                  {"listOfEditedRecords", "EditedRecords" } };
        }

        //Scan method info for the input repository type - return method info for the 'Add...' method
        private MethodInfo GetRepositoryAddMethodInfo(Type repositoryType)
        {
            MethodInfo result = null;

            if (null != repositoryType)
            {
                MethodInfo[] methods = repositoryType.GetMethods();
                foreach (var method in methods)
                {
                    Type returnType = method.ReturnParameter.ParameterType;
                    string methodName = method.Name;
                    if (typeof(System.Threading.Tasks.Task) == returnType && methodName.Contains("Add"))
                    {
                        //'Add...' method found - break
                        result = method;
                        break;
                    }
                }
            }

            //Processing complete - return
            return result;
        }


        //Constructors...
        private RepositoryContext()
        {
            RepositorySemaphore = new SemaphoreSlim(1, 1);
            RepositoryTypes = new List<Type>();
        }

        public RepositoryContext(string entityConnectionStringIn ) : this()
        {
#if (DEBUG)
            //Validate/initialize input parameters...
            if (String.IsNullOrWhiteSpace(entityConnectionStringIn))
            {
                string paramName = "entityConnectionStringIn";
                throw new ArgumentException("Invalid input parameter...", paramName);
            }
#endif
            entityConnectionString = entityConnectionStringIn;
        }

        //Properties
        public SemaphoreSlim RepositorySemaphore { get; private set; }

        public List<Type> RepositoryTypes { get; private set; }

        //Methods...

        //Retrieve the repository instance per the input type...
        // Returns null if no such instance found...
        public async Task<object> RepositoryByType(Type repositoryType)
        {
            object result = null;
            if (null != repositoryType)
            {
                using (await RepositorySemaphore.UseWaitAsync())
                {
                    if (repositoryInstances.Keys.Contains(repositoryType))
                    {
                        result = repositoryInstances[repositoryType];
                    }
                }
            }

            //Processing complete - return result
            return result;
        }

        //Retrieve the model type per the input table name...
        //Returns null if no such type found...
        public async Task<Type> ModelTypeByTableName(string tableName)
        {
            Type result = null;
            if (!String.IsNullOrWhiteSpace(tableName))
            {
                //Create a DbContext...
                var context = new ODM_1_1_1EFModel.ODM_1_1_1Entities(entityConnectionString);
                
                //Create a 'Db' method for the associated entity framework type via reflection...
                //Looking for a method signature: Db<T>(System.Data.Entity.DbContext)
                Type typeExtension = typeof(EntityFramework.Metadata.Extensions.MappingApiExtensions);

                MethodInfo methodInfoTarget = null;
                MethodInfo[] methodInfoDbs = typeExtension.GetMethods().Where(mi => mi.Name == "Db").ToArray();

                foreach (var methodInfoDb in methodInfoDbs)
                {
                    if (methodInfoDb.IsGenericMethod)
                    {
                        //Generic method...
                        ParameterInfo[] pInfos = methodInfoDb.GetParameters();

                        if (1 == pInfos.Length)
                        {
                            //Single parameter...
                            if (typeof(DbContext) == pInfos[0].ParameterType)
                            {
                                //Parameter type: DbContext...
                                methodInfoTarget = methodInfoDb;
                                break;
                            }
                        }
                    }
                }

                if (null != methodInfoTarget)
                {
                    //'Db' method information found - scan entity framework types...
                    foreach (var kvp in modelTypesToEntityFrameworkTypes)
                    {
                        Type modelType = kvp.Key;
                        Type efType = kvp.Value;

                        using (await RepositorySemaphore.UseWaitAsync())
                        {
                            //Call extension method to retrieve table name for current entity framework type...
                            MethodInfo methodInfo_G = methodInfoTarget.MakeGenericMethod(efType);
                            var efMetadata = methodInfo_G.Invoke(context, new object[] { context }) as EntityFramework.Metadata.IEntityMap;

                            if (tableName.ToLowerInvariant() == efMetadata.TableName.ToLowerInvariant())
                            {
                                //Matching table name - return model type...
                                result = modelType;
                                break;
                            }
                        }
                    }
                }
            }

            //Processing complete - return result
            return result;
        }

        //Load content from specified <fileprefix>-<modeltypename>-validated.bin files into database... 
        public async Task LoadDb(string validatedFileNamePrefix, string pathValidated, string pathProcessed, StatusContext statusContext, DbLoadContext dbLoadContext)
        {
            //Validate/initialize input parameters...
            if ((!String.IsNullOrWhiteSpace(validatedFileNamePrefix)) && 
                (!String.IsNullOrWhiteSpace(pathValidated)) &&
                (!String.IsNullOrWhiteSpace(pathProcessed)) &&
                (null != statusContext) &&
                (null != dbLoadContext))
            {
                //Dictionary of param names to generic lists for repository 'Add...' call...
                Dictionary<string, System.Collections.IList> paramNamesToLists = MakeParamNamesToLists();

                //Dictionary of param names to record types for results of repository 'Add...' call...
                Dictionary<string, string> paramNamesToRecordTypes = MakeParamNamesToRecordTypes();

                //Input parameters valid - scan available model types...
                foreach (var kvp in modelTypesToRepositoryTypes)
                {
                    //For the current model type - dynamically construct a generic list...
                    //Source: https://blog.magnusmontin.net/2014/10/31/generic-type-parameters-and-dynamic-types-in-csharp/
                    Type modelType = kvp.Key;
                    Type repositoryType = kvp.Value;
                    Type efType = modelTypesToEntityFrameworkTypes[modelType];  //Get associated entity framework type...

                    Type tGenericList = typeof(List<>);
                    Type modelListType = tGenericList.MakeGenericType(modelType);
                    System.Collections.IList iList = (System.Collections.IList)Activator.CreateInstance(modelListType);

                    //Construct validated binary file path and name...
                    string binFilePathAndName = pathValidated + validatedFileNamePrefix + "-" + modelType.Name + "-validated.bin";

                    using (await RepositorySemaphore.UseWaitAsync())
                    {
                        try
                        {
                            //For the input file stream...
                            //using (var fileStream = new FileStream(binFileNameAndPath, FileMode.Open, FileAccess.Read))
                            using (var fileStream = new FileStream(binFilePathAndName, FileMode.Open, FileAccess.Read, FileShare.Read, 65536, true))
                            {
                                //De-serialize to generic list...
                                BinaryFormatter binFor = new BinaryFormatter();
                                iList = (System.Collections.IList)binFor.Deserialize(fileStream);
                                if (null != iList && 0 < iList.Count)
                                {
                                    //Item(s) de-serialized - find the 'Add...' method from the associated repository type...
                                    MethodInfo methodAdd = GetRepositoryAddMethodInfo(repositoryType); 

                                    if (null != methodAdd)
                                    {
                                        //'Add...' method found - build parameters for method call...
                                        List<string> keys = paramNamesToLists.Keys.ToList();
                                        foreach (var key in keys)
                                        {
                                            if (null != paramNamesToLists[key])
                                            {
                                                paramNamesToLists[key].Clear();
                                                paramNamesToLists[key] = null;
                                            }

                                            paramNamesToLists[key] = (System.Collections.IList)Activator.CreateInstance(modelListType);
                                            if ("itemList" == key)
                                            {
                                                //Assign de-serialized items...
                                                var itemList = paramNamesToLists[key];
                                                foreach (var item in iList)
                                                {
                                                    itemList.Add(item);
                                                }
                                            }
                                        }

                                        //Create repository instance...
                                        ConstructorInfo constructorInfo = repositoryType.GetConstructor(Type.EmptyTypes);
                                        if (null != constructorInfo)
                                        {
                                            //Allocate repository instance...
                                            object repositoryInstance = constructorInfo.Invoke(new object[] { });

                                            //Create arguments array...
                                            object[] objArray = new object[] { paramNamesToLists["itemList"],
                                                                                entityConnectionString,
                                                                                validatedFileNamePrefix,
                                                                                paramNamesToLists["listOfIncorrectRecords"],
                                                                                paramNamesToLists["listOfCorrectRecords"],
                                                                                paramNamesToLists["listOfDuplicateRecords"],
                                                                                paramNamesToLists["listOfEditedRecords"],
                                                                                statusContext
                                                                                };
                                            //Call 'Add...' method on newly created instance...
                                            methodAdd.Invoke(repositoryInstance, objArray);

                                            //Retrieve db table name via EntityFramework metadata extensions...
                                            string tableName = String.Empty;

                                            //Create a DbContext...
                                            var context = new ODM_1_1_1EFModel.ODM_1_1_1Entities(entityConnectionString);

                                            //Create a 'Db' method for the associated entity framework type via reflection...
                                            //Looking for a method signature: Db<T>(System.Data.Entity.DbContext)
                                            Type typeExtension = typeof(EntityFramework.Metadata.Extensions.MappingApiExtensions);

                                            MethodInfo methodInfoTarget = null;
                                            MethodInfo[] methodInfoDbs = typeExtension.GetMethods().Where(mi => mi.Name == "Db").ToArray();

                                            foreach (var methodInfoDb in methodInfoDbs)
                                            {
                                                if (methodInfoDb.IsGenericMethod)
                                                {
                                                    //Generic method...
                                                    ParameterInfo[] pInfos = methodInfoDb.GetParameters();

                                                    if (1 == pInfos.Length)
                                                    {
                                                        //Single parameter...
                                                        if (typeof(DbContext) == pInfos[0].ParameterType)
                                                        {
                                                            //Parameter type: DbContext...
                                                            methodInfoTarget = methodInfoDb;
                                                            break;
                                                        }
                                                    }
                                                }
                                            }

                                            if (null != methodInfoTarget)
                                            {
                                                //'Db' method info found - set generic type, invoke method...
                                                MethodInfo methodInfo_G = methodInfoTarget.MakeGenericMethod(efType);
                                                var efMetadata = methodInfo_G.Invoke(context, new object[] { context }) as EntityFramework.Metadata.IEntityMap;

                                                //Retrieve table name...
                                                tableName = efMetadata.TableName;
                                            }

                                            if (!String.IsNullOrWhiteSpace(tableName))
                                            {
                                                //Table name retrieved - commit new and updated records, if indicated...
                                                RepositoryUtils repositoryUtils = new RepositoryUtils();
                                                Type typeRepositoryUtils = typeof(RepositoryUtils);
                                                MethodInfo methodInfoCommit = typeRepositoryUtils.GetMethod("CommitNewRecords");
                                                MethodInfo methodInfoUpdate = typeRepositoryUtils.GetMethod("CommitUpdateRecords");
                                                MethodInfo methodInfo_G = null;

                                                System.Collections.IList iList2 = paramNamesToLists["listOfCorrectRecords"];
                                                if (0 < iList2.Count)
                                                {
                                                    //New records exist - invoke 'CommitNew...' method...
                                                    methodInfo_G = methodInfoCommit.MakeGenericMethod(modelType);
                                                    object[] objArrayC = new object[] { entityConnectionString,
                                                                                        tableName,
                                                                                        iList2,
                                                                                        statusContext
                                                                                        };
                                                    methodInfo_G.Invoke(repositoryUtils, objArrayC);
                                                }

                                                iList2 = paramNamesToLists["listOfEditedRecords"];
                                                if (0 < iList2.Count)
                                                {
                                                    //Updated records exist - invoke 'CommitUpdate...' method...
                                                    methodInfo_G = methodInfoUpdate.MakeGenericMethod(modelType);
                                                    object[] objArrayU = new object[] { entityConnectionString,
                                                                                        tableName,
                                                                                        iList2
                                                                                        };
                                                    methodInfo_G.Invoke(repositoryUtils, objArrayU);
                                                }

                                                //Create and Save load result...
                                                using (await dbLoadContext.DbLoadSemaphore.UseWaitAsync())
                                                {
                                                    var dbLoadResult = new DbLoadResult(tableName,
                                                                                        paramNamesToLists["listOfCorrectRecords"].Count,      //inserted
                                                                                        paramNamesToLists["listOfEditedRecords"].Count,       //updated
                                                                                        paramNamesToLists["listOfIncorrectRecords"].Count,    //rejected
                                                                                        paramNamesToLists["listOfDuplicateRecords"].Count);   //duplicated

                                                    dbLoadContext.DbLoadResults.Add(dbLoadResult);
                                                }
                                            }

                                            //Retrieve UpdateableItem<> initializing constructor definition
                                            Type tUpdateableItem = typeof(UpdateableItem<>);
                                            Type gUpdateableItem = tUpdateableItem.MakeGenericType(modelType);
                                            ConstructorInfo ciUpdateableItem = gUpdateableItem.GetConstructor(new Type[] { modelType, typeof(int) });
                                      
                                            //For each record type from the 'Add...' call...
                                            foreach (var kvpair in paramNamesToRecordTypes)
                                            {
                                                //Retrieve associated results list...
                                                string recordType = paramNamesToRecordTypes[kvpair.Key];
                                                System.Collections.IList recordList = paramNamesToLists[kvpair.Key];

                                                //Create a list of UpdateableItems for output to binary file...
                                                Type updateableItemsListType = tGenericList.MakeGenericType(gUpdateableItem);
                                                System.Collections.IList iUpdateableItemsList = (System.Collections.IList)Activator.CreateInstance(updateableItemsListType);
                                                int initialId = 100;
                                                int currentIndex = 0;

                                                foreach (var record in recordList)
                                                {
                                                    var updateableItem = ciUpdateableItem.Invoke(new object[] {record, ++initialId });
                                                    iUpdateableItemsList.Add(updateableItem);

                                                    if ("IncorrectRecords" == recordType)
                                                    {
                                                        //Processing incorrect records - update 'IsError' status message ItemIds...
                                                        using (await statusContext.StatusMessagesSemaphore.UseWaitAsync())
                                                        {
                                                            if (statusContext.StatusMessages.ContainsKey(modelType.Name))
                                                            {
                                                                var statusMessages = statusContext.StatusMessages[modelType.Name];
                                                                foreach (var statusMessage in statusMessages)
                                                                {
                                                                    if (statusMessage.IsError && (currentIndex == statusMessage.ItemId))
                                                                    {
                                                                        statusMessage.ItemId = initialId;
                                                                    }
                                                                }
                                                            }
                                                        }
                                                        ++currentIndex;
                                                    }
                                                }

                                                //Create file path and name...
                                                var binFilePathAndName_1 = pathProcessed + validatedFileNamePrefix + "-" +
                                                                         modelType.Name + "-" + recordType + ".bin";
                                                try
                                                {
                                                    //For the output file stream...
                                                    //using (var fileStream_1 = new FileStream(binFilePathAndName_1, FileMode.Create))
                                                    using (var fileStream_1 = new FileStream(binFilePathAndName_1, FileMode.Create, FileAccess.Write, FileShare.None, 65536, true))
                                                    {
                                                        //Serialize validated records to file stream as binary...
                                                        BinaryFormatter binFor_1 = new BinaryFormatter();

                                                        //binFor_1.Serialize(fileStream_1, recordList);
                                                        binFor_1.Serialize(fileStream_1, iUpdateableItemsList);
                                                        fileStream_1.Flush();
                                                    }
                                                }
                                                catch (Exception ex)
                                                {
                                                    //For now take no action...
                                                    string msg = ex.Message;
                                                    int nn = 5;

                                                    ++nn;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            //File not found - for now, take no action...
                            string msg = ex.Message;
                        }
                    }
                }
            }
        }

        //Update input database table with the contents of the input updated items, 
        // adjust input StatusContext and DbLoadContext instances accordingly
        public async Task<TableUpdateResult> UpdateDbTable<tModelType>(IUpdateableItemsData<tModelType> iUpdateableItems,
                                                     string pathProcessed,
                                                     StatusContext statusContext,
                                                     DbLoadContext dbLoadContext)
        {
             TableUpdateResult result = null;

            //Validate/initialize input parameters...
            if ((null != iUpdateableItems) &&
                (!String.IsNullOrWhiteSpace(pathProcessed)) &&
                (null != statusContext) &&
                (null != dbLoadContext))
            {
                //Check model type...
                Type modelType = typeof(tModelType);
                if (modelTypesToRepositoryTypes.ContainsKey(modelType))
                {
                    //Known model type - retrieve associated repository and entity framework types...
                    Type repositoryType = modelTypesToRepositoryTypes[modelType];
                    Type efType = modelTypesToEntityFrameworkTypes[modelType];

                    //Allocate dictionary of param names to generic lists for repository 'Add...' call...
                    Dictionary<string, System.Collections.IList> paramNamesToLists = MakeParamNamesToLists();
                    //Allocate dictionary of param names to record types for results of repository 'Add...' call...
                    Dictionary<string, string> paramNamesToRecordTypes = MakeParamNamesToRecordTypes();

                    //Allocate a generic list for model type...
                    Type tGenericList = typeof(List<>);
                    Type modelListType = tGenericList.MakeGenericType(modelType);
                    System.Collections.IList iList = (System.Collections.IList)Activator.CreateInstance(modelListType);

                    //Populate generic list from input...
                    foreach (var updateableItem in iUpdateableItems.UpdateableItems)
                    {
                        iList.Add(updateableItem.Item);
                    }

                    if (null != iList && 0 < iList.Count)
                    {
                        using (await RepositorySemaphore.UseWaitAsync())
                        {
                            //Updated items exist - find the 'Add...' method from the associated repository type...
                            MethodInfo methodAdd = GetRepositoryAddMethodInfo(repositoryType);

                            if (null != methodAdd)
                            {
                                //'Add...' method found - build parameters for method call... 
                                List<string> keys = paramNamesToLists.Keys.ToList();
                                foreach (var key in keys)
                                {
                                    if (null != paramNamesToLists[key])
                                    {
                                        paramNamesToLists[key].Clear();
                                        paramNamesToLists[key] = null;
                                    }

                                    paramNamesToLists[key] = ("itemList" == key) ? iList : (System.Collections.IList)Activator.CreateInstance(modelListType);
                                }

                                //Create repository instance...
                                ConstructorInfo constructorInfo = repositoryType.GetConstructor(Type.EmptyTypes);
                                if (null != constructorInfo)
                                {
                                    //Allocate repository instance...
                                    object repositoryInstance = constructorInfo.Invoke(new object[] { });

                                    //Create arguments array...
                                    object[] objArray = new object[] { paramNamesToLists["itemList"],
                                                                        entityConnectionString,
                                                                        iUpdateableItems.UploadId,
                                                                        paramNamesToLists["listOfIncorrectRecords"],
                                                                        paramNamesToLists["listOfCorrectRecords"],
                                                                        paramNamesToLists["listOfDuplicateRecords"],
                                                                        paramNamesToLists["listOfEditedRecords"],
                                                                        statusContext
                                                                        };
                                    //Call 'Add...' method on newly created instance...
                                    methodAdd.Invoke(repositoryInstance, objArray);

                                    //Retrieve db table name from input...
                                    string tableName = iUpdateableItems.TableName;
                                    if (!String.IsNullOrWhiteSpace(tableName))
                                    {
                                        //Table name retrieved - commit new and updated records, if indicated...
                                        RepositoryUtils repositoryUtils = new RepositoryUtils();
                                        Type typeRepositoryUtils = typeof(RepositoryUtils);
                                        MethodInfo methodInfoCommit = typeRepositoryUtils.GetMethod("CommitNewRecords");
                                        MethodInfo methodInfoUpdate = typeRepositoryUtils.GetMethod("CommitUpdateRecords");
                                        MethodInfo methodInfo_G = null;

                                        System.Collections.IList iList2 = paramNamesToLists["listOfCorrectRecords"];
                                        if (0 < iList2.Count)
                                        {
                                            //New records exist - invoke 'CommitNew...' method...
                                            methodInfo_G = methodInfoCommit.MakeGenericMethod(modelType);
                                            object[] objArrayC = new object[] { entityConnectionString,
                                                                                tableName,
                                                                                iList2,
                                                                                statusContext
                                                                                };
                                            methodInfo_G.Invoke(repositoryUtils, objArrayC);
                                        }

                                        iList2 = paramNamesToLists["listOfEditedRecords"];
                                        if (0 < iList2.Count)
                                        {
                                            //Updated records exist - invoke 'CommitUpdate...' method...
                                            methodInfo_G = methodInfoUpdate.MakeGenericMethod(modelType);
                                            object[] objArrayU = new object[] { entityConnectionString,
                                                                                tableName,
                                                                                iList2
                                                                                };
                                            methodInfo_G.Invoke(repositoryUtils, objArrayU);
                                        }

                                        //'CommitNew...' and 'CommitUpdate' processing complete - allocate table update result instance...
                                        result = new TableUpdateResult(tableName);

                                        Dictionary<string, List<int>> listKeysToItemIds = new Dictionary<string, List<int>>();
                                        foreach (var kvp in paramNamesToLists)
                                        {
                                            switch (kvp.Key)
                                            {
                                                case "listOfCorrectRecords":
                                                    listKeysToItemIds.Add(kvp.Key, result.CorrectItemIds);
                                                    break;
                                                case "listOfDuplicateRecords":
                                                    listKeysToItemIds.Add(kvp.Key, result.DuplicateItemIds);
                                                    break;
                                                case "listOfEditedRecords":
                                                    listKeysToItemIds.Add(kvp.Key, result.EditedItemIds);
                                                    break;
                                                case "listOfIncorrectRecords":
                                                    listKeysToItemIds.Add(kvp.Key, result.IncorrectItemIds);
                                                    break;
                                                default:
                                                    //"itemList" or any other values...
                                                    continue;
                                            }
                                        }

                                        //For each input updateable item...
                                        foreach (var updateableItem in iUpdateableItems.UpdateableItems)
                                        {
                                            var item = updateableItem.Item;
                                            var itemId = updateableItem.ItemId;

                                            //For each 'listOf...Records' list...
                                            foreach (var kvp in listKeysToItemIds)
                                            {
                                                var recordsList = paramNamesToLists[kvp.Key];
                                                foreach (var record in recordsList)
                                                {
                                                    //NOTE: Using default operator == for reference types here
                                                    // ASSUMPTION: Repository Method 'Add' implementations COPY model class references 
                                                    //                   - from the input 'items' list 
                                                    //                   - to the 'listOf...Records' lists!!
                                                    // Thus use of the default operator == (which for reference types other than string 
                                                    //  returns true if the two operands refer to the same object) is safe here... 
                                                    if ((object)item == record)
                                                    {
                                                        var itemIds = kvp.Value;
                                                        itemIds.Add(itemId);
                                                    }
                                                }
                                            }
                                        }

                                        int y = 10;

                                        ++y;

                                        //TO DO - Update the associated dbLoadResult in the input dbLoadContext

                                        //TO DO - Update the binary files...

                                        //TO DO - Update the statusContext...
                                    }
                                }
                            }
                        }
                    }
                }
            }

            //Processing complete - return result
            return result;
        }

    }
}