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
            //Test1();
            BuildSystemA();
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

            Plugin oPluginA = CreatePluginA();
            Plugin oPluginB = CreatePluginB();

            PluginReference pluginReferenceA = pluginSystem.PluginAdd(oPluginA, "AppDomain1");            
            PluginReference pluginReferenceB = pluginSystem.PluginAdd(oPluginB, "AppDomain1");
            pluginReferenceB.PluginDomain.OutputAssemblies(pluginReferenceB);


            object resA = pluginReferenceA.PluginDomain.RunPlugin(pluginReferenceA.Plugin, "DynamicPlugins.CodeMirror", "MirrorInt", new object[] {(int)7});
            string sResA = "NULL";
            if(resA!=null)
                sResA = resA.ToString();
            System.Console.WriteLine(sResA);

            
            object resB = pluginReferenceB.PluginDomain.RunPlugin(pluginReferenceB.Plugin, "DynamicPlugins.CodeMultiplier", "MultBy2", new object[] {(int)7});
            string sResB = "NULL";
            if(resB!=null)
                sResB = resB.ToString();
            System.Console.WriteLine(sResB);



            /*
            PluginReference pluginReferenceA2 = pluginSystem.PluginAdd(oPluginA, "AppDomain2");            
            //PluginReference pluginReferenceB2 = pluginSystem.PluginAdd(oPluginB, "AppDomain2");            
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

        
        protected Plugin CreatePluginB()
        {
            /*
            string code1 = @"
                using System;
                namespace DynamicPlugins {
                  public class CSCodeEvaler {
                    public int MirrorCode(int x) {
                      return x;
                    }
                  }
                }
            ";
            */
            string code2 = 
                "using System;" + Environment.NewLine +
                "namespace DynamicPlugins {" + Environment.NewLine +
                "  public class CodeMultiplier {" + Environment.NewLine +
                "    public int MultBy2(int x) {" + Environment.NewLine +
                //"      CodeMirror obj = new CodeMirror();" + Environment.NewLine +
                //"      return (int)obj.MirrorInt(x) * 2;" + Environment.NewLine +
                "        return x * 2;" + Environment.NewLine +
                "    }" + Environment.NewLine +
                "  }" + Environment.NewLine +
                "}";
            return CreatePlugin("CodeMultiplier", "Return the input int multiplied by 2.", new string[] {code2}, 
                //"DynamicPlugins.CodeMultiplier", "CodeMultiplier.dll", null);
                "DynamicPlugins.CodeMultiplier", "CodeMultiplier.dll", new string[] {"CodeMirror.dll"});
        }

        protected Plugin CreatePluginA()
        {
            string code1 = 
                "using System;" + Environment.NewLine +
                "namespace DynamicPlugins {" + Environment.NewLine +
                "  public class CodeMirror {" + Environment.NewLine +
                "    public int MirrorInt(int x) {" + Environment.NewLine +
                "      return x;" + Environment.NewLine +
                "    }" + Environment.NewLine +
                "  }" + Environment.NewLine +
                "}";
            return CreatePlugin("CodeMirror", "Return the input int.", new string[] {code1}, 
                "DynamicPlugins.CodeMirror", "CodeMirror.dll", null);
        }
        

        protected Plugin CreatePlugin(string name, string desc, string[] code, string codeNamespacePath, string dllFileName, string[] dllCustomRefs)
        {
            string plugginRoot = Application.StartupPath + @"\DynamicPlugins\";

            List<string> referencedAssemblySet = new List<string>();
            referencedAssemblySet.Add("system.dll");
            referencedAssemblySet.Add("system.drawing.dll");
            referencedAssemblySet.Add("saas_plugins.dll");
            if(dllCustomRefs != null) {
                foreach(string reference in dllCustomRefs)
                    referencedAssemblySet.Add(plugginRoot + reference);
            }

            
            //string plugginRoot = Application.StartupPath + @"\";
            System.IO.Directory.CreateDirectory(plugginRoot);
            Plugin oPlugin = new Plugin("", "", plugginRoot, dllFileName, referencedAssemblySet, codeNamespacePath, code);

            return oPlugin;
        }

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

            
            PluginA = CreatePluginA();
            PluginB = CreatePluginB();

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
            */

            pluginDomain.ResetDomain();

            pluginDomain.CompilePlugin(PluginA);
            pluginDomain.CompilePlugin(PluginB);

            pluginDomain.LoadPlugin(PluginA);
            pluginDomain.LoadPlugin(PluginB);
            //oPluginDomain.OutputAssemblies(PluginA);

            
            object resA = pluginDomain.RunPlugin(PluginA, "DynamicPlugins.CodeMirror", "MirrorInt", new object[] {(int)7});
            string sResA = "NULL";
            if(resA!=null)
                sResA = resA.ToString();
            System.Console.WriteLine(sResA);
            
            object resB = pluginDomain.RunPlugin(PluginB, "DynamicPlugins.CodeMultiplier", "MultBy2", new object[] {(int)7});
            string sResB = "NULL";
            if(resB!=null)
                sResB = resB.ToString();
            System.Console.WriteLine(sResB);
            


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
                "      return saas_plugins.SaaS.EvalEngine2.GetTestValue(x*2);" + Environment.NewLine +
                "    }" + Environment.NewLine +
                "  }" + Environment.NewLine +
                "}" + Environment.NewLine;
            AddCode(new string[] {code2, code1}, "DynamicPlugins.CSCodeEvaler");
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
                "      return saas_plugins.SaaS.EvalEngine2.GetTestValue(x*2);" + Environment.NewLine +
                "    }" + Environment.NewLine +
                "  }" + Environment.NewLine +
                "}" + Environment.NewLine;
            AddCode(new string[] {code2}, "DynamicPlugins.RockStar", "PluginB.dll");
            AddCode(new string[] {code1}, "DynamicPlugins.CSCodeEvaler", "PluginA.dll");       // Add a dynamic assembly ????
            */
            
            //AddCode(code1, "ad2csv.SaaS.CompilerRunner.CSCodeEvaler");
        }


        protected Plugin AddCode(string[] code, string codeNamespacePath, string dllFileName, string[] dllCustomRefs) {
            List<string> referencedAssemblySet = new List<string>();
            referencedAssemblySet.Add("system.dll");
            referencedAssemblySet.Add("system.drawing.dll");
            referencedAssemblySet.Add("saas_plugins.dll");
            if(dllCustomRefs != null) {
                foreach(string reference in dllCustomRefs)
                    referencedAssemblySet.Add(reference);
            }

            string plugginRoot = Application.StartupPath + @"\DynamicPlugins\";
            //string plugginRoot = Application.StartupPath + @"\";
            System.IO.Directory.CreateDirectory(plugginRoot);
            Plugin oPlugin = new Plugin("", "", plugginRoot, dllFileName, referencedAssemblySet, 
                codeNamespacePath, code);

            //pluginDomain.CompilePlugin(oPlugin);

            return oPlugin;
        }

        #endregion
    }
}
