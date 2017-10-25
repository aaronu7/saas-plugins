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

        private Dictionary<string, PluginDomain> _pluginDomainSet = null;               // domainName -> PluginDomain
        private Dictionary<string, List<PluginReference>> _pluginDomainReferences = null;  // pluginID   -> List<PluginDomain>


        /// <summary>
        /// 
        /// </summary>
        /// <param name="defaultDomainName">ex. MyPluginDomain</param>
        /// <param name="pluginRunnerTypePath">ex. saas_plugins.SaaS.PluginRunner</param>
        public PluginSystem(string defaultDomainName, string pluginRunnerTypePath) {
            this._defaultDomainName = defaultDomainName;
            this._defaultDomainNameTemp = this._defaultDomainName + "TEMP";
            this._pluginRunnerTypePath = pluginRunnerTypePath;
            //_pluginLookup = new Dictionary<string, Dictionary<string, Plugin>>();
            _pluginDomainSet = new Dictionary<string, PluginDomain>();
            _pluginDomainReferences = new Dictionary<string, List<PluginReference>>();
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


        /*
        public PluginReference PluginCompile(Plugin plugin) {
            // Recompile this plugin (uses a temporary domain)
            pluginDomain.CompilePlugin(plugin);
            plugin.IsCompiled = true;
            OnLogNotify("Plugin Compiled: " + plugin.PluginID);
        }
        */

        /// <summary>
        /// Recompile and add the plugin to the default domain
        /// </summary>
        /// <param name="plugin"></param>
        /// <param name="domainName">The domain name to add this plugin to.</param>
        /// <returns></returns>
        public PluginReference PluginAdd(Plugin plugin, string domainName) {

            PluginReference pluginReference = null;
            bool compileTriggered = false;

            if(domainName=="")
                domainName = this._defaultDomainName;

            //--------------------------------
            // Create/Get the PluginDomain
            PluginDomain pluginDomain = null;
            if(_pluginDomainSet.ContainsKey(domainName)) {
                pluginDomain = _pluginDomainSet[domainName];

            } else {
                pluginDomain = new PluginDomain(domainName, this._pluginRunnerTypePath);
                _pluginDomainSet.Add(domainName, pluginDomain);
                OnLogNotify("Domain Created: " + domainName);
            }

            string refMatchKey = pluginDomain.InstanceDomainName + "." + plugin.PluginID;

            //--------------------------------
            // Compile Plugin if not already compiled
            if(!plugin.IsCompiled) {
                
                //--------------------------------
                // Reset Domain
                //      ONLY RESET THE DOMAIN IF IT HAS AN ACTIVE REFERENCE TO THIS PLUGIN
                /*
                if(pluginDomain.PluginReferenceSet.ContainsKey(plugin.PluginID)) {
                    PluginReference domRef = pluginDomain.PluginReferenceSet[plugin.PluginID];
                    if(domRef.PluginRunner != null) {
                        pluginDomain.ResetDomain();
                        OnLogNotify("Domain Reset: " + domainName);
                    }
                }
                */

                // Reset all domains that have an ACTIVE reference to this plugin
                if(_pluginDomainReferences.ContainsKey(plugin.PluginID)) {
                    foreach(PluginReference pluginRef in this._pluginDomainReferences[plugin.PluginID]) {
                        if(pluginRef.PluginRunner != null) {
                            pluginRef.PluginDomain.ResetDomain();
                            OnLogNotify("Domain Reset: " + pluginRef.PluginDomain.InstanceDomainName);
                        }
                    }
                }     

                // Recompile this plugin (uses a temporary domain)
                plugin.IsCompiled = pluginDomain.CompilePlugin(plugin);
                if(plugin.IsCompiled) {
                    compileTriggered = true;
                    OnLogNotify("Plugin Compiled: " + plugin.PluginID);
                } else {
                    compileTriggered = false;
                    OnLogNotify("Plugin FAILED to compile: " + plugin.PluginID);
                }
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
        
    }
}
