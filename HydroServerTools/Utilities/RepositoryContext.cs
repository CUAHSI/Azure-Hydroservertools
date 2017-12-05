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

        //Dictionary of param names to generic lists for repository 'Add...' call
        Dictionary<string, System.Collections.IList> paramNamesToLists = new Dictionary<string, System.Collections.IList>
                                                                             { {"itemList", null},
                                                                               {"listOfIncorrectRecords", null},
                                                                               {"listOfCorrectRecords", null},
                                                                               {"listOfDuplicateRecords", null},
                                                                               {"listOfEditedRecords", null}
                                                                             };

        //Dictionary of repository types to repository instances...
        private Dictionary<Type, object> repositoryInstances = new Dictionary<Type, object>();

        private string entityConnectionString;

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

            return result;
        }

        //Load content from specified <fileprefix>-<modeltypename>-validated.bin files into database... 
        public async Task LoadDb(string validatedFileNamePrefix, string pathValidated)
        {
            //Validate/initialize input parameters...
            if ((!String.IsNullOrWhiteSpace(validatedFileNamePrefix)) && (!String.IsNullOrWhiteSpace(pathValidated)))
            {
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
                    string binFileNameAndPath = pathValidated + validatedFileNamePrefix + "-" + modelType.Name + "-validated.bin";

                    try
                    {
                        //For the input file stream...
                        using (var fileStream = new FileStream(binFileNameAndPath, FileMode.Open, FileAccess.Read))
                        {
                            using (var memoryStream = new MemoryStream())
                            {
                                //Copy to memory stream...
                                await fileStream.CopyToAsync(memoryStream);
                                memoryStream.Position = 0;

                                //De-serialize to generic list...
                                BinaryFormatter binFor = new BinaryFormatter();
                                iList = (System.Collections.IList) binFor.Deserialize(memoryStream);

                                if (null != iList && 0 < iList.Count)
                                {
                                    //Item(s) de-serialized - find the 'Add...' method from the associated repository type...
                                    MethodInfo[] methods = repositoryType.GetMethods();
                                    MethodInfo methodAdd = null;
                                    foreach (var method in methods)
                                    {
                                        string returnTypeName = method.ReturnParameter.ParameterType.FullName;
                                        string methodName = method.Name;
                                        if ("System.Void" == returnTypeName && methodName.Contains("Add"))
                                        {
                                            //'Add...' method found - break
                                            methodAdd = method;
                                            break;
                                        }
                                    }

                                    if (null != methodAdd)
                                    {
                                        //'Add...' method found - build parameters...
                                        List<string> keys = paramNamesToLists.Keys.ToList();
                                        foreach (var key in keys)
                                        {
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
                                        if ( null != constructorInfo )
                                        {
                                            object repositoryInstance = constructorInfo.Invoke(new object[] { });

                                            //Call 'Add...' method on newly created instance...
                                            var indexCorrectRecords = 4;
                                            var indexEditedRecords = 6;
                                            object[] objArray = new object[] { paramNamesToLists["itemList"],
                                                                               entityConnectionString,
                                                                               validatedFileNamePrefix,
                                                                               paramNamesToLists["listOfIncorrectRecords"],
                                                                               paramNamesToLists["listOfCorrectRecords"],
                                                                               paramNamesToLists["listOfDuplicateRecords"],
                                                                               paramNamesToLists["listOfEditedRecords"]
                                                                             };

                                            //NOTE: Contents of 'Out' parameters appear in the objArray items... 
                                            methodAdd.Invoke(repositoryInstance, objArray);

                                            //Retrieve db table name via EntityFramework metadata extensions...
                                            string tableName = String.Empty;

                                            //Create a DbContext...
                                            var context = new ODM_1_1_1EFModel.ODM_1_1_1Entities(entityConnectionString);

                                            //Create a 'Db' method for the associated entity framework type via reflection...
                                            //Looking for a method signature: Db<T>(System.Data.Entity.DbContext)
                                            Type typeExtension = typeof (EntityFramework.Metadata.Extensions.MappingApiExtensions);

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

                                            if ( null != methodInfoTarget)
                                            {
                                                //'Db' method info found - set generic type, invoke method...
                                                MethodInfo methodInfo_G = methodInfoTarget.MakeGenericMethod(efType);
                                                var efMetadata = methodInfo_G.Invoke(context, new object[] { context }) as EntityFramework.Metadata.IEntityMap;

                                                //Retrieve table name...
                                                tableName = efMetadata.TableName;
                                            }

                                            if ( !String.IsNullOrWhiteSpace(tableName))
                                            {
                                                //Table name retrieved - commit new and updated records, if indicated...
                                                RepositoryUtils repositoryUtils = new RepositoryUtils();
                                                Type typeRepositoryUtils = typeof(RepositoryUtils);
                                                MethodInfo methodInfoCommit = typeRepositoryUtils.GetMethod("CommitNewRecords");
                                                MethodInfo methodInfoUpdate = typeRepositoryUtils.GetMethod("CommitUpdateRecords");
                                                MethodInfo methodInfo_G = null;

                                                System.Collections.IList iList2 = (System.Collections.IList)objArray[indexCorrectRecords];
                                                if (0 < iList2.Count)
                                                {
                                                    //New records exist - invoke 'CommitNew...' method...
                                                    methodInfo_G = methodInfoCommit.MakeGenericMethod(modelType);
                                                    object[] objArrayC = new object[] { entityConnectionString,
                                                                                        tableName,
                                                                                        objArray[indexCorrectRecords]
                                                                                      };
                                                    methodInfo_G.Invoke(repositoryUtils, objArrayC);
                                                }

                                                iList2 = (System.Collections.IList)objArray[indexEditedRecords];
                                                if (0 < iList2.Count)
                                                {
                                                    //Updated records exist - invoke 'CommitUpdate...' method...
                                                    methodInfo_G = methodInfoUpdate.MakeGenericMethod(modelType);
                                                    object[] objArrayU = new object[] { entityConnectionString,
                                                                                        tableName,
                                                                                        objArray[indexEditedRecords]
                                                                                      };
                                                    methodInfo_G.Invoke(repositoryUtils, objArrayU);
                                                }
                                            }

                                            //TO DO - How to best return the results of the db processing - new, updated, duplicate and rejected records -
                                            //          to the client for the Summary Report??
                                            int n = 5;

                                            ++n;

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
}