namespace PK
{
    partial class CampaignsForm
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
            this.dgvPriemComp = new System.Windows.Forms.DataGridView();
            this.btCreatePriemComp = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPriemComp)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvPriemComp
            // 
            this.dgvPriemComp.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvPriemComp.Location = new System.Drawing.Point(12, 55);
            this.dgvPriemComp.Name = "dgvPriemComp";
            this.dgvPriemComp.Size = new System.Drawing.Size(632, 382);
            this.dgvPriemComp.TabIndex = 0;
            // 
            // btCreatePriemComp
            // 
            this.btCreatePriemComp.Location = new System.Drawing.Point(242, 12);
            this.btCreatePriemComp.Name = "btCreatePriemComp";
            this.btCreatePriemComp.Size = new System.Drawing.Size(171, 23);
            this.btCreatePriemComp.TabIndex = 1;
            this.btCreatePriemComp.Text = "Создать приемную кампанию";
            this.btCreatePriemComp.UseVisualStyleBackColor = true;
            this.btCreatePriemComp.Click += new System.EventHandler(this.btCreatePriemComp_Click);
            // 
            // PriemCompForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(659, 449);
            this.Controls.Add(this.btCreatePriemComp);
            this.Controls.Add(this.dgvPriemComp);
            this.Name = "PriemCompForm";
            this.Text = "Приемные кампании";
            ((System.ComponentModel.ISupportInitialize)(this.dgvPriemComp)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvPriemComp;
        private System.Windows.Forms.Button btCreatePriemComp;
    }
}