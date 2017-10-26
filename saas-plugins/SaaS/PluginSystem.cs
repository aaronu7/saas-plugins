using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public void OutputAssemblies() {
            foreach(string key in _pluginDomainSet.Keys) {
                foreach(string keyRef in this._pluginDomainSet[key].PluginReferenceSet.Keys) {
                    PluginReference pluginReference = this._pluginDomainSet[key].PluginReferenceSet[keyRef];
                    pluginReference.OutputAssemblies();
                    break;
                }
            }
        }

        #region " PluginSystemLoad "

        public void PluginSystemLoad(List<Plugin> pluginSet) {
            foreach(Plugin plugin in pluginSet) {
                PluginSystemLoad(plugin);
            }
        }

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

        protected void ResetReferencingDomains(Plugin plugin, PluginDomain pluginDomain, bool ignoreTargetDomain) {
            foreach(PluginDomain dom in this._pluginDomainSet.Values) {
                if(dom.PluginReferenceSet.ContainsKey(plugin.PluginID)) {
                    if(ignoreTargetDomain && pluginDomain.InstanceDomainName == dom.InstanceDomainName) {
                        // ignore this domain reload ... no need, the plugin did not already exist
                    } else {
                        dom.ResetDomain();
                        OnLogNotify("Domain Reset: " + dom.InstanceDomainName);
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

        protected bool CompilePlugin(PluginDomain pluginDomain, Plugin plugin) {
            if(!pluginDomain.CompilePlugin(plugin)) {
                OnLogNotify("Plugin FAILED to compile: " + plugin.PluginID);
            }
            return plugin.IsCompiled;
        }

        #endregion


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
                    if(pluginDomain.HasPlugin(plugin)) {
                        OnLogNotify("Plugin already loaded into domain: " + pluginDomain.InstanceDomainName + "." + plugin.PluginID);

                    } else {
                        pluginDomain.LoadPlugin(plugin);      
                        OnLogNotify("Plugin loaded into domain: " + pluginDomain.InstanceDomainName + "." + plugin.PluginID);
                    }
                }
            }
        }

        public void SystemReload(string domainName, List<string> pluginSet) {
            // Unload and then Reload all AppDomains

            // Reset all referencing domains (to allow recompiling)
            //ResetReferencingDomains(plugin, pluginDomain, ignoreTargetDomain);


            // reload all domains that reference this plugin
            //ReloadReferencingDomains(plugin, pluginDomain, ignoreTargetDomain);
        }

        public void SystemUpdate(string domainName, List<string> pluginSet) {
            // Recompile/Load the list of plugins which require a recompile

        }

        /*
        public void PluginRecompile(string pluginID, string domainName) {

            // Compile ALL plugins first for better performance if many changes.
            
            if(!this._pluginSet.ContainsKey(pluginID)) {
                OnLogNotify("Plugin not found in the system: " + pluginID);

            } else {
                // Get the plugin
                Plugin plugin = this._pluginSet[pluginID];

                // Get or create the domain object
                PluginDomain pluginDomain = CreateGetDomain(domainName);

                // Compile Plugin (if not compiled)
                if(plugin.IsCompiled) {
                    // Already compiled, simply Load the plugin
                    pluginDomain.LoadPlugin(plugin);      
                    OnLogNotify("Plugin loaded into domain: " + pluginDomain.InstanceDomainName + "." + plugin.PluginID);

                } else {
                    bool isPreExisting = pluginDomain.HasPlugin(plugin);
                    bool ignoreTargetDomain = !isPreExisting;

                    // Reset all referencing domains (to allow recompiling)
                    ResetReferencingDomains(plugin, pluginDomain, ignoreTargetDomain);

                    // Recompile this plugin (uses a temporary domain)
                    bool compiledOK = CompilePlugin(pluginDomain, plugin);

                    // Load the plugin (if not pre-existing, otherwise it triggers in the reload)
                    if(!isPreExisting) {
                        pluginDomain.LoadPlugin(plugin);      
                        OnLogNotify("Plugin compiled and loaded into domain: " + pluginDomain.InstanceDomainName + "." + plugin.PluginID);
                    }

                    // reload all domains that reference this plugin
                    ReloadReferencingDomains(plugin, pluginDomain, ignoreTargetDomain);
                }
            }

        }
        */

        #region " OLD PluginAdd "
        /*
        public PluginReference PluginAdd(Plugin plugin, string domainName) {

            PluginReference pluginReference = null;
            bool compileTriggered = false;


            PluginDomain pluginDomain = CreateGetDomain(domainName);

            string refMatchKey = pluginDomain.InstanceDomainName + "." + plugin.PluginID;

            //--------------------------------
            // Compile Plugin if not already compiled
            if(!plugin.IsCompiled) {
                
                // Reset all referencing domains (to allow recompiling)
                ResetReferencingDomains(plugin);

                // Recompile this plugin (uses a temporary domain)
                compileTriggered = CompilePlugin(pluginDomain, plugin);
            }

            if(plugin.IsCompiled) {
                //--------------------------------
                // Load this plugin into THIS domain (This will create the reference if it DNE)
                pluginReference = pluginDomain.LoadPlugin(plugin);      
                OnLogNotify("Plugin loaded into domain: " + pluginReference.PluginDomain.InstanceDomainName + "." + plugin.PluginID);
            
                //--------------------------------
                // Add this reference to our domain web
                if(!_pluginDomainReferences.ContainsKey(plugin.PluginID)) {
                    // Start a new reference set
                    List<PluginReference> pluginReferenceSet = new List<PluginReference>();
                    pluginReferenceSet.Add(pluginReference);
                    this._pluginDomainReferences.Add(plugin.PluginID, pluginReferenceSet);
                    OnLogNotify("ReferenceSet Started: " + domainName + "." + plugin.PluginID);
                } else {
                    // reference set exists .... search for this exact reference
                    List<PluginReference> RefSet = this._pluginDomainReferences[plugin.PluginID];
                    PluginReference pluginFound = null;

                    foreach(PluginReference pluginRef in RefSet) {
                        string refKey     = pluginRef.PluginDomain.InstanceDomainName + "." + pluginRef.Plugin.PluginID;
                    
                        if(refMatchKey == refKey) {
                            // Already exists -break and don't create a new one
                            pluginFound = pluginRef;
                            break;
                        }
                    }
                    if(pluginFound==null) {
                        RefSet.Add(pluginReference);
                        OnLogNotify("Reference Add to existing set: " + domainName + "." + plugin.PluginID);
                    } else {
                        OnLogNotify("Reference already exists: " + domainName + "." + plugin.PluginID);             // issues with multiple domains
                    }
                }

                if(compileTriggered) {
                    // SKIP THE FOLLOWING ON A FULL REBUILD

                    // Reload all references that exist IN THIS domain (except the one we are going to compile)
                    foreach(string key in pluginDomain.PluginReferenceSet.Keys) {
                        if(key != plugin.PluginID) {
                            PluginReference plugRef = pluginDomain.LoadPlugin(pluginDomain.PluginReferenceSet[key].Plugin);
                            OnLogNotify("---- Reloading: " + domainName + "." + pluginDomain.PluginReferenceSet[key].Plugin.PluginID);
                        }
                    }

                    // Reload all OTHER affected domains
                    List<PluginReference> domRefSet = this._pluginDomainReferences[plugin.PluginID];
                    foreach(PluginReference pluginRef in domRefSet) {
                        string refKey  = pluginRef.PluginDomain.InstanceDomainName + "." + pluginRef.Plugin.PluginID;

                        if(refMatchKey != refKey) {
                            // Reload this domain
                            OnLogNotify("Trigger domain reload: " + pluginRef.PluginDomain.InstanceDomainName);
                        }
                    }
                }
            }

            return pluginReference;
        }
        */
        #endregion

    }
}
