using HydroserverToolsBusinessObjects.Models;
using jQuery.DataTables.Mvc;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HydroServerToolsRepository.Repository
{
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

        void AddSites(List<SiteModel> sites, string entityConnectionString, Guid instanceGuid, out List<SiteModel> listOfIncorrectRecords, out List<SiteModel> listOfCorrectRecords, out List<SiteModel> listOfDuplicateRecords, out List<SiteModel> listOfEditedRecords);
        
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

        void AddVariables(List<VariablesModel> list, string entityConnectionString, Guid instanceGuid, out List<VariablesModel> listOfIncorrectRecords, out List<VariablesModel> listOfCorrectRecords, out List<VariablesModel> listOfDuplicateRecords, out List<VariablesModel> listOfEditedRecords);

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

        void AddOffsetTypes(List<OffsetTypesModel> list, string entityConnectionString, Guid instanceGuid, out List<OffsetTypesModel> listOfIncorrectRecords, out List<OffsetTypesModel> listOfCorrectRecords, out List<OffsetTypesModel> listOfDuplicateRecords, out List<OffsetTypesModel> listOfEditedRecords);

        void deleteAll(string entityConnectionString);
    }
    
    //ISOMetaData
    //interface IISOMetadataRepository
    //{
    //    List<ISOMetadataModel> GetAll(string connectionString);

    //    //bool Save();

    //    void AddISOMetadata(List<ISOMetadataModel> list, string entityConnectionString, Guid instanceGuid, out List<ISOMetadataModel> listOfIncorrectRecords, out List<ISOMetadataModel> listOfCorrectRecords, out List<ISOMetadataModel> listOfDuplicateRecords, out List<ISOMetadataModel> listOfEditedRecords);

    //}
    
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

        void AddSources(List<SourcesModel> list, string entityConnectionString, Guid instanceGuid, out List<SourcesModel> listOfIncorrectRecords, out List<SourcesModel> listOfCorrectRecords, out List<SourcesModel> listOfDuplicateRecords, out List<SourcesModel> listOfEditedRecords);

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

        void AddMethods(List<MethodModel> list, string entityConnectionString, Guid instanceGuid, out List<MethodModel> listOfIncorrectRecords, out List<MethodModel> listOfCorrectRecords, out List<MethodModel> listOfDuplicateRecords, out List<MethodModel> listOfEditedRecords);

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

        void AddLabMethods(List<LabMethodModel> list, string entityConnectionString, Guid instanceGuid, out List<LabMethodModel> listOfIncorrectRecords, out List<LabMethodModel> listOfCorrectRecords, out List<LabMethodModel> listOfDuplicateRecords, out List<LabMethodModel> listOfEditedRecords);

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

        void AddSamples(List<SampleModel> list, string entityConnectionString, Guid instanceGuid, out List<SampleModel> listOfIncorrectRecords, out List<SampleModel> listOfCorrectRecords, out List<SampleModel> listOfDuplicateRecords, out List<SampleModel> listOfEditedRecords);

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

        void AddQualifiers(List<QualifiersModel> list, string entityConnectionString, Guid instanceGuid, out List<QualifiersModel> listOfIncorrectRecords, out List<QualifiersModel> listOfCorrectRecords, out List<QualifiersModel> listOfDuplicateRecords, out List<QualifiersModel> listOfEditedRecords);

        void deleteAll(string entityConnectionString);
    }

    //QualityControlLevel;
    interface IQualityControlLevelRepository
    {
        List<QualityControlLevelModel> GetAll(string connectionString);

        List<QualityControlLevelModel> GetQualityControlLevels(string connectionString, int startIndex,
          int pageSize,
          ReadOnlyCollection<SortedColumn> sortedColumns,
          out int totalRecordCount,
          out int searchRecordCount,
          string searchString);


        void AddQualityControlLevel(List<QualityControlLevelModel> list, string entityConnectionString, Guid instanceGuid, out List<QualityControlLevelModel> listOfIncorrectRecords, out List<QualityControlLevelModel> listOfCorrectRecords, out List<QualityControlLevelModel> listOfDuplicateRecords, out List<QualityControlLevelModel> listOfEditedRecords);

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

        void AddDataValues(List<DataValuesModel> list, string entityConnectionString, Guid instanceGuid, out List<DataValuesModel> listOfIncorrectRecords, out List<DataValuesModel> listOfCorrectRecords, out List<DataValuesModel> listOfDuplicateRecords, out List<DataValuesModel> listOfEditedRecords);

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

        void AddGroupDescriptions(List<GroupDescriptionModel> list, string entityConnectionString, Guid instanceGuid, out List<GroupDescriptionModel> listOfIncorrectRecords, out List<GroupDescriptionModel> listOfCorrectRecords, out List<GroupDescriptionModel> listOfDuplicateRecords, out List<GroupDescriptionModel> listOfEditedRecords);

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

        void AddGroups(List<GroupsModel> list, string entityConnectionString, Guid instanceGuid, out List<GroupsModel> listOfIncorrectRecords, out List<GroupsModel> listOfCorrectRecords, out List<GroupsModel> listOfDuplicateRecords, out List<GroupsModel> listOfEditedRecords);

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

        void AddDerivedFrom(List<DerivedFromModel> list, string entityConnectionString, Guid instanceGuid, out List<DerivedFromModel> listOfIncorrectRecords, out List<DerivedFromModel> listOfCorrectRecords, out List<DerivedFromModel> listOfDuplicateRecords, out List<DerivedFromModel> listOfEditedRecords);

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

        void AddCategories(List<CategoriesModel> list, string entityConnectionString, Guid instanceGuid, out List<CategoriesModel> listOfIncorrectRecords, out List<CategoriesModel> listOfCorrectRecords, out List<CategoriesModel> listOfDuplicateRecords, out List<CategoriesModel> listOfEditedRecords);

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
