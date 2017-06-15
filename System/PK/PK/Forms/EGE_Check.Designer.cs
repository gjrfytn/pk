namespace PK.Forms
{
    partial class EGE_Check
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
            this.bExport = new System.Windows.Forms.Button();
            this.bImport = new System.Windows.Forms.Button();
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.SuspendLayout();
            // 
            // bExport
            // 
            this.bExport.Location = new System.Drawing.Point(12, 12);
            this.bExport.Name = "bExport";
            this.bExport.Size = new System.Drawing.Size(215, 23);
            this.bExport.TabIndex = 0;
            this.bExport.Text = "Выгрузить непроверенные результаты";
            this.bExport.UseVisualStyleBackColor = true;
            this.bExport.Click += new System.EventHandler(this.bExport_Click);
            // 
            // bImport
            // 
            this.bImport.Location = new System.Drawing.Point(12, 41);
            this.bImport.Name = "bImport";
            this.bImport.Size = new System.Drawing.Size(215, 23);
            this.bImport.TabIndex = 1;
            this.bImport.Text = "Загрузить проверенные результаты";
            this.bImport.UseVisualStyleBackColor = true;
            this.bImport.Click += new System.EventHandler(this.bImport_Click);
            // 
            // saveFileDialog
            // 
            this.saveFileDialog.DefaultExt = "csv";
            this.saveFileDialog.Filter = "CSV-файлы|*.csv|Все файлы|*.*";
            this.saveFileDialog.Title = "Сохранить файл для проверки";
            // 
            // openFileDialog
            // 
            this.openFileDialog.DefaultExt = "csv";
            this.openFileDialog.Filter = "CSV-файлы|*.csv|Все файлы|*.*";
            this.openFileDialog.Title = "Открыть файл с результатами проверки";
            // 
            // EGE_Check
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(241, 72);
            this.Controls.Add(this.bImport);
            this.Controls.Add(this.bExport);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = global::PK.Properties.Resources.logo;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "EGE_Check";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Проверка ЕГЭ";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button bExport;
        private System.Windows.Forms.Button bImport;
        private System.Windows.Forms.SaveFileDialog saveFileDialog;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
    }
}