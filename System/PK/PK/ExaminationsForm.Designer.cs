namespace PK
{
    partial class ExaminationsForm
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
            this.panel = new System.Windows.Forms.Panel();
            this.controlPanel = new System.Windows.Forms.Panel();
            this.bEdit = new System.Windows.Forms.Button();
            this.bAdd = new System.Windows.Forms.Button();
            this.bMarks = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit();
            this.panel.SuspendLayout();
            this.controlPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // dataGridView
            // 
            this.dataGridView.AllowUserToResizeColumns = false;
            this.dataGridView.AllowUserToResizeRows = false;
            this.dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridView_ID,
            this.dataGridView_Subject,
            this.dataGridView_Date,
            this.dataGridView_RegStartDate,
            this.dataGridView_RegEndDate});
            this.dataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView.Location = new System.Drawing.Point(0, 0);
            this.dataGridView.MultiSelect = false;
            this.dataGridView.Name = "dataGridView";
            this.dataGridView.ReadOnly = true;
            this.dataGridView.RowHeadersVisible = false;
            this.dataGridView.RowHeadersWidth = 30;
            this.dataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView.Size = new System.Drawing.Size(746, 428);
            this.dataGridView.TabIndex = 0;
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
            this.dataGridView_Subject.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dataGridView_Subject.HeaderText = "Дисциплина";
            this.dataGridView_Subject.Name = "dataGridView_Subject";
            this.dataGridView_Subject.ReadOnly = true;
            this.dataGridView_Subject.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView_Subject.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // dataGridView_Date
            // 
            this.dataGridView_Date.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dataGridView_Date.DataPropertyName = "date";
            dataGridViewCellStyle1.NullValue = null;
            this.dataGridView_Date.DefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridView_Date.HeaderText = "Дата проведения";
            this.dataGridView_Date.Name = "dataGridView_Date";
            this.dataGridView_Date.ReadOnly = true;
            // 
            // dataGridView_RegStartDate
            // 
            this.dataGridView_RegStartDate.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dataGridView_RegStartDate.DataPropertyName = "reg_start_date";
            this.dataGridView_RegStartDate.HeaderText = "Дата начала потока";
            this.dataGridView_RegStartDate.Name = "dataGridView_RegStartDate";
            this.dataGridView_RegStartDate.ReadOnly = true;
            // 
            // dataGridView_RegEndDate
            // 
            this.dataGridView_RegEndDate.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dataGridView_RegEndDate.DataPropertyName = "reg_end_date";
            this.dataGridView_RegEndDate.HeaderText = "Дата окончания потока";
            this.dataGridView_RegEndDate.Name = "dataGridView_RegEndDate";
            this.dataGridView_RegEndDate.ReadOnly = true;
            // 
            // panel
            // 
            this.panel.Controls.Add(this.dataGridView);
            this.panel.Controls.Add(this.controlPanel);
            this.panel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel.Location = new System.Drawing.Point(0, 0);
            this.panel.Name = "panel";
            this.panel.Size = new System.Drawing.Size(746, 469);
            this.panel.TabIndex = 3;
            // 
            // controlPanel
            // 
            this.controlPanel.Controls.Add(this.bMarks);
            this.controlPanel.Controls.Add(this.bEdit);
            this.controlPanel.Controls.Add(this.bAdd);
            this.controlPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.controlPanel.Location = new System.Drawing.Point(0, 428);
            this.controlPanel.Name = "controlPanel";
            this.controlPanel.Size = new System.Drawing.Size(746, 41);
            this.controlPanel.TabIndex = 0;
            // 
            // bEdit
            // 
            this.bEdit.Location = new System.Drawing.Point(84, 6);
            this.bEdit.Name = "bEdit";
            this.bEdit.Size = new System.Drawing.Size(94, 23);
            this.bEdit.TabIndex = 1;
            this.bEdit.Text = "Редактировать";
            this.bEdit.UseVisualStyleBackColor = true;
            this.bEdit.Click += new System.EventHandler(this.bEdit_Click);
            // 
            // bAdd
            // 
            this.bAdd.Location = new System.Drawing.Point(12, 6);
            this.bAdd.Name = "bAdd";
            this.bAdd.Size = new System.Drawing.Size(66, 23);
            this.bAdd.TabIndex = 0;
            this.bAdd.Text = "Добавить";
            this.bAdd.UseVisualStyleBackColor = true;
            this.bAdd.Click += new System.EventHandler(this.bAdd_Click);
            // 
            // bMarks
            // 
            this.bMarks.Location = new System.Drawing.Point(184, 6);
            this.bMarks.Name = "bMarks";
            this.bMarks.Size = new System.Drawing.Size(54, 23);
            this.bMarks.TabIndex = 2;
            this.bMarks.Text = "Оценки";
            this.bMarks.UseVisualStyleBackColor = true;
            this.bMarks.Click += new System.EventHandler(this.bMarks_Click);
            // 
            // ExaminationsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(746, 469);
            this.Controls.Add(this.panel);
            this.Name = "ExaminationsForm";
            this.Text = "ExaminationsForm";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
            this.panel.ResumeLayout(false);
            this.controlPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView;
        private System.Windows.Forms.Panel panel;
        private System.Windows.Forms.Panel controlPanel;
        private System.Windows.Forms.Button bAdd;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridView_ID;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridView_Subject;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridView_Date;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridView_RegStartDate;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridView_RegEndDate;
        private System.Windows.Forms.Button bEdit;
        private System.Windows.Forms.Button bMarks;
    }
}