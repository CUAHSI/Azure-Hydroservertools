﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18408
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace HydroserverToolsBusinessObjects {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public class HISRessources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal HISRessources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("HydroserverToolsBusinessObjects.HISRessources", typeof(HISRessources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Database connection failed. Please validate that the name of the Datasource is correct.
        /// </summary>
        public static string CONNECTION_FAILED_DATASOURCENAME {
            get {
                return ResourceManager.GetString("CONNECTION_FAILED_DATASOURCENAME", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Database connection failed. Please validate that the Username and Password are correct.
        /// </summary>
        public static string CONNECTION_FAILED_LOGIN {
            get {
                return ResourceManager.GetString("CONNECTION_FAILED_LOGIN", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Database connection failed. Please validate that the name of the Server is correct.
        /// </summary>
        public static string CONNECTION_FAILED_SERVERNAME {
            get {
                return ResourceManager.GetString("CONNECTION_FAILED_SERVERNAME", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Database connection successful.
        /// </summary>
        public static string CONNECTION_SUCCESS {
            get {
                return ResourceManager.GetString("CONNECTION_SUCCESS", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to @&quot;res://*/ODM_1_1_1EFModel.csdl|res://*/ODM_1_1_1EFModel.ssdl|res://*/ODM_1_1_1EFModel.msl&quot;.
        /// </summary>
        public static string EFMODEL {
            get {
                return ResourceManager.GetString("EFMODEL", resourceCulture);
            }
        }
    }
}
