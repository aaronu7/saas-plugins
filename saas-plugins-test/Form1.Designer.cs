namespace template_test
{
    partial class Form1
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
            this.tbLog = new System.Windows.Forms.TextBox();
            this.btnView = new System.Windows.Forms.Button();
            this.btnUpdateAll = new System.Windows.Forms.Button();
            this.btnUpdate1 = new System.Windows.Forms.Button();
            this.btnUpdate2 = new System.Windows.Forms.Button();
            this.btnSystemReload = new System.Windows.Forms.Button();
            this.btnUpdate3 = new System.Windows.Forms.Button();
            this.btnInvoke = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // tbLog
            // 
            this.tbLog.Location = new System.Drawing.Point(67, 264);
            this.tbLog.Multiline = true;
            this.tbLog.Name = "tbLog";
            this.tbLog.Size = new System.Drawing.Size(707, 583);
            this.tbLog.TabIndex = 1;
            // 
            // btnView
            // 
            this.btnView.Location = new System.Drawing.Point(919, 226);
            this.btnView.Name = "btnView";
            this.btnView.Size = new System.Drawing.Size(223, 106);
            this.btnView.TabIndex = 5;
            this.btnView.Text = "View";
            this.btnView.UseVisualStyleBackColor = true;
            this.btnView.Click += new System.EventHandler(this.btnView_Click);
            // 
            // btnUpdateAll
            // 
            this.btnUpdateAll.Location = new System.Drawing.Point(67, 61);
            this.btnUpdateAll.Name = "btnUpdateAll";
            this.btnUpdateAll.Size = new System.Drawing.Size(223, 106);
            this.btnUpdateAll.TabIndex = 6;
            this.btnUpdateAll.Text = "UpdateAll";
            this.btnUpdateAll.UseVisualStyleBackColor = true;
            this.btnUpdateAll.Click += new System.EventHandler(this.btnUpdateAll_Click);
            // 
            // btnUpdate1
            // 
            this.btnUpdate1.Location = new System.Drawing.Point(318, 12);
            this.btnUpdate1.Name = "btnUpdate1";
            this.btnUpdate1.Size = new System.Drawing.Size(223, 106);
            this.btnUpdate1.TabIndex = 7;
            this.btnUpdate1.Text = "Update1";
            this.btnUpdate1.UseVisualStyleBackColor = true;
            this.btnUpdate1.Click += new System.EventHandler(this.btnUpdate1_Click);
            // 
            // btnUpdate2
            // 
            this.btnUpdate2.Location = new System.Drawing.Point(561, 12);
            this.btnUpdate2.Name = "btnUpdate2";
            this.btnUpdate2.Size = new System.Drawing.Size(223, 106);
            this.btnUpdate2.TabIndex = 8;
            this.btnUpdate2.Text = "Update2";
            this.btnUpdate2.UseVisualStyleBackColor = true;
            this.btnUpdate2.Click += new System.EventHandler(this.btnUpdate2_Click);
            // 
            // btnSystemReload
            // 
            this.btnSystemReload.Location = new System.Drawing.Point(919, 354);
            this.btnSystemReload.Name = "btnSystemReload";
            this.btnSystemReload.Size = new System.Drawing.Size(223, 106);
            this.btnSystemReload.TabIndex = 9;
            this.btnSystemReload.Text = "System Reload";
            this.btnSystemReload.UseVisualStyleBackColor = true;
            this.btnSystemReload.Click += new System.EventHandler(this.btnSystemReload_Click);
            // 
            // btnUpdate3
            // 
            this.btnUpdate3.Location = new System.Drawing.Point(805, 12);
            this.btnUpdate3.Name = "btnUpdate3";
            this.btnUpdate3.Size = new System.Drawing.Size(223, 106);
            this.btnUpdate3.TabIndex = 10;
            this.btnUpdate3.Text = "Update3";
            this.btnUpdate3.UseVisualStyleBackColor = true;
            this.btnUpdate3.Click += new System.EventHandler(this.btnUpdate3_Click);
            // 
            // btnInvoke
            // 
            this.btnInvoke.Location = new System.Drawing.Point(318, 143);
            this.btnInvoke.Name = "btnInvoke";
            this.btnInvoke.Size = new System.Drawing.Size(223, 106);
            this.btnInvoke.TabIndex = 11;
            this.btnInvoke.Text = "Invoke Method";
            this.btnInvoke.UseVisualStyleBackColor = true;
            this.btnInvoke.Click += new System.EventHandler(this.btnInvoke_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1874, 1229);
            this.Controls.Add(this.btnInvoke);
            this.Controls.Add(this.btnUpdate3);
            this.Controls.Add(this.btnSystemReload);
            this.Controls.Add(this.btnUpdate2);
            this.Controls.Add(this.btnUpdate1);
            this.Controls.Add(this.btnUpdateAll);
            this.Controls.Add(this.btnView);
            this.Controls.Add(this.tbLog);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TextBox tbLog;
        private System.Windows.Forms.Button btnView;
        private System.Windows.Forms.Button btnUpdateAll;
        private System.Windows.Forms.Button btnUpdate1;
        private System.Windows.Forms.Button btnUpdate2;
        private System.Windows.Forms.Button btnSystemReload;
        private System.Windows.Forms.Button btnUpdate3;
        private System.Windows.Forms.Button btnInvoke;
    }
}

