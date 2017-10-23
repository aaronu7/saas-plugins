using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace saas_plugins.SaaS
{
    public class PluginManager : IDisposable
    {
        AppDomain _domain = null;
        private string _compilerRunnerNamespace = "";
        private string _instanceDomainName = "";

        Dictionary<string, PluginRunner> _runnerSet = null;

        public PluginManager(string instanceDomainName, string compilerRunnerNamespace) {
            this._instanceDomainName = instanceDomainName;
            this._compilerRunnerNamespace = compilerRunnerNamespace;

            this._domain = AppDomain.CreateDomain(this._instanceDomainName);
            this._runnerSet = new Dictionary<string, PluginRunner>();
        }

        public void Dispose() {
            AppDomain.Unload(this._domain);
            //this._runnerSet.
        }

        public void AddPlugin(Plugin oPlugin) {
              
            // Compile the plugin
            //PluginRunner cr = (PluginRunner)this._domain.CreateInstanceFromAndUnwrap(oPlugin.DllFileName, this._compilerRunnerNamespace);
            PluginRunner cr = new PluginRunner();
            string dllFilePath = oPlugin.DllFileDir + oPlugin.DllFileName;
            if(!cr.CompileToFile(oPlugin.Code, dllFilePath, oPlugin.DllFileNameReferenceSet)) {
                System.Console.WriteLine("Plugin Failed to Load: " + oPlugin.DllFileName);

            } else {
                if(!this._runnerSet.ContainsKey(oPlugin.DllFileName)) {
                    this._runnerSet.Add(oPlugin.DllFileName, cr);
                    System.Console.WriteLine("Plugin Added: " + oPlugin.DllFileName);
                } else {

                    // ********** we need to unload the entire AppDomain and reload it

                    this._runnerSet[oPlugin.DllFileName].Dispose();
                    this._runnerSet[oPlugin.DllFileName] = null;
                    this._runnerSet[oPlugin.DllFileName] = cr;
                    System.Console.WriteLine("Plugin Updated: " + oPlugin.DllFileName);
                }
            } 

            //Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            Assembly[] assemblies = this._domain.GetAssemblies();
            foreach(Assembly asm in assemblies) {
                System.Console.WriteLine(asm.FullName);
            }

            //object result = null;
            //if(cr.Compile(code, referencedAssemblySet)) {
            //    result = cr.Run(classTypeString, functionCall, functionArgs);
            //}
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
