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
        public string Load(string dllFilePath) {
            var assembly = Assembly.LoadFile(dllFilePath);
            return assembly.FullName;
        }
      }


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

        protected void LoadAssembly()
        {
            //string pathToDll = Assembly.GetExecutingAssembly().CodeBase;
            //AppDomainSetup domainSetup = new AppDomainSetup { PrivateBinPath = pathToDll };
            //var newDomain = AppDomain.CreateDomain("FooBar", null, domainSetup);

            //ProxyClass c = (ProxyClass)(newDomain.CreateInstanceFromAndUnwrap(pathToDll, typeof(ProxyClass).FullName));
            //Console.WriteLine(c == null);

            Console.ReadKey(true);
        }

        public void OutputAssemblies() {
            try {
                //Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
                Assembly[] assemblies = this._domain.GetAssemblies();
                System.Console.WriteLine("Running Assemblines");
                System.Console.WriteLine("===================");
                foreach(Assembly asm in assemblies) {
                    System.Console.WriteLine(asm.FullName);
                }
            } catch(Exception ex) {
                System.Console.WriteLine(ex.Message);
            }
        }

        public void LoadPlugin(Plugin oPlugin) {
            string dllFilePath = oPlugin.DllFileDir + oPlugin.DllFileName;
            var loader = (IDllLoader)this._domain.CreateInstanceAndUnwrap(typeof(DllLoader).Assembly.FullName, typeof(DllLoader).FullName);
            loader.Load(dllFilePath);
            System.Console.WriteLine("Loaded: " + oPlugin.DllFileName);
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
                PluginRunner cr = this._runnerSet[oPlugin.DllFileName];
                result = cr.Run(oPlugin.ClassNamespacePath, functionName, functionArgs);
            }
            return result;
        }
    }
}
