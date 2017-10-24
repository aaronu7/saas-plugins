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

        /// <summary>
        /// Recompile and add the plugin to the default domain
        /// </summary>
        /// <param name="plugin"></param>
        /// <param name="domainName">The domain name to add this plugin to.</param>
        /// <returns></returns>
        public PluginReference PluginAdd(Plugin plugin, string domainName) {

            PluginReference pluginReference = null;

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
                pluginDomain.CompilePlugin(plugin);
                plugin.IsCompiled = true;
                OnLogNotify("Plugin Compiled: " + plugin.PluginID);
            }

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
                foreach(PluginReference pluginRef in this._pluginDomainReferences[plugin.PluginID]) {
                    if(pluginRef.Plugin.PluginID == plugin.PluginID) {
                        // Already exists -break and don't create a new one
                        pluginReference = pluginRef;
                        break;
                    }
                }
                if(pluginReference==null) {
                    this._pluginDomainReferences[plugin.PluginID].Add(pluginReference);
                    OnLogNotify("Reference Add to existing set: " + domainName + "." + plugin.PluginID);
                } else {
                    OnLogNotify("Reference already exists: " + domainName + "." + plugin.PluginID);
                }
            }


            // Reload The rest of THIS domain


            // Reload all other affected domains


                        

            /*
            // Reload all references that exist IN THIS domain (except the one we are going to compile)
            foreach(string key in pluginDomain.PluginReferenceSet.Keys) {
                if(key != plugin.PluginID) {
                    PluginReference plugRef = pluginDomain.LoadPlugin(pluginDomain.PluginReferenceSet[key].Plugin);
                    OnLogNotify("---- Reloading: " + pluginDomain.PluginReferenceSet[key].Plugin.PluginID);
                    pluginDomain.OutputAssemblies(plugRef);
                }
            }
            */



            // Load this plugin into THIS domain
            //pluginReference = pluginDomain.LoadPlugin(plugin);
            //OnLogNotify("Plugin loaded into domain: " + pluginReference.PluginDomain.InstanceDomainName + "." + plugin.PluginID);

            //pluginDomain.OutputAssemblies(pluginReference);



            /*
            //--------------------------------
            // Compile Plugin if not already compiled
            if(!plugin.IsCompiled) {

                
           
                

                // Reset this domain
                pluginDomain.ResetDomain();

                // Reload all references that exist IN THIS domain (except the one we are going to compile)
                foreach(string key in pluginDomain.PluginReferenceSet.Keys) {
                    if(key != plugin.PluginID) {
                        pluginDomain.LoadPlugin(pluginDomain.PluginReferenceSet[key].Plugin);
                        OnLogNotify("---- Reloading: " + pluginDomain.PluginReferenceSet[key].Plugin.PluginID);
                    }
                }

                // Recompile this plugin (uses a temporary domain)
                pluginDomain.CompilePlugin(plugin);
                plugin.IsCompiled = true;
                OnLogNotify("Plugin Compiled: " + plugin.PluginID);

                // Load this plugin into THIS domain
                pluginReference = pluginDomain.LoadPlugin(plugin);
                OnLogNotify("Plugin loaded into domain: " + pluginReference.PluginDomain.InstanceDomainName + "." + plugin.PluginID);

                // reload all domains referencing this plugin

            }
        */




            /*
            //--------------------------------
            // Add a new reference to this plugin (if not already present)
            //      also get the reference object to return
            PluginReference pluginReference = null;
            if(!_pluginDomainReferences.ContainsKey(plugin.PluginID)) {
                // Start a new reference set
                List<PluginReference> pluginReferenceSet = new List<PluginReference>();
                pluginReference = new PluginReference(pluginDomain, plugin);
                pluginReferenceSet.Add(pluginReference);
                this._pluginDomainReferences.Add(plugin.PluginID, pluginReferenceSet);
                OnLogNotify("ReferenceSet Started: " + domainName + "." + plugin.PluginID);
            } else {
                // reference set exists .... search for this exact reference
                pluginReference = null;
                foreach(PluginReference pluginRef in this._pluginDomainReferences[plugin.PluginID]) {
                    if(pluginRef.Plugin.PluginID == plugin.PluginID) {
                        // Already exists -break and don't create a new one
                        pluginReference = pluginRef;
                        break;
                    }
                }
                if(pluginReference==null) {
                    pluginReference = new PluginReference(pluginDomain, plugin);
                    this._pluginDomainReferences[plugin.PluginID].Add(pluginReference);
                    OnLogNotify("Reference Add to existing set: " + domainName + "." + plugin.PluginID);
                } else {
                    OnLogNotify("Reference already exists: " + domainName + "." + plugin.PluginID);
                }
            }

            //--------------------------------
            // Reload all domains that reference the Plugin -update the reference with the new PluginRunner
            foreach(PluginReference pluginRef in this._pluginDomainReferences[plugin.PluginID]) {
                PluginRunner runner = pluginRef.PluginDomain.LoadPlugin(plugin);
                pluginRef.PluginRunner = runner;
                OnLogNotify("Plugin loaded into domain: " + pluginRef.PluginDomain.InstanceDomainName + "." + plugin.PluginID);
            }
            */

            return pluginReference;
        }
        
    }
}
