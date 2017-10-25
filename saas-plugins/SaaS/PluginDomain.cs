using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace saas_plugins.SaaS
{
    public class PluginDomain : IDisposable
    {
        AppDomain _domain = null;
        AppDomain _domainTemp = null;
        private string _compilerRunnerNamespace = "";
        private string _instanceDomain = "";
        private string _instanceDomainTemp = "";

        //Dictionary<string, PluginRunner> _runnerSet = null;
        Dictionary<string, PluginReference> _pluginReferences = null;
        

        public PluginDomain(string instanceDomainName, string compilerRunnerNamespace) {
            this._instanceDomain = instanceDomainName;
            this._instanceDomainTemp = instanceDomainName + "TEMP";
            this._compilerRunnerNamespace = compilerRunnerNamespace;

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
            this._domain = AppDomain.CreateDomain(this._instanceDomain);
        }


        public void OutputAssemblies(PluginReference oPluginRef) {
            if(oPluginRef!=null && oPluginRef.PluginRunner!=null) {
                try {
                    oPluginRef.PluginRunner.OutputAssemblies();
                } catch(Exception ex) {
                    System.Console.WriteLine(ex.Message);
                }
            }
        }

        public PluginReference LoadPlugin(Plugin plugin) {
            // Load into the Plugin domain
            string dllFilePath = plugin.DllFileDir + plugin.DllFileName;
            PluginRunner loader = null;
            PluginReference pluginReference = null;

            if(this._domain == null)
                this._domain = AppDomain.CreateDomain(this._instanceDomain);

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
            
            if(pluginReference.PluginRunner == null)
            {
                string a = "";
            }
            return pluginReference;
        }

        public bool CompilePlugin(Plugin plugin) {
            return CompilePlugin(plugin, true);
        }

        public bool CompilePlugin(Plugin plugin, bool unloadTempDomain) {
            
            // Reset the domain if we are trying to compile an RUNNING plugin  
            //      This will only trigger on the first in a compile sequence (clean slate after first)
            //      This will release any holds on the DLL
            //if(this._pluginReferences.ContainsKey(plugin.PluginID)) {
            //    this.ResetDomain();
            //    System.Console.WriteLine("Domain Reseting");
            //}


            // Compile the DLL  -will destroy its temporary app domain
            //HelperPlugin.CompileDLL(oPlugin, "saas_plugins.dll", "tmpCompileDomain", this._compilerRunnerNamespace);

            string mainDllFileName = Assembly.GetExecutingAssembly().GetName(false).Name + ".dll"; // "saas_plugins.dll"

            // Create a temp domain and create our plugin runner
            if(this._domainTemp==null)
                this._domainTemp = AppDomain.CreateDomain(this._instanceDomainTemp);

            PluginRunner cr = (PluginRunner)this._domainTemp.CreateInstanceFromAndUnwrap(mainDllFileName, this._compilerRunnerNamespace);

            // Compile the file within the temp domain
            string dllFilePath = plugin.DllFileDir + plugin.DllFileName;
            bool res = cr.CompileToFile(plugin.Code, dllFilePath, plugin.DllFileNameReferenceSet);

            // Discard the temp domain
            if(unloadTempDomain) {
                AppDomain.Unload(this._domainTemp);
                this._domainTemp = null;
            }
            
            return res;
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
