using HydroServerTools.Models;
using HydroserverToolsBusinessObjects.Models;
using jQuery.DataTables.Mvc;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using HydroServerToolsUtilities;

namespace HydroServerToolsRepository.Repository
{
    //A simple repository interface for actions involving multiple repositories...
    interface IRepository
    {
        //For the input table names, return a dictionary of table names to record counts
        Dictionary<string, int> GetTableRecordCounts(List<string> tableNames);
    }

    //"Sites", "Variables", "OffsetTypes", "ISOMetadata", "Sources", "Methods", "LabMethods", "Samples", "Qualifiers", "QualityControlLevels", "DataValues", "GroupDescriptions", "Groups", "DerivedFrom", "Categories"};

    //Sites
    interface ISitesRepository
    {
        List<SiteModel> GetAll(string connectionString);

        List<SiteModel> GetSites(string connectionString, int startIndex,
            int pageSize,
            ReadOnlyCollection<SortedColumn> sortedColumns,
            out int totalRecordCount,
            out int searchRecordCount,
            string searchString);

        Task AddSites(List<SiteModel> sites, string entityConnectionString, string instanceIdentifier, List<SiteModel> listOfIncorrectRecords, List<SiteModel> listOfCorrectRecords, List<SiteModel> listOfDuplicateRecords, List<SiteModel> listOfEditedRecords, StatusContext statusContext);

        void deleteAll(string entityConnectionString);
    }
    
    //Variables
    interface IVariablesRepository
    {
        List<VariablesModel> GetAll(string connectionString);

        List<VariablesModel> GetVariables(string connectionString, int startIndex,
          int pageSize,
          ReadOnlyCollection<SortedColumn> sortedColumns,
          out int totalRecordCount,
          out int searchRecordCount,
          string searchString);

        Task AddVariables(List<VariablesModel> list, string entityConnectionString, string instanceIdentifier, List<VariablesModel> listOfIncorrectRecords, List<VariablesModel> listOfCorrectRecords, List<VariablesModel> listOfDuplicateRecords, List<VariablesModel> listOfEditedRecords, StatusContext statusContext);

        void deleteAll(string entityConnectionString);
    }
    
    //OffsetTypes
    interface IOffsetTypesRepository
    {
        List<OffsetTypesModel> GetAll(string connectionString);

        List<OffsetTypesModel> GetOffsetTypes(string connectionString, int startIndex,
          int pageSize,
          ReadOnlyCollection<SortedColumn> sortedColumns,
          out int totalRecordCount,
          out int searchRecordCount,
          string searchString);

        Task AddOffsetTypes(List<OffsetTypesModel> list, string entityConnectionString, string instanceIdentifier, List<OffsetTypesModel> listOfIncorrectRecords, List<OffsetTypesModel> listOfCorrectRecords, List<OffsetTypesModel> listOfDuplicateRecords, List<OffsetTypesModel> listOfEditedRecords, StatusContext statusContext);

        void deleteAll(string entityConnectionString);
    }
    
    //Sources
    interface ISourcesRepository
    {
        List<SourcesModel> GetAll(string connectionString);

        List<SourcesModel> GetSources(string connectionString, int startIndex,
            int pageSize,
            ReadOnlyCollection<SortedColumn> sortedColumns,
            out int totalRecordCount,
            out int searchRecordCount,
            string searchString);

        Task AddSources(List<SourcesModel> list, string entityConnectionString, string instanceIdentifier, List<SourcesModel> listOfIncorrectRecords, List<SourcesModel> listOfCorrectRecords, List<SourcesModel> listOfDuplicateRecords, List<SourcesModel> listOfEditedRecords, StatusContext statusContext);

        void deleteAll(string entityConnectionString);
    }

    //Methods
    interface IMethodsRepository
    {
        List<MethodModel> GetAll(string connectionString);

        List<MethodModel> GetMethods(string connectionString, int startIndex,
           int pageSize,
           ReadOnlyCollection<SortedColumn> sortedColumns,
           out int totalRecordCount,
           out int searchRecordCount,
           string searchString);

        Task AddMethods(List<MethodModel> list, string entityConnectionString, string instanceIdentifier, List<MethodModel> listOfIncorrectRecords, List<MethodModel> listOfCorrectRecords, List<MethodModel> listOfDuplicateRecords, List<MethodModel> listOfEditedRecords, StatusContext statusContext);

        void deleteAll(string entityConnectionString);
    }

    //LabMethods
    interface ILabMethodsRepository
    {
        List<LabMethodModel> GetAll(string connectionString);

        List<LabMethodModel> GetLabMethods(string connectionString, int startIndex,
           int pageSize,
           ReadOnlyCollection<SortedColumn> sortedColumns,
           out int totalRecordCount,
           out int searchRecordCount,
           string searchString);

        Task AddLabMethods(List<LabMethodModel> list, string entityConnectionString, string instanceIdentifier, List<LabMethodModel> listOfIncorrectRecords, List<LabMethodModel> listOfCorrectRecords, List<LabMethodModel> listOfDuplicateRecords, List<LabMethodModel> listOfEditedRecords, StatusContext statusContext);

        void deleteAll(string entityConnectionString);
    }

    //Samples
    interface ISamplesRepository
    {
        List<SampleModel> GetAll(string connectionString);

        List<SampleModel> GetSamples(string connectionString, int startIndex,
                                       int pageSize,
                                       ReadOnlyCollection<SortedColumn> sortedColumns,
                                       out int totalRecordCount,
                                       out int searchRecordCount,
                                       string searchString);

        Task AddSamples(List<SampleModel> list, string entityConnectionString, string instanceIdentifier, List<SampleModel> listOfIncorrectRecords, List<SampleModel> listOfCorrectRecords, List<SampleModel> listOfDuplicateRecords, List<SampleModel> listOfEditedRecords, StatusContext statusContext);

        void deleteAll(string entityConnectionString);
    }

    //Qualifiers
    interface IQualifiersRepository
    {
        List<QualifiersModel> GetAll(string connectionString);

        List<QualifiersModel> GetQualifiers(string connectionString, int startIndex,
          int pageSize,
          ReadOnlyCollection<SortedColumn> sortedColumns,
          out int totalRecordCount,
          out int searchRecordCount,
          string searchString);

        Task AddQualifiers(List<QualifiersModel> list, string entityConnectionString, string instanceIdentifier, List<QualifiersModel> listOfIncorrectRecords, List<QualifiersModel> listOfCorrectRecords, List<QualifiersModel> listOfDuplicateRecords, List<QualifiersModel> listOfEditedRecords, StatusContext statusContext);

        void deleteAll(string entityConnectionString);
    }

    //QualityControlLevel;
    interface IQualityControlLevelsRepository
    {
        List<QualityControlLevelModel> GetAll(string connectionString);

        List<QualityControlLevelModel> GetQualityControlLevels(string connectionString, int startIndex,
          int pageSize,
          ReadOnlyCollection<SortedColumn> sortedColumns,
          out int totalRecordCount,
          out int searchRecordCount,
          string searchString);


        Task AddQualityControlLevels(List<QualityControlLevelModel> list, string entityConnectionString, string instanceIdentifier, List<QualityControlLevelModel> listOfIncorrectRecords, List<QualityControlLevelModel> listOfCorrectRecords, List<QualityControlLevelModel> listOfDuplicateRecords, List<QualityControlLevelModel> listOfEditedRecords, StatusContext statusContext);

        void deleteAll(string entityConnectionString);
    }
    //DataValues
    interface IDataValuesRepository
    {
        List<DataValuesModel> GetAll(string connectionString);

        List<DataValuesModel> GetDatavalues(string connectionString, int startIndex,
                                     int pageSize,
                                     ReadOnlyCollection<SortedColumn> sortedColumns,
                                     out int totalRecordCount,
                                     out int searchRecordCount,
                                     string searchString);

        Task AddDataValues(List<DataValuesModel> list, string entityConnectionString, string instanceIdentifier, List<DataValuesModel> listOfIncorrectRecords, List<DataValuesModel> listOfCorrectRecords, List<DataValuesModel> listOfDuplicateRecords, List<DataValuesModel> listOfEditedRecords, StatusContext statusContext);

        void deleteAll(string entityConnectionString);
    }
    
    //GroupDescription
    interface IGroupDescriptionsRepository
    {
        List<GroupDescriptionModel> GetAll(string connectionString);

        List<GroupDescriptionModel> GetGroupDescriptions(string connectionString, int startIndex,
                                   int pageSize,
                                   ReadOnlyCollection<SortedColumn> sortedColumns,
                                   out int totalRecordCount,
                                   out int searchRecordCount,
                                   string searchString);

        Task AddGroupDescriptions(List<GroupDescriptionModel> list, string entityConnectionString, string instanceIdentifier, List<GroupDescriptionModel> listOfIncorrectRecords, List<GroupDescriptionModel> listOfCorrectRecords, List<GroupDescriptionModel> listOfDuplicateRecords, List<GroupDescriptionModel> listOfEditedRecords, StatusContext statusContext);

        void deleteAll(string entityConnectionString);
    }

    //Groups
    interface IGroupsRepository
    {
        List<GroupsModel> GetAll(string connectionString);

        List<GroupsModel> GetGroups(string connectionString, int startIndex,
                                   int pageSize,
                                   ReadOnlyCollection<SortedColumn> sortedColumns,
                                   out int totalRecordCount,
                                   out int searchRecordCount,
                                   string searchString);

        Task AddGroups(List<GroupsModel> list, string entityConnectionString, string instanceIdentifier, List<GroupsModel> listOfIncorrectRecords, List<GroupsModel> listOfCorrectRecords, List<GroupsModel> listOfDuplicateRecords, List<GroupsModel> listOfEditedRecords, StatusContext statusContext);

        void deleteAll(string entityConnectionString);
    }

    //DerivedFrom
    interface IDerivedFromRepository
    {
        List<DerivedFromModel> GetAll(string connectionString);

        List<DerivedFromModel> GetDerivedFrom(string connectionString, int startIndex,
                                int pageSize,
                                ReadOnlyCollection<SortedColumn> sortedColumns,
                                out int totalRecordCount,
                                out int searchRecordCount,
                                string searchString);

        Task AddDerivedFrom(List<DerivedFromModel> list, string entityConnectionString, string instanceIdentifier, List<DerivedFromModel> listOfIncorrectRecords, List<DerivedFromModel> listOfCorrectRecords, List<DerivedFromModel> listOfDuplicateRecords, List<DerivedFromModel> listOfEditedRecords, StatusContext statusContext);

        void deleteAll(string entityConnectionString);
    }
    //Categories
    interface ICategoriesRepository
    {
        List<CategoriesModel> GetAll(string connectionString);

        List<CategoriesModel> GetCategories(string connectionString, int startIndex,
                            int pageSize,
                            ReadOnlyCollection<SortedColumn> sortedColumns,
                            out int totalRecordCount,
                            out int searchRecordCount,
                            string searchString);

        Task AddCategories(List<CategoriesModel> list, string entityConnectionString, string instanceIdentifier, List<CategoriesModel> listOfIncorrectRecords, List<CategoriesModel> listOfCorrectRecords, List<CategoriesModel> listOfDuplicateRecords, List<CategoriesModel> listOfEditedRecords, StatusContext statusContext);

        void deleteAll(string entityConnectionString);
    }
    //SeriesCatalog
    interface ISeriesCatalogRepository
    {
        List<SeriesCatalogModel> GetSeriesCatalog(string connectionString, int startIndex,
                            int pageSize,
                            ReadOnlyCollection<SortedColumn> sortedColumns,
                            out int totalRecordCount,
                            out int searchRecordCount,
                            string searchString);
    }

    interface IDatabaseRepository
    {
        DatabaseTableValueCountModel GetDatabaseTableValueCount(string connectionString);
    }
}
