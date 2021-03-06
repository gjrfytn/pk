﻿namespace PK.Forms
{
    partial class Examinations
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.dataGridView = new System.Windows.Forms.DataGridView();
            this.dataGridView_ID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridView_Subject = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridView_Date = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridView_RegStartDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridView_RegEndDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.toolStrip_Add = new System.Windows.Forms.ToolStripButton();
            this.toolStrip_Edit = new System.Windows.Forms.ToolStripButton();
            this.toolStrip_Delete = new System.Windows.Forms.ToolStripButton();
            this.toolStrip_Distribute = new System.Windows.Forms.ToolStripButton();
            this.toolStrip_Marks = new System.Windows.Forms.ToolStripButton();
            this.toolStrip_Print = new System.Windows.Forms.ToolStripButton();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit();
            this.toolStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // dataGridView
            // 
            this.dataGridView.AllowUserToAddRows = false;
            this.dataGridView.AllowUserToResizeRows = false;
            this.dataGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridView_ID,
            this.dataGridView_Subject,
            this.dataGridView_Date,
            this.dataGridView_RegStartDate,
            this.dataGridView_RegEndDate});
            this.dataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView.Location = new System.Drawing.Point(0, 25);
            this.dataGridView.MultiSelect = false;
            this.dataGridView.Name = "dataGridView";
            this.dataGridView.ReadOnly = true;
            this.dataGridView.RowHeadersVisible = false;
            this.dataGridView.RowHeadersWidth = 30;
            this.dataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView.Size = new System.Drawing.Size(624, 436);
            this.dataGridView.TabIndex = 1;
            this.dataGridView.SelectionChanged += new System.EventHandler(this.dataGridView_SelectionChanged);
            this.dataGridView.UserDeletingRow += new System.Windows.Forms.DataGridViewRowCancelEventHandler(this.dataGridView_UserDeletingRow);
            // 
            // dataGridView_ID
            // 
            this.dataGridView_ID.DataPropertyName = "id";
            this.dataGridView_ID.HeaderText = "ID";
            this.dataGridView_ID.Name = "dataGridView_ID";
            this.dataGridView_ID.ReadOnly = true;
            this.dataGridView_ID.Visible = false;
            // 
            // dataGridView_Subject
            // 
            this.dataGridView_Subject.HeaderText = "Дисциплина";
            this.dataGridView_Subject.Name = "dataGridView_Subject";
            this.dataGridView_Subject.ReadOnly = true;
            this.dataGridView_Subject.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            // 
            // dataGridView_Date
            // 
            this.dataGridView_Date.DataPropertyName = "date";
            dataGridViewCellStyle1.NullValue = null;
            this.dataGridView_Date.DefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridView_Date.FillWeight = 60F;
            this.dataGridView_Date.HeaderText = "Дата проведения";
            this.dataGridView_Date.Name = "dataGridView_Date";
            this.dataGridView_Date.ReadOnly = true;
            // 
            // dataGridView_RegStartDate
            // 
            this.dataGridView_RegStartDate.DataPropertyName = "reg_start_date";
            this.dataGridView_RegStartDate.FillWeight = 65F;
            this.dataGridView_RegStartDate.HeaderText = "Дата начала потока";
            this.dataGridView_RegStartDate.Name = "dataGridView_RegStartDate";
            this.dataGridView_RegStartDate.ReadOnly = true;
            // 
            // dataGridView_RegEndDate
            // 
            this.dataGridView_RegEndDate.DataPropertyName = "reg_end_date";
            this.dataGridView_RegEndDate.FillWeight = 75F;
            this.dataGridView_RegEndDate.HeaderText = "Дата окончания потока";
            this.dataGridView_RegEndDate.Name = "dataGridView_RegEndDate";
            this.dataGridView_RegEndDate.ReadOnly = true;
            // 
            // toolStrip
            // 
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStrip_Add,
            this.toolStrip_Edit,
            this.toolStrip_Delete,
            this.toolStrip_Distribute,
            this.toolStrip_Marks,
            this.toolStrip_Print});
            this.toolStrip.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow;
            this.toolStrip.Location = new System.Drawing.Point(0, 0);
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.Size = new System.Drawing.Size(624, 25);
            this.toolStrip.TabIndex = 0;
            this.toolStrip.Text = "toolStrip1";
            // 
            // toolStrip_Add
            // 
            this.toolStrip_Add.Image = global::PK.Properties.Resources.plus;
            this.toolStrip_Add.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStrip_Add.Name = "toolStrip_Add";
            this.toolStrip_Add.Size = new System.Drawing.Size(88, 22);
            this.toolStrip_Add.Text = "Добавить...";
            this.toolStrip_Add.Click += new System.EventHandler(this.toolStrip_Add_Click);
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
            // toolStrip_Distribute
            // 
            this.toolStrip_Distribute.Image = global::PK.Properties.Resources.seat;
            this.toolStrip_Distribute.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStrip_Distribute.Name = "toolStrip_Distribute";
            this.toolStrip_Distribute.Size = new System.Drawing.Size(103, 22);
            this.toolStrip_Distribute.Text = "Распределить";
            this.toolStrip_Distribute.Click += new System.EventHandler(this.toolStrip_Distribute_Click);
            // 
            // toolStrip_Marks
            // 
            this.toolStrip_Marks.Image = global::PK.Properties.Resources.five;
            this.toolStrip_Marks.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStrip_Marks.Name = "toolStrip_Marks";
            this.toolStrip_Marks.Size = new System.Drawing.Size(78, 22);
            this.toolStrip_Marks.Text = "Оценки...";
            this.toolStrip_Marks.Click += new System.EventHandler(this.toolStrip_Marks_Click);
            // 
            // toolStrip_Print
            // 
            this.toolStrip_Print.Image = global::PK.Properties.Resources.printer;
            this.toolStrip_Print.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStrip_Print.Name = "toolStrip_Print";
            this.toolStrip_Print.Size = new System.Drawing.Size(75, 22);
            this.toolStrip_Print.Text = "Печать...";
            this.toolStrip_Print.Click += new System.EventHandler(this.toolStrip_Print_Click);
            // 
            // Examinations
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(624, 461);
            this.Controls.Add(this.dataGridView);
            this.Controls.Add(this.toolStrip);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = global::PK.Properties.Resources.logo;
            this.MaximizeBox = false;
            this.Name = "Examinations";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Экзамены";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView;
        private System.Windows.Forms.ToolStrip toolStrip;
        private System.Windows.Forms.ToolStripButton toolStrip_Add;
        private System.Windows.Forms.ToolStripButton toolStrip_Edit;
        private System.Windows.Forms.ToolStripButton toolStrip_Marks;
        private System.Windows.Forms.ToolStripButton toolStrip_Distribute;
        private System.Windows.Forms.ToolStripButton toolStrip_Print;
        private System.Windows.Forms.ToolStripButton toolStrip_Delete;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridView_ID;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridView_Subject;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridView_Date;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridView_RegStartDate;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridView_RegEndDate;
    }
}