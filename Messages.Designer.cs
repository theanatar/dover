﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18444
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Dover.Framework {
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
    internal class Messages {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Messages() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Dover.Framework.Messages", typeof(Messages).Assembly);
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
        ///   Looks up a localized string similar to Erro in addin {0} version {1}.
        /// </summary>
        internal static string AddInError {
            get {
                return ResourceManager.GetString("AddInError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Unexpected error on Dover: {0}\n {1}.
        /// </summary>
        internal static string AddInException {
            get {
                return ResourceManager.GetString("AddInException", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Connection error on AddOn: {0}.
        /// </summary>
        internal static string ConnectionError {
            get {
                return ResourceManager.GetString("ConnectionError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Created AppFolder {0} - Initializing Core and Addins.
        /// </summary>
        internal static string CreatedAppFolder {
            get {
                return ResourceManager.GetString("CreatedAppFolder", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Dover Framework.
        /// </summary>
        internal static string DoverName {
            get {
                return ResourceManager.GetString("DoverName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Error initializing addins {0}.
        /// </summary>
        internal static string ErrorStartup {
            get {
                return ResourceManager.GetString("ErrorStartup", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Form event {0} on class {1} should be protected internal virtual, so it can be proxied..
        /// </summary>
        internal static string FormDeclarationError {
            get {
                return ResourceManager.GetString("FormDeclarationError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Error in event {0}.
        /// </summary>
        internal static string FormEventError {
            get {
                return ResourceManager.GetString("FormEventError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Error preparing Framework - {0}.
        /// </summary>
        internal static string GeneralError {
            get {
                return ResourceManager.GetString("GeneralError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Preparing Framework for initialization.
        /// </summary>
        internal static string PreparingFramework {
            get {
                return ResourceManager.GetString("PreparingFramework", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Rolling back transaction.
        /// </summary>
        internal static string RollBack {
            get {
                return ResourceManager.GetString("RollBack", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Successfully started {0}({1}).
        /// </summary>
        internal static string Started {
            get {
                return ResourceManager.GetString("Started", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Initializing {0}({1}).
        /// </summary>
        internal static string Starting {
            get {
                return ResourceManager.GetString("Starting", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Initializing assemblies on folder {0}.
        /// </summary>
        internal static string StartupFolder {
            get {
                return ResourceManager.GetString("StartupFolder", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Transaction event {0} on class {1} should be protected internal virtual, so it can be proxied..
        /// </summary>
        internal static string TransactionDeclarationError {
            get {
                return ResourceManager.GetString("TransactionDeclarationError", resourceCulture);
            }
        }
    }
}
