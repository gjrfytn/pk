namespace PK.Forms
{
    partial class Statistics
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
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea2 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend2 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series2 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tpGeneral = new System.Windows.Forms.TabPage();
            this.chartGeneral = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.tpRegistrators = new System.Windows.Forms.TabPage();
            this.chartRegistrators = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.tabControl.SuspendLayout();
            this.tpGeneral.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chartGeneral)).BeginInit();
            this.tpRegistrators.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chartRegistrators)).BeginInit();
            this.SuspendLayout();
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.tpGeneral);
            this.tabControl.Controls.Add(this.tpRegistrators);
            this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl.Location = new System.Drawing.Point(0, 0);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(680, 501);
            this.tabControl.TabIndex = 0;
            this.tabControl.Selected += new System.Windows.Forms.TabControlEventHandler(this.tabControl_Selected);
            // 
            // tpGeneral
            // 
            this.tpGeneral.Controls.Add(this.chartGeneral);
            this.tpGeneral.Location = new System.Drawing.Point(4, 22);
            this.tpGeneral.Name = "tpGeneral";
            this.tpGeneral.Padding = new System.Windows.Forms.Padding(3);
            this.tpGeneral.Size = new System.Drawing.Size(672, 475);
            this.tpGeneral.TabIndex = 0;
            this.tpGeneral.Text = "Общая";
            this.tpGeneral.UseVisualStyleBackColor = true;
            // 
            // chartGeneral
            // 
            chartArea1.Name = "ChartArea";
            this.chartGeneral.ChartAreas.Add(chartArea1);
            this.chartGeneral.Dock = System.Windows.Forms.DockStyle.Fill;
            legend1.Name = "Legend";
            this.chartGeneral.Legends.Add(legend1);
            this.chartGeneral.Location = new System.Drawing.Point(3, 3);
            this.chartGeneral.Name = "chartGeneral";
            series1.ChartArea = "ChartArea";
            series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series1.IsValueShownAsLabel = true;
            series1.Legend = "Legend";
            series1.Name = "Series";
            series1.XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.String;
            series1.YValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.UInt32;
            this.chartGeneral.Series.Add(series1);
            this.chartGeneral.Size = new System.Drawing.Size(666, 469);
            this.chartGeneral.TabIndex = 0;
            this.chartGeneral.Text = "chart1";
            // 
            // tpRegistrators
            // 
            this.tpRegistrators.Controls.Add(this.chartRegistrators);
            this.tpRegistrators.Location = new System.Drawing.Point(4, 22);
            this.tpRegistrators.Name = "tpRegistrators";
            this.tpRegistrators.Padding = new System.Windows.Forms.Padding(3);
            this.tpRegistrators.Size = new System.Drawing.Size(672, 475);
            this.tpRegistrators.TabIndex = 1;
            this.tpRegistrators.Text = "Регистраторы";
            this.tpRegistrators.UseVisualStyleBackColor = true;
            // 
            // chartRegistrators
            // 
            chartArea2.AxisX.IntervalAutoMode = System.Windows.Forms.DataVisualization.Charting.IntervalAutoMode.VariableCount;
            chartArea2.Name = "ChartArea";
            this.chartRegistrators.ChartAreas.Add(chartArea2);
            this.chartRegistrators.Dock = System.Windows.Forms.DockStyle.Fill;
            legend2.Name = "Legend";
            this.chartRegistrators.Legends.Add(legend2);
            this.chartRegistrators.Location = new System.Drawing.Point(3, 3);
            this.chartRegistrators.Name = "chartRegistrators";
            series2.ChartArea = "ChartArea";
            series2.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Bar;
            series2.IsValueShownAsLabel = true;
            series2.Legend = "Legend";
            series2.Name = "Series";
            series2.XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.String;
            series2.YValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.UInt32;
            this.chartRegistrators.Series.Add(series2);
            this.chartRegistrators.Size = new System.Drawing.Size(666, 469);
            this.chartRegistrators.TabIndex = 0;
            this.chartRegistrators.Text = "chart1";
            // 
            // Statistics
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(680, 501);
            this.Controls.Add(this.tabControl);
            this.Name = "Statistics";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Статистика";
            this.tabControl.ResumeLayout(false);
            this.tpGeneral.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.chartGeneral)).EndInit();
            this.tpRegistrators.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.chartRegistrators)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tpGeneral;
        private System.Windows.Forms.TabPage tpRegistrators;
        private System.Windows.Forms.DataVisualization.Charting.Chart chartGeneral;
        private System.Windows.Forms.DataVisualization.Charting.Chart chartRegistrators;
    }
}