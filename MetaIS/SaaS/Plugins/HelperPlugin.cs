/****************************** HelperPlugin ******************************\
This static class provides the core functions required to load and interact
with plugins in separate AppDomains. It deals with AppDomain Creation, 
Plugin Function Invocations, Plugin DLL compilation, and loading plugin
DLL's into AppDomains at runtime.

Copyright (c) Aaron Ulrich
This source is subject to the Apache License Version 2.0, January 2004
See http://www.apache.org/licenses/.
All other rights reserved.

THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

using System;
using System.Reflection;
using System.Collections.Generic;
using System.IO;

namespace MetaIS.SaaS.Plugins
{
    // OLD version caused a massive memory leak because the assemblies are never released
    //  from the application.... this version uses an AppDomain to resolve this.
    
    public static class HelperPlugin 
    {
        
        public static int GetMirrorValue(int x) {
            return x;
        }

        public static string ObjectToString(object obj) {
            string sRes = "NULL";
            if(obj!=null)
                sRes = obj.ToString();
            return sRes;
        }


        #region " Create AppDomain "

        /// <summary>
        /// Use this to properly create an AppDomain that overrides the default dll location.
        /// </summary>
        /// <param name="domainName">The name for this AppDomain.</param>
        /// <param name="baseDirectory">The base directory to expect DLL's (ex. Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location))</param>
        /// <param name="subDirectory">The sub directory to expect DLL's.</param>
        /// <param name="configFile">A configuration file (ex. AppDomain.CurrentDomain.SetupInformation.ConfigurationFile)</param>
        /// <returns></returns>
        public static AppDomain CreateAppDomain(string domainName, string baseDirectory, string subDirectory, string configFile) {
            AppDomainSetup appSetup = new AppDomainSetup()
            {
                ApplicationName = domainName,
                ApplicationBase = baseDirectory,
                PrivateBinPath = subDirectory,
                ConfigurationFile = configFile
                //ApplicationBase = AppDomain.CurrentDomain.BaseDirectory,
                //PrivateBinPath = @"Plugins",
                //ConfigurationFile = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile
            };

            AppDomain appDomain = AppDomain.CreateDomain(domainName, null, appSetup);
            return appDomain;
        }
        #endregion

        #region " RunPlugin "

        /// <summary>
        /// Execute a class method in another domain using a PluginRunner.
        /// </summary>
        /// <param name="runner">The PluginRunner MarshalObject used to execute the method.</param>
        /// <param name="plugin">The Plugin object.</param>
        /// <param name="classPath">The namespace path to the target class.</param>
        /// <param name="methodName">The function name in the target class.</param>
        /// <param name="args">An object array of arguments to pass to the function call.</param>
        /// <returns></returns>
        public static object RunMethodObject(PluginRunner runner, Plugin plugin, string classPath, string methodName, object[] args) {
            object result = null;

            if(runner != null) {
                try {
                    result = runner.Run(plugin.ClassNamespacePath, methodName, args);
                } catch(Exception ex) {
                    System.Console.WriteLine("RUN ERROR: " + ex.Message);
                    System.Console.WriteLine("RUN ERROR: " + ex.InnerException.Message);
                }
            }

            return result;
        }

        /// <summary>
        /// Execute a class method in another domain using a PluginRunner. Return a string value.
        /// </summary>
        /// <param name="runner">The PluginRunner MarshalObject used to execute the method.</param>
        /// <param name="plugin">The Plugin object.</param>
        /// <param name="classPath">The namespace path to the target class.</param>
        /// <param name="methodName">The function name in the target class.</param>
        /// <param name="args">An object array of arguments to pass to the function call.</param>
        /// <returns>Return a string value.</returns>
        public static string RunMethodString(PluginRunner runner, Plugin plugin, string classPath, string methodName, object[] args) {
            object res = RunMethodObject(runner, plugin, classPath, methodName, args);
            string sRes = "NULL";
            if(res!=null)
                sRes = res.ToString();

            return sRes;
        }

        #endregion

        #region " CreatePlugin "

        public static Plugin CreatePlugin(string pluginFileName, string coreBinDir, string pluginDir, string[] code, string codeNamespacePath, string[] coreDllRefs, string[] pluginDllRefs)
        {
            return CreatePlugin(pluginFileName, coreBinDir, pluginDir, code, codeNamespacePath, coreDllRefs, pluginDllRefs, 0);
        }

        public static Plugin CreatePlugin(string pluginFileName, string coreBinDir, string pluginDir, string[] code, string codeNamespacePath, string[] coreDllRefs, string[] pluginDllRefs, Int32 compileOrder)
        {
            List<string> referencedAssemblySet = new List<string>();
            //string projectRoot = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"/";   // path to bin

            if(coreDllRefs != null) {
                foreach(string reference in coreDllRefs) {
                    referencedAssemblySet.Add(coreBinDir + @"/" + reference);
                }
            }

            if(pluginDllRefs != null) {
                foreach(string reference in pluginDllRefs) { 
                    referencedAssemblySet.Add(pluginDir + @"/" + reference);   //  only needed if we move it out of the expected location .... which isn't working yet
                }
            }

            System.IO.Directory.CreateDirectory(pluginDir);
            Plugin oPlugin = new Plugin(pluginFileName, coreBinDir, pluginDir, referencedAssemblySet, codeNamespacePath, code, compileOrder);
            return oPlugin;
        }

        #endregion

        #region " CompilePlugin "

        /// <summary>
        /// Compile a plugins DLL file using a temporary AppDomain and PluginRunner.
        /// </summary>
        /// <param name="plugin">The plugin to compile.</param>
        /// <param name="tempDomainName">The name of the temporary AppDomain.</param>
        /// <param name="compilerRunnerNamespace">The namespace path to the PluginRunner.</param>
        /// <param name="removeExistingFirst">If set to true, then delete the existing DLL should it already exist.</param>
        /// <returns></returns>
        static public bool CompilePlugin(Plugin plugin, string tempDomainName, string compilerRunnerNamespace, bool removeExistingFirst) {
            
            // Remove any previously generated DLL's
            if(removeExistingFirst && System.IO.File.Exists(plugin.DllFilePath)) {
                System.IO.File.Delete(plugin.DllFilePath);
            }

            // Create the temporary domain
            AppDomain domainTemp = AppDomain.CreateDomain(tempDomainName);

            // Create a PluginRunner instance in the temp domain
            //string mainDllFileName = Assembly.GetExecutingAssembly().GetName(false).Name + ".dll"; // "saas_plugins.dll"
            //PluginRunner runner = (PluginRunner)domainTemp.CreateInstanceFromAndUnwrap(mainDllFileName, compilerRunnerNamespace);

            // More explicit DLL path for Unit Testing
            string mainDllFileName = Assembly.GetExecutingAssembly().GetName(false).Name + ".dll"; // "saas_plugins.dll"
            string dllRoot = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"/";   // path to bin
            string dllPath = dllRoot + mainDllFileName;
            PluginRunner runner = (PluginRunner)domainTemp.CreateInstanceFromAndUnwrap(dllPath, compilerRunnerNamespace);

            // Compile the file within the temp domain
            bool res = runner.CompileToFile(plugin.Code, plugin.DllFilePath, plugin.DllFileNameReferenceSet);
            plugin.IsCompiled = res;

            // Discard the temp domain
            AppDomain.Unload(domainTemp);
            domainTemp = null;
            
            return res;
        }

        #endregion

        #region " LoadPlugin "

        /// <summary>
        /// Load a plugin into an AppDomain by creating and returing a PluginRunner instance.
        /// </summary>
        /// <param name="plugin">The plugin to load.</param>
        /// <param name="domain">The AppDomain to load the plugin into.</param>
        /// <returns>A PluginRunner instance.</returns>
        public static PluginRunner LoadPlugin(Plugin plugin, AppDomain domain)
        {
            // Load into the Plugin domain
            PluginRunner loader = null;

            if(domain != null) {
                try {
                    loader = (PluginRunner)domain.CreateInstanceAndUnwrap(typeof(PluginRunner).Assembly.FullName, typeof(PluginRunner).FullName);
                    //loader = (PluginRunner)domain.CreateInstanceFromAndUnwrap(typeof(PluginRunner).Assembly.FullName, typeof(PluginRunner).FullName);

                    loader.Load(plugin.DllFilePath);

                } catch (Exception ex) {
                    System.Console.WriteLine(ex.Message);
                }
            }

            return loader;
        }

        #endregion

    }


}
