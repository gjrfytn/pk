namespace PK.Forms
{
    partial class KLADR_Update
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
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.lSubjects = new System.Windows.Forms.Label();
            this.lStreets = new System.Windows.Forms.Label();
            this.lHouses = new System.Windows.Forms.Label();
            this.tbSubjects = new System.Windows.Forms.TextBox();
            this.tbStreets = new System.Windows.Forms.TextBox();
            this.tbHouses = new System.Windows.Forms.TextBox();
            this.bSubjects = new System.Windows.Forms.Button();
            this.bStreets = new System.Windows.Forms.Button();
            this.bHouses = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.bUpdate = new System.Windows.Forms.Button();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.statusStrip_Label = new System.Windows.Forms.ToolStripStatusLabel();
            this.statusStrip_ProgressBar = new System.Windows.Forms.ToolStripProgressBar();
            this.statusStrip_ProgressLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.statusStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // openFileDialog
            // 
            this.openFileDialog.DefaultExt = "dbf";
            this.openFileDialog.Filter = "DBF-файлы|*.dbf|Все файлы|*.*";
            // 
            // lSubjects
            // 
            this.lSubjects.AutoSize = true;
            this.lSubjects.Location = new System.Drawing.Point(12, 28);
            this.lSubjects.Name = "lSubjects";
            this.lSubjects.Size = new System.Drawing.Size(60, 13);
            this.lSubjects.TabIndex = 1;
            this.lSubjects.Text = "Субъекты:";
            // 
            // lStreets
            // 
            this.lStreets.AutoSize = true;
            this.lStreets.Location = new System.Drawing.Point(28, 54);
            this.lStreets.Name = "lStreets";
            this.lStreets.Size = new System.Drawing.Size(44, 13);
            this.lStreets.TabIndex = 3;
            this.lStreets.Text = "Улицы:";
            // 
            // lHouses
            // 
            this.lHouses.AutoSize = true;
            this.lHouses.Location = new System.Drawing.Point(33, 80);
            this.lHouses.Name = "lHouses";
            this.lHouses.Size = new System.Drawing.Size(39, 13);
            this.lHouses.TabIndex = 5;
            this.lHouses.Text = "Дома:";
            // 
            // tbSubjects
            // 
            this.tbSubjects.Location = new System.Drawing.Point(78, 25);
            this.tbSubjects.Name = "tbSubjects";
            this.tbSubjects.ReadOnly = true;
            this.tbSubjects.Size = new System.Drawing.Size(364, 20);
            this.tbSubjects.TabIndex = 8;
            // 
            // tbStreets
            // 
            this.tbStreets.Location = new System.Drawing.Point(78, 51);
            this.tbStreets.Name = "tbStreets";
            this.tbStreets.ReadOnly = true;
            this.tbStreets.Size = new System.Drawing.Size(364, 20);
            this.tbStreets.TabIndex = 9;
            // 
            // tbHouses
            // 
            this.tbHouses.Location = new System.Drawing.Point(78, 77);
            this.tbHouses.Name = "tbHouses";
            this.tbHouses.ReadOnly = true;
            this.tbHouses.Size = new System.Drawing.Size(364, 20);
            this.tbHouses.TabIndex = 10;
            // 
            // bSubjects
            // 
            this.bSubjects.Location = new System.Drawing.Point(448, 23);
            this.bSubjects.Name = "bSubjects";
            this.bSubjects.Size = new System.Drawing.Size(24, 23);
            this.bSubjects.TabIndex = 2;
            this.bSubjects.Text = "...";
            this.bSubjects.UseVisualStyleBackColor = true;
            this.bSubjects.Click += new System.EventHandler(this.bSubjects_Click);
            // 
            // bStreets
            // 
            this.bStreets.Location = new System.Drawing.Point(448, 49);
            this.bStreets.Name = "bStreets";
            this.bStreets.Size = new System.Drawing.Size(24, 23);
            this.bStreets.TabIndex = 4;
            this.bStreets.Text = "...";
            this.bStreets.UseVisualStyleBackColor = true;
            this.bStreets.Click += new System.EventHandler(this.bStreets_Click);
            // 
            // bHouses
            // 
            this.bHouses.Location = new System.Drawing.Point(448, 75);
            this.bHouses.Name = "bHouses";
            this.bHouses.Size = new System.Drawing.Size(24, 23);
            this.bHouses.TabIndex = 6;
            this.bHouses.Text = "...";
            this.bHouses.UseVisualStyleBackColor = true;
            this.bHouses.Click += new System.EventHandler(this.bHouses_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(220, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Выберите DBF файлы с данными КЛАДР:";
            // 
            // bUpdate
            // 
            this.bUpdate.Location = new System.Drawing.Point(378, 106);
            this.bUpdate.Name = "bUpdate";
            this.bUpdate.Size = new System.Drawing.Size(64, 23);
            this.bUpdate.TabIndex = 7;
            this.bUpdate.Text = "Обновить";
            this.bUpdate.UseVisualStyleBackColor = true;
            this.bUpdate.Click += new System.EventHandler(this.bUpdate_Click);
            // 
            // statusStrip
            // 
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusStrip_Label,
            this.statusStrip_ProgressBar,
            this.statusStrip_ProgressLabel});
            this.statusStrip.Location = new System.Drawing.Point(0, 139);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(484, 22);
            this.statusStrip.TabIndex = 11;
            this.statusStrip.Text = "statusStrip1";
            // 
            // statusStrip_Label
            // 
            this.statusStrip_Label.Name = "statusStrip_Label";
            this.statusStrip_Label.Size = new System.Drawing.Size(118, 17);
            this.statusStrip_Label.Text = "Ожидание выбора...";
            // 
            // statusStrip_ProgressBar
            // 
            this.statusStrip_ProgressBar.Name = "statusStrip_ProgressBar";
            this.statusStrip_ProgressBar.Size = new System.Drawing.Size(200, 16);
            this.statusStrip_ProgressBar.Visible = false;
            // 
            // statusStrip_ProgressLabel
            // 
            this.statusStrip_ProgressLabel.Name = "statusStrip_ProgressLabel";
            this.statusStrip_ProgressLabel.Size = new System.Drawing.Size(0, 17);
            // 
            // KLADR_Update
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(484, 161);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.bUpdate);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.bHouses);
            this.Controls.Add(this.bStreets);
            this.Controls.Add(this.bSubjects);
            this.Controls.Add(this.tbHouses);
            this.Controls.Add(this.tbStreets);
            this.Controls.Add(this.tbSubjects);
            this.Controls.Add(this.lHouses);
            this.Controls.Add(this.lStreets);
            this.Controls.Add(this.lSubjects);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "KLADR_Update";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Обновление КЛАДР";
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.Label lSubjects;
        private System.Windows.Forms.Label lStreets;
        private System.Windows.Forms.Label lHouses;
        private System.Windows.Forms.TextBox tbSubjects;
        private System.Windows.Forms.TextBox tbStreets;
        private System.Windows.Forms.TextBox tbHouses;
        private System.Windows.Forms.Button bSubjects;
        private System.Windows.Forms.Button bStreets;
        private System.Windows.Forms.Button bHouses;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button bUpdate;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel statusStrip_Label;
        private System.Windows.Forms.ToolStripProgressBar statusStrip_ProgressBar;
        private System.Windows.Forms.ToolStripStatusLabel statusStrip_ProgressLabel;
    }
}