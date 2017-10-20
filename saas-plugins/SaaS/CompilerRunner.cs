using System;
using System.Collections.Generic;
using System.CodeDom.Compiler;
using System.Reflection;
using Microsoft.CSharp;

namespace ad2csv.SaaS
{
    public class CompilerRunner : MarshalByRefObject
    {
        private Assembly assembly = null;

        public void PrintDomain()
        {
            Console.WriteLine("Object is executing in AppDomain \"{0}\"",
                AppDomain.CurrentDomain.FriendlyName);
        }

        public bool Compile(string code, List<string> referencedAssemblySet)
        {
            CSharpCodeProvider codeProvider = new CSharpCodeProvider();
            CompilerParameters parameters = new CompilerParameters();
            parameters.GenerateInMemory = true;
            parameters.GenerateExecutable = false;

            //parameters.ReferencedAssemblies.Add("system.dll");
            //parameters.ReferencedAssemblies.Add("system.drawing.dll");
            foreach(string asm in referencedAssemblySet) {
                parameters.ReferencedAssemblies.Add(asm);
            }


            parameters.CompilerOptions = "/t:library";

            CompilerResults results = codeProvider.CompileAssemblyFromSource(parameters, code);
            if (!results.Errors.HasErrors) {
                this.assembly = results.CompiledAssembly;
            } else {
                this.assembly = null;
            }

            return this.assembly != null;
        }

        public object Run(string typeName, string methodName, object[] args)
        {
            Type type = this.assembly.GetType(typeName);
            MethodInfo methodInfo = type.GetMethod(methodName);
            object classInstance = Activator.CreateInstance(type, null);

            object result = methodInfo.Invoke(classInstance, null);
            classInstance = null;
            methodInfo = null;
            type = null;

            return result;

            //return type.InvokeMember(methodName, BindingFlags.InvokeMethod, null, assembly, args);
        }
    }
}
