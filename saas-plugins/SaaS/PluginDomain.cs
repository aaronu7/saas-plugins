﻿using System;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
using System.Text;
using System.Threading.Tasks;

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

        //Dictionary<string, PluginRunner> _runnerSet = null;
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

        public void Dispose() {
            UnloadDomain();
        }

        #region " Properties "

        public string InstanceDomainName {
            get { return this._instanceDomain; }
        }

        public Dictionary<string, PluginReference> PluginReferenceSet {
            get { return this._pluginReferences; }
        }

        #endregion

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
        /// Re-Initialize the domain as empty -this will allow for a recompile of any linked DLL's
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

        protected void CreateDomain() {
            //string basePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\Plugins\";   // path to bin
            this._domain = HelperPlugin.CreateAppDomain(this._instanceDomain, this._domainBasePath, this._domainSubPath, AppDomain.CurrentDomain.SetupInformation.ConfigurationFile);
                
            //this._domain = AppDomain.CreateDomain(this._instanceDomain);
            //this._domain.AssemblyResolve += _domain_AssemblyResolve;
        }

        /*
        private Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            foreach (var moduleDir in _moduleDirectories)
            {
                var di = new DirectoryInfo(moduleDir);
                var module = di.GetFiles().FirstOrDefault(i => i.Name == args.Name+".dll");
                if (module != null)
                {
                    return Assembly.LoadFrom(module.FullName);
                }
            }
            return null;
        }
        */

        //AppDomain.CurrentDomain.AssemblyLoad

        public void OutputAssemblies(PluginReference oPluginRef) {
            if(oPluginRef!=null && oPluginRef.PluginRunner!=null) {
                try {
                    oPluginRef.PluginRunner.OutputAssemblies();
                } catch(Exception ex) {
                    System.Console.WriteLine(ex.Message);
                }
            }
        }

        #region " LoadPlugin "

        public PluginReference LoadPlugin(Plugin plugin) {
            // Load into the Plugin domain
            string dllFilePath = plugin.DllFileDir + plugin.DllFileName;
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


        /*
        public PluginReference LoadPlugin(Plugin plugin) {
            // Load into the Plugin domain
            string dllFilePath = plugin.DllFileDir + plugin.DllFileName;
            PluginRunner loader = null;
            PluginReference pluginReference = null;

            if(this._domain == null)
                CreateDomain();

            try {
                loader = (PluginRunner)this._domain.CreateInstanceAndUnwrap(typeof(PluginRunner).Assembly.FullName, typeof(PluginRunner).FullName);
                string asmName = loader.Load(dllFilePath);

                // Build/Update the reference
                if(this._pluginReferences.ContainsKey(plugin.PluginID)) {
                    pluginReference = this._pluginReferences[plugin.PluginID];
                } else {
                    pluginReference = new PluginReference(this, plugin);
                    this._pluginReferences.Add(plugin.PluginID, pluginReference);
                }
                pluginReference.PluginRunner = loader;
                pluginReference.IsLoaded = true;

            } catch (Exception ex) {
                System.Console.WriteLine(ex.Message);
            }

            return pluginReference;
        }
        */

        #endregion


        public bool CompilePlugin(Plugin plugin) {            
            return HelperPlugin.CompilePlugin(plugin, this._instanceDomainTemp, this._compilerRunnerNamespace);            
        }

        public object RunPlugin(Plugin plugin, string classNamespacePath, string functionName, object[] functionArgs) {
            object result = null;
            if(!this._pluginReferences.ContainsKey(plugin.PluginID)) {
                System.Console.WriteLine("Plugin Not Found: " + plugin.DllFileName);
            } else {
                System.Console.WriteLine("Plugin Function Called: " + plugin.DllFileName);
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
