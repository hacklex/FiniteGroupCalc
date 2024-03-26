using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FiniteGroupCalc
{
    public partial class ChartForm : Form
    {
        public double ExpFactor { get; set; } = 1;
        public double X0 { get; set; } = 0;
        public double X1 { get; set; } = 10;
        public int PointCount { get; set; } = 100;
        public double Asymptote { get; set; } = 1;


        public double CosScale { get; set; } = 1;
        public double CosPhase { get; set; } = 0;
        public double CosFactor { get; set; } = 1;
        public double CosDamp { get; set; } = 1;



        public ChartForm()
        {
            InitializeComponent();
            _expFactorSlider.Value = (int)(ExpFactor * 1000);
        }

        private void ChartForm_Load(object sender, EventArgs e)
        { 
        }

        private void PlotView_Click(object sender, EventArgs e)
        {

        }

        private void _expFactorSlider_Scroll(object sender, EventArgs e)
        {
            RebuildFunc();
        }

        private void RebuildFunc()
        {
            ExpFactor = _expFactorSlider.Value * 0.001;

            //scale, damp, factor, phase
            CosScale = _cosFactorE.Value * 0.001;
            CosDamp = _cosFactorA.Value * 0.0001;
            CosFactor = _cosFactorB.Value * 0.001;
            CosPhase = _cosFactorC.Value * 0.001;
            if (PlotView.Model.Series.OfType<LineSeries>().FirstOrDefault() is not { } data)
                return;

            if (PlotView.Model.Series.OfType<FunctionSeries>().FirstOrDefault() is { } fs)
            {
                PlotView.Model.Series.Remove(fs);
                PlotView.Model.Series.Add(new FunctionSeries(x =>
                data.Points[(int)x].Y - Asymptote * (1 - Math.Exp(-x * ExpFactor))
                
                //CosScale * Math.Exp(-x * CosDamp) * -Math.Cos(CosFactor * x + CosPhase)
                
                , X0, X1, PointCount));
                PlotView.InvalidatePlot(true);
                _cosFormulaLabel.Text = 
                    $"y = {CosScale:0.####} * exp(-{CosDamp:0.####} * x) * cos({CosFactor:0.####} * x + {CosPhase:0.####})";
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void _cosPhaseTrackBar_Scroll(object sender, EventArgs e)
        {
            RebuildFunc();
        }
    }
}
