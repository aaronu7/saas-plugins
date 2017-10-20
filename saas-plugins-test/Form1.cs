using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Data;
using System.Drawing;

using ad2csv.SaaS;

namespace template_test
{
    public partial class Form1 : Form
    {
        System.Windows.Forms.Timer tmr = null;
        bool hasUpdate = false;

        public Form1()
        {
            InitializeComponent();
            tmr = new System.Windows.Forms.Timer();
            tmr.Interval = 500;
            tmr.Tick += Tmr_Tick;
            tmr.Enabled = true;
        }


        private void Form1_Load(object sender, EventArgs e)
        {

        }


        private void Tmr_Tick(object sender, EventArgs e) {
            if(hasUpdate) {
                //hasUpdate = false;
                List<string> referencedAssemblySet = new List<string>();
                referencedAssemblySet.Add("system.dll");
                referencedAssemblySet.Add("system.drawing.dll");

                string code = tbRunning.Text;
                object res = EvalEngine2.CompileRun(
                    "ad2csv.dll", "ad2csv.SaaS.CompilerRunner", "MyDomain", 
                    referencedAssemblySet, code, 
                    "ad2csv.SaaS.CompilerRunner.CSCodeEvaler", "EvalCode", new object[0]);

                if(res==null) {
                    tbValue.Text = "NULL";
                } else {
                    tbValue.Text = res.ToString();
                }
            }
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            tbRunning.Text = tbCode.Text;
            hasUpdate = true;
            /*
            string exp = "10 / 2";
            string code = 
                "using System;" + Environment.NewLine +
                "namespace ad2csv.SaaS.CompilerRunner {" + Environment.NewLine +
                "  public class CSCodeEvaler{" + Environment.NewLine +
                "    public object EvalCode(){" + Environment.NewLine +
                "      return " + exp + ";" + Environment.NewLine +
                "    }" + Environment.NewLine +
                "  }" + Environment.NewLine +
                "}" + Environment.NewLine;

            object res = EvalEngine2.RunExpression(
                "ad2csv.dll", "ad2csv.SaaS.CompilerRunner", "MyDomain", 
                code, 
                "ad2csv.SaaS.CompilerRunner.CSCodeEvaler", "EvalCode");

            tbCode.Text = res.ToString();
            */
        }
    }
}
