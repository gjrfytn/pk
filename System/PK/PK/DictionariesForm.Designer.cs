namespace PK
{
    partial class DictionariesForm
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
            this.dgvDictionaries = new System.Windows.Forms.DataGridView();
            this.dgvDictionaries_ID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvDictionaries_Name = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvDictionaryItems = new System.Windows.Forms.DataGridView();
            this.dgvDictionaryItems_ID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvDictionaryItems_Name = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDictionaries)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDictionaryItems)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvDictionaries
            // 
            this.dgvDictionaries.AllowUserToAddRows = false;
            this.dgvDictionaries.AllowUserToDeleteRows = false;
            this.dgvDictionaries.AllowUserToOrderColumns = true;
            this.dgvDictionaries.AllowUserToResizeColumns = false;
            this.dgvDictionaries.AllowUserToResizeRows = false;
            this.dgvDictionaries.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvDictionaries.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dgvDictionaries_ID,
            this.dgvDictionaries_Name});
            this.dgvDictionaries.Dock = System.Windows.Forms.DockStyle.Left;
            this.dgvDictionaries.Location = new System.Drawing.Point(0, 0);
            this.dgvDictionaries.MultiSelect = false;
            this.dgvDictionaries.Name = "dgvDictionaries";
            this.dgvDictionaries.ReadOnly = true;
            this.dgvDictionaries.RowHeadersVisible = false;
            this.dgvDictionaries.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dgvDictionaries.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvDictionaries.Size = new System.Drawing.Size(354, 608);
            this.dgvDictionaries.TabIndex = 0;
            this.dgvDictionaries.RowEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvDictionaries_RowEnter);
            // 
            // dgvDictionaries_ID
            // 
            this.dgvDictionaries_ID.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dgvDictionaries_ID.FillWeight = 10F;
            this.dgvDictionaries_ID.HeaderText = "ID";
            this.dgvDictionaries_ID.MinimumWidth = 35;
            this.dgvDictionaries_ID.Name = "dgvDictionaries_ID";
            this.dgvDictionaries_ID.ReadOnly = true;
            this.dgvDictionaries_ID.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            // 
            // dgvDictionaries_Name
            // 
            this.dgvDictionaries_Name.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dgvDictionaries_Name.HeaderText = "Наименование";
            this.dgvDictionaries_Name.Name = "dgvDictionaries_Name";
            this.dgvDictionaries_Name.ReadOnly = true;
            this.dgvDictionaries_Name.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            // 
            // dgvDictionaryItems
            // 
            this.dgvDictionaryItems.AllowUserToAddRows = false;
            this.dgvDictionaryItems.AllowUserToDeleteRows = false;
            this.dgvDictionaryItems.AllowUserToOrderColumns = true;
            this.dgvDictionaryItems.AllowUserToResizeColumns = false;
            this.dgvDictionaryItems.AllowUserToResizeRows = false;
            this.dgvDictionaryItems.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvDictionaryItems.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dgvDictionaryItems_ID,
            this.dgvDictionaryItems_Name});
            this.dgvDictionaryItems.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvDictionaryItems.Location = new System.Drawing.Point(354, 0);
            this.dgvDictionaryItems.MultiSelect = false;
            this.dgvDictionaryItems.Name = "dgvDictionaryItems";
            this.dgvDictionaryItems.ReadOnly = true;
            this.dgvDictionaryItems.RowHeadersVisible = false;
            this.dgvDictionaryItems.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dgvDictionaryItems.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvDictionaryItems.Size = new System.Drawing.Size(454, 608);
            this.dgvDictionaryItems.TabIndex = 1;
            // 
            // dgvDictionaryItems_ID
            // 
            this.dgvDictionaryItems_ID.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dgvDictionaryItems_ID.FillWeight = 10F;
            this.dgvDictionaryItems_ID.HeaderText = "ID";
            this.dgvDictionaryItems_ID.MinimumWidth = 35;
            this.dgvDictionaryItems_ID.Name = "dgvDictionaryItems_ID";
            this.dgvDictionaryItems_ID.ReadOnly = true;
            this.dgvDictionaryItems_ID.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            // 
            // dgvDictionaryItems_Name
            // 
            this.dgvDictionaryItems_Name.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dgvDictionaryItems_Name.HeaderText = "Наименование";
            this.dgvDictionaryItems_Name.Name = "dgvDictionaryItems_Name";
            this.dgvDictionaryItems_Name.ReadOnly = true;
            this.dgvDictionaryItems_Name.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            // 
            // DictionariesForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(808, 608);
            this.Controls.Add(this.dgvDictionaryItems);
            this.Controls.Add(this.dgvDictionaries);
            this.Name = "DictionariesForm";
            this.Text = "Справочники ФИС";
            ((System.ComponentModel.ISupportInitialize)(this.dgvDictionaries)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDictionaryItems)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvDictionaries;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgvDictionaries_ID;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgvDictionaries_Name;
        private System.Windows.Forms.DataGridView dgvDictionaryItems;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgvDictionaryItems_ID;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgvDictionaryItems_Name;
    }
}