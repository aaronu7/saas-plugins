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
        [SetUp] public void Setup() {
        }

        [TearDown] public void TestTearDown() {}

        #region " Helper Methods "



        #endregion

        #region " TestBasic1 " 

        #region " Test Inputs "

        public static IEnumerable Input_TestBasic2 {
            get {
                string baseDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\";   // path to bin
                string subDir = @"PluginsTest";
                string dllRoot = baseDir + subDir + @"\";
                //string dllRoot = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\PluginsTest\";   // path to bin

                string code1 = @"
                    using System;
                    namespace DynamicPlugins {
                      public class CodeMirror {
                        public int MirrorInt(int x) {
                          return saas_plugins.SaaS.HelperPlugin.GetMirrorValue(x);
                        }
                      }
                    }";
                Plugin plugin1 = HelperPlugin.CreatePlugin("CodeMirror", "Return the input int.", dllRoot, "_Test2_CodeMirror.dll", new string[] {code1}, 
                    "DynamicPlugins.CodeMirror", null);


                string code2 = @"
                    using System;
                    namespace DynamicPlugins {
                      public class CodeMultiplier {
                        public int MultBy2(int x) {
                            DynamicPlugins.CodeMirror obj = new DynamicPlugins.CodeMirror();
                            return (int)obj.MirrorInt(x) * 2;
                        }
                      }
                    }";
                Plugin plugin2 =  HelperPlugin.CreatePlugin("CodeMultiplier", "Return the input int multiplied by 2.", dllRoot, "_Test2_CodeMultiplier.dll", new string[] {code2}, 
                    "DynamicPlugins.CodeMultiplier", new string[] {"_Test2_CodeMirror.dll"});


                List<Plugin> pluginSet = new List<Plugin>(){plugin1,plugin2};

                yield return new TestCaseData("Dual Class 1", "TestDomain", baseDir, subDir, "saas_plugins.SaaS.PluginRunner", pluginSet, "7", 
                    "_Test2_CodeMirror.dll", "DynamicPlugins.CodeMirror", "MirrorInt", new object[] {(int)7});     

                yield return new TestCaseData("Dual Class 2", "TestDomain", baseDir, subDir, "saas_plugins.SaaS.PluginRunner", pluginSet, "14", 
                    "_Test2_CodeMultiplier.dll", "DynamicPlugins.CodeMultiplier", "MultBy2", new object[] {(int)7});     
            }
        }

        public static IEnumerable Input_TestBasic1 {
            get {
                string baseDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\";   // path to bin
                string subDir = @"PluginsTest";
                string dllRoot = baseDir + subDir + @"\";

                string code1 = @"
                    using System;
                    namespace DynamicPlugins {
                      public class CodeMirror {
                        public int MirrorInt(int x) {
                          return saas_plugins.SaaS.HelperPlugin.GetMirrorValue(x);
                        }
                      }
                    }";

                //@"C:\Users\aaron\OneDrive\Work\Code_GitHub\saas-plugins\saas-plugins-test\bin\Debug\", "PluginsTest"
                //string dllRoot = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\PluginsTest\";   // path to bin

                Plugin plugin = HelperPlugin.CreatePlugin("CodeMirror", "Return the input int.", dllRoot, "_Test1_CodeMirror.dll", new string[] {code1}, 
                    "DynamicPlugins.CodeMirror", null);

                List<Plugin> pluginSet = new List<Plugin>(){plugin};

                //string baseDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\";   // path to bin
                //string subDir = @"PluginsTest";
                yield return new TestCaseData("Simple Class", "TestDomain", baseDir, subDir, "saas_plugins.SaaS.PluginRunner", pluginSet, "7", 
                    "_Test1_CodeMirror.dll", "DynamicPlugins.CodeMirror", "MirrorInt", new object[] {(int)7});                                    
            }
        }

        #endregion

        //[Ignore("")]
        [Test]
        [TestCaseSource("Input_TestBasic1")]
        [TestCaseSource("Input_TestBasic2")]
        public void TestBasic1(string TestName, string domainName, string domainBaseDir, string domainSubDir, string runnerNamespace, List<Plugin> pluginSet, string expected, string runPluginID, string runClassPath, string runMethodName, object[] runArgs) {

            //AppDomain domain = AppDomain.CreateDomain(domainName);
            AppDomain domain = HelperPlugin.CreateAppDomain(domainName, domainBaseDir, domainSubDir, AppDomain.CurrentDomain.SetupInformation.ConfigurationFile);

            Plugin runPlugin = null;

            // Remove any previously generated DLL's

            // Compile Plugins - Fail if compile returns false
            //      also set the plugin object to run when matched
            foreach(Plugin plugin in pluginSet) {
                bool compileRes = HelperPlugin.CompilePlugin(plugin, domainName+"TEMP", runnerNamespace);
                Assert.True(compileRes, "Plugin compile failed: " + plugin.PluginID);
                if(plugin.PluginID == runPluginID)
                    runPlugin = plugin;
            }

            Assert.NotNull(runPlugin, "No plugin matched the run ID: "+ runPluginID);

            // Check for the generated DLL's

            // Load Plugins - Fail if any runners come back null
            Dictionary<string, PluginRunner> runnerSet = new Dictionary<string, PluginRunner>();
            foreach(Plugin plugin in pluginSet) {
                PluginRunner runner = HelperPlugin.LoadPlugin(plugin, domain);
                Assert.NotNull(runner, "Plugin load failed: "+ plugin.PluginID);
                runnerSet.Add(plugin.PluginID, runner);
            }

            // Make the method call and check its result
            PluginRunner pluginRunner = runnerSet[runPluginID];
            string sRes = HelperPlugin.RunMethodString(pluginRunner, runPlugin, runClassPath, runMethodName, runArgs);
            Assert.AreEqual(expected, sRes, "Method call returned an unexpected result: expected=" + expected + "   actual=" + sRes);

            AppDomain.Unload(domain);
        }

        #endregion
    }
}
  