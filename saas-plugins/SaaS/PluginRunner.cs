/****************************** PluginRunner ******************************\
The PluginRunner class is a type of MarshalByRefObject that allows us to 
interact with a Plugin that has been loaded into a different AppDomain.

Copyright (c) Aaron Ulrich
This source is subject to the Apache License Version 2.0, January 2004
See http://www.apache.org/licenses/.
All other rights reserved.

THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

using System;
using System.Collections.Generic;
using System.CodeDom.Compiler;
using System.Reflection;
using Microsoft.CSharp;

namespace saas_plugins.SaaS
{
    public class PluginRunner : MarshalByRefObject
    {
        private Assembly assembly = null;

        
        /// <summary>
        /// Returns the name of this AppDomain
        /// </summary>
        /// <returns></returns>
        public override string ToString() {
            return AppDomain.CurrentDomain.FriendlyName;
        }

        /// <summary>
        /// Overridden to ensure objects lifetime is as long as the hosting AppDomain
        /// </summary>
        /// <returns></returns>
        public override object InitializeLifetimeService() {
            return null;
        }




        #region " Reflection "

        /// <summary>
        /// Return a set of parameters for a method
        /// </summary>
        /// <returns></returns>
        public List<string> GetTypeMethodParams(string typeName, string methodName) {
            List<string> set = new List<string>();
            try {
                
                Type tp = assembly.GetType(typeName);
                if(tp != null) {
                    MethodInfo info = tp.GetMethod(methodName);
                    if(info != null) {
                        string entry = info.ReturnType.ToString() + ":RETURN";
                        set.Add(entry);

                        foreach (ParameterInfo param in info.GetParameters()) {
                            entry = param.ParameterType.ToString() + ":" + param.Name;
                            set.Add(entry);
                        }
                    }
                }
            } catch(Exception ex) {
                System.Console.WriteLine(ex.Message);
            }
            return set;
        }

        /// <summary>
        /// Return a list of available methods for a plugins type
        /// </summary>
        /// <returns></returns>
        public List<string> GetTypeMethods(string typeName) {
            List<string> set = new List<string>();
            try {
                
                Type tp = assembly.GetType(typeName);
                if(tp != null) {
                    //MethodInfo[] methodInfoSet = tp.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
                    MethodInfo[] methodInfoSet = tp.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly);
                    foreach (MethodInfo info in methodInfoSet) {
                        set.Add(info.Name);
                    }
                }
            } catch(Exception ex) {
                System.Console.WriteLine(ex.Message);
            }
            return set;
        }


        /// <summary>
        /// Return a list of available types in the plugins loaded assembly
        /// </summary>
        /// <returns></returns>
        public List<string> GetTypes() {
            List<string> typeSet = new List<string>();
            try {
                foreach (Type type in assembly.GetTypes()) {
                    //Console.WriteLine(type.FullName);
                    typeSet.Add(type.FullName);
                }
            } catch(Exception ex) {
                System.Console.WriteLine(ex.Message);
            }
            return typeSet;
        }


        /// <summary>
        /// Return a list of assembly FullName properties running in this AppDomain (can be split on commas)
        /// </summary>
        /// <returns></returns>
        public List<string> GetDomainAssemblies() {
            List<string> asmSet = null;
            try {
                Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
                asmSet = new List<string>();
                foreach(Assembly asm in assemblies) {
                    asmSet.Add(asm.FullName);
                }
                
            } catch(Exception ex) {
                System.Console.WriteLine(ex.Message);
            }
            return asmSet;
        }

        #endregion

        /// <summary>
        /// This will invoke the target method with the given parameters.
        /// </summary>
        /// <param name="typeName">The namespace class path.</param>
        /// <param name="methodName">The class method to invoke.</param>
        /// <param name="args">The object parameter list to pass to the method.</param>
        /// <returns></returns>
        public object Run(string typeName, string methodName, object[] args)
        {
            System.Console.WriteLine("Running from: " + AppDomain.CurrentDomain.FriendlyName);
            Type type = null;
            MethodInfo methodInfo = null;
            object classInstance = null;
            object result = null;

            try {
                type = this.assembly.GetType(typeName);
                methodInfo = type.GetMethod(methodName);
                classInstance = Activator.CreateInstance(type, null);

                // This invoke will try and link to the actual DLL file .... and by default it looks in the bin folder
                //      locations can be overriden when created the AppDomain
                result = methodInfo.Invoke(classInstance, args);
            } catch(Exception ex) {
                System.Console.WriteLine(ex.Message);
            }

            classInstance = null;
            methodInfo = null;
            type = null;
            return result;
        }
        
        /// <summary>
        /// Load an Assembly into this AppDomain
        /// </summary>
        /// <param name="dllFilePath">The path to the DLL to load.</param>
        /// <returns></returns>
        public string Load(string dllFilePath) {
            try {
                this.assembly = Assembly.LoadFile(dllFilePath);
                //AppDomain.CurrentDomain.Load(File.ReadAllBytes(assemblyFileName));
            } catch {}

            // NOTE: Cant return the Assembly.... issues with cross-domain
            //System.Console.WriteLine("Loaded: " + assembly.FullName);
            return assembly.FullName;
        }


        #region " CompileToFile "

        /// <summary>
        /// Compile these code files into a DLL.
        /// </summary>
        /// <param name="code">An array of code files.</param>
        /// <param name="dllFilePath">A path to store the DLL.</param>
        /// <param name="referencedAssemblySet">A set of references required by this DLL.</param>
        /// <returns></returns>
        public bool CompileToFile(string[] code, string dllFilePath, List<string> referencedAssemblySet)
        {
            CSharpCodeProvider codeProvider = new CSharpCodeProvider();
            CompilerParameters parameters = new CompilerParameters();

            // Determine the xml filePath
            string xmlFileName = System.IO.Path.GetFileNameWithoutExtension(dllFilePath) + ".xml";
            string xmlFilePath = System.IO.Path.GetDirectoryName(dllFilePath) + @"\" + xmlFileName;

            // Configure the parameter options (to file)
            parameters.OutputAssembly = dllFilePath;            // AppData/Local/Temp/
            parameters.GenerateInMemory = false;
            parameters.GenerateExecutable = false;

            foreach(string asm in referencedAssemblySet) {
                parameters.ReferencedAssemblies.Add(asm);
            }
            parameters.CompilerOptions = "/t:library /doc:" + xmlFilePath;

            // Compile from the source code
            CompilerResults results = codeProvider.CompileAssemblyFromSource(parameters, code);
            if (!results.Errors.HasErrors) {
                this.assembly = results.CompiledAssembly;
                System.Console.WriteLine("Assembly created: " + this.assembly.GetName() + "   ----   " + this.assembly.FullName);
            } else {
                System.Console.WriteLine("Compiler Error: " + dllFilePath);
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

        #region " CompileInMemory - Needs testing "

        // Removed, at least for now. This needs some testing.

        /*
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
        */

        #endregion


    }
}
