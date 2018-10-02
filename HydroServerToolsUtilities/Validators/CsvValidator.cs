using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using System.Reflection;

using CsvHelper;

using HydroServerToolsUtilities;
using HydroserverToolsBusinessObjects.Models;
using HydroserverToolsBusinessObjects.ModelMaps;

namespace HydroServerToolsUtilities.Validators
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
    public class CsvValidationResults : IValidationComplete
    {
        //private members...
        private string _dataAsMetadata = "datavaluessubmittedasmetadata";
        private string _metadataAsData = "metadatasubmittedasdatavalues";

        //Default constructor...
        public CsvValidationResults()
        {
            reset();
        }

        //Initializing constructor...
        public CsvValidationResults(CsvValidationResults csvValidationResults) : this()
        {
            if (null != csvValidationResults)
            {
                CandidateTypeName = csvValidationResults.CandidateTypeName;

                CandidateTypeFriendlyName = csvValidationResults.CandidateTypeFriendlyName;

                CandidateRecordCount = csvValidationResults.CandidateRecordCount;

                foreach (var headerName in csvValidationResults.InvalidHeaderNames)
                {
                    InvalidHeaderNames.Add(headerName);
                }

                foreach (var headerName in csvValidationResults.MissingRequiredHeaderNames)
                {
                    MissingRequiredHeaderNames.Add(headerName);
                }

                foreach (var headerName in csvValidationResults.ValidHeaderNames)
                {
                    ValidHeaderNames.Add(headerName);
                }

                foreach (var dataError in csvValidationResults.DataErrors)
                {
                    DataErrors.Add(new CsvDataError(dataError));
                }

                HeaderValidationIndex = csvValidationResults.HeaderValidationIndex;

                ValidationComplete = csvValidationResults.ValidationComplete;
            }
#if (DEBUG)
            else
            {
                var paramName = "csvValidationResults";
                throw new ArgumentNullException(paramName, "invalid value...");
            }
#endif
        }

        //Utility methods...
        private void reset()
        {
            CandidateTypeName = String.Empty;
            CandidateTypeFriendlyName = String.Empty;
            CandidateRecordCount = 0;
            InvalidHeaderNames = new List<string>();
            MissingRequiredHeaderNames = new List<string>();
            ValidHeaderNames = new List<string>();
            DataErrors = new List<CsvDataError>();
            HeaderValidationIndex = -1;

            ValidationComplete = false;
        }

        //Properties

        //Candidate type name (empty if no candidate type established)
        public String CandidateTypeName { get; set; }

        //Friendly name for candidate type (empty if no candidate type established)
        public String CandidateTypeFriendlyName { get; set; } 

        //Number of records available for DB loading...
        public int CandidateRecordCount { get; set; }

        //List of invalid header names for the current validation attempt
        public List<string> InvalidHeaderNames { get; private set; }

        //List of missing required header names for the current validation attempt
        public List<string> MissingRequiredHeaderNames { get; set; }

        //List of valid header names for the current validation attempt
        public List<string> ValidHeaderNames { get; private set; }

        //List of data errors...
        public List<CsvDataError> DataErrors { get; private set; }

        //Header Validation Index - referenced during header validation...
        public int HeaderValidationIndex { get; set; }

        //Validation Complete 
        public bool ValidationComplete { get; set; }

        //Methods 

        //All headers valid?
        public bool AllHeadersValid()
        {
            return ((0 >= InvalidHeaderNames.Count) &&          //No invalid headers exist
                    (0 < ValidHeaderNames.Count) &&             //Valid headers exist
                    (0 >= MissingRequiredHeaderNames.Count));   //No missing required headers
        }

        ////Some headers valid?
        //public bool SomeHeadersValid()
        //{
        //    return ((0 < InvalidHeaderNames.Count) &&  //Some invalid headers exist
        //            (0 < ValidHeaderNames.Count));      //Some valid headers exist
        //}

        //Data valid?
        public bool DataValid()
        {
            return (0 >= DataErrors.Count);      //No data errors
        }

        //Return an empty instance...
        public static CsvValidationResults MakeInstance()
        {
            return (new CsvValidationResults());
        }

        //Produce a validation message per instance state,
        //  If an error message, bErrorMessage == true, else false...
        public string ValidationMessage(ref bool bErrorMessage)
        {
            string validationMessage = String.Empty;
            bErrorMessage = false;   //Assume no error(s) exist...

            //Retrieve instance state...
            var typeName = CandidateTypeName.ToLower();
            var friendlyName = CandidateTypeFriendlyName;
            var recordCount = CandidateRecordCount;
            var invalidHeadersCount = InvalidHeaderNames.Count;
            var missingHeadersCount = MissingRequiredHeaderNames.Count;

            //Evaluate instance state...
            if (_dataAsMetadata == typeName ||
                _metadataAsData == typeName ||
                "unknown" == typeName ||
                0 < invalidHeadersCount ||
                0 < missingHeadersCount ||
                0 >= recordCount)
            {
                bErrorMessage = true;
                if (_dataAsMetadata == typeName)
                {
                    validationMessage = "Data values submitted with metadata.  Please submit data values after metadata";
                }
                else if (_metadataAsData == typeName)
                {
                    validationMessage = "Metadata submitted with data values.  Please submit metadata before data values";
                }
                else if ("unknown" == typeName)
                {
                    validationMessage = "File contents map to no known model type";
                }
                else if (String.IsNullOrEmpty(validationMessage))
                {
                    if (0 < invalidHeadersCount)
                    {
                        validationMessage = String.Format("Validates as: {0} with {1:n0} invalid column name(s).", friendlyName, invalidHeadersCount);
                    }
                    else if (0 < missingHeadersCount)
                    {
                        validationMessage = String.Format("Validates as: {0} with {1:n0} missing column name(s).", friendlyName, missingHeadersCount);
                    }
                    else if (0 >= recordCount)
                    {
                        validationMessage = String.Format("Validates as: {0} with zero ({1}) record(s)", friendlyName, recordCount);
                    }
                }
            }
            else
            {
                //Known type - no errors...
                validationMessage = String.Format("Validates as: '{0}' with {1:n0} records.", friendlyName, recordCount);
            }

            //Processing complete - return 
            return validationMessage;
        }
    }


    //A simple class for the validation of CSV-delimited files...
    public class CsvValidator : IValidationComplete
    {
        //Members...

        //Dictionary of model types used in validation to class maps...
        //12-Feb-2018 - BCC - Restrict types to six used with Basic Templates...
        private static Dictionary<Type, Type> validationTypesToClassMaps = new Dictionary<Type, Type>
                                                                                {
                                                                                  //{typeof(CategoriesModel), typeof(GenericMap<CategoriesModel>)},
                                                                                  {typeof(DataValuesModelBasicTemplate), typeof(GenericMap<DataValuesModelBasicTemplate>)},
                                                                                  //{typeof(DerivedFromModel), typeof(GenericMap<DerivedFromModel>)},
                                                                                  //{typeof(GroupDescriptionModel), typeof(GenericMap<GroupDescriptionModel>)},
                                                                                  //{typeof(GroupsModel), typeof(GenericMap<GroupsModel>)},
                                                                                  //{typeof(LabMethodModel), typeof(GenericMap<LabMethodModel>)},
                                                                                  {typeof(MethodModel), typeof(GenericMap<MethodModel>)},
                                                                                  //{typeof(OffsetTypesModel), typeof(GenericMap<OffsetTypesModel>)},
                                                                                  //{typeof(QualifiersModel), typeof(GenericMap<QualifiersModel>)},
                                                                                  {typeof(QualityControlLevelModel), typeof(GenericMap<QualityControlLevelModel>)},
                                                                                  //{typeof(SampleModel), typeof(GenericMap<SampleModel>)},
                                                                                  {typeof(SiteModelBasicTemplate), typeof(GenericMap<SiteModelBasicTemplate>) },
                                                                                  {typeof(SourcesModelBasicTemplate), typeof(GenericMap<SourcesModelBasicTemplate>)},
                                                                                  {typeof(VariablesModelBasicTemplate), typeof(GenericMap<VariablesModelBasicTemplate>)}
                                                                                };
        //Dictionary of model types used in validation to 'friendly names'...
        private static Dictionary<Type, string> validationTypesToFriendlyNames = new Dictionary<Type, string>
        {
            {typeof(DataValuesModelBasicTemplate), "Data Values"},
            {typeof(MethodModel), "Methods"},
            {typeof(QualityControlLevelModel), "Quality Control Levels"},
            {typeof(SiteModelBasicTemplate), "Sites"},
            {typeof(SourcesModelBasicTemplate), "Sources"},
            {typeof(VariablesModelBasicTemplate), "Variables"}
        };

        //CSV file path and name...
        private string csvFilePathAndName;

        //CSV content type (from HTTP header)
        private string csvContentType;

        //Validation results...
        private Dictionary<Type, CsvValidationResults> validationTypesToValidationResults;

        //IValidationComplete...
        public bool ValidationComplete { get; private set; }

        //Default constructor...
        private CsvValidator()
        {
            reset();
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
        protected void reset()
        {
            CurrentModelType = null;

            ValidatedModelType = null;

            ValidatedRecords = new List<object>();

            FileEncoding = Encoding.Default;

            validationTypesToValidationResults = new Dictionary<Type, CsvValidationResults>();

            ValidationComplete = false;
        }

        //CsvHelper - Header validation callback...
        private void headerValidationCallback(bool isValid, string[] headerNames, int headerNameIndex, CsvHelper.IReadingContext context)
        {
            //Check for invalid, optional header...
            if ((!isValid) && null != CurrentModelType)
            {
                if (validationTypesToClassMaps.ContainsKey(CurrentModelType))
                {
                    var mapType = validationTypesToClassMaps[CurrentModelType];

                    ConstructorInfo constructorInfo = mapType.GetConstructor(Type.EmptyTypes);
                    if (null != constructorInfo)
                    {
                        object mapTypeInstance = constructorInfo.Invoke(new object[] { });
                        MethodInfo miGetOptionalPropertyNames = mapType.GetMethod("GetOptionalPropertyNames");
                        List<string> optionalPropertyNames = miGetOptionalPropertyNames.Invoke(mapTypeInstance, new object[] { }) as List<string>;

                        if (-1 != optionalPropertyNames.IndexOf(headerNames[headerNameIndex]))
                        {
                            //Invalid header is optional - return early...
                            return;
                        }
                    }
                }
            }

            //Retrieve validation results per current model type...
            CsvValidationResults csvValidationResults = null;
            if (validationTypesToValidationResults.ContainsKey(CurrentModelType))
            {
                //Update the member list per validation indicator...
                csvValidationResults = validationTypesToValidationResults[CurrentModelType];
                var list = isValid ? csvValidationResults.ValidHeaderNames : csvValidationResults.InvalidHeaderNames;

                //Set/update/retain header validation index...
                //In the case of invalid header records, apparently the only way to know which element in the context.HeaderRecord array is currently being validated...
                int headerValidationIndex = (-1 == csvValidationResults.HeaderValidationIndex) ? 0 : csvValidationResults.HeaderValidationIndex + 1;
                csvValidationResults.HeaderValidationIndex = headerValidationIndex;

                var headerName = isValid ? headerNames[headerNameIndex] :
                                            (headerValidationIndex < context.HeaderRecord.Count()) ? context.HeaderRecord[headerValidationIndex] : String.Empty;

                if (!String.IsNullOrWhiteSpace(headerName))
                {
                    list.Add(headerName);
                }
            }

        }

        //CsvHelper - BadDataFound handler...
        private void handlerBadDataFound(IReadingContext iReadingContext)
        {
            //Validate/initialize input parameters...
            if (null != iReadingContext && null != CurrentModelType)
            {
                //Input parameters valid - retrieve validation results per current model type...
                if (validationTypesToValidationResults.ContainsKey(CurrentModelType))
                {
                    //Validation results found - retrieve data errors...
                    CsvValidationResults csvValidationResults = validationTypesToValidationResults[CurrentModelType];
                    List<CsvDataError> dataErrors = csvValidationResults.DataErrors;

                    //Check index value...
                    var index = iReadingContext.CurrentIndex;
                    if (0 <= index)
                    {
                        //Index value valid - create and log data error...
                        var fieldName = "unknown";
                        if (index < iReadingContext.HeaderRecord.Length)
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
        }

        //CsvHelper - MissingFieldFound handler...
        private void handlerMissingFieldFound(string[] headerNames, int index, IReadingContext iReadingContext)
        {
            handlerBadDataFound(iReadingContext);
        }

        //CsvHelper - ReadingExceptionOccurred handler 
        private void handlerReadingExceptionOccurred(Exception ex)
        {
            if (null != ex)
            {
                ValidationException vex = ex as ValidationException;
                if (null != vex)
                {
                    handlerBadDataFound(vex.ReadingContext);
                }
            }
        }

        //Properties...

        //Current model type for validation...
        private Type CurrentModelType { get; set; }

        //Validated model type of records data
        public Type ValidatedModelType { get; private set; }

        //Validated records...
        public List<object> ValidatedRecords { get; private set; }

        //File encoding
        public Encoding FileEncoding { get; private set; }

        //Validation Result
        //NOTE: After validation processing is complete the member dictionary should contain one entry...
        public CsvValidationResults ValidationResults
        {
            get
            {
                var kvp = validationTypesToValidationResults.FirstOrDefault();
                return kvp.Value;
            }
        }

        //Validation Qualifier
        //Post construction qualifier referenced during validation processing...
        public String ValidationQualifier { get; set; }

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
            FileEncoding = EncodingContext.GetFileEncoding(csvContentType, csvFilePathAndName);

            try
            {
                //Open a text-reader derived instance on the associated CSV file...
                using (var fileStream = File.Open(csvFilePathAndName, FileMode.Open, FileAccess.Read, FileShare.None))
                {
                    //using (var streamReader = new StreamReader(fileStream))
                    using (var streamReader = new StreamReader(fileStream, FileEncoding))
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
                                handlerBadDataFound(context);
                            };

                            conf.MissingFieldFound = (headerNames, index, context) =>
                            {
                                //Invoke member handler...
                                handlerMissingFieldFound(headerNames, index, context);
                            };

                            conf.ReadingExceptionOccurred = exception =>
                            {
                                handlerReadingExceptionOccurred(exception);
                            };

                            //Attempt to read the CSV header..
                            csvReader.Read();
                            if (csvReader.ReadHeader())
                            {
                                //Success - retrieve header record and record length... 
                                var headers = csvReader.Context.HeaderRecord;
                                int headersLength = headers.Length;

                                //For each known model type...  
                                CsvValidationResults csvValidationResults = null;
                                foreach (var kvp in validationTypesToClassMaps)
                                {
                                    //Attempt to validate header...
                                    var modelType = kvp.Key;
                                    var mapType = kvp.Value;
                                    var bRequiredHeadersPresent = false;    //Assume not all required headers are present in file header...

                                    var modelFriendlyName = String.Empty;
                                    if (validationTypesToFriendlyNames.Keys.Contains(modelType))
                                    {
                                        modelFriendlyName = validationTypesToFriendlyNames[modelType];
                                    }

                                    //Create dictionary entry...
                                    csvValidationResults = CsvValidationResults.MakeInstance();
                                    validationTypesToValidationResults.Add(modelType, csvValidationResults);

                                    //Register map class, validate header...
                                    conf.RegisterClassMap(mapType);

                                    CurrentModelType = modelType;
                                    csvValidationResults.CandidateTypeName = modelType.Name;
                                    csvValidationResults.CandidateTypeFriendlyName = modelFriendlyName;
                                    csvReader.ValidateHeader(modelType);

                                    //Check for presence of valid header names in file...
                                    //if ( 0 >= csvValidationResults.ValidHeaderNames.Count)
                                    //{
                                    //    //No valid header names in file - cannot validate - continue to next map type...
                                    //    conf.UnregisterClassMap(mapType);
                                    //    continue;
                                    //}

                                    var validHeaderNames = csvValidationResults.ValidHeaderNames;
                                    int validHeaderCount = validHeaderNames.Count;

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
                                                enumValue = (int)Convert.ChangeType(fieldInfo.GetValue(null), typeof(int));
                                                break;
                                            }
                                        }

                                        MethodInfo miConfigurationOptions = mapType.GetMethod("set_ConfigurationOptions");
                                        miConfigurationOptions.Invoke(mapTypeInstance, new object[] { enumValue });

                                        //Get required property names...
                                        MethodInfo miGetRequiredPropertyNames = mapType.GetMethod("GetRequiredPropertyNames");
                                        List<string> requiredPropertyNames = miGetRequiredPropertyNames.Invoke(mapTypeInstance, new object[] { }) as List<string>;
                                        int requiredCount = requiredPropertyNames.Count;

                                        //Check if header can be validated against current map type...
                                        //if (validHeaderCount < requiredCount)
                                        //{
                                        //    //Number of valid header fields < number of required fields - cannot validate - continue to next map type...
                                        //    conf.UnregisterClassMap(mapType);
                                        //    continue;
                                        //}
                                        //else
                                        //{
                                            //Number of valid header fields >= number of required fields - check valid header names for required field coverage...
                                            foreach (var vhName in validHeaderNames)
                                            {
                                                if (-1 != requiredPropertyNames.IndexOf(vhName))
                                                {
                                                    //Required property name found - decrement count, remove list element...
                                                    requiredPropertyNames.Remove(vhName);
                                                }
                                            }

                                        //Mark validation complete for current validation results...
                                        csvValidationResults.ValidationComplete = true;

                                            if (0 < requiredPropertyNames.Count)
                                            {
                                                //Not all required property names present - cannot validate - continue to next map type...
                                                csvValidationResults.MissingRequiredHeaderNames = requiredPropertyNames;

                                                conf.UnregisterClassMap(mapType);
                                                continue;
                                            }

                                            bRequiredHeadersPresent = true;
                                        //}
                                    }

                                    //Unregister map class...
                                    conf.UnregisterClassMap(mapType);

                                    if (bRequiredHeadersPresent)
                                    {
                                        //Successful validation - set validated type, break...
                                        ValidatedModelType = modelType;
                                        break;
                                    }
                                }

                                if (null != ValidatedModelType)
                                {
                                    //Successful header validation - check for data values in metadata submission --OR-- 
                                    //                                          metadata in data values submission...
                                    if (("meta_data" == ValidationQualifier &&
                                         typeof(DataValuesModelBasicTemplate) == ValidatedModelType) ||
                                        ("data_values" == ValidationQualifier &&
                                         typeof(DataValuesModelBasicTemplate) != ValidatedModelType))
                                    {
                                        //Metadata validation only - validation finds Data Values --OR-- 
                                        //Data values validation only - validation finds Metadata 
                                        // - set validation indicators...
                                        if (validationTypesToValidationResults.ContainsKey(ValidatedModelType))
                                        {
                                            var validationResults = validationTypesToValidationResults[ValidatedModelType];

                                            //Set candidate type name to allow easy identification of error on client...
                                            if ("meta_data" == ValidationQualifier)
                                            {
                                                validationResults.CandidateTypeName = "DataValuesSubmittedAsMetadata";
                                                validationResults.CandidateTypeFriendlyName = "Data values submitted as metadata";
                                            }
                                            else if ("data_values" == ValidationQualifier)
                                            {
                                                validationResults.CandidateTypeName = "MetadataSubmittedAsDataValues";
                                                validationResults.CandidateTypeFriendlyName = "Metadata submitted as data values";
                                            }
                                        }

                                        result = false; //Set return value...
                                    }
                                    else
                                    {
                                        //Scan and validate records...
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
                                                handlerBadDataFound(ex.ReadingContext);

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

                                    //Set results candidate record count...
                                    csvValidationResults.CandidateRecordCount = ValidatedRecords.Count;

                                    //Remove validation results for types other than ValidatedModelType...
                                    if (validationTypesToValidationResults.ContainsKey(ValidatedModelType))
                                    {
                                        var validationResults = validationTypesToValidationResults[ValidatedModelType];

                                        validationTypesToValidationResults.Clear();
                                        validationTypesToValidationResults[ValidatedModelType] = validationResults;
                                    }
                                }
                                else
                                {
                                    //Invalid header(s) - Select validation results with lowest missing required header names count to determine partial match or no match...
                                    var IOrderedEnumerable = validationTypesToValidationResults.OrderBy(kvp => kvp.Value.MissingRequiredHeaderNames.Count);
                                    var kvpValidationResults = IOrderedEnumerable.FirstOrDefault();

                                    if (0 >= kvpValidationResults.Value.ValidHeaderNames.Count)
                                    {
                                        //No match - clear validation results, set no match type and validation result
                                        var type = typeof(HydroserverToolsBusinessObjects.NoModel);
                                        var validationResults = new CsvValidationResults(kvpValidationResults.Value);

                                        validationResults.CandidateTypeName = "Unknown";
                                        validationResults.CandidateTypeFriendlyName = "Unknown";

                                        //Clear missing required header names to prevent misleading error messages...
                                        if ( 0 < validationResults.MissingRequiredHeaderNames.Count)
                                        {
                                            validationResults.MissingRequiredHeaderNames.Clear();
                                        }

                                        validationTypesToValidationResults.Clear();
                                        validationTypesToValidationResults.Add(type, validationResults);
                                    }
                                    else
                                    {
                                        //Partial match - Clear validation results, set partial match type and validation result
                                        var type = kvpValidationResults.Key;
                                        var validationResults = kvpValidationResults.Value;

                                        validationTypesToValidationResults.Clear();
                                        validationTypesToValidationResults.Add(type, validationResults);
                                    }

                                    //Set return value...
                                    result = false;
                                }
                            }
                        }
                    }
                }

                //Validation complete - set indicator...
                ValidationComplete = true;
            }
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
        public bool DataValid()
        {
            return ((null != ValidatedModelType) &&  //Model type validated
                    (0 < ValidatedRecords.Count));   //Validated data exists
        }
    }
}
