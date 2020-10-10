﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace BattleChess3.DefaultFigures.Localization {
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [DebuggerNonUserCode()]
    [CompilerGenerated()]
    public class Strings {
        
        private static ResourceManager resourceMan;
        
        private static CultureInfo resourceCulture;
        
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Strings() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public static ResourceManager ResourceManager {
            get {
                if (ReferenceEquals(resourceMan, null)) {
                    ResourceManager temp = new ResourceManager("BattleChess3.DefaultFigures.Localization.Strings", typeof(Strings).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public static CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Default.
        /// </summary>
        public static string DefaultFigureGroup_Name {
            get {
                return ResourceManager.GetString("DefaultFigureGroup_Name", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to 
        ///Empty tile
        ///
        ///Empty tile, where you can go. It cannot be destroyed with almost any unit. It does not stop directional attack.
        ///        .
        /// </summary>
        public static string Empty_Description {
            get {
                return ResourceManager.GetString("Empty_Description", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Empty tile.
        /// </summary>
        public static string Empty_Name {
            get {
                return ResourceManager.GetString("Empty_Name", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to 
        ///Ninja
        ///
        ///Ninja is secret warrior and with his diagonal moves can he easily surprise enemy. He is one of cheap figures so he is best in front line.
        ///        .
        /// </summary>
        public static string Ninja_Description {
            get {
                return ResourceManager.GetString("Ninja_Description", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Ninja.
        /// </summary>
        public static string Ninja_Name {
            get {
                return ResourceManager.GetString("Ninja_Name", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to 
        ///Palm
        ///
        ///Palm tile, which you can easily destroy. It cannot move and belongs to no one.
        ///        .
        /// </summary>
        public static string Palm_Description {
            get {
                return ResourceManager.GetString("Palm_Description", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Palm.
        /// </summary>
        public static string Palm_Name {
            get {
                return ResourceManager.GetString("Palm_Name", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to 
        ///Stone
        ///
        ///Stone tile, which cannot be destroyed and where you cannot go. It cannot move and belongs to no one.
        ///        .
        /// </summary>
        public static string Stone_Description {
            get {
                return ResourceManager.GetString("Stone_Description", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Stone.
        /// </summary>
        public static string Stone_Name {
            get {
                return ResourceManager.GetString("Stone_Name", resourceCulture);
            }
        }
    }
}
