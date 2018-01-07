using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Collections;

namespace HydroServerToolsUtilities
{
    //A simple interface for use by the RejectedItemsData class...
    public interface IRejectedItemsData
    {
        String TableName { get; set; }

        List<StatusMessage> StatusMessages { get; set; }

        IList RejectedItems { get; set; }

        List<string> RequiredPropertyNames { get; set; }

        List<string> OptionalPropertyNames { get; set; }

    }

    //A simple class encapsulating context-related values for the rejected items dialog...
    public class RejectedItemsData<tModelType> : IRejectedItemsData
    {
        public String TableName { get; set; }

        public List<StatusMessage> StatusMessages { get; set; }

        public IList RejectedItems { get; set; }

        public List<string> RequiredPropertyNames { get; set; }

        public List<string> OptionalPropertyNames { get; set; }
    }
}
