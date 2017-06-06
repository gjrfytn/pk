namespace PK.Forms
{
    partial class ExaminationMarks
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
            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.toolStrip_Print = new System.Windows.Forms.ToolStripButton();
            this.toolStrip_Clear = new System.Windows.Forms.ToolStripButton();
            this.dataGridView_UID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridView_ApplID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridView_Name = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridView_Mark = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit();
            this.toolStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // dataGridView
            // 
            this.dataGridView.AllowUserToAddRows = false;
            this.dataGridView.AllowUserToDeleteRows = false;
            this.dataGridView.AllowUserToResizeColumns = false;
            this.dataGridView.AllowUserToResizeRows = false;
            this.dataGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridView_UID,
            this.dataGridView_ApplID,
            this.dataGridView_Name,
            this.dataGridView_Mark});
            this.dataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView.Location = new System.Drawing.Point(0, 25);
            this.dataGridView.MultiSelect = false;
            this.dataGridView.Name = "dataGridView";
            this.dataGridView.RowHeadersVisible = false;
            this.dataGridView.Size = new System.Drawing.Size(384, 436);
            this.dataGridView.TabIndex = 0;
            this.dataGridView.CellValidating += new System.Windows.Forms.DataGridViewCellValidatingEventHandler(this.dataGridView_CellValidating);
            this.dataGridView.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView_CellValueChanged);
            // 
            // toolStrip
            // 
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStrip_Print,
            this.toolStrip_Clear});
            this.toolStrip.Location = new System.Drawing.Point(0, 0);
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.Size = new System.Drawing.Size(384, 25);
            this.toolStrip.TabIndex = 1;
            this.toolStrip.Text = "toolStrip1";
            // 
            // toolStrip_Print
            // 
            this.toolStrip_Print.Image = global::PK.Properties.Resources.printer;
            this.toolStrip_Print.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStrip_Print.Name = "toolStrip_Print";
            this.toolStrip_Print.Size = new System.Drawing.Size(66, 22);
            this.toolStrip_Print.Text = "Печать";
            this.toolStrip_Print.Click += new System.EventHandler(this.toolStrip_Print_Click);
            // 
            // toolStrip_Clear
            // 
            this.toolStrip_Clear.Image = global::PK.Properties.Resources.cross;
            this.toolStrip_Clear.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStrip_Clear.Name = "toolStrip_Clear";
            this.toolStrip_Clear.Size = new System.Drawing.Size(122, 22);
            this.toolStrip_Clear.Text = "Очистить оценки";
            this.toolStrip_Clear.Click += new System.EventHandler(this.toolStrip_Clear_Click);
            // 
            // dataGridView_UID
            // 
            this.dataGridView_UID.DataPropertyName = "entrant_id";
            this.dataGridView_UID.HeaderText = "ID";
            this.dataGridView_UID.Name = "dataGridView_UID";
            this.dataGridView_UID.Visible = false;
            // 
            // dataGridView_ApplID
            // 
            this.dataGridView_ApplID.FillWeight = 20F;
            this.dataGridView_ApplID.HeaderText = "УИН";
            this.dataGridView_ApplID.Name = "dataGridView_ApplID";
            this.dataGridView_ApplID.ReadOnly = true;
            // 
            // dataGridView_Name
            // 
            this.dataGridView_Name.HeaderText = "ФИО";
            this.dataGridView_Name.Name = "dataGridView_Name";
            this.dataGridView_Name.ReadOnly = true;
            // 
            // dataGridView_Mark
            // 
            this.dataGridView_Mark.DataPropertyName = "mark";
            this.dataGridView_Mark.FillWeight = 20F;
            this.dataGridView_Mark.HeaderText = "Оценка";
            this.dataGridView_Mark.Name = "dataGridView_Mark";
            // 
            // ExaminationMarks
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(384, 461);
            this.Controls.Add(this.dataGridView);
            this.Controls.Add(this.toolStrip);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "ExaminationMarks";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "ExaminationMarksForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ExaminationMarks_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView;
        private System.Windows.Forms.ToolStrip toolStrip;
        private System.Windows.Forms.ToolStripButton toolStrip_Print;
        private System.Windows.Forms.ToolStripButton toolStrip_Clear;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridView_UID;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridView_ApplID;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridView_Name;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridView_Mark;
    }
}