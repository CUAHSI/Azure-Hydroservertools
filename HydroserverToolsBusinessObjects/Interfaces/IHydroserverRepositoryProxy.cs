using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HydroserverToolsBusinessObjects.Interfaces
{
    //Define a generic interface for: Initializing 'proxy' values from a 'source' instance
    //                                Setting 'source' values from a 'proxy' instance 
    public interface IHydroserverRepositoryProxy<tSourceType, tProxyType> where tSourceType : class, new()
                                                                          where tProxyType : class, new()
    {
        tProxyType InitializeProxy();
        tSourceType ValueFromProxy(tProxyType proxy);

        //Compare values of source and proxy instances - return true on match, false otherwise...
        bool CompareWithProxy(tProxyType proxy);
    }
}
