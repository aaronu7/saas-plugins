using System;
using System.Text;
using System.Collections.Generic;

namespace saas_plugins.SaaS
{
    // OLD version caused a massive memory leak because the assemblies are never released
    //  from the application.... this version uses an AppDomain to resolve this.

    public static class EvalEngine2 
    {
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

        public static bool CompileDLL(Plugin oPlugin, string mainDllFileName, string tmpInstanceDomain, string compilerRunnerNamespace) {

            //RunExpression("ad2csv.dll", "ad2csv.SaaS.CompilerRunner", "MyDomain", "code goes here", "ad2csv.SaaS.CompilerRunner.CSCodeEvaler", "EvalCode", new object[0]);

            AppDomain domain = AppDomain.CreateDomain(tmpInstanceDomain);
            PluginRunner cr = (PluginRunner)domain.CreateInstanceFromAndUnwrap(mainDllFileName, compilerRunnerNamespace);

            string dllFilePath = oPlugin.DllFileDir + oPlugin.DllFileName;
            bool res = cr.CompileToFile(oPlugin.Code, dllFilePath, oPlugin.DllFileNameReferenceSet);

            AppDomain.Unload(domain);
            return res;
        }
    }


}
