﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace BattleChess3.CrossFireFigures.Localization {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "17.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Strings {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Strings() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("BattleChess3.CrossFireFigures.Localization.Strings", typeof(Strings).Assembly);
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
        ///   Looks up a localized string similar to Archer
        ///      
        ///Move:
        ///
        ///⬜⬜⬜⬜⬜⬜⬜⬜
        ///⬜⬜⬜⬜⬜⬜⬜⬜
        ///⬜⬜⬜⬜⬜⬜⬜⬜
        ///⬜⬜⬜⬜⬜⬜⬜⬜
        ///⬜⬜⬜⬜⬜⬜⬜⬜
        ///⬜⬜⬜⬜⬜⬜⬜⬜
        ///⬜⬜⬜⬜⬜⬜⬜⬜
        ///⬜⬜⬜⬛▣⬛⬜⬜
        ///
        ///Attack:
        ///
        ///⬜⬜⬜⬛⬜⬜⬜⬜
        ///⬜⬜⬜⬛⬜⬜⬜⬛
        ///⬛⬜⬜⬛⬜⬜⬛⬜
        ///⬜⬛⬜⬛⬜⬛⬜⬜
        ///⬜⬜⬛⬛⬛⬜⬜⬜
        ///⬜⬜⬜▣⬜⬜⬜⬜
        ///⬜⬜⬜⬜⬜⬜⬜⬜
        ///⬜⬜⬜⬜⬜⬜⬜⬜
        ///      
        ///Info: Good for taking control
        ///Good vs: Ninja, Knight, Spy
        ///Bad vs: Builder
        ///Special: Does not move with attack
        ///.
        /// </summary>
        internal static string Archer_Description {
            get {
                return ResourceManager.GetString("Archer_Description", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Archer.
        /// </summary>
        internal static string Archer_Name {
            get {
                return ResourceManager.GetString("Archer_Name", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Bomber
        ///
        ///Move:
        ///
        ///⬜⬜⬜⬜⬜⬜⬜⬜
        ///⬜⬜⬜⬜⬜⬜⬜⬜
        ///⬜⬜⬜⬜⬜⬜⬜⬜
        ///⬛⬜⬛⬜⬛⬜⬜⬜
        ///⬜⬜⬜▩▩▩⬜⬜
        ///⬛⬜▣▩⬛▩⬜⬜
        ///⬜⬜⬜▩▩▩⬜⬜
        ///⬛⬜⬛⬜⬛⬜⬜⬜
        ///      
        ///Attack: Cannot directly attack
        ///      
        ///Info: Hard to use, high reward
        ///Good vs: Ninja, Spy
        ///Bad vs: Builder, Archer, Knight
        ///Special: When move, near tiles are destroyed
        ///.
        /// </summary>
        internal static string Bomber_Description {
            get {
                return ResourceManager.GetString("Bomber_Description", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Bomber.
        /// </summary>
        internal static string Bomber_Name {
            get {
                return ResourceManager.GetString("Bomber_Name", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Builder
        ///
        ///Move:
        ///
        ///⬜⬜⬜⬜⬜⬜⬜⬜
        ///⬜⬜⬜⬜⬜⬜⬜⬜
        ///⬜⬜⬜⬜⬜⬜⬜⬜
        ///⬜⬜⬜⬜⬜⬜⬜⬜
        ///⬜⬜⬜⬛⬜⬜⬜⬜
        ///⬜⬜⬛▣⬛⬜⬜⬜
        ///⬜⬜⬜⬛⬜⬜⬜⬜
        ///⬜⬜⬜⬜⬜⬜⬜⬜
        ///
        ///Shield:
        ///
        ///⬜⬜⬜⬜⬜⬜⬜⬜
        ///⬜⬜⬜⬜⬜⬜⬜⬜
        ///⬜⬜⬜⬜⬜⬜⬜⬜
        ///⬜⬜▦▦▦⬜⬜⬜
        ///⬜▦⬜⬜⬜▦⬜⬜
        ///⬜▦⬜▣⬜▦⬜⬜
        ///⬜▦⬜⬜⬜▦⬜⬜
        ///⬜⬜▦▦▦⬜⬜⬜
        ///      
        ///Attack: Cannot directly attack
        ///      
        ///Info: Defensive unit
        ///Good vs: Archer, Spy
        ///Bad vs: Ninja, Knight
        ///Special: Creates shield around itself
        ///Special: Moving shield pushes units away
        ///.
        /// </summary>
        internal static string Builder_Description {
            get {
                return ResourceManager.GetString("Builder_Description", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Builder.
        /// </summary>
        internal static string Builder_Name {
            get {
                return ResourceManager.GetString("Builder_Name", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Cross fire.
        /// </summary>
        internal static string CrossFireFigureGroup_Name {
            get {
                return ResourceManager.GetString("CrossFireFigureGroup_Name", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Knight
        ///
        ///Move: As chess knight
        ///
        ///⬜⬜⬜⬜⬜⬜⬜⬜
        ///⬜⬜⬜⬜⬜⬜⬜⬜
        ///⬜⬜⬜⬜⬜⬜⬜⬜
        ///⬜⬛⬜⬛⬜⬜⬜⬜
        ///⬛⬜⬜⬜⬛⬜⬜⬜
        ///⬜⬜▣⬜⬜⬜⬜⬜
        ///⬛⬜⬜⬜⬛⬜⬜⬜
        ///⬜⬛⬜⬛⬜⬜⬜⬜
        ///
        ///Attack: Kills all units to tile
        ///
        ///⬜⬜⬜⬜⬜⬜⬜⬜
        ///⬜⬜⬜⬜⬜⬜⬜⬜
        ///⬜⬜⬛⬜⬜⬛⬜⬜
        ///⬛⬜⬛⬜⬛⬜⬜⬜
        ///⬜⬛⬛⬛⬜⬜⬜⬜
        ///⬛⬛▣⬛⬛⬛⬛⬜
        ///⬜⬛⬛⬛⬜⬜⬜⬜
        ///⬛⬜⬛⬜⬛⬜⬜⬜
        ///      
        ///Info: All round good
        ///Good vs: Builder, Spy, Ninja
        ///Bad vs: Archer
        ///.
        /// </summary>
        internal static string Knight_Description {
            get {
                return ResourceManager.GetString("Knight_Description", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Knight.
        /// </summary>
        internal static string Knight_Name {
            get {
                return ResourceManager.GetString("Knight_Name", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Ninja
        ///
        ///Move:
        ///
        ///⬜⬜⬜⬜⬜⬜⬜⬜
        ///⬜⬜⬜⬜⬜⬜⬜⬜
        ///⬜⬜⬜⬜⬜⬜⬜⬜
        ///⬜⬜⬜⬜⬜⬜⬜⬜
        ///⬜⬜⬜⬛⬜⬜⬜⬜
        ///⬜⬜⬛▦⬛⬜⬜⬜
        ///⬜⬜⬜▣⬜⬜⬜⬜
        ///⬜⬜⬜⬜⬜⬜⬜⬜
        ///
        ///⬜⬜⬜⬜⬜⬜⬜⬜
        ///⬜⬜⬜⬜⬜⬜⬜⬜
        ///⬜⬜⬜⬜⬜⬜⬜⬜
        ///⬜⬜⬜⬜⬜⬜⬜⬜
        ///⬜⬜⬜⬜⬜⬜⬜⬜
        ///⬜⬜⬛⬜⬛⬜⬜⬜
        ///⬜⬜⬜▣⬜⬜⬜⬜
        ///⬜⬜⬜⬜⬜⬜⬜⬜
        ///
        ///Attack:
        ///
        ///⬜⬜⬜⬜⬜⬜⬜⬜
        ///⬜⬜⬜⬜⬜⬜⬜⬜
        ///⬜⬜⬜⬜⬜⬜⬜⬜
        ///⬜⬜⬜⬜⬜⬜⬜⬜
        ///⬜⬜⬜⬛⬜⬜⬜⬜
        ///⬜⬜⬜⬛⬜⬜⬜⬜
        ///⬜⬜⬛▣⬛⬜⬜⬜
        ///⬜⬜⬜⬛⬜⬜⬜⬜
        ///
        ///⬜⬜⬜⬜⬜⬜⬜⬜
        ///⬜⬜⬜⬜⬜⬜⬜⬜
        ///⬜⬜⬜⬜⬜⬜⬜⬜
        ///⬜⬜⬜⬜⬜⬜⬜⬜
        ///⬜⬜⬜⬜⬜⬜⬜⬜
        ///⬜⬜⬜⬛⬜⬜⬜⬜
        ///⬜⬜⬛▣⬛⬜⬜⬜
        ///⬜⬜⬜⬛⬜⬜⬜⬜
        ///      
        ///Info: Basic unit
        ///Good vs: Builder
        ///Bad vs: Spy, Archer, Knight, Bomber
        ///Special: Can jump over object
        ///Special: On last tile turns into Bomber
        ///.
        /// </summary>
        internal static string Ninja_Description {
            get {
                return ResourceManager.GetString("Ninja_Description", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Ninja.
        /// </summary>
        internal static string Ninja_Name {
            get {
                return ResourceManager.GetString("Ninja_Name", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Spy
        ///
        ///Move + Attack: As chess king
        ///
        ///⬜⬜⬜⬜⬜⬜⬜⬜
        ///⬜⬜⬜⬜⬜⬜⬜⬜
        ///⬜⬜⬜⬜⬜⬜⬜⬜
        ///⬜⬜⬜⬜⬜⬜⬜⬜
        ///⬜⬜⬜⬜⬜⬜⬜⬜
        ///⬜⬜⬛⬛⬛⬜⬜⬜
        ///⬜⬜⬛▣⬛⬜⬜⬜
        ///⬜⬜⬛⬛⬛⬜⬜⬜
        ///
        ///Info: Strategic unit
        ///Good vs: Builder, Ninja
        ///Bad vs: Archer, Knight, Bomber
        ///Special: Can swap with allied unit
        ///.
        /// </summary>
        internal static string Spy_Description {
            get {
                return ResourceManager.GetString("Spy_Description", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Spy.
        /// </summary>
        internal static string Spy_Name {
            get {
                return ResourceManager.GetString("Spy_Name", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Wall
        ///
        ///Info: Can be destoyed by allied unit
        ///
        ///Attack: Cannot attack
        ///Movement: Cannot move 
        ///.
        /// </summary>
        internal static string Wall_Description {
            get {
                return ResourceManager.GetString("Wall_Description", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Wall.
        /// </summary>
        internal static string Wall_Name {
            get {
                return ResourceManager.GetString("Wall_Name", resourceCulture);
            }
        }
    }
}
