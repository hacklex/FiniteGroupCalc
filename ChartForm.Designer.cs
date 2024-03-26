namespace FiniteGroupCalc
{
    partial class ChartForm
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
            PlotView = new OxyPlot.WindowsForms.PlotView();
            _panel = new Panel();
            panel2 = new Panel();
            panel1 = new Panel();
            _expFormulaLabel = new Label();
            _cosFormulaLabel = new Label();
            _cosFactorC = new TrackBar();
            _cosFactorB = new TrackBar();
            _cosFactorA = new TrackBar();
            _cosFactorE = new TrackBar();
            _expFactorSlider = new TrackBar();
            _dataPointsTextBox = new TextBox();
            label1 = new Label();
            _panel.SuspendLayout();
            panel2.SuspendLayout();
            panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)_cosFactorC).BeginInit();
            ((System.ComponentModel.ISupportInitialize)_cosFactorB).BeginInit();
            ((System.ComponentModel.ISupportInitialize)_cosFactorA).BeginInit();
            ((System.ComponentModel.ISupportInitialize)_cosFactorE).BeginInit();
            ((System.ComponentModel.ISupportInitialize)_expFactorSlider).BeginInit();
            SuspendLayout();
            // 
            // PlotView
            // 
            PlotView.Dock = DockStyle.Fill;
            PlotView.Location = new Point(0, 0);
            PlotView.Name = "PlotView";
            PlotView.PanCursor = Cursors.Hand;
            PlotView.Size = new Size(792, 413);
            PlotView.TabIndex = 0;
            PlotView.Text = "plotView1";
            PlotView.ZoomHorizontalCursor = Cursors.SizeWE;
            PlotView.ZoomRectangleCursor = Cursors.SizeNWSE;
            PlotView.ZoomVerticalCursor = Cursors.SizeNS;
            PlotView.Click += PlotView_Click;
            // 
            // _panel
            // 
            _panel.BorderStyle = BorderStyle.Fixed3D;
            _panel.Controls.Add(panel2);
            _panel.Controls.Add(panel1);
            _panel.Dock = DockStyle.Fill;
            _panel.Location = new Point(0, 0);
            _panel.Name = "_panel";
            _panel.Size = new Size(800, 607);
            _panel.TabIndex = 1;
            // 
            // panel2
            // 
            panel2.BorderStyle = BorderStyle.Fixed3D;
            panel2.Controls.Add(PlotView);
            panel2.Dock = DockStyle.Fill;
            panel2.Location = new Point(0, 186);
            panel2.Name = "panel2";
            panel2.Size = new Size(796, 417);
            panel2.TabIndex = 2;
            // 
            // panel1
            // 
            panel1.Controls.Add(label1);
            panel1.Controls.Add(_dataPointsTextBox);
            panel1.Controls.Add(_expFormulaLabel);
            panel1.Controls.Add(_cosFormulaLabel);
            panel1.Controls.Add(_cosFactorC);
            panel1.Controls.Add(_cosFactorB);
            panel1.Controls.Add(_cosFactorA);
            panel1.Controls.Add(_cosFactorE);
            panel1.Controls.Add(_expFactorSlider);
            panel1.Dock = DockStyle.Top;
            panel1.Location = new Point(0, 0);
            panel1.Name = "panel1";
            panel1.Size = new Size(796, 186);
            panel1.TabIndex = 1;
            // 
            // _expFormulaLabel
            // 
            _expFormulaLabel.AutoSize = true;
            _expFormulaLabel.Location = new Point(10, 9);
            _expFormulaLabel.Name = "_expFormulaLabel";
            _expFormulaLabel.Size = new Size(53, 15);
            _expFormulaLabel.TabIndex = 3;
            _expFormulaLabel.Text = "Exp(-Cx)";
            // 
            // _cosFormulaLabel
            // 
            _cosFormulaLabel.AutoSize = true;
            _cosFormulaLabel.Location = new Point(396, 9);
            _cosFormulaLabel.Name = "_cosFormulaLabel";
            _cosFormulaLabel.Size = new Size(127, 15);
            _cosFormulaLabel.TabIndex = 2;
            _cosFormulaLabel.Text = "E*Exp(-Ax)*Sin(B*x+C)";
            // 
            // _cosFactorC
            // 
            _cosFactorC.Location = new Point(401, 121);
            _cosFactorC.Margin = new Padding(0);
            _cosFactorC.Maximum = 7000;
            _cosFactorC.Name = "_cosFactorC";
            _cosFactorC.Size = new Size(385, 45);
            _cosFactorC.TabIndex = 0;
            _cosFactorC.TickFrequency = 10;
            _cosFactorC.Value = 1;
            _cosFactorC.Scroll += _expFactorSlider_Scroll;
            // 
            // _cosFactorB
            // 
            _cosFactorB.Location = new Point(401, 90);
            _cosFactorB.Margin = new Padding(0);
            _cosFactorB.Maximum = 5000;
            _cosFactorB.Name = "_cosFactorB";
            _cosFactorB.Size = new Size(385, 45);
            _cosFactorB.TabIndex = 0;
            _cosFactorB.TickFrequency = 10;
            _cosFactorB.Value = 1;
            _cosFactorB.Scroll += _expFactorSlider_Scroll;
            // 
            // _cosFactorA
            // 
            _cosFactorA.Location = new Point(401, 57);
            _cosFactorA.Margin = new Padding(0);
            _cosFactorA.Maximum = 5000;
            _cosFactorA.Name = "_cosFactorA";
            _cosFactorA.Size = new Size(385, 45);
            _cosFactorA.TabIndex = 0;
            _cosFactorA.TickFrequency = 10;
            _cosFactorA.Value = 1;
            _cosFactorA.Scroll += _expFactorSlider_Scroll;
            // 
            // _cosFactorE
            // 
            _cosFactorE.Location = new Point(401, 27);
            _cosFactorE.Margin = new Padding(0);
            _cosFactorE.Maximum = 5000;
            _cosFactorE.Name = "_cosFactorE";
            _cosFactorE.Size = new Size(385, 45);
            _cosFactorE.TabIndex = 0;
            _cosFactorE.TickFrequency = 10;
            _cosFactorE.Value = 1;
            _cosFactorE.Scroll += _expFactorSlider_Scroll;
            // 
            // _expFactorSlider
            // 
            _expFactorSlider.Location = new Point(10, 27);
            _expFactorSlider.Maximum = 3000;
            _expFactorSlider.Minimum = 1;
            _expFactorSlider.Name = "_expFactorSlider";
            _expFactorSlider.Size = new Size(385, 45);
            _expFactorSlider.TabIndex = 0;
            _expFactorSlider.TickFrequency = 10;
            _expFactorSlider.Value = 1;
            _expFactorSlider.Scroll += _expFactorSlider_Scroll;
            // 
            // _dataPointsTextBox
            // 
            _dataPointsTextBox.Location = new Point(10, 157);
            _dataPointsTextBox.Name = "_dataPointsTextBox";
            _dataPointsTextBox.Size = new Size(776, 23);
            _dataPointsTextBox.TabIndex = 4;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(10, 139);
            label1.Name = "label1";
            label1.Size = new Size(122, 15);
            label1.TabIndex = 5;
            label1.Text = "Copyable data points:";
            // 
            // ChartForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 607);
            Controls.Add(_panel);
            Name = "ChartForm";
            Text = "Plot";
            Load += ChartForm_Load;
            _panel.ResumeLayout(false);
            panel2.ResumeLayout(false);
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)_cosFactorC).EndInit();
            ((System.ComponentModel.ISupportInitialize)_cosFactorB).EndInit();
            ((System.ComponentModel.ISupportInitialize)_cosFactorA).EndInit();
            ((System.ComponentModel.ISupportInitialize)_cosFactorE).EndInit();
            ((System.ComponentModel.ISupportInitialize)_expFactorSlider).EndInit();
            ResumeLayout(false);
        }

        #endregion
        private Panel _panel;
        public OxyPlot.WindowsForms.PlotView PlotView;
        private Panel panel2;
        private Panel panel1;
        private TrackBar _expFactorSlider;
        private TrackBar _cosFactorE;
        private Label _cosFormulaLabel;
        private Label _expFormulaLabel;
        private TrackBar _cosFactorC;
        private TrackBar _cosFactorB;
        private TrackBar _cosFactorA;
        private Label label1;
        public TextBox _dataPointsTextBox;
    }
}