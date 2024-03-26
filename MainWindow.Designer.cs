using FiniteGroupCalc.Widgets;

namespace FiniteGroupCalc
{
    partial class MainWindow
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            panel1 = new Panel();
            _properties = new PropertyGridEx();
            _processorComboBox = new ComboBox();
            _calcGrowthButton = new Button();
            _orderLimitLabel = new Label();
            _orderLimitUpDown = new NumericUpDownEx();
            _outputTextBox = new TextBox();
            _estimateLabel = new Label();
            _estimateFormula = new TextBox();
            panel2 = new Panel();
            _toolTip = new ToolTip(components);
            panel3 = new Panel();
            panel5 = new Panel();
            panel4 = new Panel();
            _progressBar = new CustomTextProgressBar();
            panel6 = new Panel();
            panel7 = new Panel();
            _elementPairStats = new Button();
            button1 = new Button();
            _testButton = new Button();
            panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)_orderLimitUpDown).BeginInit();
            panel2.SuspendLayout();
            panel3.SuspendLayout();
            panel5.SuspendLayout();
            panel4.SuspendLayout();
            panel6.SuspendLayout();
            panel7.SuspendLayout();
            SuspendLayout();
            // 
            // panel1
            // 
            panel1.Controls.Add(_properties);
            panel1.Controls.Add(_processorComboBox);
            panel1.Dock = DockStyle.Right;
            panel1.Location = new Point(526, 0);
            panel1.Name = "panel1";
            panel1.Padding = new Padding(6);
            panel1.Size = new Size(233, 461);
            panel1.TabIndex = 10;
            // 
            // _properties
            // 
            _properties.Dock = DockStyle.Fill;
            _properties.Location = new Point(6, 29);
            _properties.Name = "_properties";
            _properties.PropertySort = PropertySort.Alphabetical;
            _properties.Size = new Size(221, 426);
            _properties.TabIndex = 11;
            // 
            // _processorComboBox
            // 
            _processorComboBox.Dock = DockStyle.Top;
            _processorComboBox.FormattingEnabled = true;
            _processorComboBox.Location = new Point(6, 6);
            _processorComboBox.Name = "_processorComboBox";
            _processorComboBox.Size = new Size(221, 23);
            _processorComboBox.TabIndex = 10;
            _processorComboBox.SelectedIndexChanged += _processorComboBox_SelectedIndexChanged;
            // 
            // _calcGrowthButton
            // 
            _calcGrowthButton.Location = new Point(4, 3);
            _calcGrowthButton.Name = "_calcGrowthButton";
            _calcGrowthButton.Size = new Size(108, 23);
            _calcGrowthButton.TabIndex = 11;
            _calcGrowthButton.Text = "Calc Growth Stats";
            _calcGrowthButton.UseVisualStyleBackColor = true;
            _calcGrowthButton.Click += _calcGrowthButton_Click;
            // 
            // _orderLimitLabel
            // 
            _orderLimitLabel.AutoSize = true;
            _orderLimitLabel.Location = new Point(118, 7);
            _orderLimitLabel.Name = "_orderLimitLabel";
            _orderLimitLabel.Size = new Size(67, 15);
            _orderLimitLabel.TabIndex = 12;
            _orderLimitLabel.Text = "Order limit:";
            // 
            // _orderLimitUpDown
            // 
            _orderLimitUpDown.InnerMargin = new Padding(0, 2, 0, 0);
            _orderLimitUpDown.Location = new Point(191, 3);
            _orderLimitUpDown.Name = "_orderLimitUpDown";
            _orderLimitUpDown.Size = new Size(120, 23);
            _orderLimitUpDown.TabIndex = 13;
            _orderLimitUpDown.Value = new decimal(new int[] { 6, 0, 0, 0 });
            // 
            // _outputTextBox
            // 
            _outputTextBox.Dock = DockStyle.Fill;
            _outputTextBox.Font = new Font("Courier New", 9F, FontStyle.Regular, GraphicsUnit.Point, 204);
            _outputTextBox.Location = new Point(4, 1);
            _outputTextBox.Multiline = true;
            _outputTextBox.Name = "_outputTextBox";
            _outputTextBox.ScrollBars = ScrollBars.Both;
            _outputTextBox.Size = new Size(521, 396);
            _outputTextBox.TabIndex = 14;
            // 
            // _estimateLabel
            // 
            _estimateLabel.AutoSize = true;
            _estimateLabel.Location = new Point(317, 7);
            _estimateLabel.Name = "_estimateLabel";
            _estimateLabel.Size = new Size(55, 15);
            _estimateLabel.TabIndex = 15;
            _estimateLabel.Text = "Estimate:";
            // 
            // _estimateFormula
            // 
            _estimateFormula.BorderStyle = BorderStyle.None;
            _estimateFormula.Dock = DockStyle.Top;
            _estimateFormula.Location = new Point(2, 2);
            _estimateFormula.Name = "_estimateFormula";
            _estimateFormula.Size = new Size(142, 16);
            _estimateFormula.TabIndex = 16;
            _estimateFormula.Text = "0";
            _toolTip.SetToolTip(_estimateFormula, "Available variables: C, N, k (write 0 to disable)");
            // 
            // panel2
            // 
            panel2.BackColor = SystemColors.Window;
            panel2.BorderStyle = BorderStyle.Fixed3D;
            panel2.Controls.Add(_estimateFormula);
            panel2.Dock = DockStyle.Top;
            panel2.Location = new Point(3, 3);
            panel2.Name = "panel2";
            panel2.Padding = new Padding(2, 2, 0, 0);
            panel2.Size = new Size(148, 23);
            panel2.TabIndex = 17;
            // 
            // panel3
            // 
            panel3.Controls.Add(panel5);
            panel3.Controls.Add(panel4);
            panel3.Dock = DockStyle.Top;
            panel3.Location = new Point(0, 0);
            panel3.Name = "panel3";
            panel3.Size = new Size(526, 29);
            panel3.TabIndex = 18;
            // 
            // panel5
            // 
            panel5.Controls.Add(panel2);
            panel5.Dock = DockStyle.Fill;
            panel5.Location = new Point(375, 0);
            panel5.Name = "panel5";
            panel5.Padding = new Padding(3, 3, 0, 3);
            panel5.Size = new Size(151, 29);
            panel5.TabIndex = 19;
            // 
            // panel4
            // 
            panel4.Controls.Add(_calcGrowthButton);
            panel4.Controls.Add(_orderLimitUpDown);
            panel4.Controls.Add(_estimateLabel);
            panel4.Controls.Add(_orderLimitLabel);
            panel4.Controls.Add(_progressBar);
            panel4.Dock = DockStyle.Left;
            panel4.Location = new Point(0, 0);
            panel4.Name = "panel4";
            panel4.Size = new Size(375, 29);
            panel4.TabIndex = 18;
            // 
            // _progressBar
            // 
            _progressBar.CustomText = "15";
            _progressBar.DisplayStyle = ProgressBarDisplayText.CustomText;
            _progressBar.Location = new Point(4, 3);
            _progressBar.Name = "_progressBar";
            _progressBar.Size = new Size(108, 23);
            _progressBar.TabIndex = 16;
            // 
            // panel6
            // 
            panel6.Controls.Add(_outputTextBox);
            panel6.Controls.Add(panel7);
            panel6.Dock = DockStyle.Fill;
            panel6.Location = new Point(0, 29);
            panel6.Name = "panel6";
            panel6.Padding = new Padding(4, 1, 1, 6);
            panel6.Size = new Size(526, 432);
            panel6.TabIndex = 19;
            // 
            // panel7
            // 
            panel7.Controls.Add(_elementPairStats);
            panel7.Controls.Add(button1);
            panel7.Controls.Add(_testButton);
            panel7.Dock = DockStyle.Bottom;
            panel7.Location = new Point(4, 397);
            panel7.Name = "panel7";
            panel7.Size = new Size(521, 29);
            panel7.TabIndex = 15;
            // 
            // _elementPairStats
            // 
            _elementPairStats.Location = new Point(362, 3);
            _elementPairStats.Name = "_elementPairStats";
            _elementPairStats.Size = new Size(140, 23);
            _elementPairStats.TabIndex = 2;
            _elementPairStats.Text = "Element Pair Stats";
            _elementPairStats.UseVisualStyleBackColor = true;
            _elementPairStats.Click += _elementPairStats_Click;
            // 
            // button1
            // 
            button1.Location = new Point(155, 3);
            button1.Name = "button1";
            button1.Size = new Size(201, 23);
            button1.TabIndex = 1;
            button1.Text = "Chart of 1000 walks of 1000 moves";
            button1.UseVisualStyleBackColor = true;
            button1.Click += _getWalkChart_Click;
            // 
            // _testButton
            // 
            _testButton.Location = new Point(3, 3);
            _testButton.Name = "_testButton";
            _testButton.Size = new Size(146, 23);
            _testButton.TabIndex = 0;
            _testButton.Text = "Test Selected Processor";
            _testButton.UseVisualStyleBackColor = true;
            _testButton.Click += _testButton_Click;
            // 
            // MainWindow
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(759, 461);
            Controls.Add(panel6);
            Controls.Add(panel3);
            Controls.Add(panel1);
            DoubleBuffered = true;
            MinimumSize = new Size(775, 500);
            Name = "MainWindow";
            Text = "Matrices over Finite Rings";
            panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)_orderLimitUpDown).EndInit();
            panel2.ResumeLayout(false);
            panel2.PerformLayout();
            panel3.ResumeLayout(false);
            panel5.ResumeLayout(false);
            panel4.ResumeLayout(false);
            panel4.PerformLayout();
            panel6.ResumeLayout(false);
            panel6.PerformLayout();
            panel7.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion
        private Panel panel1;
        private ComboBox _processorComboBox;
        private PropertyGridEx _properties;
        private Button _calcGrowthButton;
        private Label _orderLimitLabel;
        private NumericUpDownEx _orderLimitUpDown;
        private TextBox _outputTextBox;
        private Label _estimateLabel;
        private TextBox _estimateFormula;
        private Panel panel2;
        private ToolTip _toolTip;
        private Panel panel3;
        private Panel panel5;
        private Panel panel4;
        private Panel panel6;
        private CustomTextProgressBar _progressBar;
        private Panel panel7;
        private Button _testButton;
        private Button button1;
        private Button _elementPairStats;
    }
}
