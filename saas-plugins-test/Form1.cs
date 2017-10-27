using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Data;
using System.Drawing;
using System.IO;
using System.Reflection;

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

            InitSystem();
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
            this.Width = 1000;
            this.Height = 700;
            this.Left = 50;
            this.Top = 50;
        }


        private void btnView_Click(object sender, EventArgs e)
        {
            pluginSystem.OutputAssemblies();
            //System.Drawing.Color.Blue.ToArgb();
        }

        private void btnInvoke_Click(object sender, EventArgs e)
        {
            Plugin oPluginA = CreatePluginA(dllRoot);
            Plugin oPluginC = CreatePluginC(dllRoot);
            Plugin oPluginB = CreatePluginB(dllRoot);

            object objA = pluginSystem.InvokeMethod("AppDomain1", oPluginA.PluginID, oPluginA.ClassNamespacePath, "MirrorInt", new object[] {7});
            System.Console.WriteLine(HelperPlugin.ObjectToString(objA));

            // Static class --- dosn't invoke
            //object objC = pluginSystem.InvokeMethod("AppDomain1", oPluginC.PluginID, oPluginC.ClassNamespacePath, "GetValue", new object[] {7});
            //System.Console.WriteLine(HelperPlugin.ObjectToString(objC));

            object objB = pluginSystem.InvokeMethod("AppDomain1", oPluginB.PluginID, oPluginB.ClassNamespacePath, "MultBy2", new object[] {7});
            System.Console.WriteLine(HelperPlugin.ObjectToString(objB));
        }


        #region " System Interface "

        private void btnUpdateAll_Click(object sender, EventArgs e)
        {
            // Recompile all plugins
            tbLog.Text = "";
            Plugin oPluginA = CreatePluginA(dllRoot);  // A simple public class
            Plugin oPluginC = CreatePluginC(dllRoot);  // A static public class
            Plugin oPluginB = CreatePluginB(dllRoot);
            List<string> pluginSet = new List<string>() {oPluginA.PluginID, oPluginC.PluginID, oPluginB.PluginID};
            pluginSystem.SystemUpdate(pluginSet);
        }

        private void btnUpdate1_Click(object sender, EventArgs e)
        {
            // Recompile plugin A
            tbLog.Text = "";
            Plugin oPluginA = CreatePluginA(dllRoot);  // A simple public class
            List<string> pluginSet = new List<string>() {oPluginA.PluginID};
            pluginSystem.SystemUpdate(pluginSet);
        }

        private void btnUpdate2_Click(object sender, EventArgs e)
        {
            // Recompile plugin B
            tbLog.Text = "";
            Plugin oPluginB = CreatePluginB(dllRoot);  // A simple public class
            List<string> pluginSet = new List<string>() {oPluginB.PluginID};
            pluginSystem.SystemUpdate(pluginSet);
        }

        private void btnUpdate3_Click(object sender, EventArgs e)
        {
            // Recompile plugin C
            tbLog.Text = "";
            Plugin oPluginC = CreatePluginC(dllRoot);  // A simple public class
            List<string> pluginSet = new List<string>() {oPluginC.PluginID};
            pluginSystem.SystemUpdate(pluginSet);
        }

        private void btnSystemReload_Click(object sender, EventArgs e)
        {
            // Unload / Reload ALL domains
            tbLog.Text = "";
            pluginSystem.SystemReload();
        }

        #endregion


        #region " System Test "

        private string baseDir = "";
        private string subDir = "";
        private string dllRoot = "";

        protected void InitSystem()
        {
            baseDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\";   // path to bin
            subDir = @"Plugins";
            //srcDir = @"PluginSource";
            dllRoot = baseDir + subDir + @"\";

            pluginSystem = new PluginSystem("MyDefaultDomain", this.baseDir, this.subDir, "saas_plugins.SaaS.PluginRunner");
            pluginSystem.LogNotify += PluginSystem_LogNotify;


            /*
            // FUTURE UPGRADE
            // Load our existing plugins into the system
            //      Load from source file --- More then just code, has a few settings also
            //          --- Plugin Publisher Project
            //
            // Check if DLL is based on latest source code .... need to store DLL source code
            */



            // Build the demo plugins
            Plugin oPluginA = CreatePluginA(dllRoot);  // A simple public class
            Plugin oPluginC = CreatePluginC(dllRoot);  // A static public class
            Plugin oPluginB = CreatePluginB(dllRoot);

            // Load plugins into System
            List<Plugin> pluginSet = new List<Plugin>() {oPluginA, oPluginC, oPluginB}; // compile order matters -- determined by references
            pluginSystem.PluginSystemLoad(pluginSet);

            // Load plugins into Domain(s)
            pluginSystem.PluginDomainLoad("AppDomain1", new List<string>() { oPluginA.PluginID, oPluginC.PluginID, oPluginB.PluginID});
            pluginSystem.PluginDomainLoad("AppDomain2", new List<string>() { oPluginA.PluginID, oPluginC.PluginID});
            pluginSystem.PluginDomainLoad("AppDomain3", new List<string>() { oPluginA.PluginID});
        }


        protected Plugin CreatePluginC(string dllRoot)
        {
            string code = @"
                using System;
                using System.Drawing;
                namespace DynamicPlugins {
                  public static class RockStar {
                    public static int GetValue(int x) {
                      //return saas_plugins.SaaS.HelperPlugin.GetMirrorValue(x);
                      return Color.Blue.ToArgb();
                    }
                  }
                }";
            
            //string dllRoot = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\";   // path to bin
            return HelperPlugin.CreatePlugin("RockStar", "Return the input int.", dllRoot,"_RockStar.dll", new string[] {code}, 
                "DynamicPlugins.RockStar", null);
        }
                        
        protected Plugin CreatePluginB(string dllRoot)
        {            
            string code2 = @"
                using System;
                using System.Reflection;

                namespace DynamicPlugins {
                  public class CodeMultiplier {
                    CodeMirror obj = null;

                    public int MultBy2(int x) {
                        DynamicPlugins.CodeMirror obj = new DynamicPlugins.CodeMirror();
                        //return (int)obj.MirrorInt(x) * 2;
                        return (int)obj.MirrorInt(x) * RockStar.GetValue(x);
                    }
                  }
                }";
            //string dllRoot = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\";   // path to bin
            return HelperPlugin.CreatePlugin("CodeMultiplier", "Return the input int multiplied by 2.", dllRoot, "_CodeMultiplier.dll", new string[] {code2}, 
                "DynamicPlugins.CodeMultiplier", new string[] {"_CodeMirror.dll", "_RockStar.dll"});
        }

        protected Plugin CreatePluginA(string dllRoot)
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
            
            //string dllRoot = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\";   // path to bin
            return HelperPlugin.CreatePlugin("CodeMirror", "Return the input int.", dllRoot, "_CodeMirror.dll", new string[] {code1}, 
                "DynamicPlugins.CodeMirror", null);
        }





        #endregion


    }
}
