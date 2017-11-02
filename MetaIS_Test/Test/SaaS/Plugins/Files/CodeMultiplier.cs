// START PLUGIN SETTINGS
// PluginLibraryName = _CodeMultiplier.dll
// PluginReferences  = _CodeMirror.dll, _RockStar.dll
// CompileOrder      = 3
// END PLUGIN SETTINGS

using System;
using System.Reflection;

namespace DynamicPlugins {

    /// <summary>
    /// A simple class that interacts with other dynamically created classes.
    /// </summary>
    public class CodeMultiplier {
        CodeMirror obj = null;

        /// <summary>
        /// Return the input int multiplied by 2. Instantiates a runtime created mirror class to do this.
        /// </summary>
        /// <param name="x">The input value to multiply.</param>
        /// <returns>Return the input value multiplied by 2.</returns>
        public int MultBy2(int x) {
            DynamicPlugins.CodeMirror obj = new DynamicPlugins.CodeMirror();
            return (int)obj.MirrorInt(x) * 2;
        }

        /// <summary>
        /// Return the input int multiplied by itself. Instantiates a runtime created mirror class to do this as well 
        /// as a runtime created static class.
        /// </summary>
        /// <param name="x">The input value to multiply.</param>
        /// <returns>Return the input value multiplied by itself.</returns>
        public int MultByMirror(int x) {
            DynamicPlugins.CodeMirror obj = new DynamicPlugins.CodeMirror();
            return (int)obj.MirrorInt(x) * RockStar.GetValue(x);
        }
    }
}