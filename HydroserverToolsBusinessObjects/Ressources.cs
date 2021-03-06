﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HydroserverToolsBusinessObjects
{
    public class Resources
    {
        public const string EFMODEL = @"res://*/ODM_1_1_1EFModel.csdl|res://*/ODM_1_1_1EFModel.ssdl|res://*/ODM_1_1_1EFModel.msl";
        public const string EFMODELDEF_IN_CONNECTIONSTRING = @"metadata=res://*/ODM_1_1_1EFModel.csdl|res://*/ODM_1_1_1EFModel.ssdl|res://*/ODM_1_1_1EFModel.msl;provider=System.Data.SqlClient;provider connection string=";

        public const string CONNECTION_SUCCESS = "Database connection successful";
        public const string CONNECTION_FAILED_SERVERNAME = "Database connection failed. Please validate that the name of the Server is correct";
        public const string CONNECTION_FAILED_DATASOURCENAME = "Database connection failed. Please validate that the name of the Datasource is correct";
        public const string CONNECTION_FAILED_LOGIN = "Database connection failed. Please validate that the Username and Password are correct";
        public const string CONNECTION_STRING_NOT_FOUND = "Database connection string not found for current user";
        public const string NOT_LINKED_TO_DATABASE = "The user account is not linked to a Database.";
        public const string HYDROSERVER_USERLOOKUP_FAILED = "The current user does not have an associated Hydroserver database. Please contact your administrator or CUAHSI";
        public const string IMPORT_FAILED = "The Import failed please validate your input. Please make sure the file contains all necessary fields.";
        public const string FILETYPE_NOT_CSV = "The uploaded file is not a CSV file. Please upload a correct file.";
        public const string FILETYPE_NOT_CSV_ZIP = "The uploaded file is not a CSV or ZIP file. Please upload a correct file.";
        public const string CSV_FILES_HYDROSERVER = "CSV Files into HydroServer...";
        public const string REQUESTPUBLICATION_FAILED = "The Request failed please contact help@cuahsi.org for support.";

        public const string UPLOAD_SITES_HELP = "Sites Help";

        public const string IMPORT_FAILED_NOVALIDDATA = "The import failed. The file {0} does not contain valid data.";
        public const string IMPORT_FAILED_MISSINGMANDATORYFIELDS = "The import failed. Missing mandatory field(s): {0} ";
        public const string IMPORT_FAILED_NOMATCHINGFIELDS = "The import failed. Please validate that your file is correct.";
        public const string IMPORT_VALUE_NOT_IN_CV = "The value {0} is not in {1} CV. Please validate your input.";
        public const string IMPORT_VALUE_NOT_IN_DATABASE = "The value {0} is not in {1} Table. Please validate your input.";
        public const string IMPORT_VALUE_INVALIDCHARACTERS = "The value in column {0} contains invalid characters";
        public const string IMPORT_VALUE_CANNOTBENULL = "The value in column {0} can not be NULL";
        public const string IMPORT_VALUE_CANNOTBEEMPTY = "The value in column {0} can not be empty";
        public const string IMPORT_VALUE_INVALIDRANGE = "The value in column {0} is invalid. Valid range:{1}";
        public const string IMPORT_VALUE_INVALIDVALUE = "The value in column {0} is invalid.";
        public const string IMPORT_VALUE_ISDUPLICATE = "The data conflicts with a record in the upload. Possible duplicate value in field: {0}.";
        public const string IMPORT_VALUE_UPDATED = "The value '{1}' in column {0} will be changed to '{2}'.";
        public const string IMPORT_VALUE_LOCALVALUE_NOT_COMPLETE = "Please make sure that record contains all required values ( LocalX, LocalY, LocalSRSName)";
        public const string IMPORT_VALUE_ELEVATION_VERTICALDATUM = "When specifying a value for Elevation_m a vertical datum from the controlled vocabulary is required. ";
        public const string IMPORT_STATUS_UPLOAD = "Uploading...";
        public const string STATUS_PROCESSING = "Processing...";
        public const string IMPORT_STATUS_PROCESSING_DATAVALUES = "The Upload is being processed. Processing record {0} of {1}; (New {2} of {0}, Rej. {3} of {0}, Dupl. {4} of {0})";
        public const string IMPORT_STATUS_PROCESSING = "The Upload is being processed. Processing record {0} of {1};";
        public const string IMPORT_STATUS_PROCESSING_RECORDS = "Processing {0} '{1}' records...";

        public const string IMPORT_STATUS_PROCESSING_TIMESERIES = "The timeseries are being processed. Processing site {0} of {1}";
        public const string IMPORT_STATUS_INSERTING = "The data is inserted";
        public const string IMPORT_STATUS_ERROR = "An error occured during processing.";
        public const string IMPORT_STATUS_EXTRACTNG = "Extracting file";
        public const string IMPORT_STATUS_TIMESERIES = "Updating timeseries...";
        public static string IMPORT_STATUS_PROCESSING_DONE = "Processing Complete";
        public static string IMPORT_UNSPECIFIED_ERROR = "An error occured inserting the data.";
        public static string IMPORT_CREATE_SERIESCATALOG = "Error updating series catalog table. Missing {0} Information.";
        public static string IMPORT_COMMIT_PROGRESS = "The data is being processed. Inserted {0} of {1} records.";
        public static string IMPORT_COMMIT_COMPLETE = "Processing Complete";
        public static string IMPORT_COMMIT_FAILED = "Processing Failed";

        public static string USERACCOUNT_NOT_LINKED = "This Google email has no associated publishing account. Please sign up by clicking 'Request New Publishing Account'. If you encounter additional problems, please reach out to help@cuahsi.org.";
    }
}
