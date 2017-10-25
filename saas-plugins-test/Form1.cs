using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Data;
using System.Drawing;

using saas_plugins.SaaS;

namespace template_test
{
    public partial class Form1 : Form
    {
        PluginSystem pluginSystem = null;

        public Form1()
        {
            InitializeComponent();
            //Tmr_Init();
            pluginSystem = new PluginSystem("MyDefaultDomain", "saas_plugins.SaaS.PluginRunner");
            pluginSystem.LogNotify += PluginSystem_LogNotify;
        }

        private void PluginSystem_LogNotify(string message) {
            tbLog.Text = message + Environment.NewLine + tbLog.Text;
        }

        #region " Timer "

        System.Windows.Forms.Timer tmr = null;
        bool hasUpdate = false;

        private void Tmr_Init() {
            tmr = new System.Windows.Forms.Timer();
            tmr.Interval = 500;
            tmr.Tick += Tmr_Tick;
            tmr.Enabled = true;
        }

        private void Tmr_Tick(object sender, EventArgs e) {
            if(hasUpdate) {
                //hasUpdate = false;
                //tbValue.Text = RunCode(tbRunning.Text);
            }
        }

        #endregion

        private void Form1_Load(object sender, EventArgs e)
        {   
        }

        private void btnTest_Click(object sender, EventArgs e)
        {
            Test1();
            //BuildSystemA();
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            BuildSystem();
            
        }



        #region " System Test "

        protected void BuildSystemA() {
            tbLog.Text = "";

            Plugin oPluginA = CreatePluginA();
            PluginReference pluginReferenceA = pluginSystem.PluginAdd(oPluginA, "AppDomain1");
            pluginReferenceA.PluginDomain.OutputAssemblies(pluginReferenceA);

            object resA = pluginReferenceA.PluginDomain.RunPlugin(pluginReferenceA.Plugin, "DynamicPlugins.CodeMirror", "MirrorInt", new object[] {(int)7});
            string sResA = "NULL";
            if(resA!=null)
                sResA = resA.ToString();
            System.Console.WriteLine(sResA);
        }

        protected void BuildSystem() {
            tbLog.Text = "";

            Plugin oPluginA = CreatePluginA();  // A simple public class
            Plugin oPluginC = CreatePluginC();  // A static public class
            Plugin oPluginB = CreatePluginB();
            
            oPluginA.IsCompiled = false;
            oPluginB.IsCompiled = false;

            PluginReference pluginReferenceA = pluginSystem.PluginAdd(oPluginA, "AppDomain1");            
            PluginReference pluginReferenceC = pluginSystem.PluginAdd(oPluginC, "AppDomain1");
            PluginReference pluginReferenceB = pluginSystem.PluginAdd(oPluginB, "AppDomain1");
            
            //pluginReferenceB.PluginDomain.OutputAssemblies(pluginReferenceB);

            System.Console.WriteLine(HelperPlugin.RunMethodString(pluginReferenceA.PluginDomain, pluginReferenceA.Plugin, pluginReferenceA.Plugin.ClassNamespacePath, 
                "MirrorInt", new object[] {(int)7}));

            System.Console.WriteLine(HelperPlugin.RunMethodString(pluginReferenceB.PluginDomain, pluginReferenceB.Plugin, pluginReferenceB.Plugin.ClassNamespacePath, 
                "MultBy2", new object[] {(int)7}));

            /*
            object resA = pluginReferenceA.PluginDomain.RunPlugin(pluginReferenceA.Plugin, pluginReferenceA.Plugin.ClassNamespacePath, 
                "MirrorInt", new object[] {(int)7});
            string sResA = "NULL";
            if(resA!=null)
                sResA = resA.ToString();
            System.Console.WriteLine(sResA);

            
            object resB = pluginReferenceB.PluginDomain.RunPlugin(pluginReferenceB.Plugin, pluginReferenceA.Plugin.ClassNamespacePath, 
                "MultBy2", new object[] {(int)7});
            string sResB = "NULL";
            if(resB!=null)
                sResB = resB.ToString();
            System.Console.WriteLine(sResB);
            */


            
            //PluginReference pluginReferenceA2 = pluginSystem.PluginAdd(oPluginA, "AppDomain2");
            //PluginReference pluginReferenceA3 = pluginSystem.PluginAdd(oPluginB, "AppDomain3");
            //PluginReference pluginReferenceB2 = pluginSystem.PluginAdd(oPluginB, "AppDomain2");            
            /*
            pluginReferenceA2.PluginDomain.OutputAssemblies(pluginReferenceA2);

            object resA2 = pluginReferenceA2.PluginDomain.RunPlugin(pluginReferenceA.Plugin, "DynamicPlugins.CodeMirror", "MirrorInt", new object[] {(int)77});
            string sResA2 = "NULL";
            if(resA2!=null)
                sResA2 = resA2.ToString();
            System.Console.WriteLine(sResA2);
            */
            /*
            object resB2 = pluginReferenceB2.PluginDomain.RunPlugin(pluginReferenceB.Plugin, "DynamicPlugins.CodeMultiplier", "MultBy2", new object[] {(int)77});
            string sResB2 = "NULL";
            if(resB2!=null)
                sResB2 = resB2.ToString();
            System.Console.WriteLine(sResB2);
            */

            /*
            
            

            tbLog.Text = "" + Environment.NewLine + tbLog.Text;


            object resA = pluginReferenceA.PluginDomain.RunPlugin(pluginReferenceA.Plugin, "DynamicPlugins.CodeMirror", "MirrorInt", new object[] {(int)7});
            string sResA = "NULL";
            if(resA!=null)
                sResA = resA.ToString();
            System.Console.WriteLine(sResA);
            */
        }

        
        protected Plugin CreatePluginC()
        {
            string code = @"
                using System;
                namespace DynamicPlugins {
                  public static class RockStar {
                    public static int GetValue(int x) {
                      return saas_plugins.SaaS.HelperPlugin.GetMirrorValue(x);
                    }
                  }
                }";
            
            return HelperPlugin.CreatePlugin("RockStar", "Return the input int.", new string[] {code}, 
                "DynamicPlugins.RockStar", "_RockStar.dll", null);
        }
                        
        protected Plugin CreatePluginB()
        {            
            string code2 = @"
                using System;
                using System.Reflection;

                namespace DynamicPlugins {
                  public class CodeMultiplier {
                    CodeMirror obj = null;

                    public int MultBy2(int x) {
                        DynamicPlugins.CodeMirror obj = new DynamicPlugins.CodeMirror();
                        return (int)obj.MirrorInt(x) * 2;
                    }
                  }
                }";
            return HelperPlugin.CreatePlugin("CodeMultiplier", "Return the input int multiplied by 2.", new string[] {code2}, 
                //"DynamicPlugins.CodeMultiplier", "CodeMultiplier.dll", null);
                "DynamicPlugins.CodeMultiplier", "_CodeMultiplier.dll", new string[] {"_CodeMirror.dll", "_RockStar.dll"});
        }

        protected Plugin CreatePluginA()
        {
            // return saas_plugins.SaaS.HelperPlugin.GetMirrorValue(x);
            string code1 = @"
                using System;
                namespace DynamicPlugins {
                  public class CodeMirror {
                    public int MirrorInt(int x) {
                      return saas_plugins.SaaS.HelperPlugin.GetMirrorValue(x);
                    }
                  }
                }";
            
            return HelperPlugin.CreatePlugin("CodeMirror", "Return the input int.", new string[] {code1}, 
                "DynamicPlugins.CodeMirror", "_CodeMirror.dll", null);
        }
        
        /*
        protected Plugin CreatePlugin(string name, string desc, string[] code, string codeNamespacePath, string dllFileName, string[] dllCustomRefs)
        {
            // alternate dll directories seem to cause issues
            //string plugginRoot = Application.StartupPath + @"\DynamicPlugins\";
            string plugginRoot = Application.StartupPath + @"\";
            //string libDllPath = Application.StartupPath + @"\saas_plugins.dll";

            List<string> referencedAssemblySet = new List<string>();
            referencedAssemblySet.Add("system.dll");
            referencedAssemblySet.Add("system.drawing.dll");
            referencedAssemblySet.Add("saas_plugins.dll");
            //referencedAssemblySet.Add(libDllPath);

            if(dllCustomRefs != null) {
                foreach(string reference in dllCustomRefs)
                    referencedAssemblySet.Add(plugginRoot + reference);
            }

            
            //string plugginRoot = Application.StartupPath + @"\";
            System.IO.Directory.CreateDirectory(plugginRoot);
            Plugin oPlugin = new Plugin("", "", plugginRoot, dllFileName, referencedAssemblySet, codeNamespacePath, code);

            return oPlugin;
        }
        */

        #endregion

        #region " Simple Domain Test - No System "

        PluginDomain pluginDomain = null;
        Plugin PluginA = null;
        Plugin PluginB = null;

        protected void Test1() {
            if(pluginDomain == null) {
                pluginDomain = new PluginDomain("MyDomain", "saas_plugins.SaaS.PluginRunner");
                System.Console.WriteLine("Domain Created: " + "MyDomain");
            }

            
            /*
            // Simple class
            string code1 = 
                "using System;" + Environment.NewLine +
                "namespace DynamicPlugins {" + Environment.NewLine +
                "  public class CSCodeEvaler {" + Environment.NewLine +
                "    public int EvalCode(int x) {" + Environment.NewLine +
                "      return x;" + Environment.NewLine +
                "    }" + Environment.NewLine +
                "  }" + Environment.NewLine +
                "}";
            //PluginA = AddCode(new string[] {code1}, "DynamicPlugins.CSCodeEvaler", "PluginA.dll", null);
            PluginA = CreatePlugin("", "", new string[] {code1}, "DynamicPlugins.CSCodeEvaler", "PluginA.dll", null);

            
            string code2 = 
                "using System;" + Environment.NewLine +
                "namespace DynamicPlugins {" + Environment.NewLine +
                "  public class CSCodeEvaler2 {" + Environment.NewLine +
                "    public int EvalCode(int x) {" + Environment.NewLine +
                "      CSCodeEvaler obj = new CSCodeEvaler();" + Environment.NewLine +
                "      return (int)obj.EvalCode(x) * 2;" + Environment.NewLine +

                //"      return x*2;" + Environment.NewLine +
                "    }" + Environment.NewLine +
                "  }" + Environment.NewLine +
                "}";
            //PluginB = AddCode(new string[] {code2}, "DynamicPlugins.CSCodeEvaler2", "PluginB.dll", new string[] {"PluginA.dll"});
            PluginB = CreatePlugin("", "", new string[] {code2}, "DynamicPlugins.CSCodeEvaler2", "PluginB.dll", new string[] {"PluginA.dll"});
            

            //PluginA = CreatePluginA();
            //PluginB = CreatePluginB();

            pluginDomain.ResetDomain();

            pluginDomain.CompilePlugin(PluginA);
            pluginDomain.CompilePlugin(PluginB);

            pluginDomain.LoadPlugin(PluginA);
            pluginDomain.LoadPlugin(PluginB);
            //oPluginDomain.OutputAssemblies(PluginA);

            OutputCall(pluginDomain, PluginA, "DynamicPlugins.CodeMirror", "MirrorInt", new object[] {(int)7});
            OutputCall(pluginDomain, PluginB, "DynamicPlugins.CodeMultiplier", "MultBy2", new object[] {(int)7});
            */


            /*
            // Two classes interacting and interacting back with main library
            //      add order matters
            string code1 = 
                "using System;" + Environment.NewLine +
                "namespace DynamicPlugins {" + Environment.NewLine +
                "  public class CSCodeEvaler {" + Environment.NewLine +
                "    public object EvalCode(int x) {" + Environment.NewLine +
                "      return RockStar.GetValue(x);" + Environment.NewLine +
                "    }" + Environment.NewLine +
                "  }" + Environment.NewLine +
                "}";
            string code2 = 
                "using System;" + Environment.NewLine +
                "namespace DynamicPlugins {" + Environment.NewLine +
                "  public static class RockStar {" + Environment.NewLine +
                "    public static int GetValue(int x){" + Environment.NewLine +
                "      return saas_plugins.SaaS.HelperPlugin.GetMirrorValue(x)*2;" + Environment.NewLine +
                "    }" + Environment.NewLine +
                "  }" + Environment.NewLine +
                "}" + Environment.NewLine;
            Plugin plugin = AddCode(new string[] {code2, code1}, "DynamicPlugins.CSCodeEvaler", "PluginA.dll", null);
            
            pluginDomain.ResetDomain();
            pluginDomain.CompilePlugin(plugin);
            pluginDomain.LoadPlugin(plugin);
            OutputCall(pluginDomain, plugin, "DynamicPlugins.CSCodeEvaler", "EvalCode", new object[] {(int)7});
            */


            /*
            // Two seperate dynamic plugins interacting (linking to their DLLs) ...... 
            string code1 = 
                "using System;" + Environment.NewLine +
                "namespace DynamicPlugins {" + Environment.NewLine +
                "  public class CSCodeEvaler {" + Environment.NewLine +
                "    public object EvalCode(int x) {" + Environment.NewLine +
                "      return RockStar.GetValue(x);" + Environment.NewLine +
                "    }" + Environment.NewLine +
                "  }" + Environment.NewLine +
                "}";
            string code2 = 
                "using System;" + Environment.NewLine +
                "namespace DynamicPlugins {" + Environment.NewLine +
                "  public static class RockStar {" + Environment.NewLine +
                "    public static int GetValue(int x){" + Environment.NewLine +
                "      return saas_plugins.SaaS.HelperPlugin.GetMirrorValue(x)*2;" + Environment.NewLine +
                "    }" + Environment.NewLine +
                "  }" + Environment.NewLine +
                "}" + Environment.NewLine;
            Plugin plugin2 = AddCode(new string[] {code2}, "DynamicPlugins.RockStar", "_PluginB.dll", null);
            Plugin plugin1 = AddCode(new string[] {code1}, "DynamicPlugins.CSCodeEvaler", "_PluginA.dll", new string[] {"_PluginB.dll"});

            pluginDomain.UnloadDomain();
            pluginDomain.CompilePlugin(plugin2);
            pluginDomain.CompilePlugin(plugin1);
            
            pluginDomain.UnloadDomain();
            pluginDomain.LoadPlugin(plugin2);
            pluginDomain.LoadPlugin(plugin1);
            
            System.Console.WriteLine(HelperPlugin.MethodCallReturnString(pluginDomain, plugin1, "DynamicPlugins.CSCodeEvaler", "EvalCode", new object[] {(int)7}));
            */

            //AddCode(new string[] {code2}, "DynamicPlugins.RockStar", "PluginB.dll");
            //AddCode(new string[] {code1}, "DynamicPlugins.CSCodeEvaler", "PluginA.dll");       // Add a dynamic assembly ????
            
            
            //AddCode(code1, "ad2csv.SaaS.CompilerRunner.CSCodeEvaler");
        }

        /*
        protected Plugin AddCode(string[] code, string codeNamespacePath, string dllFileName, string[] dllCustomRefs) {
            List<string> referencedAssemblySet = new List<string>();
            referencedAssemblySet.Add("system.dll");
            referencedAssemblySet.Add("system.drawing.dll");
            referencedAssemblySet.Add("saas_plugins.dll");

            // alternate dll directories seem to cause issues
            //string plugginRoot = Application.StartupPath + @"\DynamicPlugins\";
            string plugginRoot = Application.StartupPath + @"\";

            if(dllCustomRefs != null) {
                foreach(string reference in dllCustomRefs) {
                    //referencedAssemblySet.Add(plugginRoot + reference);
                    referencedAssemblySet.Add(reference);
                }
            }

            
            //string plugginRoot = Application.StartupPath + @"\";
            System.IO.Directory.CreateDirectory(plugginRoot);
            Plugin oPlugin = new Plugin("", "", plugginRoot, dllFileName, referencedAssemblySet, 
                codeNamespacePath, code);

            //pluginDomain.CompilePlugin(oPlugin);

            return oPlugin;
        }
        */

        #endregion

        private void btnView_Click(object sender, EventArgs e)
        {
            pluginSystem.OutputAssemblies();
        }
    }
}
