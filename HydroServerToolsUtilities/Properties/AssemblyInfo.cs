using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

using log4net;
using log4net.Config;

// General Information about an assembly is controlled through the following
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("HydroServerToolsUtilities")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct("HydroServerToolsUtilities")]
[assembly: AssemblyCopyright("Copyright ©  2017")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible
// to COM components.  If you need to access a type in this assembly from
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("cff64114-d63e-4015-a460-862f7f22d857")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers
// by using the '*' as shown below:
// [assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyVersion("1.0.0.0")]
[assembly: AssemblyFileVersion("1.0.0.0")]

////Apply to the entire assembly...
////NOTE: ConfigFile file path is relative to AppDomain.CurrentDomain.BaseDirectory
////       as explained under 'Configuration Attributes' at https://logging.apache.org/log4net/release/manual/configuration.html
////
//// BCC - 17-Sep-2018 - For a better approach, please see HydroServerTools app settings...
////
//[assembly: log4net.Config.XmlConfigurator(ConfigFile=@"..\HydroServerTools\BulkUpload.log4net", Watch=true)]


