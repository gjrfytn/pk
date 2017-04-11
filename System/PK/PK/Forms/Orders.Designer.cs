﻿namespace PK.Forms
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Orders));
            this.dataGridView = new System.Windows.Forms.DataGridView();
            this.dataGridView_Number = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridView_Type = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridView_Date = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridView_ProtNumber = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.toolStrip_New = new System.Windows.Forms.ToolStripButton();
            this.toolStrip_Edit = new System.Windows.Forms.ToolStripButton();
            this.toolStrip_Register = new System.Windows.Forms.ToolStripButton();
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
            this.dataGridView_ProtNumber});
            this.dataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView.Location = new System.Drawing.Point(0, 25);
            this.dataGridView.Name = "dataGridView";
            this.dataGridView.ReadOnly = true;
            this.dataGridView.Size = new System.Drawing.Size(669, 516);
            this.dataGridView.TabIndex = 0;
            // 
            // dataGridView_Number
            // 
            this.dataGridView_Number.HeaderText = "Номер";
            this.dataGridView_Number.Name = "dataGridView_Number";
            this.dataGridView_Number.ReadOnly = true;
            // 
            // dataGridView_Type
            // 
            this.dataGridView_Type.HeaderText = "Тип";
            this.dataGridView_Type.Name = "dataGridView_Type";
            this.dataGridView_Type.ReadOnly = true;
            // 
            // dataGridView_Date
            // 
            this.dataGridView_Date.HeaderText = "Дата";
            this.dataGridView_Date.Name = "dataGridView_Date";
            this.dataGridView_Date.ReadOnly = true;
            // 
            // dataGridView_ProtNumber
            // 
            this.dataGridView_ProtNumber.HeaderText = "Номер протокола";
            this.dataGridView_ProtNumber.Name = "dataGridView_ProtNumber";
            this.dataGridView_ProtNumber.ReadOnly = true;
            // 
            // toolStrip
            // 
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStrip_New,
            this.toolStrip_Edit,
            this.toolStrip_Register});
            this.toolStrip.Location = new System.Drawing.Point(0, 0);
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.Size = new System.Drawing.Size(669, 25);
            this.toolStrip.TabIndex = 1;
            this.toolStrip.Text = "toolStrip1";
            // 
            // toolStrip_New
            // 
            this.toolStrip_New.Image = ((System.Drawing.Image)(resources.GetObject("toolStrip_New.Image")));
            this.toolStrip_New.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStrip_New.Name = "toolStrip_New";
            this.toolStrip_New.Size = new System.Drawing.Size(65, 22);
            this.toolStrip_New.Text = "Новый";
            this.toolStrip_New.Click += new System.EventHandler(this.toolStrip_New_Click);
            // 
            // toolStrip_Edit
            // 
            this.toolStrip_Edit.Image = ((System.Drawing.Image)(resources.GetObject("toolStrip_Edit.Image")));
            this.toolStrip_Edit.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStrip_Edit.Name = "toolStrip_Edit";
            this.toolStrip_Edit.Size = new System.Drawing.Size(107, 22);
            this.toolStrip_Edit.Text = "Редактировать";
            // 
            // toolStrip_Register
            // 
            this.toolStrip_Register.Image = ((System.Drawing.Image)(resources.GetObject("toolStrip_Register.Image")));
            this.toolStrip_Register.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStrip_Register.Name = "toolStrip_Register";
            this.toolStrip_Register.Size = new System.Drawing.Size(126, 22);
            this.toolStrip_Register.Text = "Зарегестрировать";
            // 
            // Orders
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(669, 541);
            this.Controls.Add(this.dataGridView);
            this.Controls.Add(this.toolStrip);
            this.Name = "Orders";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Приказы";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridView_Number;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridView_Type;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridView_Date;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridView_ProtNumber;
        private System.Windows.Forms.ToolStrip toolStrip;
        private System.Windows.Forms.ToolStripButton toolStrip_New;
        private System.Windows.Forms.ToolStripButton toolStrip_Edit;
        private System.Windows.Forms.ToolStripButton toolStrip_Register;
    }
}