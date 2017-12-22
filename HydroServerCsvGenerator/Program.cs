using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using HydroServerToolsRepository;


namespace HydroServerCsvGenerator
{
    //A simple console program to generate uploadable csv files from a HydroServer User database...
    class Program
    {
        static void Main(string[] args)
        {
            //Set connection string...
            var connectionString = "Data Source=tcp:bhi5g2ajst.database.windows.net,1433;Initial Catalog=ODM_ShaleNetwork_08182017;User ID=HisCentralAdmin@bhi5g2ajst; Password=F@deratedResearch;Persist Security Info=true;";

            //Retrieve collections used in csv file generation...

        }
    }
}
