namespace PK.Forms
{
    partial class DirectionsDictionary
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
			this.dataGridView = new System.Windows.Forms.DataGridView();
			this.dataGridView_ID = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.dataGridView_Name = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.dataGridView_Code = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.dataGridView_QualCode = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.dataGridView_Period = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.dataGridView_UGS_Code = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.dataGridView_UGS_Name = new System.Windows.Forms.DataGridViewTextBoxColumn();
			((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit();
			this.SuspendLayout();
			// 
			// dataGridView
			// 
			this.dataGridView.AllowUserToAddRows = false;
			this.dataGridView.AllowUserToDeleteRows = false;
			this.dataGridView.AllowUserToResizeRows = false;
			this.dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridView_ID,
            this.dataGridView_Name,
            this.dataGridView_Code,
            this.dataGridView_QualCode,
            this.dataGridView_Period,
            this.dataGridView_UGS_Code,
            this.dataGridView_UGS_Name});
			this.dataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
			this.dataGridView.Location = new System.Drawing.Point(0, 25);
			this.dataGridView.MultiSelect = false;
			this.dataGridView.Name = "dataGridView";
			this.dataGridView.ReadOnly = true;
			this.dataGridView.RowHeadersVisible = false;
			this.dataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
			this.dataGridView.Size = new System.Drawing.Size(1008, 536);
			this.dataGridView.TabIndex = 0;
			// 
			// dataGridView_ID
			// 
			this.dataGridView_ID.HeaderText = "ID";
			this.dataGridView_ID.Name = "dataGridView_ID";
			this.dataGridView_ID.ReadOnly = true;
			this.dataGridView_ID.Width = 50;
			// 
			// dataGridView_Name
			// 
			this.dataGridView_Name.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
			this.dataGridView_Name.HeaderText = "Наименование";
			this.dataGridView_Name.MinimumWidth = 100;
			this.dataGridView_Name.Name = "dataGridView_Name";
			this.dataGridView_Name.ReadOnly = true;
			// 
			// dataGridView_Code
			// 
			this.dataGridView_Code.HeaderText = "Код";
			this.dataGridView_Code.Name = "dataGridView_Code";
			this.dataGridView_Code.ReadOnly = true;
			this.dataGridView_Code.Width = 50;
			// 
			// dataGridView_QualCode
			// 
			this.dataGridView_QualCode.HeaderText = "Код квалиф.";
			this.dataGridView_QualCode.Name = "dataGridView_QualCode";
			this.dataGridView_QualCode.ReadOnly = true;
			this.dataGridView_QualCode.Width = 95;
			// 
			// dataGridView_Period
			// 
			this.dataGridView_Period.HeaderText = "Период";
			this.dataGridView_Period.Name = "dataGridView_Period";
			this.dataGridView_Period.ReadOnly = true;
			this.dataGridView_Period.Width = 50;
			// 
			// dataGridView_UGS_Code
			// 
			this.dataGridView_UGS_Code.HeaderText = "Код УГС";
			this.dataGridView_UGS_Code.Name = "dataGridView_UGS_Code";
			this.dataGridView_UGS_Code.ReadOnly = true;
			this.dataGridView_UGS_Code.Width = 75;
			// 
			// dataGridView_UGS_Name
			// 
			this.dataGridView_UGS_Name.HeaderText = "Наимен. УГС";
			this.dataGridView_UGS_Name.Name = "dataGridView_UGS_Name";
			this.dataGridView_UGS_Name.ReadOnly = true;
			this.dataGridView_UGS_Name.Width = 200;
			// 
			// DirectionsDictionary
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1008, 561);
			this.Controls.Add(this.dataGridView);
			this.Icon = global::PK.Properties.Resources.logo;
			this.Name = "DirectionsDictionary";
			this.Text = "Направления (справочник №10 ФИС)";
			this.Controls.SetChildIndex(this.dataGridView, 0);
			((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridView_ID;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridView_Name;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridView_Code;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridView_QualCode;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridView_Period;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridView_UGS_Code;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridView_UGS_Name;
    }
}