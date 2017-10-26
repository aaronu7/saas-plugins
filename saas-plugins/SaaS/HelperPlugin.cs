using System;
using System.Reflection;
using System.Text;
using System.Collections.Generic;
using System.IO;

namespace saas_plugins.SaaS
{
    // OLD version caused a massive memory leak because the assemblies are never released
    //  from the application.... this version uses an AppDomain to resolve this.
    
    public static class HelperPlugin 
    {
        
        public static int GetMirrorValue(int x) {
            return x;
        }

        #region " NOTEs Binding DLL Location Issue "
        /*
        // https://stackoverflow.com/questions/15883109/createinstanceandunwrap-fails-to-load-assembly
        Use the LoadFrom context (by using the CreateInstanceFromXXX methods).

        Add the Mytypes folder to the AppDomainSetup.PrivateBinPath used to create the AppDomain. This way the Load context will be able to resolve the assebmlies located there.

        Subscribe to the AppDomain.AssemblyResolve event and resolve the assemblies yourself by looking for them and loading them from the Mytypes folder.
        Deploy all your assemblies in the base directory of your application.
        */

        /*
            // alternate dll directories seem to cause issues ...
            //  --- need to explore bindings and see where we can override this
            //  --- explore variants for loading assembly into AppDomain
            */

        /* App.config
         
          <runtime>
            <assemblyBinding xmlns="urn:schemas-microsoft-com:asm.v1">
              <probing privatePath="bin;bin\DynamicPlugins"/>
            </assemblyBinding>
          </runtime>
          */
        #endregion

        #region " Create AppDomain "

        public static AppDomain CreateAppDomain(string domainName, string baseDirectory, string subDirectory, string configFile) {
            AppDomainSetup appSetup = new AppDomainSetup()
            {
                ApplicationName = domainName,
                ApplicationBase = baseDirectory,
                PrivateBinPath = subDirectory,
                ConfigurationFile = configFile
                //ApplicationBase = AppDomain.CurrentDomain.BaseDirectory,
                //PrivateBinPath = @"Plugins",
                //ConfigurationFile = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile
            };

            AppDomain appDomain = AppDomain.CreateDomain(domainName, null, appSetup);
            return appDomain;
        }
        #endregion

        #region " RunPlugin "

        public static object RunMethodObject(PluginRunner runner, Plugin plugin, string classNamespacePath, string functionName, object[] functionArgs) {
            object result = null;

            if(runner != null) {
                try {
                    result = runner.Run(plugin.ClassNamespacePath, functionName, functionArgs);
                } catch(Exception ex) {
                    System.Console.WriteLine("RUN ERROR: " + ex.Message);
                    System.Console.WriteLine("RUN ERROR: " + ex.InnerException.Message);
                }
            }

            return result;
        }

        public static string RunMethodString(PluginRunner runner, Plugin plugin, string classPath, string methodName, object[] args) {
            object res = RunMethodObject(runner, plugin, classPath, methodName, args);
            string sRes = "NULL";
            if(res!=null)
                sRes = res.ToString();

            return sRes;
        }

        #endregion

        #region " CreatePlugin "

        public static Plugin CreatePlugin(string name, string desc, string dllRoot, string dllFileName, string[] code, string codeNamespacePath, string[] dllCustomRefs)
        {
            // alternate dll directories seem to cause issues ...
            //  --- need to explore bindings and see where we can override this
            //  --- explore variants for loading assembly into AppDomain
            //
            //string dllRoot = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\";   // path to bin
            //      dllRoot = Application.StartupPath + @"\";

            //string plugginRoot = Application.StartupPath + @"\DynamicPlugins\";
            //string plugginRoot = Application.StartupPath + @"\";
            //string libDllPath = Application.StartupPath + @"\saas_plugins.dll";
            

            List<string> referencedAssemblySet = new List<string>();
            referencedAssemblySet.Add("system.dll");
            referencedAssemblySet.Add("system.drawing.dll");
            //referencedAssemblySet.Add("saas_plugins.dll");

            string projectRoot = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\";   // path to bin
            //referencedAssemblySet.Add(dllRoot + "saas_plugins.dll");    // unit tests need this expicit
            referencedAssemblySet.Add(projectRoot + "saas_plugins.dll");    // unit tests need this expicit

            if(dllCustomRefs != null) {
                foreach(string reference in dllCustomRefs) { 
                    referencedAssemblySet.Add(dllRoot + reference);   //  only needed if we move it out of the expected location .... which isn't working yet
                    //referencedAssemblySet.Add(reference);
                }
            }

            
            //string plugginRoot = Application.StartupPath + @"\";
            System.IO.Directory.CreateDirectory(dllRoot);
            Plugin oPlugin = new Plugin("", "", dllRoot, dllFileName, referencedAssemblySet, codeNamespacePath, code);

            return oPlugin;
        }

        #endregion

        #region " CompilePlugin "

        static public bool CompilePlugin(Plugin plugin, string tempDomainName, string compilerRunnerNamespace, bool removeExistingFirst) {
            
            // Remove any previously generated DLL's
            if(removeExistingFirst && System.IO.File.Exists(plugin.DllFilePath)) {
                System.IO.File.Delete(plugin.DllFilePath);
            }

            // Create the temporary domain
            AppDomain domainTemp = AppDomain.CreateDomain(tempDomainName);

            // Create a PluginRunner instance in the temp domain
            //string mainDllFileName = Assembly.GetExecutingAssembly().GetName(false).Name + ".dll"; // "saas_plugins.dll"
            //PluginRunner runner = (PluginRunner)domainTemp.CreateInstanceFromAndUnwrap(mainDllFileName, compilerRunnerNamespace);

            // More explicit DLL path for Unit Testing
            string mainDllFileName = Assembly.GetExecutingAssembly().GetName(false).Name + ".dll"; // "saas_plugins.dll"
            string dllRoot = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\";   // path to bin
            string dllPath = dllRoot + mainDllFileName;
            PluginRunner runner = (PluginRunner)domainTemp.CreateInstanceFromAndUnwrap(dllPath, compilerRunnerNamespace);

            // Compile the file within the temp domain
            bool res = runner.CompileToFile(plugin.Code, plugin.DllFilePath, plugin.DllFileNameReferenceSet);
            plugin.IsCompiled = res;

            // Discard the temp domain
            AppDomain.Unload(domainTemp);
            domainTemp = null;
            
            return res;
        }

        #endregion

        #region " LoadPlugin "

        public static PluginRunner LoadPlugin(Plugin plugin, AppDomain domain)
        {
            // Load into the Plugin domain
            PluginRunner loader = null;

            if(domain != null) {
                try {
                    loader = (PluginRunner)domain.CreateInstanceAndUnwrap(typeof(PluginRunner).Assembly.FullName, typeof(PluginRunner).FullName);
                    //loader = (PluginRunner)domain.CreateInstanceFromAndUnwrap(typeof(PluginRunner).Assembly.FullName, typeof(PluginRunner).FullName);

                    loader.Load(plugin.DllFilePath);

                } catch (Exception ex) {
                    System.Console.WriteLine(ex.Message);
                }
            }

            return loader;
        }

        #endregion


        /*
        public static object CompileQuickRun(string asmDLLName, string compilerRunnerNamespace, 
            string instanceDomainName, List<string> referencedAssemblySet, string code, 
            string classTypeString, 
            string functionCall, object[] functionArgs) {

            //RunExpression("ad2csv.dll", "ad2csv.SaaS.CompilerRunner", "MyDomain", "code goes here", "ad2csv.SaaS.CompilerRunner.CSCodeEvaler", "EvalCode", new object[0]);

            AppDomain domain = AppDomain.CreateDomain(instanceDomainName);
            PluginRunner cr = (PluginRunner)domain.CreateInstanceFromAndUnwrap(asmDLLName, compilerRunnerNamespace);

            object result = null;
            if(cr.CompileInMemory(code, referencedAssemblySet)) {
                result = cr.Run(classTypeString, functionCall, functionArgs);
            }

            //AppDomain.Unload(tempD);
            AppDomain.Unload(domain);
            return result;
        }
        */

        /*
    public static bool CompileDLL(Plugin oPlugin, string mainDllFileName, string tmpInstanceDomain, string compilerRunnerNamespace) {

        // Create a temp domain and create our plugin runner
        AppDomain domain = AppDomain.CreateDomain(tmpInstanceDomain);
        PluginRunner cr = (PluginRunner)domain.CreateInstanceFromAndUnwrap(mainDllFileName, compilerRunnerNamespace);

        // Compile the file within the temp domain
        string dllFilePath = oPlugin.DllFileDir + oPlugin.DllFileName;
        bool res = cr.CompileToFile(oPlugin.Code, dllFilePath, oPlugin.DllFileNameReferenceSet);

        // Discard the temp domain
        AppDomain.Unload(domain);
        return res;
    }
    */

        /*

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
        */
    }


}
