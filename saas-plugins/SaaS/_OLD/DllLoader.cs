using System;
using System.Reflection;

namespace saas_plugins.SaaS
{
    /*
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
      */
}
