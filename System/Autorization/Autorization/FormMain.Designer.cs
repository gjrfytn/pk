namespace Autorization
{
    partial class FormMain
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
            this.tpRegistrator = new System.Windows.Forms.TabPage();
            this.tpAdministrator = new System.Windows.Forms.TabPage();
            this.tpInspector = new System.Windows.Forms.TabPage();
            this.tabControl1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tpRegistrator);
            this.tabControl1.Controls.Add(this.tpInspector);
            this.tabControl1.Controls.Add(this.tpAdministrator);
            this.tabControl1.Location = new System.Drawing.Point(12, 12);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(903, 428);
            this.tabControl1.TabIndex = 0;
            // 
            // tpRegistrator
            // 
            this.tpRegistrator.BackColor = System.Drawing.SystemColors.Control;
            this.tpRegistrator.Location = new System.Drawing.Point(4, 22);
            this.tpRegistrator.Name = "tpRegistrator";
            this.tpRegistrator.Padding = new System.Windows.Forms.Padding(3);
            this.tpRegistrator.Size = new System.Drawing.Size(895, 402);
            this.tpRegistrator.TabIndex = 0;
            this.tpRegistrator.Text = "Регистратор";
            // 
            // tpAdministrator
            // 
            this.tpAdministrator.BackColor = System.Drawing.SystemColors.Control;
            this.tpAdministrator.Location = new System.Drawing.Point(4, 22);
            this.tpAdministrator.Name = "tpAdministrator";
            this.tpAdministrator.Padding = new System.Windows.Forms.Padding(3);
            this.tpAdministrator.Size = new System.Drawing.Size(895, 402);
            this.tpAdministrator.TabIndex = 1;
            this.tpAdministrator.Text = "Администратор";
            // 
            // tpInspector
            // 
            this.tpInspector.BackColor = System.Drawing.SystemColors.Control;
            this.tpInspector.Location = new System.Drawing.Point(4, 22);
            this.tpInspector.Name = "tpInspector";
            this.tpInspector.Size = new System.Drawing.Size(895, 402);
            this.tpInspector.TabIndex = 2;
            this.tpInspector.Text = "Проверяющий";
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(927, 452);
            this.Controls.Add(this.tabControl1);
            this.Name = "FormMain";
            this.Text = "FormMain";
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.Shown += new System.EventHandler(this.FormMain_Shown);
            this.VisibleChanged += new System.EventHandler(this.FormMain_VisibleChanged);
            this.tabControl1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tpRegistrator;
        private System.Windows.Forms.TabPage tpInspector;
        private System.Windows.Forms.TabPage tpAdministrator;
    }
}