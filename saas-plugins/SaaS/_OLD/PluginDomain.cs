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
        private string _compilerRunnerNamespace = "";
        private string _instanceDomain = "";
        private string _instanceDomainTemp = "";

        Dictionary<string, PluginRunner> _runnerSet = null;

        public PluginDomain(string instanceDomainName, string compilerRunnerNamespace) {
            this._instanceDomain = instanceDomainName;
            this._instanceDomainTemp = instanceDomainName + "TEMP";
            this._compilerRunnerNamespace = compilerRunnerNamespace;

            this.ResetDomain();
            //this._domain = AppDomain.CreateDomain(this._instanceDomain);
            //this._runnerSet = new Dictionary<string, PluginRunner>();
        }

        public void Dispose() {
            AppDomain.Unload(this._domain);
            //this._runnerSet.
        }

        protected void ResetDomain() {
            if(this._domain!=null) {
                AppDomain.Unload(this._domain);
                this._runnerSet = null;
            }
            this._domain = AppDomain.CreateDomain(this._instanceDomain);
            this._runnerSet = new Dictionary<string, PluginRunner>();
        }


        public void OutputAssemblies(Plugin oPlugin) {
            try {
                this._runnerSet[oPlugin.DllFileName].OutputAssemblies();
            } catch(Exception ex) {
                System.Console.WriteLine(ex.Message);
            }
            
        }

        public void LoadPlugin(Plugin oPlugin) {
            // Load into the Plugin domain
            string dllFilePath = oPlugin.DllFileDir + oPlugin.DllFileName;
            PluginRunner loader = (PluginRunner)this._domain.CreateInstanceAndUnwrap(typeof(PluginRunner).Assembly.FullName, typeof(PluginRunner).FullName);
            string asmName = loader.Load(dllFilePath);
            
            this._runnerSet.Add(oPlugin.DllFileName, loader);
            System.Console.WriteLine("Plugin Added: " + oPlugin.DllFileName);
        }

        public void CompilePlugin(Plugin oPlugin) {
            
            // Reset the domain if we are trying to compile an RUNNING plugin  
            if(this._runnerSet.ContainsKey(oPlugin.DllFileName)) {
                this.ResetDomain();
                System.Console.WriteLine("Domain Reseting");
            }


            // Compile the DLL  -will destroy its temporary app domain
            //HelperPlugin.CompileDLL(oPlugin, "saas_plugins.dll", "tmpCompileDomain", this._compilerRunnerNamespace);

            string mainDllFileName = Assembly.GetExecutingAssembly().GetName(false).Name + ".dll"; // "saas_plugins.dll"

            // Create a temp domain and create our plugin runner
            AppDomain domain = AppDomain.CreateDomain(this._instanceDomainTemp);
            PluginRunner cr = (PluginRunner)domain.CreateInstanceFromAndUnwrap(mainDllFileName, this._compilerRunnerNamespace);

            // Compile the file within the temp domain
            string dllFilePath = oPlugin.DllFileDir + oPlugin.DllFileName;
            bool res = cr.CompileToFile(oPlugin.Code, dllFilePath, oPlugin.DllFileNameReferenceSet);

            // Discard the temp domain
            AppDomain.Unload(domain);
        }

        public object RunPlugin(Plugin oPlugin, string classNamespacePath, string functionName, object[] functionArgs) {
            object result = null;
            if(!this._runnerSet.ContainsKey(oPlugin.DllFileName)) {
                System.Console.WriteLine("Plugin Not Found: " + oPlugin.DllFileName);
            } else {
                System.Console.WriteLine("Plugin Function Called: " + oPlugin.DllFileName);
                PluginRunner cr = this._runnerSet[oPlugin.DllFileName];
                result = cr.Run(oPlugin.ClassNamespacePath, functionName, functionArgs);
            }
            return result;
        }
    }
}
