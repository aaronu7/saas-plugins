﻿namespace MetaIS_Test
{
    partial class frmTest
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tpSourceCode = new System.Windows.Forms.TabPage();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.lbPlugins = new System.Windows.Forms.ListBox();
            this.tbSourceCode = new System.Windows.Forms.TextBox();
            this.tpPluginSystem = new System.Windows.Forms.TabPage();
            this.lbMethodParams = new System.Windows.Forms.ListBox();
            this.label5 = new System.Windows.Forms.Label();
            this.lbClasses = new System.Windows.Forms.ListBox();
            this.label4 = new System.Windows.Forms.Label();
            this.lbAssemblies = new System.Windows.Forms.ListBox();
            this.label3 = new System.Windows.Forms.Label();
            this.lbDomainPlugins = new System.Windows.Forms.ListBox();
            this.lbDomains = new System.Windows.Forms.ListBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.tbLog = new System.Windows.Forms.TextBox();
            this.btnRunMethod = new System.Windows.Forms.Button();
            this.tbParams = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.splitContainer3 = new System.Windows.Forms.SplitContainer();
            this.btnRecompileCode = new System.Windows.Forms.Button();
            this.btnCompileAll = new System.Windows.Forms.Button();
            this.btnSystemReload = new System.Windows.Forms.Button();
            this.tabControl1.SuspendLayout();
            this.tpSourceCode.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.tpPluginSystem.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).BeginInit();
            this.splitContainer3.Panel1.SuspendLayout();
            this.splitContainer3.Panel2.SuspendLayout();
            this.splitContainer3.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tpSourceCode);
            this.tabControl1.Controls.Add(this.tpPluginSystem);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1774, 896);
            this.tabControl1.TabIndex = 0;
            // 
            // tpSourceCode
            // 
            this.tpSourceCode.Controls.Add(this.splitContainer1);
            this.tpSourceCode.Location = new System.Drawing.Point(8, 39);
            this.tpSourceCode.Name = "tpSourceCode";
            this.tpSourceCode.Padding = new System.Windows.Forms.Padding(3);
            this.tpSourceCode.Size = new System.Drawing.Size(1758, 849);
            this.tpSourceCode.TabIndex = 0;
            this.tpSourceCode.Text = "Source Code";
            this.tpSourceCode.UseVisualStyleBackColor = true;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.Location = new System.Drawing.Point(3, 3);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.lbPlugins);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer3);
            this.splitContainer1.Size = new System.Drawing.Size(1752, 843);
            this.splitContainer1.SplitterDistance = 528;
            this.splitContainer1.TabIndex = 0;
            // 
            // lbPlugins
            // 
            this.lbPlugins.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbPlugins.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.125F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbPlugins.FormattingEnabled = true;
            this.lbPlugins.ItemHeight = 31;
            this.lbPlugins.Location = new System.Drawing.Point(0, 0);
            this.lbPlugins.Name = "lbPlugins";
            this.lbPlugins.Size = new System.Drawing.Size(528, 843);
            this.lbPlugins.TabIndex = 0;
            this.lbPlugins.SelectedIndexChanged += new System.EventHandler(this.lbFileNames_SelectedIndexChanged);
            // 
            // tbSourceCode
            // 
            this.tbSourceCode.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbSourceCode.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbSourceCode.Location = new System.Drawing.Point(0, 0);
            this.tbSourceCode.Multiline = true;
            this.tbSourceCode.Name = "tbSourceCode";
            this.tbSourceCode.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.tbSourceCode.Size = new System.Drawing.Size(1220, 736);
            this.tbSourceCode.TabIndex = 0;
            this.tbSourceCode.WordWrap = false;
            this.tbSourceCode.TextChanged += new System.EventHandler(this.tbSourceCode_TextChanged);
            // 
            // tpPluginSystem
            // 
            this.tpPluginSystem.Controls.Add(this.btnSystemReload);
            this.tpPluginSystem.Controls.Add(this.btnCompileAll);
            this.tpPluginSystem.Controls.Add(this.label6);
            this.tpPluginSystem.Controls.Add(this.tbParams);
            this.tpPluginSystem.Controls.Add(this.btnRunMethod);
            this.tpPluginSystem.Controls.Add(this.lbMethodParams);
            this.tpPluginSystem.Controls.Add(this.label5);
            this.tpPluginSystem.Controls.Add(this.lbClasses);
            this.tpPluginSystem.Controls.Add(this.label4);
            this.tpPluginSystem.Controls.Add(this.lbAssemblies);
            this.tpPluginSystem.Controls.Add(this.label3);
            this.tpPluginSystem.Controls.Add(this.lbDomainPlugins);
            this.tpPluginSystem.Controls.Add(this.lbDomains);
            this.tpPluginSystem.Controls.Add(this.label2);
            this.tpPluginSystem.Controls.Add(this.label1);
            this.tpPluginSystem.Location = new System.Drawing.Point(8, 39);
            this.tpPluginSystem.Name = "tpPluginSystem";
            this.tpPluginSystem.Padding = new System.Windows.Forms.Padding(3);
            this.tpPluginSystem.Size = new System.Drawing.Size(1758, 849);
            this.tpPluginSystem.TabIndex = 1;
            this.tpPluginSystem.Text = "Plugin System";
            this.tpPluginSystem.UseVisualStyleBackColor = true;
            // 
            // lbMethodParams
            // 
            this.lbMethodParams.FormattingEnabled = true;
            this.lbMethodParams.ItemHeight = 25;
            this.lbMethodParams.Location = new System.Drawing.Point(934, 362);
            this.lbMethodParams.Name = "lbMethodParams";
            this.lbMethodParams.Size = new System.Drawing.Size(685, 129);
            this.lbMethodParams.TabIndex = 9;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.875F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(929, 321);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(290, 25);
            this.label5.TabIndex = 8;
            this.label5.Text = "Plugin Method Parameters";
            // 
            // lbClasses
            // 
            this.lbClasses.FormattingEnabled = true;
            this.lbClasses.ItemHeight = 25;
            this.lbClasses.Location = new System.Drawing.Point(842, 188);
            this.lbClasses.Name = "lbClasses";
            this.lbClasses.Size = new System.Drawing.Size(777, 104);
            this.lbClasses.TabIndex = 7;
            this.lbClasses.SelectedIndexChanged += new System.EventHandler(this.lbClasses_SelectedIndexChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.875F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(837, 147);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(241, 25);
            this.label4.TabIndex = 6;
            this.label4.Text = "Plugin Class Methods";
            // 
            // lbAssemblies
            // 
            this.lbAssemblies.FormattingEnabled = true;
            this.lbAssemblies.ItemHeight = 25;
            this.lbAssemblies.Location = new System.Drawing.Point(359, 436);
            this.lbAssemblies.Name = "lbAssemblies";
            this.lbAssemblies.Size = new System.Drawing.Size(462, 179);
            this.lbAssemblies.TabIndex = 5;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.875F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(354, 395);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(226, 25);
            this.label3.TabIndex = 4;
            this.label3.Text = "Running Assemblies";
            // 
            // lbDomainPlugins
            // 
            this.lbDomainPlugins.FormattingEnabled = true;
            this.lbDomainPlugins.ItemHeight = 25;
            this.lbDomainPlugins.Location = new System.Drawing.Point(359, 188);
            this.lbDomainPlugins.Name = "lbDomainPlugins";
            this.lbDomainPlugins.Size = new System.Drawing.Size(462, 179);
            this.lbDomainPlugins.TabIndex = 3;
            this.lbDomainPlugins.SelectedIndexChanged += new System.EventHandler(this.lbDomainPlugins_SelectedIndexChanged);
            // 
            // lbDomains
            // 
            this.lbDomains.FormattingEnabled = true;
            this.lbDomains.ItemHeight = 25;
            this.lbDomains.Location = new System.Drawing.Point(18, 186);
            this.lbDomains.Name = "lbDomains";
            this.lbDomains.Size = new System.Drawing.Size(320, 429);
            this.lbDomains.TabIndex = 2;
            this.lbDomains.SelectedIndexChanged += new System.EventHandler(this.lbDomains_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.875F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(354, 147);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(176, 25);
            this.label2.TabIndex = 1;
            this.label2.Text = "Domain Plugins";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.875F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(13, 145);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(103, 25);
            this.label1.TabIndex = 0;
            this.label1.Text = "Domains";
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.tabControl1);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.tbLog);
            this.splitContainer2.Size = new System.Drawing.Size(1774, 1229);
            this.splitContainer2.SplitterDistance = 896;
            this.splitContainer2.TabIndex = 1;
            // 
            // tbLog
            // 
            this.tbLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbLog.Font = new System.Drawing.Font("Courier New", 10.125F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbLog.Location = new System.Drawing.Point(0, 0);
            this.tbLog.Multiline = true;
            this.tbLog.Name = "tbLog";
            this.tbLog.Size = new System.Drawing.Size(1774, 329);
            this.tbLog.TabIndex = 1;
            // 
            // btnRunMethod
            // 
            this.btnRunMethod.Location = new System.Drawing.Point(1447, 526);
            this.btnRunMethod.Name = "btnRunMethod";
            this.btnRunMethod.Size = new System.Drawing.Size(172, 83);
            this.btnRunMethod.TabIndex = 10;
            this.btnRunMethod.Text = "Run Method";
            this.btnRunMethod.UseVisualStyleBackColor = true;
            this.btnRunMethod.Click += new System.EventHandler(this.btnRunMethod_Click);
            // 
            // tbParams
            // 
            this.tbParams.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.125F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbParams.Location = new System.Drawing.Point(934, 569);
            this.tbParams.Name = "tbParams";
            this.tbParams.Size = new System.Drawing.Size(432, 38);
            this.tbParams.TabIndex = 11;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.875F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(929, 526);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(466, 25);
            this.label6.TabIndex = 12;
            this.label6.Text = "Method Call Arguments (comma separated)";
            // 
            // splitContainer3
            // 
            this.splitContainer3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer3.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer3.Location = new System.Drawing.Point(0, 0);
            this.splitContainer3.Name = "splitContainer3";
            this.splitContainer3.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer3.Panel1
            // 
            this.splitContainer3.Panel1.Controls.Add(this.btnRecompileCode);
            // 
            // splitContainer3.Panel2
            // 
            this.splitContainer3.Panel2.Controls.Add(this.tbSourceCode);
            this.splitContainer3.Size = new System.Drawing.Size(1220, 843);
            this.splitContainer3.SplitterDistance = 103;
            this.splitContainer3.TabIndex = 1;
            // 
            // btnRecompileCode
            // 
            this.btnRecompileCode.Location = new System.Drawing.Point(24, 17);
            this.btnRecompileCode.Name = "btnRecompileCode";
            this.btnRecompileCode.Size = new System.Drawing.Size(248, 77);
            this.btnRecompileCode.TabIndex = 0;
            this.btnRecompileCode.Text = "Compile";
            this.btnRecompileCode.UseVisualStyleBackColor = true;
            this.btnRecompileCode.Click += new System.EventHandler(this.btnRecompileCode_Click);
            // 
            // btnCompileAll
            // 
            this.btnCompileAll.Location = new System.Drawing.Point(38, 20);
            this.btnCompileAll.Name = "btnCompileAll";
            this.btnCompileAll.Size = new System.Drawing.Size(248, 77);
            this.btnCompileAll.TabIndex = 14;
            this.btnCompileAll.Text = "Compile ALL";
            this.btnCompileAll.UseVisualStyleBackColor = true;
            this.btnCompileAll.Click += new System.EventHandler(this.btnCompileAll_Click_1);
            // 
            // btnSystemReload
            // 
            this.btnSystemReload.Location = new System.Drawing.Point(332, 20);
            this.btnSystemReload.Name = "btnSystemReload";
            this.btnSystemReload.Size = new System.Drawing.Size(248, 77);
            this.btnSystemReload.TabIndex = 15;
            this.btnSystemReload.Text = "System Reload";
            this.btnSystemReload.UseVisualStyleBackColor = true;
            this.btnSystemReload.Click += new System.EventHandler(this.btnSystemReload_Click);
            // 
            // frmTest
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1774, 1229);
            this.Controls.Add(this.splitContainer2);
            this.Name = "frmTest";
            this.Text = "Interactive Test";
            this.Load += new System.EventHandler(this.frmTest_Load);
            this.tabControl1.ResumeLayout(false);
            this.tpSourceCode.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.tpPluginSystem.ResumeLayout(false);
            this.tpPluginSystem.PerformLayout();
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            this.splitContainer2.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.splitContainer3.Panel1.ResumeLayout(false);
            this.splitContainer3.Panel2.ResumeLayout(false);
            this.splitContainer3.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).EndInit();
            this.splitContainer3.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tpSourceCode;
        private System.Windows.Forms.TabPage tpPluginSystem;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ListBox lbPlugins;
        private System.Windows.Forms.TextBox tbSourceCode;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.TextBox tbLog;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListBox lbDomains;
        private System.Windows.Forms.ListBox lbDomainPlugins;
        private System.Windows.Forms.ListBox lbAssemblies;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ListBox lbClasses;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ListBox lbMethodParams;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button btnRunMethod;
        private System.Windows.Forms.TextBox tbParams;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.SplitContainer splitContainer3;
        private System.Windows.Forms.Button btnRecompileCode;
        private System.Windows.Forms.Button btnSystemReload;
        private System.Windows.Forms.Button btnCompileAll;
    }
}