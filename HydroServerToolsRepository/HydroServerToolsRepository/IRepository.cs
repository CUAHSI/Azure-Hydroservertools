using HydroserverToolsBusinessObjects.Models;
using System;
using System.Collections.Generic;
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

        //bool Save();

        void AddSites(List<SiteModel> sites, string entityConnectionString, out List<SiteModel> listOfIncorrectRecords, out List<SiteModel> listOfCorrectRecords, out List<SiteModel> listOfDuplicateRecords, out List<SiteModel> listOfEditedRecords);

    }
    
    //Variables
    interface IVariablesRepository
    {
        List<VariablesModel> GetAll(string connectionString);

        //bool Save();

        void AddVariables(List<VariablesModel> list, string entityConnectionString, out List<VariablesModel> listOfIncorrectRecords, out List<VariablesModel> listOfCorrectRecords, out List<VariablesModel> listOfDuplicateRecords, out List<VariablesModel> listOfEditedRecords);

    }
    
    //OffsetTypes
    interface IOffsetTypesRepository
    {
        List<OffsetTypesModel> GetAll(string connectionString);

        //bool Save();

        void AddOffsetTypes(List<OffsetTypesModel> list, string entityConnectionString, out List<OffsetTypesModel> listOfIncorrectRecords, out List<OffsetTypesModel> listOfCorrectRecords, out List<OffsetTypesModel> listOfDuplicateRecords);

    }
    
    //ISOMetaData
    interface IISOMetadataRepository
    {
        List<ISOMetadataModel> GetAll(string connectionString);

        //bool Save();

        void AddISOMetadata(List<ISOMetadataModel> list, string entityConnectionString, out List<ISOMetadataModel> listOfIncorrectRecords, out List<ISOMetadataModel> listOfCorrectRecords, out List<ISOMetadataModel> listOfDuplicateRecords);

    }
    
    //Sources
    interface ISourcesRepository
    {
        List<SourcesModel> GetAll(string connectionString);

        //bool Save();

        void AddSources(List<SourcesModel> list, string entityConnectionString, out List<SourcesModel> listOfIncorrectRecords, out List<SourcesModel> listOfCorrectRecords, out List<SourcesModel> listOfDuplicateRecords, out List<SourcesModel> listOfEditedRecords);

    }

    //Methods
    interface IMethodsRepository
    {
        List<MethodModel> GetAll(string connectionString);

        //bool Save();

        void AddMethods(List<MethodModel> list, string entityConnectionString, out List<MethodModel> listOfIncorrectRecords, out List<MethodModel> listOfCorrectRecords, out List<MethodModel> listOfDuplicateRecords, out List<MethodModel> listOfEditedRecords);

    }

    //LabMethods
    interface ILabMethodsRepository
    {
        List<LabMethodModel> GetAll(string connectionString);

        //bool Save();

        void AddLabMethods(List<LabMethodModel> list, string entityConnectionString, out List<LabMethodModel> listOfIncorrectRecords, out List<LabMethodModel> listOfCorrectRecords, out List<LabMethodModel> listOfDuplicateRecords);

    }

    //Samples
    interface ISamplesRepository
    {
        List<SampleModel> GetAll(string connectionString);

        //bool Save();

        void AddSamples(List<SampleModel> list, string entityConnectionString, out List<SampleModel> listOfIncorrectRecords, out List<SampleModel> listOfCorrectRecords, out List<SampleModel> listOfDuplicateRecords);

    }

    //Qualifiers
    interface IQualifiersRepository
    {
        List<QualifiersModel> GetAll(string connectionString);

        //bool Save();

        void AddQualifiers(List<QualifiersModel> list, string entityConnectionString, out List<QualifiersModel> listOfIncorrectRecords, out List<QualifiersModel> listOfCorrectRecords, out List<QualifiersModel> listOfDuplicateRecords);

    }

    //QualityControlLevel;
    interface IQualityControlLevelRepository
    {
        List<QualityControlLevelModel> GetAll(string connectionString);

        //bool Save();

        void AddQualityControlLevel(List<QualityControlLevelModel> list, string entityConnectionString, out List<QualityControlLevelModel> listOfIncorrectRecords, out List<QualityControlLevelModel> listOfCorrectRecords, out List<QualityControlLevelModel> listOfDuplicateRecords);

    }
    //DataValues
    interface IDataValuesRepository
    {
        List<DataValuesModel> GetAll(string connectionString);

        //bool Save();

        void AddDataValues(List<DataValuesModel> list, string entityConnectionString, out List<DataValuesModel> listOfIncorrectRecords, out List<DataValuesModel> listOfCorrectRecords, out List<DataValuesModel> listOfDuplicateRecords, out List<DataValuesModel> listOfEditedRecords);

    }

    //GroupDescription
    interface IGroupDescriptionsRepository
    {
        List<GroupDescriptionModel> GetAll(string connectionString);

        //bool Save();

        void AddGroupDescriptions(List<GroupDescriptionModel> list, string entityConnectionString, out List<GroupDescriptionModel> listOfIncorrectRecords, out List<GroupDescriptionModel> listOfCorrectRecords, out List<GroupDescriptionModel> listOfDuplicateRecords);

    }

    //Groups
    interface IGroupsRepository
    {
        List<GroupsModel> GetAll(string connectionString);

        //bool Save();

        void AddGroups(List<GroupsModel> list, string entityConnectionString, out List<GroupsModel> listOfIncorrectRecords, out List<GroupsModel> listOfCorrectRecords, out List<GroupsModel> listOfDuplicateRecords);

    }

    //DerivedFrom
    interface IDerivedFromRepository
    {
        List<DerivedFromModel> GetAll(string connectionString);

        //bool Save();

        void AddDerivedFrom(List<DerivedFromModel> list, string entityConnectionString, out List<DerivedFromModel> listOfIncorrectRecords, out List<DerivedFromModel> listOfCorrectRecords, out List<DerivedFromModel> listOfDuplicateRecords);

    }
    //Categories
    interface ICategoriesRepository
    {
        List<CategoriesModel> GetAll(string connectionString);

        //bool Save();

        void AddCategories(List<CategoriesModel> list, string entityConnectionString, out List<CategoriesModel> listOfIncorrectRecords, out List<CategoriesModel> listOfCorrectRecords, out List<CategoriesModel> listOfDuplicateRecords);

    }

}
