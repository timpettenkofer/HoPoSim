﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace HoPoSim.Properties {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "15.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("HoPoSim.Properties.Resources", typeof(Resources).Assembly);
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
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to AGR / KWF.
        /// </summary>
        internal static string ApplicationOwner {
            get {
                return ResourceManager.GetString("ApplicationOwner", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Ihre Lizenz ist abgelaufen!
        ///(Ablaufdatum ist {0})
        ///Wenden Sie sich bitte an mail@philippe-guigue.de.
        /// </summary>
        internal static string ExpiredLicenseError {
            get {
                return ResourceManager.GetString("ExpiredLicenseError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Ihre Lizenz ist keine gültige &apos;HoPoSim {0}&apos; Lizenz!
        ///Wenden Sie sich bitte an mail@philippe-guigue.de.
        /// </summary>
        internal static string LicenseToError {
            get {
                return ResourceManager.GetString("LicenseToError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Keine gültige Lizenz wurde gefunden.
        ///Stellen Sie bitte sicher, dass mindestens eine von dieser Dateien
        ///
        ///{0}
        ///
        ///existiert oder wenden Sie sich bitte an mail@philippe-guigue.de.
        /// </summary>
        internal static string NoLicenseError {
            get {
                return ResourceManager.GetString("NoLicenseError", resourceCulture);
            }
        }
    }
}