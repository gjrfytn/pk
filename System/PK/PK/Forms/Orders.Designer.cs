namespace PK.Forms
{
    partial class Orders
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
            this.toolStrip_New = new System.Windows.Forms.ToolStripButton();
            this.toolStrip_Edit = new System.Windows.Forms.ToolStripButton();
            this.toolStrip_Delete = new System.Windows.Forms.ToolStripButton();
            this.toolStrip_Register = new System.Windows.Forms.ToolStripButton();
            this.toolStrip_Print = new System.Windows.Forms.ToolStripButton();
            this.dataGridView_Number = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridView_Type = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridView_Date = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridView_ProtNumber = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridView_EduSource = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridView_EduForm = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridView_Direction = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridView_Profile = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit();
            this.toolStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // dataGridView
            // 
            this.dataGridView.AllowUserToAddRows = false;
            this.dataGridView.AllowUserToResizeColumns = false;
            this.dataGridView.AllowUserToResizeRows = false;
            this.dataGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridView_Number,
            this.dataGridView_Type,
            this.dataGridView_Date,
            this.dataGridView_ProtNumber,
            this.dataGridView_EduSource,
            this.dataGridView_EduForm,
            this.dataGridView_Direction,
            this.dataGridView_Profile});
            this.dataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView.Location = new System.Drawing.Point(0, 25);
            this.dataGridView.MultiSelect = false;
            this.dataGridView.Name = "dataGridView";
            this.dataGridView.ReadOnly = true;
            this.dataGridView.RowHeadersVisible = false;
            this.dataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView.Size = new System.Drawing.Size(984, 436);
            this.dataGridView.TabIndex = 1;
            this.dataGridView.RowEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView_RowEnter);
            this.dataGridView.UserDeletingRow += new System.Windows.Forms.DataGridViewRowCancelEventHandler(this.dataGridView_UserDeletingRow);
            // 
            // toolStrip
            // 
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStrip_New,
            this.toolStrip_Edit,
            this.toolStrip_Delete,
            this.toolStrip_Register,
            this.toolStrip_Print});
            this.toolStrip.Location = new System.Drawing.Point(0, 0);
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.Size = new System.Drawing.Size(984, 25);
            this.toolStrip.TabIndex = 0;
            this.toolStrip.Text = "toolStrip1";
            // 
            // toolStrip_New
            // 
            this.toolStrip_New.Image = global::PK.Properties.Resources.plus;
            this.toolStrip_New.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStrip_New.Name = "toolStrip_New";
            this.toolStrip_New.Size = new System.Drawing.Size(74, 22);
            this.toolStrip_New.Text = "Новый...";
            this.toolStrip_New.Click += new System.EventHandler(this.toolStrip_New_Click);
            // 
            // toolStrip_Edit
            // 
            this.toolStrip_Edit.Image = global::PK.Properties.Resources.pen;
            this.toolStrip_Edit.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStrip_Edit.Name = "toolStrip_Edit";
            this.toolStrip_Edit.Size = new System.Drawing.Size(116, 22);
            this.toolStrip_Edit.Text = "Редактировать...";
            this.toolStrip_Edit.Click += new System.EventHandler(this.toolStrip_Edit_Click);
            // 
            // toolStrip_Delete
            // 
            this.toolStrip_Delete.Image = global::PK.Properties.Resources.cross;
            this.toolStrip_Delete.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStrip_Delete.Name = "toolStrip_Delete";
            this.toolStrip_Delete.Size = new System.Drawing.Size(71, 22);
            this.toolStrip_Delete.Text = "Удалить";
            this.toolStrip_Delete.Click += new System.EventHandler(this.toolStrip_Delete_Click);
            // 
            // toolStrip_Register
            // 
            this.toolStrip_Register.Image = global::PK.Properties.Resources.check;
            this.toolStrip_Register.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStrip_Register.Name = "toolStrip_Register";
            this.toolStrip_Register.Size = new System.Drawing.Size(126, 22);
            this.toolStrip_Register.Text = "Зарегестрировать";
            this.toolStrip_Register.Click += new System.EventHandler(this.toolStrip_Register_Click);
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
            // dataGridView_Number
            // 
            this.dataGridView_Number.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.dataGridView_Number.HeaderText = "Номер";
            this.dataGridView_Number.Name = "dataGridView_Number";
            this.dataGridView_Number.ReadOnly = true;
            this.dataGridView_Number.Width = 80;
            // 
            // dataGridView_Type
            // 
            this.dataGridView_Type.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.dataGridView_Type.HeaderText = "Тип";
            this.dataGridView_Type.Name = "dataGridView_Type";
            this.dataGridView_Type.ReadOnly = true;
            // 
            // dataGridView_Date
            // 
            this.dataGridView_Date.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.dataGridView_Date.HeaderText = "Дата";
            this.dataGridView_Date.Name = "dataGridView_Date";
            this.dataGridView_Date.ReadOnly = true;
            this.dataGridView_Date.Width = 60;
            // 
            // dataGridView_ProtNumber
            // 
            this.dataGridView_ProtNumber.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.dataGridView_ProtNumber.HeaderText = "Номер протокола";
            this.dataGridView_ProtNumber.Name = "dataGridView_ProtNumber";
            this.dataGridView_ProtNumber.ReadOnly = true;
            this.dataGridView_ProtNumber.Width = 65;
            // 
            // dataGridView_EduSource
            // 
            this.dataGridView_EduSource.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.dataGridView_EduSource.HeaderText = "Основание";
            this.dataGridView_EduSource.Name = "dataGridView_EduSource";
            this.dataGridView_EduSource.ReadOnly = true;
            // 
            // dataGridView_EduForm
            // 
            this.dataGridView_EduForm.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.dataGridView_EduForm.HeaderText = "Форма обучения";
            this.dataGridView_EduForm.Name = "dataGridView_EduForm";
            this.dataGridView_EduForm.ReadOnly = true;
            // 
            // dataGridView_Direction
            // 
            this.dataGridView_Direction.HeaderText = "Направление";
            this.dataGridView_Direction.Name = "dataGridView_Direction";
            this.dataGridView_Direction.ReadOnly = true;
            // 
            // dataGridView_Profile
            // 
            this.dataGridView_Profile.HeaderText = "Профиль";
            this.dataGridView_Profile.Name = "dataGridView_Profile";
            this.dataGridView_Profile.ReadOnly = true;
            // 
            // Orders
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(984, 461);
            this.Controls.Add(this.dataGridView);
            this.Controls.Add(this.toolStrip);
            this.MaximizeBox = false;
            this.Name = "Orders";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Приказы";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView;
        private System.Windows.Forms.ToolStrip toolStrip;
        private System.Windows.Forms.ToolStripButton toolStrip_New;
        private System.Windows.Forms.ToolStripButton toolStrip_Edit;
        private System.Windows.Forms.ToolStripButton toolStrip_Register;
        private System.Windows.Forms.ToolStripButton toolStrip_Print;
        private System.Windows.Forms.ToolStripButton toolStrip_Delete;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridView_Number;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridView_Type;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridView_Date;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridView_ProtNumber;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridView_EduSource;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridView_EduForm;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridView_Direction;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridView_Profile;
    }
}