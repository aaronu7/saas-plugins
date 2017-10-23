using System;
using System.Collections.Generic;
using System.CodeDom.Compiler;
using System.Reflection;
using Microsoft.CSharp;

namespace saas_plugins.SaaS
{
    public class PluginRunner : MarshalByRefObject, IDisposable
    {
        private Assembly assembly = null;

        public override string ToString() {
            return "This object is running in the " + AppDomain.CurrentDomain.FriendlyName + " AppDomain";
        }

        public void Dispose() {
            this.assembly = null;
        }

        #region " CompileToFile "

        public bool CompileToFile(string[] code, string dllFilePath, List<string> referencedAssemblySet)
        {
            CSharpCodeProvider codeProvider = new CSharpCodeProvider();
            CompilerParameters parameters = new CompilerParameters();

            // Configure the parameter options (to file)
            parameters.OutputAssembly = dllFilePath;            // AppData/Local/Temp/
            parameters.GenerateInMemory = false;
            parameters.GenerateExecutable = false;
            foreach(string asm in referencedAssemblySet) {
                parameters.ReferencedAssemblies.Add(asm);
            }
            parameters.CompilerOptions = "/t:library";

            // Compile from the source code
            CompilerResults results = codeProvider.CompileAssemblyFromSource(parameters, code);
            if (!results.Errors.HasErrors) {
                this.assembly = results.CompiledAssembly;
                System.Console.WriteLine("Assembly created: " + this.assembly.GetName() + "   ----   " + this.assembly.FullName);
            } else {
                foreach(CompilerError err in results.Errors) {
                    System.Console.WriteLine(err.ErrorText);
                }
                this.assembly = null;
            }

            // Dispose and return success indicator
            codeProvider.Dispose();
            codeProvider = null;
            parameters = null;
            return this.assembly != null;
        }

        #endregion

        #region " CompileInMemory "

        // this is good for th QuickRun algorithm ... no artefacts to deal with

        public bool CompileInMemory(string code, List<string> referencedAssemblySet) {
            return CompileInMemory(new string[] {code}, referencedAssemblySet);
        }

        public bool CompileInMemory(string[] code, List<string> referencedAssemblySet)
        {
            CSharpCodeProvider codeProvider = new CSharpCodeProvider();
            CompilerParameters parameters = new CompilerParameters();

            // Configure the parameter options (in memory)
            parameters.GenerateInMemory = true;
            parameters.GenerateExecutable = false;
            foreach(string asm in referencedAssemblySet) {
                parameters.ReferencedAssemblies.Add(asm);
            }
            parameters.CompilerOptions = "/t:library";

            // Compile from the source code
            CompilerResults results = codeProvider.CompileAssemblyFromSource(parameters, code);
            if (!results.Errors.HasErrors) {
                this.assembly = results.CompiledAssembly;
                System.Console.WriteLine("Assembly created: " + this.assembly.GetName() + "   ----   " + this.assembly.FullName);
            } else {
                foreach(CompilerError err in results.Errors) {
                    System.Console.WriteLine(err.ErrorText);
                }
                this.assembly = null;
            }

            // Dispose and return success indicator
            codeProvider.Dispose();
            codeProvider = null;
            parameters = null;
            return this.assembly != null;
        }

        #endregion

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

            //return type.InvokeMember(methodName, BindingFlags.InvokeMethod, null, assembly, args);
        }
    }
}
