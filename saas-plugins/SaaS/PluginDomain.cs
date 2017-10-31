/****************************** PluginDomain ******************************\
This class provides the entity model of a "PluginDomain". A PluginDomain
is used to help manage AppDomains and the plugins loaded into them.

Copyright (c) Aaron Ulrich
This source is subject to the Apache License Version 2.0, January 2004
See http://www.apache.org/licenses/.
All other rights reserved.

THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

using System;
using System.Collections.Generic;

namespace saas_plugins.SaaS
{
    public class PluginDomain : IDisposable
    {
        AppDomain _domain = null;
        AppDomain _domainTemp = null;
        private string _domainBasePath = "";
        private string _domainSubPath = "";
        private string _compilerRunnerNamespace = "";
        private string _instanceDomain = "";
        private string _instanceDomainTemp = "";

        Dictionary<string, PluginReference> _pluginReferences = null;
        

        public PluginDomain(string instanceDomainName, string domainBasePath, string domainSubPath, string compilerRunnerNamespace) {
            this._instanceDomain = instanceDomainName;
            this._instanceDomainTemp = instanceDomainName + "TEMP";
            this._compilerRunnerNamespace = compilerRunnerNamespace;
            this._domainBasePath = domainBasePath;
            this._domainSubPath = domainSubPath;

            this.ResetDomain();
            this._pluginReferences = new Dictionary<string, PluginReference>();
        }

        /// <summary>
        /// Dispose with unload the running domain.
        /// </summary>
        public void Dispose() {
            UnloadDomain();
        }

        #region " Properties "

        /// <summary>
        /// The name of this domain instance.
        /// </summary>
        public string InstanceDomainName {
            get { return this._instanceDomain; }
        }

        /// <summary>
        /// A set of PluginReference's that have been loaded into this domain.
        /// </summary>
        public Dictionary<string, PluginReference> PluginReferenceSet {
            get { return this._pluginReferences; }
        }

        #endregion

        /// <summary>
        /// Unload the running domain and clear all PluginReferences.
        /// </summary>
        public void UnloadDomain() {
            if(this._domain!=null) {
                AppDomain.Unload(this._domain);
                this._domain = null;
            }

            if(this._domainTemp!=null) {
                AppDomain.Unload(this._domainTemp);
                this._domainTemp = null;
            }
            this._pluginReferences.Clear();
            this._pluginReferences = null;
            this._pluginReferences = new Dictionary<string, PluginReference>();
        }

        /// <summary>
        /// Unload the running domain, and re-initialize the domain as empty -this will allow for a recompile of any linked DLL's
        /// </summary>
        public void ResetDomain() {
            if(this._domain!=null) {
                AppDomain.Unload(this._domain);
                foreach(string key in this._pluginReferences.Keys) {
                    this._pluginReferences[key].PluginRunner = null;
                    this._pluginReferences[key].IsLoaded = false;
                }
            }
            CreateDomain();
        }

        /// <summary>
        /// Load all unloaded PluginReferences back into the AppDomain.
        /// </summary>
        public void ReloadDomain() {
            foreach(PluginReference oRef in _pluginReferences.Values) {
                if(oRef.PluginRunner == null) {
                    LoadPlugin(oRef.Plugin);
                }
            }
        }


        protected void CreateDomain() {
            this._domain = HelperPlugin.CreateAppDomain(this._instanceDomain, this._domainBasePath, this._domainSubPath, AppDomain.CurrentDomain.SetupInformation.ConfigurationFile);
        }

        /// <summary>
        /// Check if this PluginID has been loaded into this AppDomain.
        /// </summary>
        /// <param name="pluginID"></param>
        /// <returns></returns>
        public bool HasPlugin(string pluginID) {
            return this._pluginReferences.ContainsKey(pluginID);
        }

        /// <summary>
        /// Check if any of these PluginID's have been loaded into this AppDomain.
        /// </summary>
        /// <param name="pluginSet"></param>
        /// <returns></returns>
        public bool HasPlugin(List<string> pluginSet) {
            bool hasPlugin = false;
            foreach(string pluginID in pluginSet) {
                hasPlugin = HasPlugin(pluginID);
                if(hasPlugin)
                    break;
            }
            return hasPlugin;
        }


        /// <summary>
        /// Load the plugin into this AppDomain.
        /// </summary>
        /// <param name="plugin">The plugin to load.</param>
        /// <returns>Returns a PluginReference.</returns>
        public PluginReference LoadPlugin(Plugin plugin) {
            // Load into the Plugin domain
            PluginReference pluginReference = null;

            if(this._domain == null)
                CreateDomain();

            PluginRunner loader = HelperPlugin.LoadPlugin(plugin, this._domain);

            if(loader != null) {
                // Build/Update the reference
                if(this._pluginReferences.ContainsKey(plugin.PluginID)) {
                    pluginReference = this._pluginReferences[plugin.PluginID];
                } else {
                    pluginReference = new PluginReference(this, plugin);
                    this._pluginReferences.Add(plugin.PluginID, pluginReference);
                }
                pluginReference.PluginRunner = loader;
                pluginReference.IsLoaded = true;
            }

            return pluginReference;
        }

        /// <summary>
        /// Compile a plugin using this domains temporary AppDomain.
        /// </summary>
        /// <param name="plugin">The plugin to compile.</param>
        /// <returns></returns>
        public bool CompilePlugin(Plugin plugin) {            
            return HelperPlugin.CompilePlugin(plugin, this._instanceDomainTemp, this._compilerRunnerNamespace, true);            
        }

        /// <summary>
        /// Run a plugins target method.
        /// </summary>
        /// <param name="plugin">The plugin which encapsulates this method.</param>
        /// <param name="classNamespacePath">The namespace path to the class.</param>
        /// <param name="functionName">The function name to call.</param>
        /// <param name="functionArgs">The arguments to pass to the function.</param>
        /// <returns>Returns an object from the function call.</returns>
        public object RunPlugin(Plugin plugin, string classNamespacePath, string functionName, object[] functionArgs) {
            object result = null;
            if(!this._pluginReferences.ContainsKey(plugin.PluginID)) {
                System.Console.WriteLine("Plugin Not Found: " + plugin.DllFilePath);
            } else {
                System.Console.WriteLine("Plugin Function Called: " + plugin.PluginID);
                PluginRunner cr = this._pluginReferences[plugin.PluginID].PluginRunner;
                if(cr != null) {
                    try {
                        result = cr.Run(plugin.ClassNamespacePath, functionName, functionArgs);
                    } catch(Exception ex) {
                        System.Console.WriteLine("RUN ERROR: " + ex.Message);
                        System.Console.WriteLine("RUN ERROR: " + ex.InnerException.Message);
                    }
                }
            }
            return result;
        }
    }
}
