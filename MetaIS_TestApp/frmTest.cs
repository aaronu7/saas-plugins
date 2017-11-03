using System;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;

using MetaIS.SaaS.Plugins;

namespace MetaIS_Test
{
    public partial class frmTest : Form
    {
        PluginSystem pluginSystem = null;
        const string testFilesSubPath = @"/Test/SaaS/Plugins/Files/";
        const string runnerNameSpace = @"MetaIS.SaaS.Plugins.PluginRunner";
        const string pluginsSubDir = @"Plugins";

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

            if(lbDomains.Items.Count > 0) {
                lbDomains.SelectedIndex = 1;
                lbDomainPlugins.SelectedIndex = 2;
                lbClasses.SelectedIndex = 0;
                tbParams.Text = "7";
                btnRunMethod.BackColor = System.Drawing.Color.Yellow;
            } else {
                tbLog.Text = "*** FAILED TO LOAD EXAMPLE ***";
            }
        }

        #region " InitSystem "

        protected void InitSystem()
        {
            string baseDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"/";   // path to bin
            string subDir = pluginsSubDir;
            //srcDir = @"PluginSource";
            string dllRoot = baseDir + subDir + @"/";

            pluginSystem = new PluginSystem("MyDefaultDomain", baseDir, subDir, runnerNameSpace);
            pluginSystem.LogNotify += PluginSystem_LogNotify;

            List<Plugin> pluginSet = LoadPluginsFromSource(dllRoot);

            // Load plugins into example Domain(s)
            if(pluginSet!=null && pluginSet.Count == 3) {
                pluginSystem.PluginDomainLoad("AppDomain1", new List<string>() { pluginSet[0].PluginID, pluginSet[1].PluginID, pluginSet[2].PluginID});
                pluginSystem.PluginDomainLoad("AppDomain2", new List<string>() { pluginSet[0].PluginID, pluginSet[1].PluginID});
                pluginSystem.PluginDomainLoad("AppDomain3", new List<string>() { pluginSet[0].PluginID});
                UpdateDomainSet();
            }
        }

        private void PluginSystem_LogNotify(string message) {
            LogWrite(message);
        }

        protected void LogWrite(string message)
        {
            tbLog.Text = message + Environment.NewLine + tbLog.Text;
        }
        #endregion

        #region " LoadPluginsFromSource "

        protected List<Plugin> LoadPluginsFromSource(string dllRoot) {
            string binFolderPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string fullPath = binFolderPath + testFilesSubPath;
            List<Plugin> pluginSetSorted = null;

            if(!System.IO.Directory.Exists(fullPath)) {
                System.Console.WriteLine("File path not found: " + fullPath);
            } else {
                string[] fileSet = System.IO.Directory.GetFiles(fullPath);
                List<Plugin> pluginSet = new List<Plugin>();
                lbPlugins.Items.Clear();
                foreach(string file in fileSet) {
                    Plugin plugin = CreatePlugin(file, dllRoot);
                    if(plugin!=null) {
                        pluginSet.Add(plugin);
                        lbPlugins.Items.Add(plugin);
                    }
                }

                // compile order matters -- a production solution should determine order based on the web of references
                pluginSetSorted = pluginSet.OrderBy(o=>o.CompileOrder).ToList();
                pluginSystem.PluginSystemLoad(pluginSetSorted);
            }
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
            if(lbPlugins.SelectedIndex == -1) {
                tbSourceCode.Text = "";
            } else {
                Plugin plugin = (Plugin)lbPlugins.SelectedItem;
                tbSourceCode.Text = plugin.Code[0];
            }
        }

        private void tbSourceCode_TextChanged(object sender, EventArgs e)
        {
            if(lbPlugins.SelectedIndex != -1) {
                Plugin plugin = (Plugin)lbPlugins.SelectedItem;
                plugin.IsCompiled = false;
                plugin.Code = new string[] {tbSourceCode.Text};
            }
        }

        private void btnRecompileCode_Click(object sender, EventArgs e)
        {
            if(lbPlugins.SelectedIndex != -1) {
                // Recompile just this plugin
                tbLog.Text = "";
                Plugin plugin = (Plugin)lbPlugins.SelectedItem;

                List<string> pluginSet = new List<string>() {plugin.PluginID};
                pluginSystem.SystemUpdate(pluginSet);
                UpdateDomainSet();
            }
        }

        #endregion

        #region " Plugin System "

        private void btnCompileAll_Click_1(object sender, EventArgs e)
        {
            // Recompile all plugins
            tbLog.Text = "";
            List<string> pluginSet = new List<string>();
            foreach(Plugin plugin in lbPlugins.Items) {
                pluginSet.Add(plugin.PluginID);
            }
            pluginSystem.SystemUpdate(pluginSet);
            UpdateDomainSet();
        }

        private void btnSystemReload_Click(object sender, EventArgs e)
        {
            // Unload / Reload ALL domains
            tbLog.Text = "";
            pluginSystem.SystemReload();
        }


        protected void UpdateDomainSet()
        {
            lbDomains.Items.Clear();
            lbDomainPlugins.Items.Clear();
            lbAssemblies.Items.Clear();
            lbClasses.Items.Clear();
            lbMethodParams.Items.Clear();
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
                List<string> asmSet = domain.GetDomainAssemblies();
                foreach(string asm in asmSet) {
                    lbAssemblies.Items.Add(asm);
                }
            }
        }

        private void lbDomains_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateDomainPluginSet();
            lbClasses.Items.Clear();
            lbMethodParams.Items.Clear();
        }


        private void lbDomainPlugins_SelectedIndexChanged(object sender, EventArgs e)
        {
            // reflection on this plugin
            lbClasses.Items.Clear();
            lbMethodParams.Items.Clear();
            if(lbDomains.SelectedIndex != -1) {
                PluginReference pluginRef = (PluginReference)lbDomainPlugins.SelectedItem;

                List<string> typeSet = pluginRef.GetTypes();
                if(typeSet != null) {
                    foreach(string tp in typeSet) {
                        //lbClasses.Items.Add(tp);

                        List<string> methodSet = pluginRef.GetTypeMethods(tp);
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
            lbMethodParams.Items.Clear();
            if(lbClasses.SelectedIndex != -1) {
                PluginReference pluginRef = (PluginReference)lbDomainPlugins.SelectedItem;
                string classMethodPath = (string)lbClasses.SelectedItem;
                string methodName = "";
                string classPath = "";
                GetClassMethodParts(classMethodPath, ref classPath, ref methodName);

                List<string> paramSet = pluginRef.GetTypeMethodParams(classPath, methodName);
                foreach(string param in paramSet) {
                    lbMethodParams.Items.Add(param);
                }
            }
        }


        private void btnRunMethod_Click(object sender, EventArgs e)
        {
            tbLog.Text = "";
            
            btnRunMethod.BackColor = tpPluginSystem.BackColor;
            if(lbClasses.SelectedIndex != -1) {
                PluginDomain domain = (PluginDomain)lbDomains.SelectedItem;
                PluginReference pluginRef = (PluginReference)lbDomainPlugins.SelectedItem;
                string classMethodPath = (string)lbClasses.SelectedItem;
                string methodName = "";
                string classPath = "";
                GetClassMethodParts(classMethodPath, ref classPath, ref methodName);

                // Build the argument objects
                List<string> paramSet = pluginRef.GetTypeMethodParams(classPath, methodName);
                string[] paramValues = tbParams.Text.Split(',');
                List<object> args = new List<object>();
                bool isOK = true;

                if(paramSet.Count-1 != paramValues.Length) {
                    LogWrite("Run Result: ERROR");
                } else {
                    try {
                        for(int ix=1; ix<paramSet.Count; ix++) {
                            string dataType = paramSet[ix].Split(':')[0];
                            string dataValue = paramValues[ix-1];
                            if(dataType=="System.Int32") {
                                Int32 i32Value = Int32.Parse(dataValue);
                                args.Add(i32Value);
                            } else if(dataType=="System.Int64") {
                                args.Add(Int64.Parse(dataValue));
                            } else if(dataType=="System.String") {
                                args.Add(dataValue);
                            } else {
                                LogWrite("Run type not supported: " + dataType);
                                isOK = false;
                            }
                        }
                        if(isOK) {
                            object objA = pluginSystem.InvokeMethod(domain.InstanceDomainName, 
                                pluginRef.Plugin.PluginID, 
                                classPath, methodName, args.ToArray<object>());

                            LogWrite("Run Result: " + HelperPlugin.ObjectToString(objA));
                        }

                    } catch {
                        LogWrite("Run Result: ERROR");
                    }
                }
            }
        }

        protected void GetClassMethodParts(string classMethodPath, ref string classPath, ref string methodName)
        {
            string[] parts = classMethodPath.Split('.');
            methodName = parts[parts.Length-1];
                
            parts[parts.Length-1] = "";
            foreach(string part in parts) {
                if(part != "") {
                    classPath = (classPath == "" ? part : classPath + "." + part);
                }
            }
        }

        #endregion


    }
}
