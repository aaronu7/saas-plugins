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
        System.Windows.Forms.Timer tmr = null;
        bool hasUpdate = false;
        PluginManager oPluginManager = null;

        public Form1()
        {
            InitializeComponent();
            tmr = new System.Windows.Forms.Timer();
            tmr.Interval = 500;
            tmr.Tick += Tmr_Tick;
            tmr.Enabled = true;

            oPluginManager = new PluginManager("MyDomain", "saas_plugins.SaaS.PluginRunner");
        }


        private void Form1_Load(object sender, EventArgs e)
        {

            
        }

        private void Tmr_Tick(object sender, EventArgs e) {
            if(hasUpdate) {
                //hasUpdate = false;
                //tbValue.Text = RunCode(tbRunning.Text);

            }
        }

        Plugin PluginA = null;
        Plugin PluginB = null;

        private void btnTest_Click(object sender, EventArgs e)
        {
            oPluginManager.LoadPlugin(PluginA);
            oPluginManager.LoadPlugin(PluginB);
            oPluginManager.OutputAssemblies();


            object resA = oPluginManager.RunPlugin(PluginA, "DynamicPlugins.CSCodeEvaler", "EvalCode", new object[] {(int)7});
            string sResA = "NULL";
            if(resA!=null)
                sResA = resA.ToString();
            System.Console.WriteLine(sResA);


            object resB = oPluginManager.RunPlugin(PluginB, "DynamicPlugins.CSCodeEvaler2", "EvalCode", new object[] {(int)7});
            string sResB = "NULL";
            if(resB!=null)
                sResB = resB.ToString();
            System.Console.WriteLine(sResB);


            /*
            object res = oPluginManager.RunPlugin(oPluginA, "DynamicPlugins.CSCodeEvaler", "EvalCode", new object[] {(int)77});

            string sRes = "NULL";
            if(res!=null)
                sRes = res.ToString();

            tbValue.Text = sRes;
            */
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {

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
            //PluginA = AddCode(new string[] {code1}, "DynamicPlugins.CSCodeEvaler", "PluginA.dll", new string[] {"PluginB.dll"});  // can't refernce before it has been added to domain
            PluginA = AddCode(new string[] {code1}, "DynamicPlugins.CSCodeEvaler", "PluginA.dll", null);

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
            PluginB = AddCode(new string[] {code2}, "DynamicPlugins.CSCodeEvaler2", "PluginB.dll", new string[] {"PluginA.dll"});

            
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
            Plugin oPlugin = new Plugin(plugginRoot, dllFileName, referencedAssemblySet, 
                "saas_plugins.SaaS.PluginRunner", "MyDomain",
                codeNamespacePath, code);

            oPluginManager.CompilePlugin(oPlugin);

            return oPlugin;
            /*
            object res = EvalEngine2.CompileQuickRun(
                "ad2csv.dll", "ad2csv.SaaS.CompilerRunner", "MyDomain", 
                referencedAssemblySet, code, 
                "ad2csv.SaaS.CompilerRunner.CSCodeEvaler", "EvalCode", new object[0]);

            string sRes = "NULL";
            if(res!=null)
                sRes = res.ToString();
            return sRes;
            */
        }


        /*
        protected string RunCode(string code) {
            List<string> referencedAssemblySet = new List<string>();
            referencedAssemblySet.Add("system.dll");
            referencedAssemblySet.Add("system.drawing.dll");
            referencedAssemblySet.Add("ad2csv.dll");
            
            object res = EvalEngine2.CompileQuickRun(
                "ad2csv.dll", "ad2csv.SaaS.CompilerRunner", "MyDomain", 
                referencedAssemblySet, code, 
                "ad2csv.SaaS.CompilerRunner.CSCodeEvaler", "EvalCode", new object[0]);

            string sRes = "NULL";
            if(res!=null)
                sRes = res.ToString();
            return sRes;
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            tbRunning.Text = tbCode.Text;
            hasUpdate = true;
        }

        private void btnTest_Click(object sender, EventArgs e)
        {
            string exp = "10 / 2";
            string code = 
                "using System;" + Environment.NewLine +
                "namespace ad2csv.SaaS.CompilerRunner {" + Environment.NewLine +
                "  public class CSCodeEvaler{" + Environment.NewLine +
                "    public object EvalCode(){" + Environment.NewLine +
                "      return RockStar.GetValue();" + Environment.NewLine +
                "    }" + Environment.NewLine +
                "  }" + Environment.NewLine +

                "  public static class RockStar {" + Environment.NewLine +
                "    public static int GetValue(){" + Environment.NewLine +
                //"      return " + exp + ";" + Environment.NewLine +
                "      return ad2csv.SaaS.EvalEngine2.GetTestValue();" + Environment.NewLine +
                "    }" + Environment.NewLine +
                "  }" + Environment.NewLine +

                "}" + Environment.NewLine;

            tbValue.Text = RunCode(code);
        }
        */
    }
}
