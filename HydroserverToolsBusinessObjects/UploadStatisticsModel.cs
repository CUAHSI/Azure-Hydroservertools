using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HydroserverToolsBusinessObjects
{
    public class UploadStatisticsModel
    {       
        public int NewRecordCount { get; set; }
        public int RejectedRecordCount { get; set; }
        public int UpdatedRecordCount { get; set; }
        public int DuplicateRecordCount { get; set; }            
    }
}
