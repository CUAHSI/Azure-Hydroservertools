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
        public int SiteCount { get; set; }
        [DisplayName("Variables")]
        public int VariablesCount { get; set; }
        public int OffsetTypesCount { get; set; }
        public int SourcesCount { get; set; }
        public int MethodsCount { get; set; }
        public int LabMethodsCount { get; set; }
        public int SamplesCount { get; set; }
        public int QualifiersCount { get; set; }
        public int QualityControlLevelsCount { get; set; }
        public int DataValuesCount { get; set; }
        public int GroupDescriptionsCount { get; set; }
        public int GroupsCount { get; set; }
        public int DerivedFromCount { get; set; }
        public int CategoriesCount { get; set; }
        public int SeriesCatalog { get; set; }
    }
}
