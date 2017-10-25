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

        #region " TestHelper - Tests the HelperPlugin static class - The raw functionality without any imposed implementation. " 

        #region " Test Inputs "

        public static IEnumerable Input_TestHelper3 {
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
                Plugin plugin1 = HelperPlugin.CreatePlugin("CodeMirror", "Return the input int.", dllRoot, "_TestHelper3_CodeMirror.dll", new string[] {code1}, 
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
                Plugin plugin2 =  HelperPlugin.CreatePlugin("CodeMultiplier", "Return the input int multiplied by 2.", dllRoot, "_TestHelper3_CodeMultiplier.dll", new string[] {code2}, 
                    "DynamicPlugins.CodeMultiplier", new string[] {"_TestHelper3_CodeMirror.dll"});


                List<Plugin> pluginSet = new List<Plugin>(){plugin1,plugin2};

                yield return new TestCaseData("Dual Class 1", "TestDomain", baseDir, subDir, "saas_plugins.SaaS.PluginRunner", pluginSet, "7", 
                    "_TestHelper3_CodeMirror.dll", "DynamicPlugins.CodeMirror", "MirrorInt", new object[] {(int)7});     

                yield return new TestCaseData("Dual Class 2", "TestDomain", baseDir, subDir, "saas_plugins.SaaS.PluginRunner", pluginSet, "14", 
                    "_TestHelper3_CodeMultiplier.dll", "DynamicPlugins.CodeMultiplier", "MultBy2", new object[] {(int)7});     
            }
        }

        public static IEnumerable Input_TestHelper2 {
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

                Plugin plugin = HelperPlugin.CreatePlugin("CodeMirror", "Return the input int.", dllRoot, "_TestHelper2_CodeMirror.dll", new string[] {code1}, 
                    "DynamicPlugins.CodeMirror", null);

                List<Plugin> pluginSet = new List<Plugin>(){plugin};
                yield return new TestCaseData("Simple Class calling main library.", "TestDomain", baseDir, subDir, "saas_plugins.SaaS.PluginRunner", pluginSet, "7", 
                    "_TestHelper2_CodeMirror.dll", "DynamicPlugins.CodeMirror", "MirrorInt", new object[] {(int)7});                                    
            }
        }

        public static IEnumerable Input_TestHelper1 {
            get {
                string baseDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\";   // path to bin
                string subDir = @"PluginsTest";
                string dllRoot = baseDir + subDir + @"\";

                string code1 = @"
                    using System;
                    namespace DynamicPlugins {
                      public class CodeMirror {
                        public int MirrorInt(int x) {
                          return x;
                        }
                      }
                    }";

                Plugin plugin = HelperPlugin.CreatePlugin("CodeMirror1", "Return the input int.", dllRoot, "_TestHelper1_CodeMirror.dll", new string[] {code1}, 
                    "DynamicPlugins.CodeMirror", null);

                List<Plugin> pluginSet = new List<Plugin>(){plugin};

                yield return new TestCaseData("Simple Class", "TestDomain", baseDir, subDir, "saas_plugins.SaaS.PluginRunner", pluginSet, "7", 
                    "_TestHelper1_CodeMirror.dll", "DynamicPlugins.CodeMirror", "MirrorInt", new object[] {(int)7});                                    
            }
        }

        #endregion

        //[Ignore("")]
        [Test]
        [TestCaseSource("Input_TestHelper1")]
        [TestCaseSource("Input_TestHelper2")]
        [TestCaseSource("Input_TestHelper3")]
        public void TestHelper(string TestName, string domainName, string domainBaseDir, string domainSubDir, string runnerNamespace, List<Plugin> pluginSet, string expected, string runPluginID, string runClassPath, string runMethodName, object[] runArgs) {

            //AppDomain domain = AppDomain.CreateDomain(domainName);
            AppDomain domain = HelperPlugin.CreateAppDomain(domainName, domainBaseDir, domainSubDir, AppDomain.CurrentDomain.SetupInformation.ConfigurationFile);

            Plugin runPlugin = null;

            // Compile Plugins - Fail if compile returns false  - removes existing DLL before attempting compile
            //      also set the plugin object to run when matched
            foreach(Plugin plugin in pluginSet) {
                bool compileRes = HelperPlugin.CompilePlugin(plugin, domainName+"TEMP", runnerNamespace, true);
                Assert.True(compileRes, "Plugin compile failed: " + plugin.PluginID);
                if(plugin.PluginID == runPluginID)
                    runPlugin = plugin;
            }
            Assert.NotNull(runPlugin, "No plugin matched the run ID: "+ runPluginID);

            // Check for the generated DLL's
            foreach(Plugin plugin in pluginSet) {
                Assert.True(System.IO.File.Exists(plugin.DllFilePath));
            }

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

        #region " TestPluginDomain - Adds a layer of implementation to HelperPlugin "


        //[Ignore("")]
        [Test]
        [TestCaseSource("Input_TestPluginDomain1")]
        public void TestPluginDomain(string TestName, string domainName, string domainBaseDir, string domainSubDir, string runnerNamespace, List<Plugin> pluginSet, string expected, string runPluginID, string runClassPath, string runMethodName, object[] runArgs) {

            // Create our domain
            PluginDomain domain = new PluginDomain(domainName, domainBaseDir, domainSubDir, runnerNamespace);


            // Compile & Verify Set

            // Load Set

            // GetReference (from pluginID)

            /*
            // Compile Plugins - Fail if compile returns false  - removes existing DLL before attempting compile
            //      also set the plugin object to run when matched
            Plugin runPlugin = null;
            foreach(Plugin plugin in pluginSet) {
                bool compileRes = domain.CompilePlugin(plugin);
                Assert.True(compileRes, "Plugin compile failed: " + plugin.PluginID);
                if(plugin.PluginID == runPluginID)
                    runPlugin = plugin;
            }
            Assert.NotNull(runPlugin, "No plugin matched the run ID: "+ runPluginID);

            // Check for the generated DLL's
            foreach(Plugin plugin in pluginSet) {
                Assert.True(System.IO.File.Exists(plugin.DllFilePath));
            }


            // Load Plugins - Fail if any references come back null
            Dictionary<string, PluginReference> referenceSet = new Dictionary<string, PluginReference>();
            foreach(Plugin plugin in pluginSet) {
                PluginReference pluginRef = domain.LoadPlugin(plugin);    // The domain alreay stores its references
                Assert.NotNull(pluginRef, "Plugin load failed: "+ plugin.PluginID);

                referenceSet.Add(plugin.PluginID, pluginRef);
            }

            // Make the method call and check its result
            PluginReference pluginReference = referenceSet[runPluginID];
            string sRes = HelperPlugin.RunMethodString(pluginReference.PluginRunner, runPlugin, runClassPath, runMethodName, runArgs);
            Assert.AreEqual(expected, sRes, "Method call returned an unexpected result: expected=" + expected + "   actual=" + sRes);
            */

            // Unload our domain
            domain.Dispose();
        }

        #endregion

        #region " TestPluginSystem "


        #endregion
    }
}
  