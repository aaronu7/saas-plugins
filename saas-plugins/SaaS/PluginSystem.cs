/****************************** PluginSystem ******************************\
This class provides the entity model of a "PluginSystem". A PluginSystem
is used to manage plugins that have been loaded in multiple domains. Using
the PluginSystem will allow the runtime recompiles of plugins spanning 
multiple domains.

Copyright (c) Aaron Ulrich
This source is subject to the Apache License Version 2.0, January 2004
See http://www.apache.org/licenses/.
All other rights reserved.

THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

using System.Collections.Generic;

namespace saas_plugins.SaaS
{
    public class PluginSystem
    {
        private string _defaultDomainNameTemp = "";
        private string _defaultDomainName = "";
        private string _pluginRunnerTypePath = "";
        private string _domainBasePath = "";
        private string _domainSubPath = "";

        private PluginDomain _defaultDomain = null;
        private Dictionary<string, PluginDomain> _pluginDomainSet = null;         // domainName -> PluginDomain
        private Dictionary<string, Plugin> _pluginSet = null;               // pluginID -> Plugin

        //private Dictionary<string, List<PluginReference>> _pluginDomainReferences = null;  // pluginID   -> List<PluginDomain>


        /// <summary>
        /// 
        /// </summary>
        /// <param name="defaultDomainName">ex. MyPluginDomain</param>
        /// <param name="pluginRunnerTypePath">ex. saas_plugins.SaaS.PluginRunner</param>
        public PluginSystem(string defaultDomainName, string domainBasePath, string domainSubPath, string pluginRunnerTypePath) {
            this._defaultDomainName = defaultDomainName;
            this._defaultDomainNameTemp = this._defaultDomainName + "TEMP";
            this._domainBasePath = domainBasePath;
            this._domainSubPath = domainSubPath;

            this._pluginRunnerTypePath = pluginRunnerTypePath;
            //_pluginLookup = new Dictionary<string, Dictionary<string, Plugin>>();
            _pluginDomainSet = new Dictionary<string, PluginDomain>();
            _pluginSet = new Dictionary<string, Plugin>();
            //_pluginDomainReferences = new Dictionary<string, List<PluginReference>>();

            // Get or create the domain object
            this._defaultDomain = CreateGetDomain(this._defaultDomainName);
        }


        #region " Events "

        public delegate void EventLogNotify(string message);
        public event EventLogNotify LogNotify;
        public void OnLogNotify(string message) {
            if(LogNotify != null)
                LogNotify(message);
        }

        #endregion

        /// <summary>
        /// Invoke a method in a running Plugin.
        /// </summary>
        /// <param name="domainName">The domain to target.</param>
        /// <param name="pluginID">The plugin to target.</param>
        /// <param name="classNamespacePath">The class namespace path.</param>
        /// <param name="functionName">The function name.</param>
        /// <param name="functionArgs">The function arguments.</param>
        /// <returns></returns>
        public object InvokeMethod(string domainName, string pluginID, string classNamespacePath, string functionName, object[] functionArgs) {
            object res = null;

            if(!this._pluginDomainSet.ContainsKey(domainName) || !this._pluginSet.ContainsKey(pluginID)) {
                OnLogNotify("Domain/Plugin not found in the system: " + domainName + "." + pluginID);

            } else {
                // Get the domain/plugin
                PluginDomain domain = this._pluginDomainSet[domainName];
                Plugin plugin = this._pluginSet[pluginID];

                res = domain.RunPlugin(plugin, classNamespacePath, functionName, functionArgs);
            }

            return res;
        }

        #region " Output/Get Assemblies "

        /// <summary>
        /// Output a list of running assemblies for each active domain.
        /// </summary>
        public void OutputAssemblies() {
            foreach(string key in _pluginDomainSet.Keys) {
                foreach(string keyRef in this._pluginDomainSet[key].PluginReferenceSet.Keys) {
                    PluginReference pluginReference = this._pluginDomainSet[key].PluginReferenceSet[keyRef];
                    List<string> asmSet = pluginReference.GetAssemblies();

                    System.Console.WriteLine("===================");
                    System.Console.WriteLine("Domain: " + pluginReference.DomainName);
                    System.Console.WriteLine("===================");
                    foreach(string asm in asmSet) {
                        string[] asmParts = asm.Split(',');
                        System.Console.WriteLine(asmParts[0]);
                    }

                    break;
                }
            }
        }

        /// <summary>
        /// Get a list of running assemblies for each active domain.
        /// </summary>
        /// <returns>A dictionary with domainName keys and items containing lists of running assemblies.</returns>
        public Dictionary<string, List<string>> GetAssemblies() {
            Dictionary<string, List<string>> asmSets = new Dictionary<string, List<string>>();

            foreach(string key in _pluginDomainSet.Keys) {
                foreach(string keyRef in this._pluginDomainSet[key].PluginReferenceSet.Keys) {
                    PluginReference pluginReference = this._pluginDomainSet[key].PluginReferenceSet[keyRef];
                    List<string> asmSetFullNames = pluginReference.GetAssemblies();
                    
                    List<string> asmSet = new List<string>();
                    foreach(string asm in asmSetFullNames) {
                        string[] asmParts = asm.Split(',');
                        asmSet.Add(asmParts[0]);
                    }
                    asmSets.Add(pluginReference.DomainName, asmSet);
                    break;
                }
            }

            return asmSets;
        }

        #endregion

        #region " PluginSystemLoad "

        /// <summary>
        /// Load a set of plugins into the system.
        /// </summary>
        /// <param name="pluginSet"></param>
        public void PluginSystemLoad(List<Plugin> pluginSet) {
            foreach(Plugin plugin in pluginSet) {
                PluginSystemLoad(plugin);
            }
        }

        /// <summary>
        /// Load the given plugin into the system.
        /// </summary>
        /// <param name="plugin"></param>
        public void PluginSystemLoad(Plugin plugin) {

            if(this._pluginSet.ContainsKey(plugin.PluginID)) {
                OnLogNotify("Plugin already exists in the system: " + plugin.PluginID);
            } else {
                if(plugin.IsCompiled) {
                    // Already compiled, simply Load the plugin
                    this._pluginSet.Add(plugin.PluginID, plugin);
                    OnLogNotify("Plugin loaded into system: " + plugin.PluginID);

                } else {

                    // Recompile this plugin (uses a temporary domain)
                    bool compiledOK = CompilePlugin(this._defaultDomain, plugin);

                   if(compiledOK) {
                        this._pluginSet.Add(plugin.PluginID, plugin);
                        OnLogNotify("Plugin compiled and loaded into system: " + plugin.PluginID);
                    } else {
                        OnLogNotify("Plugin failed to compiled: " + plugin.PluginID);
                    }

                }
            }
        }

        #endregion


        #region " Local Helpers "

        protected PluginDomain CreateGetDomain(string domainName)
        {
            if(domainName=="")
                domainName = this._defaultDomainName;

            //--------------------------------
            // Create/Get the PluginDomain
            PluginDomain pluginDomain = null;
            if(_pluginDomainSet.ContainsKey(domainName)) {
                pluginDomain = _pluginDomainSet[domainName];

            } else {
                pluginDomain = new PluginDomain(domainName, this._domainBasePath, this._domainSubPath, this._pluginRunnerTypePath);
                _pluginDomainSet.Add(domainName, pluginDomain);
                OnLogNotify("Domain Created: " + domainName);
            }

            return pluginDomain;
        }

        protected void UnloadReferencingDomains(List<string> pluginSet) {
            foreach(PluginDomain dom in this._pluginDomainSet.Values) {
                if(dom.HasPlugin(pluginSet)) { 
                    dom.ResetDomain();
                    OnLogNotify("Domain Unloaded: " + dom.InstanceDomainName);
                }
            }
        }
        protected void ReloadReferencingDomains(List<string> pluginSet) {
            foreach(PluginDomain dom in this._pluginDomainSet.Values) {
                if(dom.HasPlugin(pluginSet)) { 
                    dom.ReloadDomain();
                    OnLogNotify("Domain Reloaded: " + dom.InstanceDomainName);
                }
            }
        }

        /*
        protected void UnloadReferencingDomains(string[] pluginSet, PluginDomain pluginDomain, bool ignoreTargetDomain) {
            foreach(PluginDomain dom in this._pluginDomainSet.Values) {
                if(dom.HasPlugin(pluginSet)) { 
                    if(ignoreTargetDomain && pluginDomain.InstanceDomainName == dom.InstanceDomainName) {
                        // ignore this domain reload ... no need, the plugin did not already exist
                    } else {
                        dom.ResetDomain();
                        OnLogNotify("Domain Unloaded: " + dom.InstanceDomainName);
                    }
                }
            }
        }

        protected void ReloadReferencingDomains(Plugin plugin, PluginDomain pluginDomain, bool ignoreTargetDomain) {
            foreach(PluginDomain dom in this._pluginDomainSet.Values) {
                if(dom.PluginReferenceSet.ContainsKey(plugin.PluginID)) {
                    if(ignoreTargetDomain && pluginDomain.InstanceDomainName == dom.InstanceDomainName) {
                        // ignore this domain reload ... no need, the plugin did not already exist
                    } else {
                        dom.ReloadDomain();
                        OnLogNotify("Domain Reloaded: " + dom.InstanceDomainName);
                    }
                }
            }
        }
        */
        protected bool CompilePlugin(PluginDomain pluginDomain, Plugin plugin) {
            if(!pluginDomain.CompilePlugin(plugin)) {
                OnLogNotify("Plugin FAILED to compile: " + plugin.PluginID);
            }
            return plugin.IsCompiled;
        }

        #endregion


        /// <summary>
        /// Load a set of plugins into the specified domain. This will create the domain if it does not already exist.
        /// </summary>
        /// <param name="domainName"></param>
        /// <param name="pluginSet"></param>
        public void PluginDomainLoad(string domainName, List<string> pluginSet) {

            // Compile ALL plugins first for better performance if many changes.
            
            foreach(string pluginID in pluginSet) {
                if(!this._pluginSet.ContainsKey(pluginID)) {
                    OnLogNotify("Plugin not found in the system: " + pluginID);

                } else {
                    // Get the plugin
                    Plugin plugin = this._pluginSet[pluginID];

                    // Get or create the domain object
                    PluginDomain pluginDomain = CreateGetDomain(domainName);

                    // Simply Load the plugin
                    if(pluginDomain.HasPlugin(plugin.PluginID)) {
                        OnLogNotify("Plugin already loaded into domain: " + pluginDomain.InstanceDomainName + "." + plugin.PluginID);

                    } else {
                        pluginDomain.LoadPlugin(plugin);      
                        OnLogNotify("Plugin loaded into domain: " + pluginDomain.InstanceDomainName + "." + plugin.PluginID);
                    }
                }
            }
        }

        /// <summary>
        /// Unload all domains and plugins and then reload all domains and plugins.
        /// </summary>
        public void SystemReload() {
            // Unload and then Reload all AppDomains

            // Unload ALL domains
            foreach(PluginDomain dom in this._pluginDomainSet.Values) {
                dom.ResetDomain();
                OnLogNotify("Domain Unloaded: " + dom.InstanceDomainName);
            }

            // Reload ALL domains
            foreach(PluginDomain dom in this._pluginDomainSet.Values) {
                dom.ReloadDomain();
                OnLogNotify("Domain Reloaded: " + dom.InstanceDomainName);
            }
        }

        /// <summary>
        /// Unload all domains referencing any of the specified plugins. Recompile these plugins. Reload all referencing domains.
        /// </summary>
        /// <param name="pluginSet"></param>
        public void SystemUpdate(List<string> pluginSet) {
            // Recompile/Load the list of plugins -- No checks on IsCompiled, it will always attempt a recompile

            UnloadReferencingDomains(pluginSet);

            foreach(string pluginID in pluginSet) {
                Plugin plugin = this._pluginSet[pluginID];

                // Recompile this plugin (uses a temporary domain)
                bool compiledOK = CompilePlugin(this._defaultDomain, plugin);

                if(compiledOK) {
                    OnLogNotify("Plugin recompiled: " + plugin.PluginID);
                } else {
                    OnLogNotify("Plugin failed to recompile: " + plugin.PluginID);
                }
            }

            ReloadReferencingDomains(pluginSet);
        }
    }
}
