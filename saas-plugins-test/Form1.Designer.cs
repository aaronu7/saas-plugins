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
            this.btnSubmit = new System.Windows.Forms.Button();
            this.tbCode = new System.Windows.Forms.TextBox();
            this.tbRunning = new System.Windows.Forms.TextBox();
            this.tbValue = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // btnSubmit
            // 
            this.btnSubmit.Location = new System.Drawing.Point(416, 73);
            this.btnSubmit.Name = "btnSubmit";
            this.btnSubmit.Size = new System.Drawing.Size(223, 106);
            this.btnSubmit.TabIndex = 0;
            this.btnSubmit.Text = "Submit";
            this.btnSubmit.UseVisualStyleBackColor = true;
            this.btnSubmit.Click += new System.EventHandler(this.btnSubmit_Click);
            // 
            // tbCode
            // 
            this.tbCode.Location = new System.Drawing.Point(295, 218);
            this.tbCode.Multiline = true;
            this.tbCode.Name = "tbCode";
            this.tbCode.Size = new System.Drawing.Size(523, 265);
            this.tbCode.TabIndex = 1;
            this.tbCode.Text = "using System;\r\n\r\nnamespace ad2csv.SaaS.CompilerRunner {\r\n  public class CSCodeEva" +
    "ler {\r\n    public object EvalCode() {\r\n      return System.Drawing.Color.Black.T" +
    "oArgb();\r\n    }\r\n  }\r\n}";
            // 
            // tbRunning
            // 
            this.tbRunning.BackColor = System.Drawing.SystemColors.MenuBar;
            this.tbRunning.Enabled = false;
            this.tbRunning.Location = new System.Drawing.Point(295, 525);
            this.tbRunning.Multiline = true;
            this.tbRunning.Name = "tbRunning";
            this.tbRunning.Size = new System.Drawing.Size(523, 265);
            this.tbRunning.TabIndex = 2;
            // 
            // tbValue
            // 
            this.tbValue.BackColor = System.Drawing.SystemColors.MenuBar;
            this.tbValue.Enabled = false;
            this.tbValue.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbValue.ForeColor = System.Drawing.Color.Black;
            this.tbValue.Location = new System.Drawing.Point(892, 560);
            this.tbValue.Multiline = true;
            this.tbValue.Name = "tbValue";
            this.tbValue.Size = new System.Drawing.Size(215, 166);
            this.tbValue.TabIndex = 3;
            this.tbValue.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1249, 924);
            this.Controls.Add(this.tbValue);
            this.Controls.Add(this.tbRunning);
            this.Controls.Add(this.tbCode);
            this.Controls.Add(this.btnSubmit);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnSubmit;
        private System.Windows.Forms.TextBox tbCode;
        private System.Windows.Forms.TextBox tbRunning;
        private System.Windows.Forms.TextBox tbValue;
    }
}

