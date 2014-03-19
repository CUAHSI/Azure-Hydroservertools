using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.SqlServer;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ODM_1_1_1EFModel
{
    public partial class ODM_1_1_1Entities : DbContext
    {
        public ODM_1_1_1Entities(string connectionString)
            : base(connectionString)
        {
        }
    }
    //// EF follows a Code based Configuration model and will look for a class that
    //// derives from DbConfiguration for executing any Connection Resiliency strategies
    //public class EFConfiguration : DbConfiguration
    //{
    //    public EFConfiguration()
    //    {
    //        SetExecutionStrategy("System.Data.SqlClient", () => new SqlAzureExecutionStrategy(1, TimeSpan.FromSeconds(30)));
    //    }
       

    //}

    public static class myHelper
    {
      

    }

}
