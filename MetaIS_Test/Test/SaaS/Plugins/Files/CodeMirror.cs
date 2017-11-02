// START PLUGIN SETTINGS
// PluginLibraryName = _CodeMirror.dll
// PluginReferences  =
// CompileOrder      = 1
// END PLUGIN SETTINGS

using System;

namespace DynamicPlugins {

    /// <summary>
    /// A simple class that calls a static method in the core library.
    /// </summary>
    public class CodeMirror {

        /// <summary>
        /// Return the input int by calling a static method in the core library.
        /// </summary>
        /// <param name="x">The input value to return.</param>
        /// <returns>Return the input value.</returns>
        public int MirrorInt(int x) {
            return MetaIS.SaaS.Plugins.HelperPlugin.GetMirrorValue(x);
        }
    }
}