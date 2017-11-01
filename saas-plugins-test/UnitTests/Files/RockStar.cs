// START PLUGIN SETTINGS
// PluginLibraryName = _RockStar.dll
// PluginReferences  =
// CompileOrder      = 2
// END PLUGIN SETTINGS

using System;
using System.Drawing;

namespace DynamicPlugins {

    /// <summary>
    /// A trivial static class.
    /// </summary>
    public static class RockStar {

        /// <summary>
        /// Return the input int by calling HelperPlugin.GetMirrorValue in the core library.
        /// </summary>
        /// <param name="x">The input value to return.</param>
        /// <returns>Return the input value x.</returns>
        public static int GetValue(int x)
        {
            return saas_plugins.SaaS.HelperPlugin.GetMirrorValue(x);
        }
    }
}