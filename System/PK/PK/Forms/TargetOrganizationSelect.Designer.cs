namespace PK
{
    partial class TargetOrganizationSelect
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
            this.btSelect = new System.Windows.Forms.Button();
            this.lbSelection = new System.Windows.Forms.ListBox();
            this.tbSearchString = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btSelect
            // 
            this.btSelect.Location = new System.Drawing.Point(161, 227);
            this.btSelect.Name = "btSelect";
            this.btSelect.Size = new System.Drawing.Size(75, 23);
            this.btSelect.TabIndex = 0;
            this.btSelect.Text = "Выбрать";
            this.btSelect.UseVisualStyleBackColor = true;
            this.btSelect.Click += new System.EventHandler(this.btSelect_Click);
            // 
            // lbSelection
            // 
            this.lbSelection.FormattingEnabled = true;
            this.lbSelection.Location = new System.Drawing.Point(28, 82);
            this.lbSelection.Name = "lbSelection";
            this.lbSelection.Size = new System.Drawing.Size(330, 134);
            this.lbSelection.TabIndex = 1;
            // 
            // tbSearchString
            // 
            this.tbSearchString.Location = new System.Drawing.Point(28, 35);
            this.tbSearchString.Name = "tbSearchString";
            this.tbSearchString.Size = new System.Drawing.Size(330, 20);
            this.tbSearchString.TabIndex = 2;
            this.tbSearchString.TextChanged += new System.EventHandler(this.tbSearchString_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(144, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(106, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Строка для поиска:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(144, 66);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(104, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Выберите вариант:";
            // 
            // TargetOrganizationSelect
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(389, 262);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tbSearchString);
            this.Controls.Add(this.lbSelection);
            this.Controls.Add(this.btSelect);
            this.Name = "TargetOrganizationSelect";
            this.Text = "TargetOrganizationSelect";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btSelect;
        private System.Windows.Forms.ListBox lbSelection;
        private System.Windows.Forms.TextBox tbSearchString;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
    }
}