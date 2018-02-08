using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations;

using HydroserverToolsBusinessObjects.Interfaces;

namespace HydroserverToolsBusinessObjects.Models
{
    //A 'subset' of the VariablesModel class - for use with the Basic Template...
    [Serializable()]
    public class VariablesModelBasicTemplate : IHydroserverRepositoryProxy<VariablesModelBasicTemplate, VariablesModel>
    {
        //Required fields...
        [Required]
        public string VariableCode { get; set; }
        [Required]
        public string VariableName { get; set; }
        [Required]
        public string VariableUnitsName { get; set; }
        [Required]
        public string DataType { get; set; }
        [Required]
        public string SampleMedium { get; set; }
        [Required]
        public string ValueType { get; set; }
        [Required]
        public string IsRegular { get; set; }
        [Required]
        public string TimeSupport { get; set; }
        [Required]
        public string TimeUnitsName { get; set; }
        [Required]
        public string GeneralCategory { get; set; }
        [Required]
        public string NoDataValue { get; set; }

        //For validation error reporting...
        public string Errors { get; set; }

        //Interface methods...
        public VariablesModel InitializeProxy()
        {
            var variablesModel = new VariablesModel();

            //Assign values from current instance...
            variablesModel.VariableCode = VariableCode;
            variablesModel.VariableName = VariableName;
            variablesModel.VariableUnitsName = VariableUnitsName;
            variablesModel.DataType = DataType;
            variablesModel.SampleMedium = SampleMedium;
            variablesModel.ValueType = ValueType;
            variablesModel.IsRegular = IsRegular; 
            variablesModel.TimeSupport = TimeSupport;
            variablesModel.TimeUnitsName = TimeUnitsName;
            variablesModel.GeneralCategory = GeneralCategory;
            variablesModel.NoDataValue = NoDataValue;

            //Assign default values...
            var strNotApplicable = "Not applicable";

            variablesModel.Speciation = strNotApplicable;

            variablesModel.VariableUnitsID = null;
            variablesModel.TimeUnitsID = null;

            variablesModel.Errors = null;

            //Processing complete - return...
            return variablesModel;
        }

        public VariablesModelBasicTemplate ValueFromProxy(VariablesModel proxy)
        {
            if ( null != proxy)
            {
                //Assign values from current instance...
                VariableCode = proxy.VariableCode;
                VariableName = proxy.VariableName;
                VariableUnitsName = proxy.VariableUnitsName;
                DataType = proxy.DataType;
                SampleMedium = proxy.SampleMedium;
                ValueType = proxy.ValueType;
                IsRegular = proxy.IsRegular;
                TimeSupport = proxy.TimeSupport;
                TimeUnitsName = proxy.TimeUnitsName;
                GeneralCategory = proxy.GeneralCategory;
                NoDataValue = proxy.NoDataValue;

                Errors = proxy.Errors;
            }

            //Processing complete - return current instance
            return this;
        }

        public bool CompareWithProxy(VariablesModel proxy)
        {
            return ((null != proxy) &&
                //Compare proxy values to current instance...
                VariableCode == proxy.VariableCode &&
                VariableName == proxy.VariableName &&
                VariableUnitsName == proxy.VariableUnitsName &&
                DataType == proxy.DataType &&
                SampleMedium == proxy.SampleMedium &&
                ValueType == proxy.ValueType &&
                IsRegular == proxy.IsRegular &&
                TimeSupport == proxy.TimeSupport &&
                TimeUnitsName == proxy.TimeUnitsName &&
                GeneralCategory == proxy.GeneralCategory &&
                NoDataValue == proxy.NoDataValue );
        }

    }
}
