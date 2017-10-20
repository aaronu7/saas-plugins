using System;
using System.Text;
using System.Collections.Generic;

namespace ad2csv.SaaS
{
    // OLD version caused a massive memory leak because the assemblies are never released
    //  from the application.... this version uses an AppDomain to resolve this.

    public static class EvalEngine2 
    {
        public static object CompileRun(string asmDLLName, string compilerRunnerNamespace, 
            string instanceDomainName, List<string> referencedAssemblySet, string code, 
            string classTypeString, 
            string functionCall, object[] functionArgs) {

            //RunExpression("ad2csv.dll", "ad2csv.SaaS.CompilerRunner", "MyDomain", "code goes here", "ad2csv.SaaS.CompilerRunner.CSCodeEvaler", "EvalCode", new object[0]);

            AppDomain domain = AppDomain.CreateDomain(instanceDomainName);
            CompilerRunner cr = (CompilerRunner)domain.CreateInstanceFromAndUnwrap(asmDLLName, compilerRunnerNamespace);

            object result = null;
            if(cr.Compile(code, referencedAssemblySet)) {
                result = cr.Run(classTypeString, functionCall, functionArgs);
            }

            AppDomain.Unload(domain);
            return result;
        }
    }


}
