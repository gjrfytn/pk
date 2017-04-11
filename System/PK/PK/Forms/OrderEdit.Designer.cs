﻿namespace PK.Forms
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
            this.cbDirOrProfile = new System.Windows.Forms.ComboBox();
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
            this.lDirOrProfile = new System.Windows.Forms.Label();
            this.panel = new System.Windows.Forms.Panel();
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
            this.cbType.Location = new System.Drawing.Point(12, 64);
            this.cbType.Name = "cbType";
            this.cbType.Size = new System.Drawing.Size(121, 21);
            this.cbType.TabIndex = 1;
            // 
            // tbNumber
            // 
            this.tbNumber.Location = new System.Drawing.Point(12, 25);
            this.tbNumber.Name = "tbNumber";
            this.tbNumber.Size = new System.Drawing.Size(136, 20);
            this.tbNumber.TabIndex = 0;
            // 
            // cbDirOrProfile
            // 
            this.cbDirOrProfile.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbDirOrProfile.FormattingEnabled = true;
            this.cbDirOrProfile.Location = new System.Drawing.Point(443, 24);
            this.cbDirOrProfile.Name = "cbDirOrProfile";
            this.cbDirOrProfile.Size = new System.Drawing.Size(259, 21);
            this.cbDirOrProfile.TabIndex = 4;
            this.cbDirOrProfile.DropDown += new System.EventHandler(this.cbDirOrProfile_DropDown);
            this.cbDirOrProfile.SelectedIndexChanged += new System.EventHandler(this.cbDirOrProfile_SelectedIndexChanged);
            // 
            // gbEduSource
            // 
            this.gbEduSource.Controls.Add(this.rbQuota);
            this.gbEduSource.Controls.Add(this.rbTarget);
            this.gbEduSource.Controls.Add(this.rbPaid);
            this.gbEduSource.Controls.Add(this.rbBudget);
            this.gbEduSource.Location = new System.Drawing.Point(154, 12);
            this.gbEduSource.Name = "gbEduSource";
            this.gbEduSource.Size = new System.Drawing.Size(92, 119);
            this.gbEduSource.TabIndex = 5;
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
            // 
            // rbTarget
            // 
            this.rbTarget.AutoSize = true;
            this.rbTarget.Location = new System.Drawing.Point(7, 68);
            this.rbTarget.Name = "rbTarget";
            this.rbTarget.Size = new System.Drawing.Size(69, 17);
            this.rbTarget.TabIndex = 2;
            this.rbTarget.Tag = "target";
            this.rbTarget.Text = "Целевой";
            this.rbTarget.UseVisualStyleBackColor = true;
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
            // 
            // gbEduForm
            // 
            this.gbEduForm.Controls.Add(this.rbZ);
            this.gbEduForm.Controls.Add(this.rbOZ);
            this.gbEduForm.Controls.Add(this.rbO);
            this.gbEduForm.Location = new System.Drawing.Point(252, 12);
            this.gbEduForm.Name = "gbEduForm";
            this.gbEduForm.Size = new System.Drawing.Size(170, 100);
            this.gbEduForm.TabIndex = 6;
            this.gbEduForm.TabStop = false;
            this.gbEduForm.Text = "Форма обучения";
            // 
            // rbZ
            // 
            this.rbZ.AutoSize = true;
            this.rbZ.Location = new System.Drawing.Point(7, 68);
            this.rbZ.Name = "rbZ";
            this.rbZ.Size = new System.Drawing.Size(67, 17);
            this.rbZ.TabIndex = 2;
            this.rbZ.Tag = "z";
            this.rbZ.Text = "Заочная";
            this.rbZ.UseVisualStyleBackColor = true;
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
            // 
            // lNumber
            // 
            this.lNumber.AutoSize = true;
            this.lNumber.Location = new System.Drawing.Point(12, 9);
            this.lNumber.Name = "lNumber";
            this.lNumber.Size = new System.Drawing.Size(44, 13);
            this.lNumber.TabIndex = 7;
            this.lNumber.Text = "Номер:";
            // 
            // lType
            // 
            this.lType.AutoSize = true;
            this.lType.Location = new System.Drawing.Point(12, 48);
            this.lType.Name = "lType";
            this.lType.Size = new System.Drawing.Size(29, 13);
            this.lType.TabIndex = 8;
            this.lType.Text = "Тип:";
            // 
            // lDirOrProfile
            // 
            this.lDirOrProfile.AutoSize = true;
            this.lDirOrProfile.Location = new System.Drawing.Point(440, 9);
            this.lDirOrProfile.Name = "lDirOrProfile";
            this.lDirOrProfile.Size = new System.Drawing.Size(75, 13);
            this.lDirOrProfile.TabIndex = 9;
            this.lDirOrProfile.Text = "Направление";
            // 
            // panel
            // 
            this.panel.Controls.Add(this.bSave);
            this.panel.Controls.Add(this.cbShowAdmitted);
            this.panel.Controls.Add(this.tbNumber);
            this.panel.Controls.Add(this.cbType);
            this.panel.Controls.Add(this.cbDirOrProfile);
            this.panel.Controls.Add(this.gbEduSource);
            this.panel.Controls.Add(this.gbEduForm);
            this.panel.Controls.Add(this.lDirOrProfile);
            this.panel.Controls.Add(this.lType);
            this.panel.Controls.Add(this.lNumber);
            this.panel.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel.Location = new System.Drawing.Point(0, 0);
            this.panel.Name = "panel";
            this.panel.Size = new System.Drawing.Size(1008, 143);
            this.panel.TabIndex = 10;
            // 
            // bSave
            // 
            this.bSave.Location = new System.Drawing.Point(852, 62);
            this.bSave.Name = "bSave";
            this.bSave.Size = new System.Drawing.Size(75, 23);
            this.bSave.TabIndex = 11;
            this.bSave.Text = "Сохранить";
            this.bSave.UseVisualStyleBackColor = true;
            // 
            // cbShowAdmitted
            // 
            this.cbShowAdmitted.AutoSize = true;
            this.cbShowAdmitted.Enabled = false;
            this.cbShowAdmitted.Location = new System.Drawing.Point(443, 95);
            this.cbShowAdmitted.Name = "cbShowAdmitted";
            this.cbShowAdmitted.Size = new System.Drawing.Size(157, 17);
            this.cbShowAdmitted.TabIndex = 10;
            this.cbShowAdmitted.Text = "Отображать зачисленных";
            this.cbShowAdmitted.UseVisualStyleBackColor = true;
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
            this.dataGridView_Foreign});
            this.dataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView.Location = new System.Drawing.Point(0, 143);
            this.dataGridView.Name = "dataGridView";
            this.dataGridView.RowHeadersVisible = false;
            this.dataGridView.Size = new System.Drawing.Size(1008, 418);
            this.dataGridView.TabIndex = 11;
            this.dataGridView.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView_CellContentClick);
            // 
            // dataGridView_Added
            // 
            this.dataGridView_Added.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dataGridView_Added.FillWeight = 30F;
            this.dataGridView_Added.HeaderText = "В приказе";
            this.dataGridView_Added.Name = "dataGridView_Added";
            // 
            // dataGridView_ID
            // 
            this.dataGridView_ID.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dataGridView_ID.FillWeight = 30F;
            this.dataGridView_ID.HeaderText = "ID";
            this.dataGridView_ID.Name = "dataGridView_ID";
            this.dataGridView_ID.ReadOnly = true;
            // 
            // dataGridView_Name
            // 
            this.dataGridView_Name.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dataGridView_Name.HeaderText = "ФИО";
            this.dataGridView_Name.Name = "dataGridView_Name";
            this.dataGridView_Name.ReadOnly = true;
            // 
            // dataGridView_Status
            // 
            this.dataGridView_Status.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dataGridView_Status.FillWeight = 30F;
            this.dataGridView_Status.HeaderText = "Статус";
            this.dataGridView_Status.Name = "dataGridView_Status";
            this.dataGridView_Status.ReadOnly = true;
            // 
            // dataGridView_MFR
            // 
            this.dataGridView_MFR.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dataGridView_MFR.FillWeight = 25F;
            this.dataGridView_MFR.HeaderText = "МФР";
            this.dataGridView_MFR.Name = "dataGridView_MFR";
            this.dataGridView_MFR.ReadOnly = true;
            // 
            // dataGridView_MOR
            // 
            this.dataGridView_MOR.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dataGridView_MOR.FillWeight = 25F;
            this.dataGridView_MOR.HeaderText = "МОР";
            this.dataGridView_MOR.Name = "dataGridView_MOR";
            this.dataGridView_MOR.ReadOnly = true;
            // 
            // dataGridView_ROI
            // 
            this.dataGridView_ROI.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dataGridView_ROI.FillWeight = 25F;
            this.dataGridView_ROI.HeaderText = "РОИ";
            this.dataGridView_ROI.Name = "dataGridView_ROI";
            this.dataGridView_ROI.ReadOnly = true;
            // 
            // dataGridView_Math
            // 
            this.dataGridView_Math.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dataGridView_Math.FillWeight = 20F;
            this.dataGridView_Math.HeaderText = "М";
            this.dataGridView_Math.Name = "dataGridView_Math";
            this.dataGridView_Math.ReadOnly = true;
            // 
            // dataGridView_Physics
            // 
            this.dataGridView_Physics.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dataGridView_Physics.FillWeight = 20F;
            this.dataGridView_Physics.HeaderText = "Ф";
            this.dataGridView_Physics.Name = "dataGridView_Physics";
            this.dataGridView_Physics.ReadOnly = true;
            // 
            // dataGridView_Russian
            // 
            this.dataGridView_Russian.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dataGridView_Russian.FillWeight = 20F;
            this.dataGridView_Russian.HeaderText = "Р";
            this.dataGridView_Russian.Name = "dataGridView_Russian";
            this.dataGridView_Russian.ReadOnly = true;
            // 
            // dataGridView_Social
            // 
            this.dataGridView_Social.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dataGridView_Social.FillWeight = 20F;
            this.dataGridView_Social.HeaderText = "О";
            this.dataGridView_Social.Name = "dataGridView_Social";
            this.dataGridView_Social.ReadOnly = true;
            // 
            // dataGridView_Foreign
            // 
            this.dataGridView_Foreign.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dataGridView_Foreign.FillWeight = 20F;
            this.dataGridView_Foreign.HeaderText = "И";
            this.dataGridView_Foreign.Name = "dataGridView_Foreign";
            this.dataGridView_Foreign.ReadOnly = true;
            // 
            // OrderEdit
            // 
            this.AcceptButton = this.bSave;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1008, 561);
            this.Controls.Add(this.dataGridView);
            this.Controls.Add(this.panel);
            this.Name = "OrderEdit";
            this.Text = "Приказ";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
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
        private System.Windows.Forms.ComboBox cbDirOrProfile;
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
        private System.Windows.Forms.Label lDirOrProfile;
        private System.Windows.Forms.Panel panel;
        private System.Windows.Forms.DataGridView dataGridView;
        private System.Windows.Forms.CheckBox cbShowAdmitted;
        private System.Windows.Forms.Button bSave;
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
    }
}