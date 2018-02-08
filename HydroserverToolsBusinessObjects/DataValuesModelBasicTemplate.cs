using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations;

using HydroserverToolsBusinessObjects.Interfaces;

namespace HydroserverToolsBusinessObjects.Models
{
    //A 'subset' of the DataValuesModel class - for use with the Basic Template...
    [Serializable()]
    public class DataValuesModelBasicTemplate : IHydroserverRepositoryProxy<DataValuesModelBasicTemplate, DataValuesModel>
    {
        //Required fields...
        [Required]
        public string DataValue { get; set; }
        [Required]
        public string LocalDateTime { get; set; }
        [Required]
        public string UTCOffset { get; set; }
        [Required]
        public string DateTimeUTC { get; set; }
        [Required]
        public string SiteCode { get; set; }
        [Required]
        public string VariableCode { get; set; }
        [Required]
        public string MethodCode { get; set; }
        [Required]
        public string SourceCode { get; set; }
        [Required]
        public string QualityControlLevelCode { get; set; }

        //For validation error reporting...
        public string Errors { get; set; }

        //Interface methods...
        public DataValuesModel InitializeProxy()
        {
            var dataValuesModel = new DataValuesModel();

            //Assign values from current instance...
            dataValuesModel.DataValue = DataValue;
            dataValuesModel.LocalDateTime = LocalDateTime;
            dataValuesModel.UTCOffset = UTCOffset;
            dataValuesModel.DateTimeUTC = DateTimeUTC;
            dataValuesModel.SiteCode = SiteCode;
            dataValuesModel.VariableCode = VariableCode;
            dataValuesModel.MethodCode = MethodCode;
            dataValuesModel.SourceCode = SourceCode;
            dataValuesModel.QualityControlLevelCode = QualityControlLevelCode;

            //Assign default values...
            dataValuesModel.CensorCode = "nc";

            dataValuesModel.ValueAccuracy = null;
            dataValuesModel.OffsetValue = null;
            dataValuesModel.OffsetTypeCode = null;
            dataValuesModel.QualifierCode = null;
            dataValuesModel.SampleCode = null;
            dataValuesModel.LabSampleCode = null;
            dataValuesModel.DerivedFromID = null;

            dataValuesModel.ValueID = null;
            dataValuesModel.SiteID = null;
            dataValuesModel.VariableID = null;
            dataValuesModel.OffsetTypeID = null;
            dataValuesModel.QualifierID = null;
            dataValuesModel.MethodID = null;
            dataValuesModel.MethodDescription = null;
            dataValuesModel.SampleID = null;
            dataValuesModel.QualityControlLevelID = null;

            dataValuesModel.Errors = null;

            //Processing complete - return
            return dataValuesModel;
        }

        public DataValuesModelBasicTemplate ValueFromProxy(DataValuesModel proxy)
        {
            if (null != proxy)
            {
                //Assign values to current instance...
                DataValue = proxy.DataValue;
                LocalDateTime = proxy.LocalDateTime;
                UTCOffset = proxy.UTCOffset;
                DateTimeUTC = proxy.DateTimeUTC;
                SiteCode = proxy.SiteCode;
                VariableCode = proxy.VariableCode;
                MethodCode = proxy.MethodCode;
                SourceCode = proxy.SourceCode;
                QualityControlLevelCode = proxy.QualityControlLevelCode;

                Errors = proxy.Errors;
            }

            //Processing complete - return current instance
            return this;
        }

        public bool CompareWithProxy(DataValuesModel proxy)
        {
            return ((null != proxy) &&
                //Compare proxy values to current instance...
                DataValue == proxy.DataValue &&
                LocalDateTime == proxy.LocalDateTime &&
                UTCOffset == proxy.UTCOffset &&
                DateTimeUTC == proxy.DateTimeUTC &&
                SiteCode == proxy.SiteCode &&
                VariableCode == proxy.VariableCode &&
                MethodCode == proxy.MethodCode &&
                SourceCode == proxy.SourceCode &&
                QualityControlLevelCode == proxy.QualityControlLevelCode );
        }

    }
}
