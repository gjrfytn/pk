using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PK
{
    public partial class MainForm : Form
    {
        bool _JustLoaded = true;

        public MainForm()
        {
            InitializeComponent();            
        }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.tsbCreateApplication = new System.Windows.Forms.ToolStripButton();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.приемнаяКампанияToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.приемныеКампанииToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.абитуриентыToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.создатьЗаявлениеToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsbCreateApplication});
            this.toolStrip1.Location = new System.Drawing.Point(0, 24);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(682, 25);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // tsbCreateApplication
            // 
            this.tsbCreateApplication.Image = ((System.Drawing.Image)(resources.GetObject("tsbCreateApplication.Image")));
            this.tsbCreateApplication.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbCreateApplication.Name = "tsbCreateApplication";
            this.tsbCreateApplication.Size = new System.Drawing.Size(129, 22);
            this.tsbCreateApplication.Text = "Создать заявление";
            this.tsbCreateApplication.Click += new System.EventHandler(this.tsbCreateApplication_Click);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.приемнаяКампанияToolStripMenuItem,
            this.абитуриентыToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(682, 24);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // приемнаяКампанияToolStripMenuItem
            // 
            this.приемнаяКампанияToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.приемныеКампанииToolStripMenuItem});
            this.приемнаяКампанияToolStripMenuItem.Name = "приемнаяКампанияToolStripMenuItem";
            this.приемнаяКампанияToolStripMenuItem.Size = new System.Drawing.Size(133, 20);
            this.приемнаяКампанияToolStripMenuItem.Text = "Приемная кампания";
            // 
            // приемныеКампанииToolStripMenuItem
            // 
            this.приемныеКампанииToolStripMenuItem.Name = "приемныеКампанииToolStripMenuItem";
            this.приемныеКампанииToolStripMenuItem.Size = new System.Drawing.Size(192, 22);
            this.приемныеКампанииToolStripMenuItem.Text = "Приемные кампании";
            this.приемныеКампанииToolStripMenuItem.Click += new System.EventHandler(this.приемныеКампанииToolStripMenuItem_Click);
            // 
            // абитуриентыToolStripMenuItem
            // 
            this.абитуриентыToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.создатьЗаявлениеToolStripMenuItem});
            this.абитуриентыToolStripMenuItem.Name = "абитуриентыToolStripMenuItem";
            this.абитуриентыToolStripMenuItem.Size = new System.Drawing.Size(93, 20);
            this.абитуриентыToolStripMenuItem.Text = "Абитуриенты";
            // 
            // создатьЗаявлениеToolStripMenuItem
            // 
            this.создатьЗаявлениеToolStripMenuItem.Name = "создатьЗаявлениеToolStripMenuItem";
            this.создатьЗаявлениеToolStripMenuItem.Size = new System.Drawing.Size(176, 22);
            this.создатьЗаявлениеToolStripMenuItem.Text = "Создать заявление";
            this.создатьЗаявлениеToolStripMenuItem.Click += new System.EventHandler(this.создатьЗаявлениеToolStripMenuItem_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(682, 461);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainForm";
            this.Text = "ИС \"Приемная комиссия\"";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.Shown += new System.EventHandler(this.MainForm_Shown);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        

        private void MainForm_Load(object sender, EventArgs e)
        {
            AutorizationForm authForm = new AutorizationForm(this);
            authForm.Show();
        }

        private void MainForm_Shown(object sender, EventArgs e)
        {
            if (_JustLoaded) 
            {
                _JustLoaded = false;
                this.Hide();
            }

        }

        private void приемныеКампанииToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PriemCompForm PCForm = new PriemCompForm();
            PCForm.Show();
        }

        private void создатьЗаявлениеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NewApplicForm NAForm = new NewApplicForm();
            NAForm.Show();
        }

        private void tsbCreateApplication_Click(object sender, EventArgs e)
        {
            NewApplicForm NAForm = new NewApplicForm();
            NAForm.Show();
        }
    }
}
