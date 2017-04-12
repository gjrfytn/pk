namespace PK.Forms
{
    partial class Constants
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
            this.dgvConstants = new System.Windows.Forms.DataGridView();
            this.btSave = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.cbCampaign = new System.Windows.Forms.ComboBox();
            this.dgvConstants_ColumnName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvConstants_Name = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvConstants_Value = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dgvConstants)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvConstants
            // 
            this.dgvConstants.AllowUserToAddRows = false;
            this.dgvConstants.AllowUserToDeleteRows = false;
            this.dgvConstants.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvConstants.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dgvConstants_ColumnName,
            this.dgvConstants_Name,
            this.dgvConstants_Value});
            this.dgvConstants.Location = new System.Drawing.Point(12, 55);
            this.dgvConstants.Name = "dgvConstants";
            this.dgvConstants.RowHeadersVisible = false;
            this.dgvConstants.Size = new System.Drawing.Size(611, 270);
            this.dgvConstants.TabIndex = 0;
            // 
            // btSave
            // 
            this.btSave.Location = new System.Drawing.Point(281, 342);
            this.btSave.Name = "btSave";
            this.btSave.Size = new System.Drawing.Size(75, 23);
            this.btSave.TabIndex = 1;
            this.btSave.Text = "Сохранить";
            this.btSave.UseVisualStyleBackColor = true;
            this.btSave.Click += new System.EventHandler(this.btSave_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(232, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(169, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Выберите приемную кампанию:";
            // 
            // cbCampaign
            // 
            this.cbCampaign.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbCampaign.FormattingEnabled = true;
            this.cbCampaign.Location = new System.Drawing.Point(205, 25);
            this.cbCampaign.Name = "cbCampaign";
            this.cbCampaign.Size = new System.Drawing.Size(220, 21);
            this.cbCampaign.TabIndex = 3;
            this.cbCampaign.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // dgvConstants_ColumnName
            // 
            this.dgvConstants_ColumnName.HeaderText = "Название колонки";
            this.dgvConstants_ColumnName.Name = "dgvConstants_ColumnName";
            this.dgvConstants_ColumnName.ReadOnly = true;
            this.dgvConstants_ColumnName.Visible = false;
            // 
            // dgvConstants_Name
            // 
            this.dgvConstants_Name.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dgvConstants_Name.HeaderText = "Имя";
            this.dgvConstants_Name.Name = "dgvConstants_Name";
            this.dgvConstants_Name.ReadOnly = true;
            // 
            // dgvConstants_Value
            // 
            this.dgvConstants_Value.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.dgvConstants_Value.HeaderText = "Значение";
            this.dgvConstants_Value.Name = "dgvConstants_Value";
            this.dgvConstants_Value.Width = 80;
            // 
            // Constants
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(635, 377);
            this.Controls.Add(this.cbCampaign);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btSave);
            this.Controls.Add(this.dgvConstants);
            this.Name = "Constants";
            this.Text = "Основная информация";
            ((System.ComponentModel.ISupportInitialize)(this.dgvConstants)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvConstants;
        private System.Windows.Forms.Button btSave;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cbCampaign;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgvConstants_ColumnName;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgvConstants_Name;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgvConstants_Value;
    }
}