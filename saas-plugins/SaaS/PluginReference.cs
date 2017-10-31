using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace saas_plugins.SaaS
{
    public class PluginReference
    {
        private PluginDomain _pluginDomain = null;
        private Plugin _plugin = null;
        private PluginRunner _pluginRunner = null;
        private bool _isLoaded = false;

        public PluginReference(PluginDomain pluginDomain, Plugin plugin) {
            this._pluginDomain = pluginDomain;
            this._pluginRunner = null;
            this._plugin = plugin;
        }
        
        /// <summary>
        /// The domain name that this reference exists in.
        /// </summary>
        public string DomainName
        {
            get {return this._pluginDomain.InstanceDomainName; }
        }

        /// <summary>
        /// A list of assemblies running in this AppDomain.
        /// </summary>
        /// <returns></returns>
        public List<string> GetAssemblies() {
            List<string> asmSet = null;
            if(this.PluginRunner!=null) {
                try {
                    asmSet = this.PluginRunner.GetAssemblies();
                } catch(Exception ex) {
                    System.Console.WriteLine(ex.Message);
                }
            }
            return asmSet;
        }
        
        #region " Properties "

        /// <summary>
        /// Is this reference currently loaded in the AppDomain.
        /// </summary>
        public bool IsLoaded {
            get {return this._isLoaded; }
            set {this._isLoaded = value; }
        }

        /// <summary>
        /// The PluginDomain to which this PluginReference exists.
        /// </summary>
        public PluginDomain PluginDomain {
            get {return this._pluginDomain; }
            set {this._pluginDomain = value; }
        }

        /// <summary>
        /// The PluginRunner linking this reference to the running AppDomain.
        /// </summary>
        public PluginRunner PluginRunner {
            get {return this._pluginRunner; }
            set {this._pluginRunner = value; }
        }

        /// <summary>
        /// The Plugin object.
        /// </summary>
        public Plugin Plugin {
            get {return this._plugin; }
        }

        #endregion
    }
}
