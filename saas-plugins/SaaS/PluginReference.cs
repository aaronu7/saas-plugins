/****************************** PluginReference ******************************\
This class provides the entity model of a "PluginReference". A PluginReference
represents an instance of a Plugin that has been loaded into a PluginDomain.

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

        public override string ToString()
        {
            return this._plugin.ToString();
        }

        /// <summary>
        /// The domain name that this reference exists in.
        /// </summary>
        public string DomainName
        {
            get {return this._pluginDomain.InstanceDomainName; }
        }


        #region " Reflection "

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<string> GetTypeMethodParams(string typeName, string methodName) {
            List<string> set = null;
            if(this.PluginRunner!=null) {
                try {
                    set = this.PluginRunner.GetTypeMethodParams(typeName, methodName);
                } catch(Exception ex) {
                    System.Console.WriteLine(ex.Message);
                }
            }
            return set;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<string> GetTypeMethods(string typeName) {
            List<string> set = null;
            if(this.PluginRunner!=null) {
                try {
                    set = this.PluginRunner.GetTypeMethods(typeName);
                } catch(Exception ex) {
                    System.Console.WriteLine(ex.Message);
                }
            }
            return set;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<string> GetTypes() {
            List<string> typeSet = null;
            if(this.PluginRunner!=null) {
                try {
                    typeSet = this.PluginRunner.GetTypes();
                } catch(Exception ex) {
                    System.Console.WriteLine(ex.Message);
                }
            }
            return typeSet;
        }

        /// <summary>
        /// A list of assemblies running in this AppDomain.
        /// </summary>
        /// <returns></returns>
        public List<string> GetDomainAssemblies() {
            List<string> asmSet = null;
            if(this.PluginRunner!=null) {
                try {
                    asmSet = this.PluginRunner.GetDomainAssemblies();
                } catch(Exception ex) {
                    System.Console.WriteLine(ex.Message);
                }
            }
            return asmSet;
        }

        #endregion

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
