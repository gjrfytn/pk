namespace PK.Forms
{
    partial class DictionaryBase
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
            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.toolStrip_Update = new System.Windows.Forms.ToolStripButton();
            this.toolStrip_Load = new System.Windows.Forms.ToolStripButton();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.toolStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip
            // 
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStrip_Update,
            this.toolStrip_Load});
            this.toolStrip.Location = new System.Drawing.Point(0, 0);
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.Size = new System.Drawing.Size(384, 25);
            this.toolStrip.TabIndex = 2;
            this.toolStrip.Text = "toolStrip1";
            // 
            // toolStrip_Update
            // 
            this.toolStrip_Update.Image = global::PK.Properties.Resources.refresh;
            this.toolStrip_Update.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStrip_Update.Name = "toolStrip_Update";
            this.toolStrip_Update.Size = new System.Drawing.Size(144, 22);
            this.toolStrip_Update.Text = "Обновить через ФИС";
            this.toolStrip_Update.Click += new System.EventHandler(this.toolStrip_Update_Click);
            // 
            // toolStrip_Load
            // 
            this.toolStrip_Load.Image = global::PK.Properties.Resources.from_xml;
            this.toolStrip_Load.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStrip_Load.Name = "toolStrip_Load";
            this.toolStrip_Load.Size = new System.Drawing.Size(132, 22);
            this.toolStrip_Load.Text = "Загрузить из XML...";
            this.toolStrip_Load.Click += new System.EventHandler(this.toolStrip_Load_Click);
            // 
            // openFileDialog
            // 
            this.openFileDialog.DefaultExt = "xml";
            this.openFileDialog.Filter = "XML-файлы|*.xml|Все файлы|*.*";
            // 
            // DictionaryBase
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(384, 262);
            this.Controls.Add(this.toolStrip);
            this.Icon = global::PK.Properties.Resources.logo;
            this.Name = "DictionaryBase";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "DictionaryBase";
            this.Load += new System.EventHandler(this.DictionaryBase_Load);
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStripButton toolStrip_Update;
        private System.Windows.Forms.ToolStripButton toolStrip_Load;
        private System.Windows.Forms.ToolStrip toolStrip;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
    }
}