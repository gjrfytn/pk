namespace PK.Forms
{
    partial class OrderEdit
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
            this.cbType = new System.Windows.Forms.ComboBox();
            this.tbNumber = new System.Windows.Forms.TextBox();
            this.cbFDP = new System.Windows.Forms.ComboBox();
            this.gbEduSource = new System.Windows.Forms.GroupBox();
            this.rbQuota = new System.Windows.Forms.RadioButton();
            this.rbTarget = new System.Windows.Forms.RadioButton();
            this.rbPaid = new System.Windows.Forms.RadioButton();
            this.rbBudget = new System.Windows.Forms.RadioButton();
            this.gbEduForm = new System.Windows.Forms.GroupBox();
            this.rbZ = new System.Windows.Forms.RadioButton();
            this.rbOZ = new System.Windows.Forms.RadioButton();
            this.rbO = new System.Windows.Forms.RadioButton();
            this.lNumber = new System.Windows.Forms.Label();
            this.lType = new System.Windows.Forms.Label();
            this.lFDP = new System.Windows.Forms.Label();
            this.panel = new System.Windows.Forms.Panel();
            this.dtpDate = new System.Windows.Forms.DateTimePicker();
            this.lDate = new System.Windows.Forms.Label();
            this.bSave = new System.Windows.Forms.Button();
            this.cbShowAdmitted = new System.Windows.Forms.CheckBox();
            this.dataGridView = new System.Windows.Forms.DataGridView();
            this.dataGridView_Added = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.dataGridView_ID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridView_Name = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridView_Status = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridView_MFR = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridView_MOR = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridView_ROI = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridView_Math = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridView_Physics = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridView_Russian = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridView_Social = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridView_Foreign = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridView_Sum = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridView_Exam = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridView_IndAch = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridView_Honors = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.gbEduSource.SuspendLayout();
            this.gbEduForm.SuspendLayout();
            this.panel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // cbType
            // 
            this.cbType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbType.FormattingEnabled = true;
            this.cbType.Location = new System.Drawing.Point(12, 25);
            this.cbType.Name = "cbType";
            this.cbType.Size = new System.Drawing.Size(136, 21);
            this.cbType.TabIndex = 1;
            this.cbType.SelectedIndexChanged += new System.EventHandler(this.cbType_SelectedIndexChanged);
            // 
            // tbNumber
            // 
            this.tbNumber.Location = new System.Drawing.Point(12, 69);
            this.tbNumber.MaxLength = 50;
            this.tbNumber.Name = "tbNumber";
            this.tbNumber.Size = new System.Drawing.Size(136, 20);
            this.tbNumber.TabIndex = 3;
            // 
            // cbFDP
            // 
            this.cbFDP.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbFDP.FormattingEnabled = true;
            this.cbFDP.Location = new System.Drawing.Point(431, 25);
            this.cbFDP.Name = "cbFDP";
            this.cbFDP.Size = new System.Drawing.Size(441, 21);
            this.cbFDP.TabIndex = 9;
            this.cbFDP.DropDown += new System.EventHandler(this.cbFDP_DropDown);
            this.cbFDP.SelectionChangeCommitted += new System.EventHandler(this.cbFDP_SelectionChangeCommitted);
            // 
            // gbEduSource
            // 
            this.gbEduSource.Controls.Add(this.rbQuota);
            this.gbEduSource.Controls.Add(this.rbTarget);
            this.gbEduSource.Controls.Add(this.rbPaid);
            this.gbEduSource.Controls.Add(this.rbBudget);
            this.gbEduSource.Location = new System.Drawing.Point(154, 9);
            this.gbEduSource.Name = "gbEduSource";
            this.gbEduSource.Size = new System.Drawing.Size(92, 119);
            this.gbEduSource.TabIndex = 6;
            this.gbEduSource.TabStop = false;
            this.gbEduSource.Text = "Основание";
            // 
            // rbQuota
            // 
            this.rbQuota.AutoSize = true;
            this.rbQuota.Location = new System.Drawing.Point(7, 92);
            this.rbQuota.Name = "rbQuota";
            this.rbQuota.Size = new System.Drawing.Size(55, 17);
            this.rbQuota.TabIndex = 3;
            this.rbQuota.Tag = "quota";
            this.rbQuota.Text = "Квота";
            this.rbQuota.UseVisualStyleBackColor = true;
            this.rbQuota.CheckedChanged += new System.EventHandler(this.rb_CheckedChanged);
            // 
            // rbTarget
            // 
            this.rbTarget.AutoSize = true;
            this.rbTarget.Location = new System.Drawing.Point(7, 68);
            this.rbTarget.Name = "rbTarget";
            this.rbTarget.Size = new System.Drawing.Size(69, 17);
            this.rbTarget.TabIndex = 2;
            this.rbTarget.Tag = "";
            this.rbTarget.Text = "Целевой";
            this.rbTarget.UseVisualStyleBackColor = true;
            this.rbTarget.CheckedChanged += new System.EventHandler(this.rb_CheckedChanged);
            // 
            // rbPaid
            // 
            this.rbPaid.AutoSize = true;
            this.rbPaid.Location = new System.Drawing.Point(7, 44);
            this.rbPaid.Name = "rbPaid";
            this.rbPaid.Size = new System.Drawing.Size(68, 17);
            this.rbPaid.TabIndex = 1;
            this.rbPaid.Text = "Платная";
            this.rbPaid.UseVisualStyleBackColor = true;
            this.rbPaid.CheckedChanged += new System.EventHandler(this.rbPaid_CheckedChanged);
            // 
            // rbBudget
            // 
            this.rbBudget.AutoSize = true;
            this.rbBudget.Checked = true;
            this.rbBudget.Location = new System.Drawing.Point(7, 20);
            this.rbBudget.Name = "rbBudget";
            this.rbBudget.Size = new System.Drawing.Size(65, 17);
            this.rbBudget.TabIndex = 0;
            this.rbBudget.TabStop = true;
            this.rbBudget.Tag = "budget";
            this.rbBudget.Text = "Бюджет";
            this.rbBudget.UseVisualStyleBackColor = true;
            this.rbBudget.CheckedChanged += new System.EventHandler(this.rb_CheckedChanged);
            // 
            // gbEduForm
            // 
            this.gbEduForm.Controls.Add(this.rbZ);
            this.gbEduForm.Controls.Add(this.rbOZ);
            this.gbEduForm.Controls.Add(this.rbO);
            this.gbEduForm.Location = new System.Drawing.Point(252, 9);
            this.gbEduForm.Name = "gbEduForm";
            this.gbEduForm.Size = new System.Drawing.Size(170, 100);
            this.gbEduForm.TabIndex = 7;
            this.gbEduForm.TabStop = false;
            this.gbEduForm.Text = "Форма обучения";
            // 
            // rbZ
            // 
            this.rbZ.AutoSize = true;
            this.rbZ.Enabled = false;
            this.rbZ.Location = new System.Drawing.Point(7, 68);
            this.rbZ.Name = "rbZ";
            this.rbZ.Size = new System.Drawing.Size(67, 17);
            this.rbZ.TabIndex = 2;
            this.rbZ.Tag = "z";
            this.rbZ.Text = "Заочная";
            this.rbZ.UseVisualStyleBackColor = true;
            this.rbZ.CheckedChanged += new System.EventHandler(this.rb_CheckedChanged);
            // 
            // rbOZ
            // 
            this.rbOZ.AutoSize = true;
            this.rbOZ.Location = new System.Drawing.Point(7, 44);
            this.rbOZ.Name = "rbOZ";
            this.rbOZ.Size = new System.Drawing.Size(150, 17);
            this.rbOZ.TabIndex = 1;
            this.rbOZ.Tag = "oz";
            this.rbOZ.Text = "Очно-заочная (вечерняя)";
            this.rbOZ.UseVisualStyleBackColor = true;
            this.rbOZ.CheckedChanged += new System.EventHandler(this.rb_CheckedChanged);
            // 
            // rbO
            // 
            this.rbO.AutoSize = true;
            this.rbO.Checked = true;
            this.rbO.Location = new System.Drawing.Point(7, 20);
            this.rbO.Name = "rbO";
            this.rbO.Size = new System.Drawing.Size(56, 17);
            this.rbO.TabIndex = 0;
            this.rbO.TabStop = true;
            this.rbO.Tag = "o";
            this.rbO.Text = "Очная";
            this.rbO.UseVisualStyleBackColor = true;
            this.rbO.CheckedChanged += new System.EventHandler(this.rb_CheckedChanged);
            // 
            // lNumber
            // 
            this.lNumber.AutoSize = true;
            this.lNumber.Location = new System.Drawing.Point(12, 53);
            this.lNumber.Name = "lNumber";
            this.lNumber.Size = new System.Drawing.Size(44, 13);
            this.lNumber.TabIndex = 2;
            this.lNumber.Text = "Номер:";
            // 
            // lType
            // 
            this.lType.AutoSize = true;
            this.lType.Location = new System.Drawing.Point(12, 9);
            this.lType.Name = "lType";
            this.lType.Size = new System.Drawing.Size(29, 13);
            this.lType.TabIndex = 0;
            this.lType.Text = "Тип:";
            // 
            // lFDP
            // 
            this.lFDP.AutoSize = true;
            this.lFDP.Location = new System.Drawing.Point(428, 9);
            this.lFDP.Name = "lFDP";
            this.lFDP.Size = new System.Drawing.Size(78, 13);
            this.lFDP.TabIndex = 8;
            this.lFDP.Text = "Направление:";
            // 
            // panel
            // 
            this.panel.Controls.Add(this.dtpDate);
            this.panel.Controls.Add(this.lDate);
            this.panel.Controls.Add(this.bSave);
            this.panel.Controls.Add(this.cbShowAdmitted);
            this.panel.Controls.Add(this.tbNumber);
            this.panel.Controls.Add(this.cbType);
            this.panel.Controls.Add(this.cbFDP);
            this.panel.Controls.Add(this.gbEduSource);
            this.panel.Controls.Add(this.gbEduForm);
            this.panel.Controls.Add(this.lFDP);
            this.panel.Controls.Add(this.lType);
            this.panel.Controls.Add(this.lNumber);
            this.panel.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel.Location = new System.Drawing.Point(0, 0);
            this.panel.Name = "panel";
            this.panel.Size = new System.Drawing.Size(884, 143);
            this.panel.TabIndex = 10;
            // 
            // dtpDate
            // 
            this.dtpDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpDate.Location = new System.Drawing.Point(12, 108);
            this.dtpDate.MaxDate = new System.DateTime(2030, 12, 31, 0, 0, 0, 0);
            this.dtpDate.MinDate = new System.DateTime(2017, 1, 1, 0, 0, 0, 0);
            this.dtpDate.Name = "dtpDate";
            this.dtpDate.Size = new System.Drawing.Size(90, 20);
            this.dtpDate.TabIndex = 5;
            this.dtpDate.ValueChanged += new System.EventHandler(this.dtpDate_ValueChanged);
            // 
            // lDate
            // 
            this.lDate.AutoSize = true;
            this.lDate.Location = new System.Drawing.Point(12, 92);
            this.lDate.Name = "lDate";
            this.lDate.Size = new System.Drawing.Size(36, 13);
            this.lDate.TabIndex = 4;
            this.lDate.Text = "Дата:";
            // 
            // bSave
            // 
            this.bSave.Enabled = false;
            this.bSave.Location = new System.Drawing.Point(797, 114);
            this.bSave.Name = "bSave";
            this.bSave.Size = new System.Drawing.Size(75, 23);
            this.bSave.TabIndex = 12;
            this.bSave.Text = "Сохранить";
            this.bSave.UseVisualStyleBackColor = true;
            this.bSave.Click += new System.EventHandler(this.bSave_Click);
            // 
            // cbShowAdmitted
            // 
            this.cbShowAdmitted.AutoSize = true;
            this.cbShowAdmitted.Enabled = false;
            this.cbShowAdmitted.Location = new System.Drawing.Point(259, 113);
            this.cbShowAdmitted.Name = "cbShowAdmitted";
            this.cbShowAdmitted.Size = new System.Drawing.Size(157, 17);
            this.cbShowAdmitted.TabIndex = 10;
            this.cbShowAdmitted.Text = "Отображать зачисленных";
            this.cbShowAdmitted.UseVisualStyleBackColor = true;
            this.cbShowAdmitted.CheckedChanged += new System.EventHandler(this.cbShowAdmitted_CheckedChanged);
            // 
            // dataGridView
            // 
            this.dataGridView.AllowUserToAddRows = false;
            this.dataGridView.AllowUserToDeleteRows = false;
            this.dataGridView.AllowUserToResizeRows = false;
            this.dataGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridView_Added,
            this.dataGridView_ID,
            this.dataGridView_Name,
            this.dataGridView_Status,
            this.dataGridView_MFR,
            this.dataGridView_MOR,
            this.dataGridView_ROI,
            this.dataGridView_Math,
            this.dataGridView_Physics,
            this.dataGridView_Russian,
            this.dataGridView_Social,
            this.dataGridView_Foreign,
            this.dataGridView_Sum,
            this.dataGridView_Exam,
            this.dataGridView_IndAch,
            this.dataGridView_Honors});
            this.dataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView.Location = new System.Drawing.Point(0, 143);
            this.dataGridView.Name = "dataGridView";
            this.dataGridView.RowHeadersVisible = false;
            this.dataGridView.Size = new System.Drawing.Size(884, 418);
            this.dataGridView.TabIndex = 11;
            this.dataGridView.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView_CellValueChanged);
            this.dataGridView.CurrentCellDirtyStateChanged += new System.EventHandler(this.dataGridView_CurrentCellDirtyStateChanged);
            // 
            // dataGridView_Added
            // 
            this.dataGridView_Added.FillWeight = 20F;
            this.dataGridView_Added.HeaderText = "В приказе";
            this.dataGridView_Added.Name = "dataGridView_Added";
            // 
            // dataGridView_ID
            // 
            this.dataGridView_ID.FillWeight = 20F;
            this.dataGridView_ID.HeaderText = "УИН";
            this.dataGridView_ID.Name = "dataGridView_ID";
            this.dataGridView_ID.ReadOnly = true;
            // 
            // dataGridView_Name
            // 
            this.dataGridView_Name.HeaderText = "ФИО";
            this.dataGridView_Name.Name = "dataGridView_Name";
            this.dataGridView_Name.ReadOnly = true;
            // 
            // dataGridView_Status
            // 
            this.dataGridView_Status.FillWeight = 30F;
            this.dataGridView_Status.HeaderText = "Статус";
            this.dataGridView_Status.Name = "dataGridView_Status";
            this.dataGridView_Status.ReadOnly = true;
            // 
            // dataGridView_MFR
            // 
            this.dataGridView_MFR.FillWeight = 15F;
            this.dataGridView_MFR.HeaderText = "МФР";
            this.dataGridView_MFR.Name = "dataGridView_MFR";
            this.dataGridView_MFR.ReadOnly = true;
            // 
            // dataGridView_MOR
            // 
            this.dataGridView_MOR.FillWeight = 15F;
            this.dataGridView_MOR.HeaderText = "МОР";
            this.dataGridView_MOR.Name = "dataGridView_MOR";
            this.dataGridView_MOR.ReadOnly = true;
            // 
            // dataGridView_ROI
            // 
            this.dataGridView_ROI.FillWeight = 15F;
            this.dataGridView_ROI.HeaderText = "РОИ";
            this.dataGridView_ROI.Name = "dataGridView_ROI";
            this.dataGridView_ROI.ReadOnly = true;
            // 
            // dataGridView_Math
            // 
            this.dataGridView_Math.FillWeight = 10F;
            this.dataGridView_Math.HeaderText = "М";
            this.dataGridView_Math.Name = "dataGridView_Math";
            this.dataGridView_Math.ReadOnly = true;
            // 
            // dataGridView_Physics
            // 
            this.dataGridView_Physics.FillWeight = 10F;
            this.dataGridView_Physics.HeaderText = "Ф";
            this.dataGridView_Physics.Name = "dataGridView_Physics";
            this.dataGridView_Physics.ReadOnly = true;
            // 
            // dataGridView_Russian
            // 
            this.dataGridView_Russian.FillWeight = 10F;
            this.dataGridView_Russian.HeaderText = "Р";
            this.dataGridView_Russian.Name = "dataGridView_Russian";
            this.dataGridView_Russian.ReadOnly = true;
            // 
            // dataGridView_Social
            // 
            this.dataGridView_Social.FillWeight = 10F;
            this.dataGridView_Social.HeaderText = "О";
            this.dataGridView_Social.Name = "dataGridView_Social";
            this.dataGridView_Social.ReadOnly = true;
            // 
            // dataGridView_Foreign
            // 
            this.dataGridView_Foreign.FillWeight = 10F;
            this.dataGridView_Foreign.HeaderText = "И";
            this.dataGridView_Foreign.Name = "dataGridView_Foreign";
            this.dataGridView_Foreign.ReadOnly = true;
            // 
            // dataGridView_Sum
            // 
            this.dataGridView_Sum.FillWeight = 15F;
            this.dataGridView_Sum.HeaderText = "Сумма";
            this.dataGridView_Sum.Name = "dataGridView_Sum";
            this.dataGridView_Sum.ReadOnly = true;
            this.dataGridView_Sum.Visible = false;
            // 
            // dataGridView_Exam
            // 
            this.dataGridView_Exam.FillWeight = 15F;
            this.dataGridView_Exam.HeaderText = "Оценка";
            this.dataGridView_Exam.Name = "dataGridView_Exam";
            this.dataGridView_Exam.ReadOnly = true;
            this.dataGridView_Exam.Visible = false;
            // 
            // dataGridView_IndAch
            // 
            this.dataGridView_IndAch.FillWeight = 20F;
            this.dataGridView_IndAch.HeaderText = "Достижение";
            this.dataGridView_IndAch.Name = "dataGridView_IndAch";
            this.dataGridView_IndAch.ReadOnly = true;
            this.dataGridView_IndAch.Visible = false;
            // 
            // dataGridView_Honors
            // 
            this.dataGridView_Honors.FillWeight = 15F;
            this.dataGridView_Honors.HeaderText = "Отличие";
            this.dataGridView_Honors.Name = "dataGridView_Honors";
            this.dataGridView_Honors.ReadOnly = true;
            this.dataGridView_Honors.Visible = false;
            // 
            // OrderEdit
            // 
            this.AcceptButton = this.bSave;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(884, 561);
            this.Controls.Add(this.dataGridView);
            this.Controls.Add(this.panel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = global::PK.Properties.Resources.logo;
            this.MaximizeBox = false;
            this.Name = "OrderEdit";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Приказ";
            this.gbEduSource.ResumeLayout(false);
            this.gbEduSource.PerformLayout();
            this.gbEduForm.ResumeLayout(false);
            this.gbEduForm.PerformLayout();
            this.panel.ResumeLayout(false);
            this.panel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox cbType;
        private System.Windows.Forms.TextBox tbNumber;
        private System.Windows.Forms.ComboBox cbFDP;
        private System.Windows.Forms.GroupBox gbEduSource;
        private System.Windows.Forms.RadioButton rbQuota;
        private System.Windows.Forms.RadioButton rbTarget;
        private System.Windows.Forms.RadioButton rbPaid;
        private System.Windows.Forms.RadioButton rbBudget;
        private System.Windows.Forms.GroupBox gbEduForm;
        private System.Windows.Forms.RadioButton rbZ;
        private System.Windows.Forms.RadioButton rbOZ;
        private System.Windows.Forms.RadioButton rbO;
        private System.Windows.Forms.Label lNumber;
        private System.Windows.Forms.Label lType;
        private System.Windows.Forms.Label lFDP;
        private System.Windows.Forms.Panel panel;
        private System.Windows.Forms.DataGridView dataGridView;
        private System.Windows.Forms.CheckBox cbShowAdmitted;
        private System.Windows.Forms.Button bSave;
        private System.Windows.Forms.Label lDate;
        private System.Windows.Forms.DateTimePicker dtpDate;
        private System.Windows.Forms.DataGridViewCheckBoxColumn dataGridView_Added;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridView_ID;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridView_Name;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridView_Status;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridView_MFR;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridView_MOR;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridView_ROI;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridView_Math;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridView_Physics;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridView_Russian;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridView_Social;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridView_Foreign;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridView_Sum;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridView_Exam;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridView_IndAch;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridView_Honors;
    }
}