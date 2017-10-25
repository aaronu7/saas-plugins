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

        public void OutputAssemblies() {
            if(this.PluginRunner!=null) {
                try {
                    this.PluginRunner.OutputAssemblies();
                } catch(Exception ex) {
                    System.Console.WriteLine(ex.Message);
                }
            }
        }

        #region " Properties "

        public bool IsLoaded {
            get {return this._isLoaded; }
            set {this._isLoaded = value; }
        }

        public PluginDomain PluginDomain {
            get {return this._pluginDomain; }
            set {this._pluginDomain = value; }
        }

        public PluginRunner PluginRunner {
            get {return this._pluginRunner; }
            set {this._pluginRunner = value; }
        }
        public Plugin Plugin {
            get {return this._plugin; }
        }

        #endregion
    }
}
