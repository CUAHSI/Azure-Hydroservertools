﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;

using CsvHelper;

using HydroserverToolsBusinessObjects.Models;

using HydroserverToolsBusinessObjects.ModelMaps;

namespace HydroServerTools.Validators
{
    //A simple class for CSV data validation error...
    public class CsvDataError
    {
        //Default constructor...
        private CsvDataError() { }

        //Initializing constructor 
        public CsvDataError(string reason, string recordText, int rowIndex, int fieldIndex)
        {
            //Validate/initialize input parameters...
            if ((!String.IsNullOrWhiteSpace(reason)) &&
                (!String.IsNullOrWhiteSpace(recordText)))
            {
                Reason = reason;
                RecordText = recordText;
                RowIndex = rowIndex;
                FieldIndex = fieldIndex;
            }
#if (DEBUG)
            else
            {
                var paramName = String.IsNullOrWhiteSpace(reason) ? "reason" : "recordText";
                throw new ArgumentNullException(paramName, "invalid value...");
            }
#endif
        }

        //Copy constructor...
        public CsvDataError( CsvDataError csvDataError)
        {
            //Validate/initialize input parameters...
            if (null != csvDataError)
            {
                Reason = csvDataError.Reason;
                RecordText = csvDataError.RecordText;
                RowIndex = csvDataError.RowIndex;
                FieldIndex = csvDataError.FieldIndex;
            }
#if (DEBUG)
            else
            {
                var paramName = "csvDataError";
                throw new ArgumentNullException(paramName, "invalid value...");
            }
#endif
        }

        //Properties...
        public string Reason { get; private set; }
        public string RecordText { get; private set; }
        public int RowIndex { get; private set; }
        public int FieldIndex { get; private set; }
    }

    //A simple class for the validation results of a CSV-delimited file...
    public class CsvValidationResults
    {
        //Default constructor...
        protected CsvValidationResults()
        {
            reset();
        }

        //Initializing constructor...
        public CsvValidationResults(CsvValidator csvValidator) : this()
        {
            if (null != csvValidator)
            {
                foreach (var headerName in csvValidator.InvalidHeaderNames)
                {
                    InvalidHeaderNames.Add(headerName);
                }

                foreach (var headerName in csvValidator.ValidHeaderNames)
                {
                    ValidHeaderNames.Add(headerName);
                }

                foreach (var dataError in csvValidator.DataErrors)
                {
                    DataErrors.Add(new CsvDataError(dataError));
                }
            }
#if (DEBUG)
            else
            {
                var paramName = "csvValidator";
                throw new ArgumentNullException(paramName, "invalid value...");
            }
#endif
        }

        //Utility methods...
        protected virtual void reset()
        {
            InvalidHeaderNames = new List<string>();
            ValidHeaderNames = new List<string>();
            DataErrors = new List<CsvDataError>();
        }

        //Properties

        //List of invalid header names for the current validation attempt
        public List<string> InvalidHeaderNames { get; private set; }

        //List of valid header names for the current validation attempt
        public List<string> ValidHeaderNames { get; private set; }

        //List of data errors...
        public List<CsvDataError> DataErrors { get; private set; }

        //Methods 

        //Header valid?
        public bool HeadersValid()
        {
            return ((0 >= InvalidHeaderNames.Count) &&  //No invalid headers
                    (0 < ValidHeaderNames.Count));      //Headers exist
        }

        //Data valid?
        public virtual bool DataValid()
        {
            return (0 >= DataErrors.Count);      //No data errors
        }
    }


    //A simple class for the validation of CSV-delimited files...
    public class CsvValidator : CsvValidationResults
    {
        //Members...

        //Dictionary of model types used in validation to class maps...
        private static Dictionary<Type, Type> validationTypesToClassMaps = new Dictionary<Type, Type>
                                                                                { {typeof(CategoriesModel), typeof(CategoriesModelMap)},
                                                                                  {typeof(DataValuesModel), typeof(DataValuesModelMap)},
                                                                                  {typeof(DerivedFromModel), typeof(DerivedFromModelMap)},
                                                                                  {typeof(GroupDescriptionModel), typeof(GroupDescriptionModelMap)},
                                                                                  {typeof(GroupsModel), typeof(GroupsModelMap)},
                                                                                  {typeof(LabMethodModel), typeof(LabMethodModelMap)},
                                                                                  {typeof(MethodModel), typeof(MethodModelMap)},
                                                                                  {typeof(OffsetTypesModel), typeof(OffsetTypesModelMap)},
                                                                                  {typeof(QualifiersModel), typeof(QualifiersModelMap)},
                                                                                  {typeof(QualityControlLevelModel), typeof(QualityControlLevelModelMap)},
                                                                                  {typeof(SampleModel), typeof(SampleModelMap)},
                                                                                  {typeof(SiteModel), typeof(SiteModelMap)},
                                                                                  {typeof(SourcesModel), typeof(SourcesModelMap)},
                                                                                  {typeof(VariablesModel), typeof(VariablesModelMap)}
                                                                                };
        //CSV file path and name...
        private string csvFilePathAndName;

        //Default constructor...
        private CsvValidator() : base()
        {
        }

        //Initializing constructor
        public CsvValidator(string csvFilePathAndNameIn) : this()
        {
            //Validate/initialize input parameters...
            if (!String.IsNullOrWhiteSpace(csvFilePathAndNameIn))
            {
                csvFilePathAndName = csvFilePathAndNameIn;
            }
#if (DEBUG)
            else
            {
                var paramName = "csvFilePathAndNameIn";
                throw new ArgumentNullException(paramName, "invalid value...");
            }
#endif
        }

        //Utility methods...
        protected override void reset()
        {
            base.reset();

            ValidatedModelType = null;

            ValidatedRecords = new List<object>();
        }

        //CsvHelper - Header validation callback...
        private void headerValidationCallback(bool isValid, string[] headerNames, int headerNameIndex, CsvHelper.IReadingContext context)
        {
            //Update the member list per validation indicator...
            var list = isValid ? ValidHeaderNames : InvalidHeaderNames;
            list.Add(headerNames[headerNameIndex]);
        }

        //Properties...

        //Validated model type of records data
        public Type ValidatedModelType { get; private set; }

        //Validated records...
        public List<object> ValidatedRecords { get; private set; }

        //Methods 

        //Validate the associated CSV file...
        //Returns: true - success, false - otherwise;
        public async Task<bool> ValidateFileContents()
        {
            //Reset members...
            reset();

            //Set return value
            bool result = true; //Assume success

            try
            {
                //Open a text reader on the associated CSV file...
                using (TextReader tReaderCsv = File.OpenText(csvFilePathAndName))
                {
                    //Allocate a CsvReader instance...
                    using (var csvReader = new CsvHelper.CsvReader(tReaderCsv))
                    {
                        //Configure the csv reader instance...
                        var conf = csvReader.Configuration;

                        //Headers...
                        conf.HasHeaderRecord = true;
                        conf.HeaderValidated = headerValidationCallback;

                        //Reading...
                        conf.IgnoreBlankLines = true;

                        //Attempt to read the CSV header..
                        csvReader.Read();
                        if (csvReader.ReadHeader())
                        {
                            //Success - for each known model type...  
                            foreach (var kvp in validationTypesToClassMaps)
                            {
                                //Attempt to validate header...
                                var modelType = kvp.Key;
                                var mapType = kvp.Value;

                                reset();

                                //Register map class...
                                conf.RegisterClassMap(mapType);

                                csvReader.ValidateHeader(modelType);

                                if (HeadersValid())
                                {
                                    //Successful validation - set validated type...
                                    ValidatedModelType = modelType;
                                    break;
                                }

                                //Unregister map class...
                                conf.UnregisterClassMap(mapType);
                            }

                            if (null != ValidatedModelType)
                            {
                                //Successful header validation - scan and validate records...
                                while (await csvReader.ReadAsync())
                                {
                                    try
                                    {
                                        //Add validated record to list...
                                        var record = csvReader.GetRecord(ValidatedModelType);

                                        ValidatedRecords.Add(record);
                                    }
                                    catch (ValidationException ex)
                                    {
                                        //Validation error - log...
                                        //NOTE: A sucessful ValidateFileContents() call can find validation error(s) 
                                        //      on one or more records.  Thus the catch block does not set the result
                                        //      indicator...
                                        var rContext = ex.ReadingContext;

                                        var index = rContext.CurrentIndex;
                                        var fieldName = rContext.HeaderRecord[index];
                                        var fieldValue = rContext.Record[index];
                                        var message = String.IsNullOrEmpty(fieldValue) ? "empty" : "invalid format";

                                        var csvDataError = new CsvDataError( String.Format("Field: {0} - {1}", fieldName, message ),
                                                                             rContext.RawRecordBuilder.ToString(),
                                                                             rContext.RawRow,
                                                                             index);
                                        DataErrors.Add(csvDataError);
                                    }
                                }
                            }
                            else
                            {
                                //Invalid header(s)...
                                result = false;
                            }
                        }
                    }
                }
            }
            //catch (Exception ex)
            catch (Exception)
            {
                //For now take no action...
                reset();
                result = false;
            }

            //Processing complete - return
            return result;
        }

        //Data valid?
        public override bool DataValid()
        {
            return (base.DataValid() &&              //No data errors
                    (null != ValidatedModelType) &&  //Model type validated
                    (0 < ValidatedRecords.Count));   //Validated data exists
        }

    }
}