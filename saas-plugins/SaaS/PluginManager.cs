using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace saas_plugins.SaaS
{
      public interface IDllLoader {
        string Load(string dllFilePath);
      }

      public class DllLoader : MarshalByRefObject, IDllLoader
      {
        Assembly assembly = null;

        public string Load(string dllFilePath) {
            try {
                this.assembly = Assembly.LoadFile(dllFilePath);
            } catch {}

            // NOTE: Cant return the Assembly.... issues with cross-domain
            //System.Console.WriteLine("Loaded: " + assembly.FullName);
            return assembly.FullName;
        }

        public object Run(string typeName, string methodName, object[] args)
        {
            Type type = this.assembly.GetType(typeName);
            MethodInfo methodInfo = type.GetMethod(methodName);
            object classInstance = Activator.CreateInstance(type, null);

            object result = methodInfo.Invoke(classInstance, args);
            classInstance = null;
            methodInfo = null;
            type = null;

            return result;
        }
      }


    public class PluginManager : IDisposable
    {
        AppDomain _domain = null;
        private string _compilerRunnerNamespace = "";
        private string _instanceDomainName = "";

        Dictionary<string, DllLoader> _runnerSet = null;

        public PluginManager(string instanceDomainName, string compilerRunnerNamespace) {
            this._instanceDomainName = instanceDomainName;
            this._compilerRunnerNamespace = compilerRunnerNamespace;

            this._domain = AppDomain.CreateDomain(this._instanceDomainName);
            this._runnerSet = new Dictionary<string, DllLoader>();
        }

        public void Dispose() {
            AppDomain.Unload(this._domain);
            //this._runnerSet.
        }


        public void OutputAssemblies() {
            try {
                /*
                Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
                System.Console.WriteLine("===================");
                System.Console.WriteLine("Current App Domain");
                System.Console.WriteLine("===================");
                foreach(Assembly asm in assemblies) {
                    System.Console.WriteLine(asm.FullName);
                }

                assemblies = this._domain.GetAssemblies();
                System.Console.WriteLine("===================");
                System.Console.WriteLine("Plugin App Domain");
                System.Console.WriteLine("===================");
                foreach(Assembly asm in assemblies) {
                    System.Console.WriteLine(asm.FullName);
                }
                */
            } catch(Exception ex) {
                System.Console.WriteLine(ex.Message);
            }
            
        }

        public void LoadPlugin(Plugin oPlugin) {
            // Load into the Plugin domain
            string dllFilePath = oPlugin.DllFileDir + oPlugin.DllFileName;
            DllLoader loader = (DllLoader)this._domain.CreateInstanceAndUnwrap(typeof(DllLoader).Assembly.FullName, typeof(DllLoader).FullName);
            string asmName = loader.Load(dllFilePath);
            //System.Console.WriteLine("Loaded: " + asmName);

            // Load  into the current running domain .... cannot "unload" and has memory issues
            //Assembly asm = Assembly.LoadFile(dllFilePath);
            //System.Console.WriteLine("Loaded: " + asm.FullName);


            if(!this._runnerSet.ContainsKey(oPlugin.DllFileName)) {
                this._runnerSet.Add(oPlugin.DllFileName, loader);
                System.Console.WriteLine("Plugin Added: " + oPlugin.DllFileName);
            } else {

                // ********** we need to unload the entire AppDomain and reload it

                //this._runnerSet[oPlugin.DllFileName].Dispose();
                //this._runnerSet[oPlugin.DllFileName] = null;
                //this._runnerSet[oPlugin.DllFileName] = cr;
                //System.Console.WriteLine("Plugin Updated: " + oPlugin.DllFileName);
            }
        }

        public void CompilePlugin(Plugin oPlugin) {
              
            // Compile the DLL  -will destroy its temporary app domain
            EvalEngine2.CompileDLL(oPlugin, "saas_plugins.dll", "tmpCompileDomain", this._compilerRunnerNamespace);


            // Load into this plugin appDomain
            //string dllFilePath = oPlugin.DllFileDir + oPlugin.DllFileName;
            //var loader = (IDllLoader)this._domain.CreateInstanceAndUnwrap(typeof(DllLoader).Assembly.FullName, typeof(DllLoader).FullName);
            //loader.Load(dllFilePath);



            /*
            // Compile the plugin
            //this._domain.cre
            //PluginRunner cr = (PluginRunner)this._domain.CreateInstanceFromAndUnwrap(oPlugin.DllFileName, this._compilerRunnerNamespace);
            //PluginRunner cr = (PluginRunner)this._domain.CreateInstanceFromAndUnwrap("abc", this._compilerRunnerNamespace);
            PluginRunner cr = (PluginRunner)this._domain.CreateInstance();
            //PluginRunner cr = new PluginRunner();
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

            //this._domain.Load
            //Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            Assembly[] assemblies = this._domain.GetAssemblies();
            System.Console.WriteLine("Running Assemblines");
            System.Console.WriteLine("===================");
            foreach(Assembly asm in assemblies) {
                System.Console.WriteLine(asm.FullName);
            }
            */
        }

        public object RunPlugin(Plugin oPlugin, string classNamespacePath, string functionName, object[] functionArgs) {
            object result = null;
            if(!this._runnerSet.ContainsKey(oPlugin.DllFileName)) {
                System.Console.WriteLine("Plugin Not Found: " + oPlugin.DllFileName);
            } else {
                System.Console.WriteLine("Plugin Function Called: " + oPlugin.DllFileName);
                DllLoader cr = this._runnerSet[oPlugin.DllFileName];
                result = cr.Run(oPlugin.ClassNamespacePath, functionName, functionArgs);
            }
            return result;
        }
    }
}
