using NUnit.Framework;
using System;
using System.Reflection;
using System.IO;
using System.Collections;
using System.Collections.Generic;


using saas_plugins.SaaS;

namespace template_test.UnitTests
{
    [TestFixture]
    public class BasicTests
    {
        [SetUp] public void Setup() {}
        [TearDown] public void TestTearDown() {}

        #region " Helper Methods "



        #endregion

        #region " Input_TestBasic1 " 

        public static IEnumerable Input_TestBasic1 {
            get {
                string dllRoot = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

                string code1 = @"
                    using System;
                    namespace DynamicPlugins {
                      public class CodeMirror {
                        public int MirrorInt(int x) {
                          return saas_plugins.SaaS.HelperPlugin.GetMirrorValue(x);
                        }
                      }
                    }";
                Plugin plugin = HelperPlugin.CreatePlugin("CodeMirror", "Return the input int.", new string[] {code1}, 
                    "DynamicPlugins.CodeMirror", "_TestBasic1.dll", null);

                List<Plugin> pluginSet = new List<Plugin>(){plugin};
                yield return new TestCaseData("TestDomain", "saas_plugins.SaaS.PluginRunner", pluginSet, "7", "DynamicPlugins.CodeMirror", "MirrorInt", new object[] {(int)7});  


                /*
            pluginDomain.CompilePlugin(PluginA);
            pluginDomain.CompilePlugin(PluginB);

            pluginDomain.LoadPlugin(PluginA);
            pluginDomain.LoadPlugin(PluginB);
            //oPluginDomain.OutputAssemblies(PluginA);

            OutputCall(pluginDomain, PluginA, "DynamicPlugins.CodeMirror", "MirrorInt", new object[] {(int)7});
                */

                                  
            }
        }

        //[Ignore("")]
        [Test]
        [TestCaseSource("Input_TestBasic1")]
        public void TestBasic1(string domainName, string runnerNamespace, List<Plugin> pluginSet, string expected, string classPath, string methodName, object[] args) {

            AppDomain domain = AppDomain.CreateDomain(domainName);

            // Remove any previously generated DLL's

            // Compile Plugins - Fail if compile returns false
            foreach(Plugin plugin in pluginSet) {
                bool res = HelperPlugin.CompilePlugin(plugin, domainName+"TEMP", runnerNamespace);
                Assert.True(res, "Plugin compile failed: " + plugin.PluginID);
            }

            // Check for the generated DLL's

            // Load Plugins - Fail if any runners come back null
            List<PluginRunner> runnerSet = new List<PluginRunner>();
            foreach(Plugin plugin in pluginSet) {
                PluginRunner runner = HelperPlugin.LoadPlugin(plugin, domain);
                Assert.NotNull(runner, "Plugin load failed: "+ plugin.PluginID);
                runnerSet.Add(runner);
            }

            //Assert.IsFalse(false);

            AppDomain.Unload(domain);
        }

        #endregion
    }
}
  