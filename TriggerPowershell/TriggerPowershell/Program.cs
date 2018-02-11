using System;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Text;

namespace WebJobs
{
    class Execute_Batch_File
    {
        static void Main(string[] args)
        {
            copytemplate();
        }

        private static string RunScript(string scripts)
        {
            // create Powershell runspace
            Runspace runspace = RunspaceFactory.CreateRunspace();
            // open it
            runspace.Open();
            // create a pipeline and feed it the script text 
            Pipeline pipeline = runspace.CreatePipeline();

            pipeline.Commands.AddScript(scripts);
            pipeline.Commands.Add("Out-String");
            //execute the script
            Collection<PSObject> results = pipeline.Invoke();
            //close the runspace
            runspace.Close();
            // convert the script result into a single string
            StringBuilder stringBuilder = new StringBuilder();
            foreach (PSObject obj in results)
            {
                stringBuilder.AppendLine(obj.ToString());
            }
            return stringBuilder.ToString();
        }

        private static void copytemplate()
        {
            string connectionString = "Data Source=tcp:bhi5g2ajst.database.windows.net,1433;Initial Catalog=master;User ID=HisCentralAdmin@bhi5g2ajst; Password=F@deratedResearch;Persist Security Info=true;";
            string targetDBName = "ODM_1_1_1_placeholder";
            string sourceDBName = "ODM_1_1_1_template";
            var sourceExists = CheckDatabaseExists(connectionString, sourceDBName);
            if (sourceExists) copyDatabase(connectionString, sourceDBName, targetDBName);
        }
        private static void copyDatabase(string connectionString, string sourceDBName, string targetDBName)
        {

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    using (SqlCommand command = new SqlCommand(
                                                 "CREATE DATABASE " + targetDBName + " AS COPY OF " + sourceDBName, connection))
                    {
                        connection.Open();
                        command.CommandTimeout = 300;
                        string result = (string)command.ExecuteScalar();
                        connection.Close();
                    }
                }
                catch (Exception ex)
                {
                    connection.Close();
                    //send email
                }
            }

        }

        private static bool CheckDatabaseExists(string connectionString, string databaseName)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                using (var command = new SqlCommand("SELECT [name] FROM [sys].[databases] WHERE [name] = '" + databaseName + "'", connection))
                {
                    connection.Open();
                    return (command.ExecuteScalar() != DBNull.Value);
                }
            }
        }
    }
}