using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Threading;

using HydroServerTools.Validators;

namespace HydroServerTools.Utilities
{
    //A simple generic class for the association of file validator instances with an access semaphore...
    public class ValidationContext<TValidator> where TValidator: class, IValidationComplete
    {
        //Constructors...

        //Default...
        public ValidationContext()
        {
            ValidationResults = new List<ValidationResult<TValidator>>();
            ValidationResultSemaphore = new SemaphoreSlim(1, 1);
        }

        //Initializing...
        public ValidationContext( List<ValidationResult<TValidator>> validationResults) : this()
        {
#if (DEBUG)
            //Validate/initialize input parameters...
            if (null == validationResults || 0 >= validationResults.Count)
            {
                string paramName = "validationResults";
                throw new ArgumentException("Invalid input parameter...", paramName);
            }
#endif
            //Set instance member(s)...
            ValidationResults = validationResults;
        }

        //Properties...
        public List<ValidationResult<TValidator>> ValidationResults { get; private set; }

        public SemaphoreSlim ValidationResultSemaphore { get; private set; }
    }
}