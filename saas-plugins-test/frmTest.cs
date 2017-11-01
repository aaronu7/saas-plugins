using System;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;

using saas_plugins.SaaS;

namespace saas_plugins_test
{
    public partial class frmTest : Form
    {
        PluginSystem pluginSystem = null;

        public frmTest()
        {
            InitializeComponent();
            //this.WindowState = FormWindowState.Maximized;
        }

        private void frmTest_Load(object sender, EventArgs e)
        {
            // This quick fixes some control sizing issues
            this.Width = 1000;
            this.Height = 700;
            this.Left = 50;
            this.Top = 50;
            this.splitContainer1.SplitterDistance = this.tabControl1.Width / 4;
            this.splitContainer2.SplitterDistance = this.Height / 2;
            this.tabControl1.SelectedTab = tpPluginSystem;
            InitSystem();
        }

        #region " InitSystem "

        protected void InitSystem()
        {
            string baseDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\";   // path to bin
            string subDir = @"Plugins";
            //srcDir = @"PluginSource";
            string dllRoot = baseDir + subDir + @"\";

            pluginSystem = new PluginSystem("MyDefaultDomain", baseDir, subDir, "saas_plugins.SaaS.PluginRunner");
            pluginSystem.LogNotify += PluginSystem_LogNotify;

            List<Plugin> pluginSet = LoadPluginsFromSource(dllRoot);

            // Load plugins into example Domain(s)
            pluginSystem.PluginDomainLoad("AppDomain1", new List<string>() { pluginSet[0].PluginID, pluginSet[1].PluginID, pluginSet[2].PluginID});
            pluginSystem.PluginDomainLoad("AppDomain2", new List<string>() { pluginSet[0].PluginID, pluginSet[1].PluginID});
            pluginSystem.PluginDomainLoad("AppDomain3", new List<string>() { pluginSet[0].PluginID});
            UpdateDomainSet();
        }

        private void PluginSystem_LogNotify(string message) {
            tbLog.Text = message + Environment.NewLine + tbLog.Text;
        }

        #endregion

        #region " LoadPluginsFromSource "

        protected List<Plugin> LoadPluginsFromSource(string dllRoot) {
            string binFolderPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string fullPath = binFolderPath + @"\UnitTests\Files\";

            string[] fileSet = System.IO.Directory.GetFiles(fullPath);
            List<Plugin> pluginSet = new List<Plugin>();
            lbFileNames.Items.Clear();
            foreach(string file in fileSet) {
                Plugin plugin = CreatePlugin(file, dllRoot);
                if(plugin!=null) {
                    pluginSet.Add(plugin);
                    lbFileNames.Items.Add(plugin);
                }
            }

            // compile order matters -- a production solution should determine order based on the web of references
            List<Plugin> pluginSetSorted = pluginSet.OrderBy(o=>o.CompileOrder).ToList();
            pluginSystem.PluginSystemLoad(pluginSetSorted);

            return pluginSetSorted;
        }

        protected Plugin CreatePlugin(string srcFilePath, string dllRoot)
        {
            // This is a quick and dirty reader to get some plugin settings
            //      definetly NOT recommend for production
            //      it expects this EXACT structure at the top of the file
                        // START PLUGIN SETTINGS
                        // PluginLibraryName = _CodeMirror.dll
                        // PluginReferences  =
                        // END PLUGIN SETTINGS

            string code = System.IO.File.ReadAllText(srcFilePath);
            string[] lines = code.Split('\n');
            string pluginLibName = "";
            List<string> pluginRefs = new List<string>();
            Int32 compileOrder = 0;

            bool watchOn = false;
            foreach(string line in lines) {
                if(line.StartsWith("// START PLUGIN SETTINGS")) {
                    watchOn = true;
                } else if(line.StartsWith("// END PLUGIN SETTINGS")) {
                    break;  
                } else if(watchOn) {
                    if(line.StartsWith("// PluginLibraryName")) {
                        string[] parts = line.Split('=');
                        pluginLibName = parts[1].Trim();
                    } else if(line.StartsWith("// PluginReferences")) {
                        string[] parts = line.Split('=');
                        string[] refs = parts[1].Split(',');
                        foreach(string plugRef in refs) {
                            if(plugRef.Trim() != "")
                                pluginRefs.Add(plugRef.Trim());
                        }
                    } else if(line.StartsWith("// CompileOrder")) {
                        string[] parts = line.Split('=');
                        Int32.TryParse(parts[1].Trim(), out compileOrder);
                    }
                }
            }

            return HelperPlugin.CreatePlugin(pluginLibName, "", dllRoot, pluginLibName, new string[] {code}, "", pluginRefs.ToArray(), compileOrder);
        }

        #endregion

        #region " Source Code "

        private void lbFileNames_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(lbFileNames.SelectedIndex == -1) {
                tbSourceCode.Text = "";
            } else {
                Plugin plugin = (Plugin)lbFileNames.SelectedItem;
                tbSourceCode.Text = plugin.Code[0];
            }
        }

        private void tbSourceCode_TextChanged(object sender, EventArgs e)
        {
            if(lbFileNames.SelectedIndex != -1) {
                Plugin plugin = (Plugin)lbFileNames.SelectedItem;
                plugin.IsCompiled = false;
                plugin.Code = new string[] {tbSourceCode.Text};
            }
        }

        #endregion

        #region " Plugin System "

        protected void UpdateDomainSet()
        {
            lbDomains.Items.Clear();
            lbDomainPlugins.Items.Clear();
            foreach(string domainName in pluginSystem.DomainSet.Keys) {
                lbDomains.Items.Add(pluginSystem.DomainSet[domainName]);
            }
        }

        protected void UpdateDomainPluginSet()
        {
            // Get a list of Plugins
            lbDomainPlugins.Items.Clear();
            if(lbDomains.SelectedIndex != -1) {
                PluginDomain domain = (PluginDomain)lbDomains.SelectedItem;
                foreach(PluginReference pluginRef in domain.PluginReferenceSet.Values) {
                    lbDomainPlugins.Items.Add(pluginRef);
                }
            }

            // Get a list of running assemblies
            lbAssemblies.Items.Clear();
            if(lbDomains.SelectedIndex != -1) {
                PluginDomain domain = (PluginDomain)lbDomains.SelectedItem;
                List<string> asmSet = domain.GetAssemblies();
                foreach(string asm in asmSet) {
                    lbAssemblies.Items.Add(asm);
                }
            }
        }

        private void lbDomains_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateDomainPluginSet();
        }


        private void lbDomainPlugins_SelectedIndexChanged(object sender, EventArgs e)
        {
            // reflection on this plugin
            lbClasses.Items.Clear();
            if(lbDomains.SelectedIndex != -1) {
                PluginReference domain = (PluginReference)lbDomainPlugins.SelectedItem;

                List<string> typeSet = domain.GetPluginAssemblyTypes();
                if(typeSet != null) {
                    foreach(string tp in typeSet) {
                        //lbClasses.Items.Add(tp);

                        List<string> methodSet = domain.GetTypeMethods(tp);
                        foreach(string method in methodSet) {
                            lbClasses.Items.Add(tp + "." + method);
                        }

                        //Type oTp = Type.GetType(tp);
                        //MethodInfo[] methodInfos = Type.GetType(tp).GetMethods(BindingFlags.Public | BindingFlags.Instance);
                        //foreach(MethodInfo method in methodInfos) {
                        //}
                        //string a = "";
                    }
                    
                }
            }
        }

        private void lbClasses_SelectedIndexChanged(object sender, EventArgs e)
        {

        }


        /*

        private void lbClasses_SelectedIndexChanged(object sender, EventArgs e)
        {
            lbMethods.Items.Clear();
            if(lbClasses.SelectedIndex != -1) {
                PluginReference domain = (PluginReference)lbDomainPlugins.SelectedItem;
                string typeName = (string)lbClasses.SelectedItem;
                List<string> methodSet = domain.GetTypeMethods(typeName);

                foreach(string method in methodSet) {

                }
            }
        }
        */
        #endregion


    }
}
