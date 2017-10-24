using System;
using System.Text;
using System.Collections.Generic;

namespace saas_plugins.SaaS
{
    // OLD version caused a massive memory leak because the assemblies are never released
    //  from the application.... this version uses an AppDomain to resolve this.
    
    public static class HelperPlugin 
    {
        /*
        public static int GetTestValue(int x) {
            return x;
        }

        
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
