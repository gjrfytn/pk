namespace PK
{
    partial class NewCampaignForm
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
            this.label1 = new System.Windows.Forms.Label();
            this.tbName = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.cbStartYear = new System.Windows.Forms.ComboBox();
            this.cbEndYear = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.cbType = new System.Windows.Forms.ComboBox();
            this.gbEduForms = new System.Windows.Forms.GroupBox();
            this.btRemoveEduLevel = new System.Windows.Forms.Button();
            this.btAddEduLevel = new System.Windows.Forms.Button();
            this.lbEduFormsChoosen = new System.Windows.Forms.ListBox();
            this.lbEduFormsAll = new System.Windows.Forms.ListBox();
            this.gbEduLevel = new System.Windows.Forms.GroupBox();
            this.cbEduLevelQual = new System.Windows.Forms.CheckBox();
            this.cbEduLevelSPO = new System.Windows.Forms.CheckBox();
            this.cbEduLevelSpec = new System.Windows.Forms.CheckBox();
            this.cbEduLevelMag = new System.Windows.Forms.CheckBox();
            this.cbEduLevelBacc = new System.Windows.Forms.CheckBox();
            this.label5 = new System.Windows.Forms.Label();
            this.cbState = new System.Windows.Forms.ComboBox();
            this.dgvDirections = new System.Windows.Forms.DataGridView();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.dgvTargetOrganizatons = new System.Windows.Forms.DataGridView();
            this.сSelected = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.cType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.сName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.сCode = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cOOOF = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cOOOZF = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cOOZF = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cOKOF = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cOKOZF = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cOKZF = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cCPOF = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cCPOZF = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cPMOF = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cPMOZF = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cPMZF = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cOrgName = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.cDirName = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.cCode = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.cOF = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cOZF = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.gbEduForms.SuspendLayout();
            this.gbEduLevel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDirections)).BeginInit();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTargetOrganizatons)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(16, 28);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(60, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Название:";
            // 
            // tbName
            // 
            this.tbName.Location = new System.Drawing.Point(94, 25);
            this.tbName.Name = "tbName";
            this.tbName.Size = new System.Drawing.Size(238, 20);
            this.tbName.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(345, 28);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(66, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Год начала:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(485, 28);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(84, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = "Год окончания:";
            // 
            // cbStartYear
            // 
            this.cbStartYear.FormattingEnabled = true;
            this.cbStartYear.Location = new System.Drawing.Point(417, 24);
            this.cbStartYear.Name = "cbStartYear";
            this.cbStartYear.Size = new System.Drawing.Size(62, 21);
            this.cbStartYear.TabIndex = 4;
            // 
            // cbEndYear
            // 
            this.cbEndYear.FormattingEnabled = true;
            this.cbEndYear.Location = new System.Drawing.Point(575, 24);
            this.cbEndYear.Name = "cbEndYear";
            this.cbEndYear.Size = new System.Drawing.Size(62, 21);
            this.cbEndYear.TabIndex = 5;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(16, 64);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(135, 13);
            this.label4.TabIndex = 6;
            this.label4.Text = "Тип приемной кампании:";
            // 
            // cbType
            // 
            this.cbType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbType.FormattingEnabled = true;
            this.cbType.Items.AddRange(new object[] {
            "Прием на обучение на бакалавриат/специалитет",
            "Прием на обучение в магистратуру",
            "Прием на обучение в СПО",
            "Прием на подготовку кадров высшей квалификации",
            "Прием иностранце по направлениям Минобрнауки"});
            this.cbType.Location = new System.Drawing.Point(157, 61);
            this.cbType.Name = "cbType";
            this.cbType.Size = new System.Drawing.Size(272, 21);
            this.cbType.TabIndex = 7;
            this.cbType.SelectedIndexChanged += new System.EventHandler(this.cbType_SelectedIndexChanged);
            // 
            // gbEduForms
            // 
            this.gbEduForms.Controls.Add(this.btRemoveEduLevel);
            this.gbEduForms.Controls.Add(this.btAddEduLevel);
            this.gbEduForms.Controls.Add(this.lbEduFormsChoosen);
            this.gbEduForms.Controls.Add(this.lbEduFormsAll);
            this.gbEduForms.Location = new System.Drawing.Point(13, 95);
            this.gbEduForms.Name = "gbEduForms";
            this.gbEduForms.Size = new System.Drawing.Size(315, 89);
            this.gbEduForms.TabIndex = 10;
            this.gbEduForms.TabStop = false;
            this.gbEduForms.Text = "Форма обучения";
            // 
            // btRemoveEduLevel
            // 
            this.btRemoveEduLevel.Font = new System.Drawing.Font("ESSTIXOne", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btRemoveEduLevel.Location = new System.Drawing.Point(133, 48);
            this.btRemoveEduLevel.Name = "btRemoveEduLevel";
            this.btRemoveEduLevel.Size = new System.Drawing.Size(46, 23);
            this.btRemoveEduLevel.TabIndex = 3;
            this.btRemoveEduLevel.Text = "f";
            this.btRemoveEduLevel.UseVisualStyleBackColor = true;
            this.btRemoveEduLevel.Click += new System.EventHandler(this.btRemoveEduLevel_Click);
            // 
            // btAddEduLevel
            // 
            this.btAddEduLevel.Font = new System.Drawing.Font("ESSTIXOne", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btAddEduLevel.Location = new System.Drawing.Point(133, 24);
            this.btAddEduLevel.Name = "btAddEduLevel";
            this.btAddEduLevel.Size = new System.Drawing.Size(46, 23);
            this.btAddEduLevel.TabIndex = 2;
            this.btAddEduLevel.Text = "g";
            this.btAddEduLevel.UseVisualStyleBackColor = true;
            this.btAddEduLevel.Click += new System.EventHandler(this.btAddEduLevel_Click);
            // 
            // lbEduFormsChoosen
            // 
            this.lbEduFormsChoosen.FormattingEnabled = true;
            this.lbEduFormsChoosen.Location = new System.Drawing.Point(185, 19);
            this.lbEduFormsChoosen.Name = "lbEduFormsChoosen";
            this.lbEduFormsChoosen.Size = new System.Drawing.Size(120, 56);
            this.lbEduFormsChoosen.Sorted = true;
            this.lbEduFormsChoosen.TabIndex = 1;
            // 
            // lbEduFormsAll
            // 
            this.lbEduFormsAll.FormattingEnabled = true;
            this.lbEduFormsAll.Items.AddRange(new object[] {
            "Заочная форма",
            "Очная форма",
            "Очно-заочная форма"});
            this.lbEduFormsAll.Location = new System.Drawing.Point(6, 19);
            this.lbEduFormsAll.Name = "lbEduFormsAll";
            this.lbEduFormsAll.Size = new System.Drawing.Size(120, 56);
            this.lbEduFormsAll.Sorted = true;
            this.lbEduFormsAll.TabIndex = 0;
            // 
            // gbEduLevel
            // 
            this.gbEduLevel.Controls.Add(this.cbEduLevelQual);
            this.gbEduLevel.Controls.Add(this.cbEduLevelSPO);
            this.gbEduLevel.Controls.Add(this.cbEduLevelSpec);
            this.gbEduLevel.Controls.Add(this.cbEduLevelMag);
            this.gbEduLevel.Controls.Add(this.cbEduLevelBacc);
            this.gbEduLevel.Location = new System.Drawing.Point(355, 95);
            this.gbEduLevel.Name = "gbEduLevel";
            this.gbEduLevel.Size = new System.Drawing.Size(284, 89);
            this.gbEduLevel.TabIndex = 11;
            this.gbEduLevel.TabStop = false;
            this.gbEduLevel.Text = "Уровень образования";
            // 
            // cbEduLevelQual
            // 
            this.cbEduLevelQual.AutoSize = true;
            this.cbEduLevelQual.Enabled = false;
            this.cbEduLevelQual.Location = new System.Drawing.Point(6, 67);
            this.cbEduLevelQual.Name = "cbEduLevelQual";
            this.cbEduLevelQual.Size = new System.Drawing.Size(179, 17);
            this.cbEduLevelQual.TabIndex = 4;
            this.cbEduLevelQual.Text = "Кадры высшей квалификации";
            this.cbEduLevelQual.UseVisualStyleBackColor = true;
            // 
            // cbEduLevelSPO
            // 
            this.cbEduLevelSPO.AutoSize = true;
            this.cbEduLevelSPO.Enabled = false;
            this.cbEduLevelSPO.Location = new System.Drawing.Point(191, 44);
            this.cbEduLevelSPO.Name = "cbEduLevelSPO";
            this.cbEduLevelSPO.Size = new System.Drawing.Size(49, 17);
            this.cbEduLevelSPO.TabIndex = 3;
            this.cbEduLevelSPO.Text = "СПО";
            this.cbEduLevelSPO.UseVisualStyleBackColor = true;
            // 
            // cbEduLevelSpec
            // 
            this.cbEduLevelSpec.AutoSize = true;
            this.cbEduLevelSpec.Enabled = false;
            this.cbEduLevelSpec.Location = new System.Drawing.Point(191, 20);
            this.cbEduLevelSpec.Name = "cbEduLevelSpec";
            this.cbEduLevelSpec.Size = new System.Drawing.Size(91, 17);
            this.cbEduLevelSpec.TabIndex = 2;
            this.cbEduLevelSpec.Text = "Специалитет";
            this.cbEduLevelSpec.UseVisualStyleBackColor = true;
            // 
            // cbEduLevelMag
            // 
            this.cbEduLevelMag.AutoSize = true;
            this.cbEduLevelMag.Enabled = false;
            this.cbEduLevelMag.Location = new System.Drawing.Point(7, 44);
            this.cbEduLevelMag.Name = "cbEduLevelMag";
            this.cbEduLevelMag.Size = new System.Drawing.Size(97, 17);
            this.cbEduLevelMag.TabIndex = 1;
            this.cbEduLevelMag.Text = "Магистратура";
            this.cbEduLevelMag.UseVisualStyleBackColor = true;
            // 
            // cbEduLevelBacc
            // 
            this.cbEduLevelBacc.AutoSize = true;
            this.cbEduLevelBacc.Enabled = false;
            this.cbEduLevelBacc.Location = new System.Drawing.Point(7, 20);
            this.cbEduLevelBacc.Name = "cbEduLevelBacc";
            this.cbEduLevelBacc.Size = new System.Drawing.Size(92, 17);
            this.cbEduLevelBacc.TabIndex = 0;
            this.cbEduLevelBacc.Text = "Бакалавриат";
            this.cbEduLevelBacc.UseVisualStyleBackColor = true;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(435, 64);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(44, 13);
            this.label5.TabIndex = 12;
            this.label5.Text = "Статус:";
            // 
            // cbState
            // 
            this.cbState.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbState.FormattingEnabled = true;
            this.cbState.Items.AddRange(new object[] {
            "Набор не начался",
            "Идет набор",
            "Завершена"});
            this.cbState.Location = new System.Drawing.Point(485, 61);
            this.cbState.Name = "cbState";
            this.cbState.Size = new System.Drawing.Size(152, 21);
            this.cbState.TabIndex = 13;
            // 
            // dgvDirections
            // 
            this.dgvDirections.AllowUserToAddRows = false;
            this.dgvDirections.AllowUserToDeleteRows = false;
            this.dgvDirections.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvDirections.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.сSelected,
            this.cType,
            this.сName,
            this.сCode,
            this.cOOOF,
            this.cOOOZF,
            this.cOOZF,
            this.cOKOF,
            this.cOKOZF,
            this.cOKZF,
            this.cCPOF,
            this.cCPOZF,
            this.cPMOF,
            this.cPMOZF,
            this.cPMZF});
            this.dgvDirections.Location = new System.Drawing.Point(0, 18);
            this.dgvDirections.Name = "dgvDirections";
            this.dgvDirections.ReadOnly = true;
            this.dgvDirections.RowHeadersVisible = false;
            this.dgvDirections.RowHeadersWidth = 20;
            this.dgvDirections.Size = new System.Drawing.Size(805, 251);
            this.dgvDirections.TabIndex = 1;
            // 
            // panel1
            // 
            this.panel1.AutoScroll = true;
            this.panel1.Controls.Add(this.dgvTargetOrganizatons);
            this.panel1.Controls.Add(this.label7);
            this.panel1.Controls.Add(this.label6);
            this.panel1.Controls.Add(this.dgvDirections);
            this.panel1.Location = new System.Drawing.Point(12, 190);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(808, 547);
            this.panel1.TabIndex = 15;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(333, 2);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(145, 13);
            this.label6.TabIndex = 2;
            this.label6.Text = "Объем и структура приема";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(357, 272);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(121, 13);
            this.label7.TabIndex = 3;
            this.label7.Text = "Целевые организации";
            // 
            // dgvTargetOrganizatons
            // 
            this.dgvTargetOrganizatons.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvTargetOrganizatons.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.cOrgName,
            this.cDirName,
            this.cCode,
            this.cOF,
            this.cOZF});
            this.dgvTargetOrganizatons.Location = new System.Drawing.Point(3, 295);
            this.dgvTargetOrganizatons.Name = "dgvTargetOrganizatons";
            this.dgvTargetOrganizatons.RowHeadersWidth = 20;
            this.dgvTargetOrganizatons.Size = new System.Drawing.Size(802, 249);
            this.dgvTargetOrganizatons.TabIndex = 4;
            this.dgvTargetOrganizatons.RowsAdded += new System.Windows.Forms.DataGridViewRowsAddedEventHandler(this.dgvTargetOrganizatons_RowsAdded);
            // 
            // сSelected
            // 
            this.сSelected.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.сSelected.Frozen = true;
            this.сSelected.HeaderText = "";
            this.сSelected.Name = "сSelected";
            this.сSelected.ReadOnly = true;
            this.сSelected.Width = 5;
            // 
            // cType
            // 
            this.cType.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.cType.Frozen = true;
            this.cType.HeaderText = "Тип";
            this.cType.Name = "cType";
            this.cType.ReadOnly = true;
            this.cType.Width = 30;
            // 
            // сName
            // 
            this.сName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
            this.сName.Frozen = true;
            this.сName.HeaderText = "Направление";
            this.сName.Name = "сName";
            this.сName.ReadOnly = true;
            // 
            // сCode
            // 
            this.сCode.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
            this.сCode.Frozen = true;
            this.сCode.HeaderText = "Код направления";
            this.сCode.Name = "сCode";
            this.сCode.ReadOnly = true;
            this.сCode.Width = 110;
            // 
            // cOOOF
            // 
            this.cOOOF.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.cOOOF.HeaderText = "Общие основания, очная форма";
            this.cOOOF.Name = "cOOOF";
            this.cOOOF.ReadOnly = true;
            this.cOOOF.Width = 80;
            // 
            // cOOOZF
            // 
            this.cOOOZF.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.cOOOZF.HeaderText = "Общие основания, очно-заочная форма";
            this.cOOOZF.Name = "cOOOZF";
            this.cOOOZF.ReadOnly = true;
            this.cOOOZF.Width = 80;
            // 
            // cOOZF
            // 
            this.cOOZF.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.cOOZF.HeaderText = "Общие основания, заочная форма";
            this.cOOZF.Name = "cOOZF";
            this.cOOZF.ReadOnly = true;
            this.cOOZF.Width = 80;
            // 
            // cOKOF
            // 
            this.cOKOF.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.cOKOF.HeaderText = "Особая квота, очная форма";
            this.cOKOF.Name = "cOKOF";
            this.cOKOF.ReadOnly = true;
            this.cOKOF.Width = 80;
            // 
            // cOKOZF
            // 
            this.cOKOZF.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.cOKOZF.HeaderText = "Особая квота, очно-заочная форма";
            this.cOKOZF.Name = "cOKOZF";
            this.cOKOZF.ReadOnly = true;
            this.cOKOZF.Width = 80;
            // 
            // cOKZF
            // 
            this.cOKZF.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.cOKZF.HeaderText = "Особая квота, заочная форма";
            this.cOKZF.Name = "cOKZF";
            this.cOKZF.ReadOnly = true;
            this.cOKZF.Width = 80;
            // 
            // cCPOF
            // 
            this.cCPOF.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.cCPOF.HeaderText = "Целевой прием, очная форма";
            this.cCPOF.Name = "cCPOF";
            this.cCPOF.ReadOnly = true;
            this.cCPOF.Width = 80;
            // 
            // cCPOZF
            // 
            this.cCPOZF.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.cCPOZF.HeaderText = "Целевой прием, очно-заочная форма";
            this.cCPOZF.Name = "cCPOZF";
            this.cCPOZF.ReadOnly = true;
            this.cCPOZF.Width = 80;
            // 
            // cPMOF
            // 
            this.cPMOF.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.cPMOF.HeaderText = "Платные места, очная форма";
            this.cPMOF.Name = "cPMOF";
            this.cPMOF.ReadOnly = true;
            this.cPMOF.Width = 80;
            // 
            // cPMOZF
            // 
            this.cPMOZF.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.cPMOZF.HeaderText = "Платные места, очно-заочная форма";
            this.cPMOZF.Name = "cPMOZF";
            this.cPMOZF.ReadOnly = true;
            this.cPMOZF.Width = 80;
            // 
            // cPMZF
            // 
            this.cPMZF.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.cPMZF.HeaderText = "Платные места, заочная форма";
            this.cPMZF.Name = "cPMZF";
            this.cPMZF.ReadOnly = true;
            this.cPMZF.Width = 80;
            // 
            // cOrgName
            // 
            this.cOrgName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.cOrgName.DisplayStyle = System.Windows.Forms.DataGridViewComboBoxDisplayStyle.ComboBox;
            this.cOrgName.HeaderText = "Название организации";
            this.cOrgName.Name = "cOrgName";
            // 
            // cDirName
            // 
            this.cDirName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.cDirName.DisplayStyle = System.Windows.Forms.DataGridViewComboBoxDisplayStyle.ComboBox;
            this.cDirName.HeaderText = "Название направления";
            this.cDirName.Name = "cDirName";
            // 
            // cCode
            // 
            this.cCode.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
            this.cCode.DisplayStyle = System.Windows.Forms.DataGridViewComboBoxDisplayStyle.ComboBox;
            this.cCode.HeaderText = "Код направления";
            this.cCode.Name = "cCode";
            this.cCode.Width = 101;
            // 
            // cOF
            // 
            this.cOF.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.cOF.HeaderText = "Места на очной форме обучения";
            this.cOF.Name = "cOF";
            this.cOF.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.cOF.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // cOZF
            // 
            this.cOZF.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.cOZF.HeaderText = "Места на очно-заочной форме обучения";
            this.cOZF.Name = "cOZF";
            this.cOZF.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.cOZF.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // NewCampaignForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(832, 749);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.cbState);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.gbEduLevel);
            this.Controls.Add(this.gbEduForms);
            this.Controls.Add(this.cbType);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.cbEndYear);
            this.Controls.Add(this.cbStartYear);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.tbName);
            this.Controls.Add(this.label1);
            this.Name = "NewCampaignForm";
            this.Text = "Создание приемной кампании";
            this.Load += new System.EventHandler(this.NewCampaignForm_Load);
            this.gbEduForms.ResumeLayout(false);
            this.gbEduLevel.ResumeLayout(false);
            this.gbEduLevel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDirections)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTargetOrganizatons)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tbName;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cbStartYear;
        private System.Windows.Forms.ComboBox cbEndYear;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox cbType;
        private System.Windows.Forms.GroupBox gbEduForms;
        private System.Windows.Forms.Button btRemoveEduLevel;
        private System.Windows.Forms.Button btAddEduLevel;
        private System.Windows.Forms.ListBox lbEduFormsChoosen;
        private System.Windows.Forms.ListBox lbEduFormsAll;
        private System.Windows.Forms.GroupBox gbEduLevel;
        private System.Windows.Forms.CheckBox cbEduLevelQual;
        private System.Windows.Forms.CheckBox cbEduLevelSPO;
        private System.Windows.Forms.CheckBox cbEduLevelSpec;
        private System.Windows.Forms.CheckBox cbEduLevelMag;
        private System.Windows.Forms.CheckBox cbEduLevelBacc;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox cbState;
        private System.Windows.Forms.DataGridView dgvDirections;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.DataGridView dgvTargetOrganizatons;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.DataGridViewCheckBoxColumn сSelected;
        private System.Windows.Forms.DataGridViewTextBoxColumn cType;
        private System.Windows.Forms.DataGridViewTextBoxColumn сName;
        private System.Windows.Forms.DataGridViewTextBoxColumn сCode;
        private System.Windows.Forms.DataGridViewTextBoxColumn cOOOF;
        private System.Windows.Forms.DataGridViewTextBoxColumn cOOOZF;
        private System.Windows.Forms.DataGridViewTextBoxColumn cOOZF;
        private System.Windows.Forms.DataGridViewTextBoxColumn cOKOF;
        private System.Windows.Forms.DataGridViewTextBoxColumn cOKOZF;
        private System.Windows.Forms.DataGridViewTextBoxColumn cOKZF;
        private System.Windows.Forms.DataGridViewTextBoxColumn cCPOF;
        private System.Windows.Forms.DataGridViewTextBoxColumn cCPOZF;
        private System.Windows.Forms.DataGridViewTextBoxColumn cPMOF;
        private System.Windows.Forms.DataGridViewTextBoxColumn cPMOZF;
        private System.Windows.Forms.DataGridViewTextBoxColumn cPMZF;
        private System.Windows.Forms.DataGridViewComboBoxColumn cOrgName;
        private System.Windows.Forms.DataGridViewComboBoxColumn cDirName;
        private System.Windows.Forms.DataGridViewComboBoxColumn cCode;
        private System.Windows.Forms.DataGridViewTextBoxColumn cOF;
        private System.Windows.Forms.DataGridViewTextBoxColumn cOZF;
    }
}