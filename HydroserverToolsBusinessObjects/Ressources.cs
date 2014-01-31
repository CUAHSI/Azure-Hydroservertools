using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HydroserverToolsBusinessObjects
{
    public class Ressources
    {
        public const string EFMODEL = @"res://*/ODM_1_1_1EFModel.csdl|res://*/ODM_1_1_1EFModel.ssdl|res://*/ODM_1_1_1EFModel.msl";
        public const string CONNECTION_SUCCESS = "Database connection succesful";
        public const string CONNECTION_FAILED_SERVERNAME = "Database connection failed. Please validate that the name of the Server is correct";
        public const string CONNECTION_FAILED_DATASOURCENAME = "Database connection failed. Please validate that the name of the Datasource is correct";
        public const string CONNECTION_FAILED_LOGIN = "Database connection failed. Please validate that the Username and Password are correct";
        public const string HYDROSERVER_USERLOOKUP_FAILED = "The current user does not have an associated Hydroserver database. Please contact your administrator or CUAHSI";
        public const string IMPORT_FAILED = "The Import failed please validate your input";
        //public const string ENTITYCONNECTIONSTRING_LOOKUP_FAILED = "THe "
    }
}
