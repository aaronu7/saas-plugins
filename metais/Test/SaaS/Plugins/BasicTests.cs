using NUnit.Framework;
using System;
using System.Reflection;
using System.IO;
using System.Collections;
using System.Collections.Generic;


using MetaIS.SaaS.Plugins;

namespace template_test.UnitTests
{
    [TestFixture]
    public class BasicTests
    {
        //const string testFilesSubPath = @"\Test\SaaS\Plugins\Files\";
        const string runnerNameSpace = @"MetaIS.SaaS.Plugins.PluginRunner";
        const string pluginsSubDir = @"PluginsTest";

        [SetUp] public void Setup() {
        }

        [TearDown] public void TestTearDown() {}

        #region " Plugin Creators "

        static public Plugin CreatePlugin_RockStar(string dllRoot, string dllName, string[] dllRefs)
        {
            string code = @"
                using System;
                using System.Drawing;
                namespace DynamicPlugins {
                  public static class RockStar {
                    public static int GetValue(int x) {
                      return MetaIS.SaaS.Plugins.HelperPlugin.GetMirrorValue(x);
                    }
                  }
                }";
            return HelperPlugin.CreatePlugin("RockStar", "Return the input int.", dllRoot, dllName, new string[] {code}, 
                "DynamicPlugins.RockStar", dllRefs);
        }

        public static Plugin CreatePlugin_CodeMirror(string dllRoot, string dllName, string[] dllRefs) {
            string code = @"
                using System;
                namespace DynamicPlugins {
                    public class CodeMirror {
                    public int MirrorInt(int x) {
                        return MetaIS.SaaS.Plugins.HelperPlugin.GetMirrorValue(x);
                    }
                    }
                }";
            Plugin plugin = HelperPlugin.CreatePlugin("CodeMirror", "Return the input int.", 
                dllRoot, dllName, new string[] {code}, "DynamicPlugins.CodeMirror", dllRefs);

            return plugin;
        }

        public static Plugin CreatePlugin_CodeMultiplier(string dllRoot, string dllName, string[] dllRefs) {
            string code = @"
                using System;
                namespace DynamicPlugins {
                    public class CodeMultiplier {
                    public int MultBy2(int x) {
                        DynamicPlugins.CodeMirror obj = new DynamicPlugins.CodeMirror();
                        return (int)obj.MirrorInt(x) * 2;
                    }
                    }
                }";
            Plugin plugin =  HelperPlugin.CreatePlugin("CodeMultiplier", "Return the input int multiplied by 2.", 
                dllRoot, dllName, new string[] {code}, "DynamicPlugins.CodeMultiplier", dllRefs);

            return plugin;
        }

        #endregion

        #region " TestCase-Helper - Tests the HelperPlugin static class - The raw functionality without any imposed implementation. " 

        #region " Test Inputs "

        public static IEnumerable Input_TestHelper3 {
            get {
                string baseDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\";   // path to bin
                string subDir = pluginsSubDir;
                string dllRoot = baseDir + subDir + @"\";

                string mirrorName = "_TestHelper3_CodeMirror.dll";
                string multName = "_TestHelper3_CodeMultiplier.dll";

                //string dllRoot = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\PluginsTest\";   // path to bin

                Plugin plugin1 = CreatePlugin_CodeMirror(dllRoot, mirrorName, null);
                Plugin plugin2 = CreatePlugin_CodeMultiplier(dllRoot, multName, new string[] {mirrorName});
                List<Plugin> pluginSet = new List<Plugin>(){plugin1,plugin2};
                
                yield return new TestCaseData("Dual Class 1", "TestDomain", baseDir, subDir, runnerNameSpace, 
                    pluginSet, "7", mirrorName, "DynamicPlugins.CodeMirror", "MirrorInt", new object[] {(int)7});     

                yield return new TestCaseData("Dual Class 2", "TestDomain", baseDir, subDir, runnerNameSpace,
                    pluginSet, "14", multName, "DynamicPlugins.CodeMultiplier", "MultBy2", new object[] {(int)7});     
            }
        }

        public static IEnumerable Input_TestHelper2 {
            get {
                string baseDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\";   // path to bin
                string subDir = pluginsSubDir;
                string dllRoot = baseDir + subDir + @"\";

                string mirrorName = "_TestHelper2_CodeMirror.dll";

                Plugin plugin1 = CreatePlugin_CodeMirror(dllRoot, mirrorName, null);
                List<Plugin> pluginSet = new List<Plugin>(){plugin1};

                yield return new TestCaseData("Simple Class calling main library.", "TestDomain", baseDir, subDir, runnerNameSpace, 
                    pluginSet, "7", mirrorName, "DynamicPlugins.CodeMirror", "MirrorInt", new object[] {(int)7});                                    
            }
        }

        public static IEnumerable Input_TestHelper1 {
            get {
                string baseDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\";   // path to bin
                string subDir = pluginsSubDir;
                string dllRoot = baseDir + subDir + @"\";

                string mirrorName = "_TestHelper1_CodeMirror.dll";

                Plugin plugin1 = CreatePlugin_CodeMirror(dllRoot, mirrorName, null);
                List<Plugin> pluginSet = new List<Plugin>(){plugin1};

                yield return new TestCaseData("Simple Class", "TestDomain", baseDir, subDir, runnerNameSpace, 
                    pluginSet, "7", mirrorName, "DynamicPlugins.CodeMirror", "MirrorInt", new object[] {(int)7});                                    
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

        #region " TestCase-Domain - Adds implementation to HelperPlugin "

        #region " Test Inputs "

        public static IEnumerable Input_TestPluginDomain1 {
            get {
                string baseDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\";   // path to bin
                string subDir = pluginsSubDir;
                string dllRoot = baseDir + subDir + @"\";

                string mirrorName = "_TestPluginDomain1_CodeMirror.dll";

                Plugin plugin1 = CreatePlugin_CodeMirror(dllRoot, mirrorName, null);
                List<Plugin> pluginSet = new List<Plugin>(){plugin1};

                yield return new TestCaseData("Simple Class", "TestDomain", baseDir, subDir, runnerNameSpace, pluginSet, "7", 
                    "_TestPluginDomain1_CodeMirror.dll", "DynamicPlugins.CodeMirror", "MirrorInt", new object[] {(int)7});                                    
            }
        }

        #endregion

        //[Ignore("")]
        [Test]
        [TestCaseSource("Input_TestPluginDomain1")]
        public void TestPluginDomain(string TestName, string domainName, string domainBaseDir, string domainSubDir, string runnerNamespace, List<Plugin> pluginSet, string expected, string runPluginID, string runClassPath, string runMethodName, object[] runArgs) {

            // Create our domain
            PluginDomain domain = new PluginDomain(domainName, domainBaseDir, domainSubDir, runnerNamespace);

            // Before recompiling a plugin for a domain, we have to unload it
            //      Not really needed here except for completeness of example
            domain.ResetDomain();

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


            // Unload our domain
            domain.Dispose();
        }

        #endregion

        #region " TestCase-System - Adds more implementation to HelperPlugin "

        #region " Test Inputs "

        public static IEnumerable Input_TestPluginSystem1 {
            get {
                string baseDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\";   // path to bin
                string subDir = pluginsSubDir;
                string dllRoot = baseDir + subDir + @"\";

                string mirrorName = "_TestPluginSystem1_CodeMirror.dll";
                string multName = "_TestPluginSystem1_CodeMultiplier.dll";
                string rockName = "_TestPluginSystem1_RockStar.dll";

                Plugin plugin1 = CreatePlugin_CodeMirror(dllRoot, mirrorName, null);
                Plugin plugin2 = CreatePlugin_RockStar(dllRoot, rockName, null);
                Plugin plugin3 = CreatePlugin_CodeMultiplier(dllRoot, multName, new string[] {mirrorName, rockName});

                List<Plugin> pluginSet = new List<Plugin>(){plugin1, plugin2, plugin3};

                yield return new TestCaseData("Simple Class", "TestDomain", baseDir, subDir, runnerNameSpace, pluginSet); 
                    //pluginSet, "7", "_TestPluginSystem1_CodeMirror.dll", "DynamicPlugins.CodeMirror", "MirrorInt", new object[] {(int)7});                                    
                    
            }
        }

        #endregion

        //[Ignore("")]
        [Test]
        [TestCaseSource("Input_TestPluginSystem1")]
        public void TestPluginSystem(string TestName, string domainName, string domainBaseDir, string domainSubDir, string runnerNamespace, List<Plugin> pluginSet) {

            // , List<Plugin> pluginSet, string expected, string runPluginID, string runClassPath, string runMethodName, object[] runArgs

            // Load the plugins into the system
            PluginSystem pluginSystem = new PluginSystem(domainName, domainBaseDir, domainSubDir, runnerNamespace);            
            pluginSystem.PluginSystemLoad(pluginSet);

            // Load the plugins into the domains
            pluginSystem.PluginDomainLoad("AppDomain1", new List<string>() { pluginSet[0].PluginID, pluginSet[1].PluginID, pluginSet[2].PluginID});
            pluginSystem.PluginDomainLoad("AppDomain2", new List<string>() { pluginSet[0].PluginID, pluginSet[1].PluginID});
            pluginSystem.PluginDomainLoad("AppDomain3", new List<string>() { pluginSet[0].PluginID});

            object objA = pluginSystem.InvokeMethod("AppDomain1", pluginSet[0].PluginID, pluginSet[0].ClassNamespacePath, "MirrorInt", new object[] {7});
            string sA = HelperPlugin.ObjectToString(objA);
            Assert.NotNull(objA);
            Assert.AreEqual("7", sA, "Expected: 7  but got: " + sA);

            object objB = pluginSystem.InvokeMethod("AppDomain1", pluginSet[2].PluginID, pluginSet[2].ClassNamespacePath, "MultBy2", new object[] {7});
            string sB = HelperPlugin.ObjectToString(objB);
            Assert.NotNull(objB);
            Assert.AreEqual("14", sB, "Expected: 14  but got: " + sB);
        }

        #endregion


    }
}
  