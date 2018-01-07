using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using System.Reflection;

using CsvHelper;

using HydroServerTools.Utilities;
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

        //All headers valid?
        public bool AllHeadersValid()
        {
            return ((0 >= InvalidHeaderNames.Count) &&  //No invalid headers exist
                    (0 < ValidHeaderNames.Count));      //Valid headers exist
        }

        ////Some headers valid?
        //public bool SomeHeadersValid()
        //{
        //    return ((0 < InvalidHeaderNames.Count) &&  //Some invalid headers exist
        //            (0 < ValidHeaderNames.Count));      //Some valid headers exist
        //}

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
                                                                                {
                                                                                  //{typeof(CategoriesModel), typeof(CategoriesModelMap)},
                                                                                  //{typeof(DataValuesModel), typeof(DataValuesModelMap)},
                                                                                  //{typeof(DerivedFromModel), typeof(DerivedFromModelMap)},
                                                                                  //{typeof(GroupDescriptionModel), typeof(GroupDescriptionModelMap)},
                                                                                  //{typeof(GroupsModel), typeof(GroupsModelMap)},
                                                                                  //{typeof(LabMethodModel), typeof(LabMethodModelMap)},
                                                                                  //{typeof(MethodModel), typeof(MethodModelMap)},
                                                                                  //{typeof(OffsetTypesModel), typeof(OffsetTypesModelMap)},
                                                                                  //{typeof(QualifiersModel), typeof(QualifiersModelMap)},
                                                                                  //{typeof(QualityControlLevelModel), typeof(QualityControlLevelModelMap)},
                                                                                  //{typeof(SampleModel), typeof(SampleModelMap)},
                                                                                  //{typeof(SiteModel), typeof(SiteModelMap)},
                                                                                  //{typeof(SourcesModel), typeof(SourcesModelMap)},
                                                                                  //{typeof(VariablesModel), typeof(VariablesModelMap)}
                                                                                  {typeof(CategoriesModel), typeof(GenericMap<CategoriesModel>)},
                                                                                  {typeof(DataValuesModel), typeof(GenericMap<DataValuesModel>)},
                                                                                  {typeof(DerivedFromModel), typeof(GenericMap<DerivedFromModel>)},
                                                                                  {typeof(GroupDescriptionModel), typeof(GenericMap<GroupDescriptionModel>)},
                                                                                  {typeof(GroupsModel), typeof(GenericMap<GroupsModel>)},
                                                                                  {typeof(LabMethodModel), typeof(GenericMap<LabMethodModel>)},
                                                                                  {typeof(MethodModel), typeof(GenericMap<MethodModel>)},
                                                                                  {typeof(OffsetTypesModel), typeof(GenericMap<OffsetTypesModel>)},
                                                                                  {typeof(QualifiersModel), typeof(GenericMap<QualifiersModel>)},
                                                                                  {typeof(QualityControlLevelModel), typeof(GenericMap<QualityControlLevelModel>)},
                                                                                  {typeof(SampleModel), typeof(GenericMap<SampleModel>)},
                                                                                  {typeof(SiteModel), typeof(GenericMap<SiteModel>)},
                                                                                  {typeof(SourcesModel), typeof(GenericMap<SourcesModel>)},
                                                                                  {typeof(VariablesModel), typeof(GenericMap<VariablesModel>)}
                                                                                };
        //CSV file path and name...
        private string csvFilePathAndName;

        //CSV content type (from HTTP header)
        private string csvContentType;

        //Default constructor...
        private CsvValidator() : base()
        {
        }

        //Initializing constructor
        public CsvValidator(string csvContentTypeIn, string csvFilePathAndNameIn) : this()
        {
            //Validate/initialize input parameters...
            if ((!String.IsNullOrWhiteSpace(csvContentTypeIn)) && (!String.IsNullOrWhiteSpace(csvFilePathAndNameIn)))
            {
                csvContentType = csvContentTypeIn;
                csvFilePathAndName = csvFilePathAndNameIn;
            }
#if (DEBUG)
            else
            {
                var paramName = String.IsNullOrWhiteSpace(csvContentTypeIn) ? "csvContentTypeIn" : "csvFilePathAndNameIn";
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

        //CsvHelper - BadDataFound handler...
        private static void handlerBadDataFound(IReadingContext iReadingContext, List<CsvDataError> dataErrors)
        {
            //Validate/initialize input parameters...
            if (null != iReadingContext && null != dataErrors)
            {
                //Input parameters valid - check index value...
                var index = iReadingContext.CurrentIndex;
                if (0 <= index)
                {
                    //Index value valid - create and log data error...
                    var fieldName = "unknown";
                    if ( index < iReadingContext.HeaderRecord.Length)
                    {
                        fieldName = iReadingContext.HeaderRecord[index];
                    }

                    var fieldValue = "unknown";
                    if (index < iReadingContext.Record.Length)
                    {
                        fieldValue = iReadingContext.Record[index];
                    }

                    var message = String.IsNullOrEmpty(fieldValue) ? "empty" : "invalid format";
                    var csvDataError = new CsvDataError(String.Format("Field: {0} - {1}", fieldName, message),
                                                         (null != iReadingContext.RawRecordBuilder) ? iReadingContext.RawRecordBuilder.ToString() : "unknown",
                                                         iReadingContext.RawRow,
                                                         index);
                    dataErrors.Add(csvDataError);
                }
            }
        }

        //CsvHelper - MissingFieldFound handler...
        private static void handlerMissingFieldFound(string[] headerNames, int index, IReadingContext iReadingContext, List<CsvDataError> dataErrors )
        {
            handlerBadDataFound(iReadingContext, dataErrors);
        }

        //CsvHelper - ReadingExceptionOccurred handler 
        private static void handlerReadingExceptionOccurred(Exception ex, List<CsvDataError> dataErrors)
        {
            if (null != ex)
            {
                ValidationException vex = ex as ValidationException;
                if (null != vex)
                {
                    handlerBadDataFound(vex.ReadingContext, dataErrors);
                }
            }
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

            //Retrieve the current encoding...
            Encoding currentEncoding = EncodingContext.GetFileEncoding(csvContentType, csvFilePathAndName);

            try
            {
                //Open a text-reader derived instance on the associated CSV file...
                using (var fileStream = File.Open(csvFilePathAndName, FileMode.Open, FileAccess.Read, FileShare.None))
                {
                    //using (var streamReader = new StreamReader(fileStream))
                    using (var streamReader = new StreamReader(fileStream, currentEncoding))
                    {
                        //Allocate a CsvReader instance...
                        using (var csvReader = new CsvHelper.CsvReader(streamReader))
                        {
                            //Configure the csv reader instance...
                            var conf = csvReader.Configuration;

                            //Headers...
                            conf.HasHeaderRecord = true;
                            conf.HeaderValidated = headerValidationCallback;

                            //Reading...
                            conf.IgnoreBlankLines = true;

                            //BC - 18-Dec-2017 - Add custom error handling so that 
                            //  reading of records does not trigger ValidationExceptions...
                            conf.BadDataFound = context =>
                            {
                                //Invoke member handler...
                                handlerBadDataFound(context, DataErrors);
                            };

                            conf.MissingFieldFound = (headerNames, index, context) =>
                            {
                                //Invoke member handler...
                                handlerMissingFieldFound(headerNames, index, context, DataErrors);
                            };

                            conf.ReadingExceptionOccurred = exception =>
                            {
                                handlerReadingExceptionOccurred(exception, DataErrors);
                            };

                            //Attempt to read the CSV header..
                            csvReader.Read();
                            if (csvReader.ReadHeader())
                            {
                                //Success - retrieve header record and record length... 
                                var headers = csvReader.Context.HeaderRecord;
                                int headersLength = headers.Length;

                                //For each known model type...  
                                foreach (var kvp in validationTypesToClassMaps)
                                {
                                    //Attempt to validate header...
                                    var modelType = kvp.Key;
                                    var mapType = kvp.Value;
                                    var bRequiredHeadersPresent = false;    //Assume not all required headers are present in file header...

                                    reset();

                                    //Register map class, validate header...
                                    conf.RegisterClassMap(mapType);
                                    csvReader.ValidateHeader(modelType);

                                    //Check for presence of valid header names in file...
                                    if ( 0 >= ValidHeaderNames.Count)
                                    {
                                        //No valid header names in file - cannot validate - continue to next map type...
                                        conf.UnregisterClassMap(mapType);
                                        continue;
                                    }

                                    //Construct map type instance...
                                    ConstructorInfo constructorInfo = mapType.GetConstructor(Type.EmptyTypes);
                                    if (null != constructorInfo)
                                    {
                                        //Instantiate a map class instance...
                                        object mapTypeInstance = constructorInfo.Invoke(new object[] { });

                                        //NOTE: Appears to work only on a static type declaration - like typeof (GenericMapConfiguration)
                                        //var enumValue = System.Enum.Parse( enumType.GetType(), "SuppressEmptyRequiredFieldsChecks");

                                        //Suppress empty required field checks...
                                        var enumType = mapType.GetNestedType("GenericMapConfiguration");
                                        FieldInfo[] fieldInfos;

                                        fieldInfos = enumType.GetFields();
                                        int enumValue = 0;
                                        foreach (var fieldInfo in fieldInfos)
                                        {
                                            if ("SuppressEmptyRequiredFieldsChecks" == fieldInfo.Name)
                                            {
                                                enumValue = (int) Convert.ChangeType(fieldInfo.GetValue(null), typeof(int));
                                            }
                                        }

                                        MethodInfo miConfigurationOptions = mapType.GetMethod("set_ConfigurationOptions");
                                        miConfigurationOptions.Invoke(mapTypeInstance, new object[] { enumValue });

                                        //Get required property names...
                                        MethodInfo miGetRequiredPropertyNames = mapType.GetMethod("GetRequiredPropertyNames");
                                        List<string> requiredPropertyNames = miGetRequiredPropertyNames.Invoke(mapTypeInstance, new object[] { }) as List<string>;
                                        int requiredCount = requiredPropertyNames.Count;

                                        //Check if header can be validated against current map type...
                                        if (headersLength < requiredCount)
                                        {
                                            //Number of header fields < number of required fields - cannot validate - continue to next map type...
                                            conf.UnregisterClassMap(mapType);
                                            continue;
                                        }
                                        else
                                        {
                                            //Number of header fields >= number of required fields - check valid header names for required field coverage...
                                            foreach (var vhName in ValidHeaderNames)
                                            {
                                                if (-1 != requiredPropertyNames.IndexOf(vhName))
                                                {
                                                    //Required property name found - decrement count...
                                                    --requiredCount;
                                                }
                                            }

                                            if (0 < requiredCount)
                                            {
                                                //Not all required property names present - cannot validate - continue to next map type...
                                                conf.UnregisterClassMap(mapType);
                                                continue;
                                            }

                                            bRequiredHeadersPresent = true;
                                        }
                                    }

                                    if ((AllHeadersValid())|| bRequiredHeadersPresent)
                                    {
                                        //Successful validation - set validated type...
                                        ValidatedModelType = modelType;
                                        break;
                                    }
                                    //else
                                    //{
                                    //    //Unsuccessful validation - current file header incomplete or contains errors
                                    //    //Attempt to determine header state...

                                    //    //Check for missing required fields...
                                    //    ConstructorInfo constructorInfo = mapType.GetConstructor(Type.EmptyTypes);
                                    //    if ( null != constructorInfo)
                                    //    {
                                    //        //Instantiate a map class instance...
                                    //        object mapTypeInstance = constructorInfo.Invoke(new object[] { });

                                    //        //Get required property names...
                                    //        MethodInfo miGetRequiredPropertyNames = mapType.GetMethod("GetRequiredPropertyNames");
                                    //        MethodInfo miGetOptionalPropertyNames = mapType.GetMethod("GetOptionalPropertyNames");
                                    //        List<string> requiredPropertyNames = miGetRequiredPropertyNames.Invoke(mapTypeInstance, new object[] { }) as List<string>;
                                    //        List<string> optionalPropertyNames = miGetOptionalPropertyNames.Invoke(mapTypeInstance, new object[] { }) as List<string>;

                                    //        if (null != requiredPropertyNames && null != optionalPropertyNames)
                                    //        {
                                    //            //Scan valid header names for ***ALL*** required property names...
                                    //            int requiredCount = requiredPropertyNames.Count;
                                    //            foreach (var vhName in ValidHeaderNames)
                                    //            {
                                    //                if (-1 != requiredPropertyNames.IndexOf(vhName))
                                    //                {
                                    //                    //Required property name found - decrement count...
                                    //                    --requiredCount;
                                    //                }
                                    //            }

                                    //            if ( 0 >= requiredCount)
                                    //            {
                                    //                //All required property names present - scan invalid header names for optional property names...
                                    //                int optionalCount = optionalPropertyNames.Count;
                                    //                foreach (var ihName in InvalidHeaderNames)
                                    //                {
                                    //                    if (-1 != optionalPropertyNames.IndexOf(ihName))
                                    //                    {
                                    //                        //optional property name found - decrement count...
                                    //                        --optionalCount;
                                    //                    }
                                    //                }

                                    //                if (0 >= optionalCount)
                                    //                {
                                    //                    //Successful validation - set validated type...
                                    //                    ValidatedModelType = modelType;
                                    //                    break;
                                    //                }
                                    //            }
                                    //        }
                                    //    }
                                    //}

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
                                            handlerBadDataFound(ex.ReadingContext, DataErrors);

                                            //var rContext = ex.ReadingContext;

                                            //var index = rContext.CurrentIndex;
                                            //var fieldName = rContext.HeaderRecord[index];
                                            //var fieldValue = rContext.Record[index];
                                            //var message = String.IsNullOrEmpty(fieldValue) ? "empty" : "invalid format";

                                            //var csvDataError = new CsvDataError(String.Format("Field: {0} - {1}", fieldName, message),
                                            //                                     rContext.RawRecordBuilder.ToString(),
                                            //                                     rContext.RawRow,
                                            //                                     index);
                                            //DataErrors.Add(csvDataError);
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
            }
            //catch (Exception ex)
            catch (Exception Ex)
            {
                string msg = Ex.Message;

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
