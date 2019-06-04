namespace PK.Forms
{
    partial class OnlineApplSearch
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OnlineApplSearch));
            this.panel1 = new System.Windows.Forms.Panel();
            this.label3 = new System.Windows.Forms.Label();
            this.tbOnlineNumber = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.tbOnlineSeries = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tbOnlineApplicationID = new System.Windows.Forms.TextBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.dgvOnlineApplications = new System.Windows.Forms.DataGridView();
            this.btnCopyOnlineApplication = new System.Windows.Forms.Button();
            this.panel4 = new System.Windows.Forms.Panel();
            this.tvOnlineSelectedApplication = new System.Windows.Forms.TreeView();
            this.dgvOnlineApplications_application_id = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvOnlineApplications_last_name = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvOnlineApplications_first_name = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvOnlineApplications_middle_name = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvOnlineApplications_series = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvOnlineApplications_number = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvOnlineApplications)).BeginInit();
            this.panel4.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.tbOnlineNumber);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.tbOnlineSeries);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.tbOnlineApplicationID);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1924, 65);
            this.panel1.TabIndex = 6;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label3.Location = new System.Drawing.Point(505, 15);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(96, 29);
            this.label3.TabIndex = 9;
            this.label3.Text = "Номер:";
            // 
            // tbOnlineNumber
            // 
            this.tbOnlineNumber.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.tbOnlineNumber.Location = new System.Drawing.Point(607, 12);
            this.tbOnlineNumber.Name = "tbOnlineNumber";
            this.tbOnlineNumber.Size = new System.Drawing.Size(115, 35);
            this.tbOnlineNumber.TabIndex = 8;
            this.tbOnlineNumber.TextChanged += new System.EventHandler(this.tbOnlineNumber_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label2.Location = new System.Drawing.Point(288, 15);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(93, 29);
            this.label2.TabIndex = 7;
            this.label2.Text = "Серия:";
            this.label2.Click += new System.EventHandler(this.label2_Click);
            // 
            // tbOnlineSeries
            // 
            this.tbOnlineSeries.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.tbOnlineSeries.Location = new System.Drawing.Point(387, 12);
            this.tbOnlineSeries.Name = "tbOnlineSeries";
            this.tbOnlineSeries.Size = new System.Drawing.Size(115, 35);
            this.tbOnlineSeries.TabIndex = 6;
            this.tbOnlineSeries.TextChanged += new System.EventHandler(this.tbOnlineSeries_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label1.Location = new System.Drawing.Point(2, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(147, 29);
            this.label1.TabIndex = 5;
            this.label1.Text = "Рег. номер:";
            // 
            // tbOnlineApplicationID
            // 
            this.tbOnlineApplicationID.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.tbOnlineApplicationID.Location = new System.Drawing.Point(155, 12);
            this.tbOnlineApplicationID.Name = "tbOnlineApplicationID";
            this.tbOnlineApplicationID.Size = new System.Drawing.Size(115, 35);
            this.tbOnlineApplicationID.TabIndex = 4;
            this.tbOnlineApplicationID.TextChanged += new System.EventHandler(this.tbOnlineApplicationID_TextChanged);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.panel4);
            this.panel2.Controls.Add(this.btnCopyOnlineApplication);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel2.Location = new System.Drawing.Point(1117, 65);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(807, 486);
            this.panel2.TabIndex = 7;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.dgvOnlineApplications);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(0, 65);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(1117, 486);
            this.panel3.TabIndex = 10;
            // 
            // dgvOnlineApplications
            // 
            this.dgvOnlineApplications.AllowUserToAddRows = false;
            this.dgvOnlineApplications.AllowUserToDeleteRows = false;
            this.dgvOnlineApplications.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvOnlineApplications.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvOnlineApplications.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvOnlineApplications.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dgvOnlineApplications_application_id,
            this.dgvOnlineApplications_last_name,
            this.dgvOnlineApplications_first_name,
            this.dgvOnlineApplications_middle_name,
            this.dgvOnlineApplications_series,
            this.dgvOnlineApplications_number});
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvOnlineApplications.DefaultCellStyle = dataGridViewCellStyle2;
            this.dgvOnlineApplications.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvOnlineApplications.EnableHeadersVisualStyles = false;
            this.dgvOnlineApplications.Location = new System.Drawing.Point(0, 0);
            this.dgvOnlineApplications.Name = "dgvOnlineApplications";
            this.dgvOnlineApplications.ReadOnly = true;
            this.dgvOnlineApplications.RowHeadersVisible = false;
            this.dgvOnlineApplications.RowTemplate.Height = 28;
            this.dgvOnlineApplications.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvOnlineApplications.Size = new System.Drawing.Size(1117, 486);
            this.dgvOnlineApplications.TabIndex = 10;
            this.dgvOnlineApplications.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvOnlineApplications_CellClick);
            // 
            // btnCopyOnlineApplication
            // 
            this.btnCopyOnlineApplication.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.btnCopyOnlineApplication.Location = new System.Drawing.Point(0, 440);
            this.btnCopyOnlineApplication.Name = "btnCopyOnlineApplication";
            this.btnCopyOnlineApplication.Size = new System.Drawing.Size(807, 46);
            this.btnCopyOnlineApplication.TabIndex = 11;
            this.btnCopyOnlineApplication.Text = "Копировать";
            this.btnCopyOnlineApplication.UseVisualStyleBackColor = true;
            this.btnCopyOnlineApplication.Click += new System.EventHandler(this.btnCopyOnlineApplication_Click);
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.tvOnlineSelectedApplication);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel4.Location = new System.Drawing.Point(0, 0);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(807, 440);
            this.panel4.TabIndex = 12;
            // 
            // tvOnlineSelectedApplication
            // 
            this.tvOnlineSelectedApplication.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tvOnlineSelectedApplication.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.tvOnlineSelectedApplication.ItemHeight = 22;
            this.tvOnlineSelectedApplication.Location = new System.Drawing.Point(0, 0);
            this.tvOnlineSelectedApplication.Name = "tvOnlineSelectedApplication";
            this.tvOnlineSelectedApplication.Size = new System.Drawing.Size(807, 440);
            this.tvOnlineSelectedApplication.TabIndex = 11;
            // 
            // dgvOnlineApplications_application_id
            // 
            this.dgvOnlineApplications_application_id.DataPropertyName = "application_id";
            this.dgvOnlineApplications_application_id.HeaderText = "Рег. номер";
            this.dgvOnlineApplications_application_id.Name = "dgvOnlineApplications_application_id";
            this.dgvOnlineApplications_application_id.ReadOnly = true;
            // 
            // dgvOnlineApplications_last_name
            // 
            this.dgvOnlineApplications_last_name.DataPropertyName = "last_name";
            this.dgvOnlineApplications_last_name.HeaderText = "Фамилия";
            this.dgvOnlineApplications_last_name.Name = "dgvOnlineApplications_last_name";
            this.dgvOnlineApplications_last_name.ReadOnly = true;
            // 
            // dgvOnlineApplications_first_name
            // 
            this.dgvOnlineApplications_first_name.DataPropertyName = "first_name";
            this.dgvOnlineApplications_first_name.HeaderText = "Имя";
            this.dgvOnlineApplications_first_name.Name = "dgvOnlineApplications_first_name";
            this.dgvOnlineApplications_first_name.ReadOnly = true;
            // 
            // dgvOnlineApplications_middle_name
            // 
            this.dgvOnlineApplications_middle_name.DataPropertyName = "middle_name";
            this.dgvOnlineApplications_middle_name.HeaderText = "Отчество";
            this.dgvOnlineApplications_middle_name.Name = "dgvOnlineApplications_middle_name";
            this.dgvOnlineApplications_middle_name.ReadOnly = true;
            // 
            // dgvOnlineApplications_series
            // 
            this.dgvOnlineApplications_series.DataPropertyName = "series";
            this.dgvOnlineApplications_series.HeaderText = "Серия";
            this.dgvOnlineApplications_series.Name = "dgvOnlineApplications_series";
            this.dgvOnlineApplications_series.ReadOnly = true;
            // 
            // dgvOnlineApplications_number
            // 
            this.dgvOnlineApplications_number.DataPropertyName = "number";
            this.dgvOnlineApplications_number.HeaderText = "Номер";
            this.dgvOnlineApplications_number.Name = "dgvOnlineApplications_number";
            this.dgvOnlineApplications_number.ReadOnly = true;
            // 
            // OnlineApplSearch
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1924, 551);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "OnlineApplSearch";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Online заявления";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvOnlineApplications)).EndInit();
            this.panel4.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tbOnlineSeries;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tbOnlineApplicationID;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox tbOnlineNumber;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.DataGridView dgvOnlineApplications;
        private System.Windows.Forms.Button btnCopyOnlineApplication;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.TreeView tvOnlineSelectedApplication;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgvOnlineApplications_application_id;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgvOnlineApplications_last_name;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgvOnlineApplications_first_name;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgvOnlineApplications_middle_name;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgvOnlineApplications_series;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgvOnlineApplications_number;
    }
}