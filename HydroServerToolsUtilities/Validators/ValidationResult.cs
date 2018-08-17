using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HydroServerToolsUtilities.Validators
{
    //A simple generic class for the association of a validator with a file name
    public class ValidationResult<TValidator> where TValidator : class, IValidationComplete
    {
        //Constructors...

        //Default...
        private ValidationResult() { }

        //Initializing...
        public ValidationResult(string fileName, TValidator fileValidator)
        {
#if (DEBUG)
            if (String.IsNullOrWhiteSpace(fileName) || null == fileValidator)
            {
                string paramName = String.IsNullOrWhiteSpace(fileName) ? "fileName" : "fileValidator";
                throw new ArgumentException("Invalid input parameter...", paramName);
            }
#endif
            FileName = fileName;
            FileValidator = fileValidator;

            //ValidationComplete = false;
        }

        //Properties
        public string FileName { get; private set; }

        public TValidator FileValidator { get; private set; }

        public bool ValidationComplete
        {
            get
            {
                return FileValidator.ValidationComplete;
            }
        }
    }
}