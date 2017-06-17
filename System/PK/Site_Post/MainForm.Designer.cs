namespace SitePost
{
    partial class MainForm
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.cbCampaigns = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.cbAdress = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.cbInterval = new System.Windows.Forms.ComboBox();
            this.btStart = new System.Windows.Forms.Button();
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.cbUnits = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // cbCampaigns
            // 
            this.cbCampaigns.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbCampaigns.FormattingEnabled = true;
            this.cbCampaigns.Location = new System.Drawing.Point(47, 58);
            this.cbCampaigns.Name = "cbCampaigns";
            this.cbCampaigns.Size = new System.Drawing.Size(231, 21);
            this.cbCampaigns.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(111, 32);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(124, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Кампания бакалавров:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(125, 103);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(91, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Адрес отправки:";
            // 
            // cbAdress
            // 
            this.cbAdress.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbAdress.FormattingEnabled = true;
            this.cbAdress.Location = new System.Drawing.Point(12, 130);
            this.cbAdress.Name = "cbAdress";
            this.cbAdress.Size = new System.Drawing.Size(300, 21);
            this.cbAdress.TabIndex = 4;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 184);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(141, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Частота отправки: 1 раз в";
            // 
            // cbInterval
            // 
            this.cbInterval.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbInterval.FormattingEnabled = true;
            this.cbInterval.Location = new System.Drawing.Point(166, 181);
            this.cbInterval.Name = "cbInterval";
            this.cbInterval.Size = new System.Drawing.Size(69, 21);
            this.cbInterval.TabIndex = 6;
            // 
            // btStart
            // 
            this.btStart.Location = new System.Drawing.Point(128, 228);
            this.btStart.Name = "btStart";
            this.btStart.Size = new System.Drawing.Size(75, 23);
            this.btStart.TabIndex = 7;
            this.btStart.Text = "Старт";
            this.btStart.UseVisualStyleBackColor = true;
            this.btStart.Click += new System.EventHandler(this.btStart_Click);
            // 
            // timer
            // 
            this.timer.Interval = 10000;
            this.timer.Tick += new System.EventHandler(this.timer_Tick);
            // 
            // cbUnits
            // 
            this.cbUnits.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbUnits.FormattingEnabled = true;
            this.cbUnits.Items.AddRange(new object[] {
            "ч",
            "мин.",
            "сек."});
            this.cbUnits.Location = new System.Drawing.Point(250, 181);
            this.cbUnits.Name = "cbUnits";
            this.cbUnits.Size = new System.Drawing.Size(62, 21);
            this.cbUnits.TabIndex = 8;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(324, 269);
            this.Controls.Add(this.cbUnits);
            this.Controls.Add(this.btStart);
            this.Controls.Add(this.cbInterval);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.cbAdress);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cbCampaigns);
            this.Name = "MainForm";
            this.Text = "Отправка данных на сайт ПК МАДИ";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ComboBox cbCampaigns;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cbAdress;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cbInterval;
        private System.Windows.Forms.Button btStart;
        private System.Windows.Forms.Timer timer;
        private System.Windows.Forms.ComboBox cbUnits;
    }
}

