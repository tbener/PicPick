﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace PicPick.Properties {
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
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("PicPick.Properties.Resources", typeof(Resources).Assembly);
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
        ///   Looks up a localized string similar to The file {0} already exists in this location:.
        /// </summary>
        internal static string DLG_FILE_EXISTS_TITLE {
            get {
                return ResourceManager.GetString("DLG_FILE_EXISTS_TITLE", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized resource of type System.Drawing.Bitmap.
        /// </summary>
        internal static System.Drawing.Bitmap Folder_16x16 {
            get {
                object obj = ResourceManager.GetObject("Folder_16x16", resourceCulture);
                return ((System.Drawing.Bitmap)(obj));
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Source.
        /// </summary>
        internal static string WIZARD_TITLE_0 {
            get {
                return ResourceManager.GetString("WIZARD_TITLE_0", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Destination Base Path.
        /// </summary>
        internal static string WIZARD_TITLE_1 {
            get {
                return ResourceManager.GetString("WIZARD_TITLE_1", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Destination Pattern.
        /// </summary>
        internal static string WIZARD_TITLE_2 {
            get {
                return ResourceManager.GetString("WIZARD_TITLE_2", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Finish.
        /// </summary>
        internal static string WIZARD_TITLE_3 {
            get {
                return ResourceManager.GetString("WIZARD_TITLE_3", resourceCulture);
            }
        }
    }
}
