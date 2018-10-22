using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HydroserverToolsBusinessObjects.Models
{
    public class DatabaseTableValueCountModel
    {
        [DisplayName("Sites")]
        public Int64 SiteCount { get; set; }
        [DisplayName("Variables")]
        public Int64 VariablesCount { get; set; }
        public Int64 OffsetTypesCount { get; set; }
        public Int64 SourcesCount { get; set; }
        public Int64 MethodsCount { get; set; }
        public Int64 LabMethodsCount { get; set; }
        public Int64 SamplesCount { get; set; }
        public Int64 QualifiersCount { get; set; }
        public Int64 QualityControlLevelsCount { get; set; }
        public Int64 DataValuesCount { get; set; }
        public Int64 GroupDescriptionsCount { get; set; }
        public Int64 GroupsCount { get; set; }
        public Int64 DerivedFromCount { get; set; }
        public Int64 CategoriesCount { get; set; }
        public Int64 SeriesCatalog { get; set; }
    }
}
