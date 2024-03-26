using Microsoft.CodeAnalysis.CSharp.Scripting;
using OxyPlot;
using OxyPlot.Series;
using OxyPlot.WindowsForms;
using System.Text;
using ULCache = FiniteGroupCalc.PersistableCachers.PersistableUlongListCacher;
using FiniteGroupCalc.Widgets;
using FiniteGroupCalc.UlongProcessors;


namespace FiniteGroupCalc
{
    public partial class MainWindow : Form
    {
        private readonly DistanceStatistics _distanceStatsHelper;

        public List<UlongProcessorBase> Processors { get; } = new List<UlongProcessorBase>()
        {
            new TriangularZ2MatrixProcessor(),
            new TriangularZnMatrixProcessor(),
            new HeisenbergZ2MatrixProcessor(),
            new HeisenbergZnMatrixProcessor(),
            new PermutationProcessor(),
        };

        public MainWindow()
        {
            InitializeComponent();
            AllowTransparency = true;            
            _distanceStatsHelper = new DistanceStatistics(this);
            PropertyGridEx.SetLabelColumnWidth(_properties, 190);
            _processorComboBox.DataSource = Processors;
            _processorComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            _processorComboBox.SelectedIndex = 4;
            if (_processorComboBox.SelectedItem is not UlongProcessorBase processor) return;
            processor.Order = 3;
            if (processor is TriangularZnMatrixProcessor up) up.BitsPerElement = 3;
        }
        private void _processorComboBox_SelectedIndexChanged(object sender, EventArgs e) => _properties.SelectedObject = _processorComboBox.SelectedItem;

        async Task<Func<ulong[,], ulong, ulong, ulong>> CompileEstimate(string functionString, string cName = "C", string orderName = "N", string iName = "k")
        {
            var code = $"({cName}, {orderName}, {iName}) => {functionString}";
            Func<ulong[,], ulong, ulong, ulong> body = await CSharpScript.EvaluateAsync<Func<ulong[,], ulong, ulong, ulong>>(code);
            return body;
        }

        async Task<sbyte[]> GetDistanceTableAsync(UlongProcessorBase processor, ulong initial)
        {
            sbyte[] distances = [];
            await Task.Run(() => distances = _distanceStatsHelper.GetDistanceTable(processor, initial, (a, b) =>
            {
                _progressBar.Minimum = 0;
                _progressBar.Maximum = b;
                _progressBar.Value = Math.Clamp(a, 0, b);
                _progressBar.CustomText = $"Order {processor.Order}: {(double)a * 100 / b:0.0}%";
            }));
            return distances;
        }

        private async void _calcGrowthButton_Click(object sender, EventArgs e)
        {
            StringBuilder output = new();
            _calcGrowthButton.Visible = false;
            var processor = _processorComboBox.SelectedItem as UlongProcessorBase;
            if (processor == null) return;
            _progressBar.CustomText = "Order 1: 0%";
            var oldOrder = processor.Order;
            List<Action> basisSetupActions = new List<Action>() { () => { } };

            if (processor.GetType().GetProperty("BasisType") is { } prop)
            {
                if (prop.PropertyType.IsEnum)
                {
                    basisSetupActions.Clear();
                    foreach(var enumValue in Enum.GetValues(prop.PropertyType))
                    {
                        basisSetupActions.Add(() => prop.SetValue(processor, enumValue));
                    }
                }
            }
            foreach (var action in basisSetupActions)
            {
                action();
                for (int order = 1; order <= _orderLimitUpDown.Value; order++)
                {
                    processor.Order = order;
                    output.AppendLine(processor.DisplayName);
                    output.AppendLine(processor.StandardBasisDescription);

                    var key = $"{processor.CacheId}_growth_{order}";
                    int[] histogram;
                    if (ULCache.Contains(key))
                    {
                        histogram = ULCache.GetInts(key);
                    }
                    else
                    {
                        var distances = await GetDistanceTableAsync(processor, processor.Identity);
                        histogram = _distanceStatsHelper.GetDistanceHistogram(distances);
                        ULCache.SetInts(key, histogram);
                    }
                

                    var len = histogram.Length;
                    var indices = Enumerable.Range(0, len).ToArray();
                    var body = await CompileEstimate(_estimateFormula.Text);
                    var c_row = indices.Select(i => (int)(body(BinomialCoefficients.C, (ulong)order, (ulong)i))).ToArray();
                    var deltas = indices.Select(i => c_row[i] - histogram[i]).ToArray();
                    if (!c_row.All(x => x == 0))
                    {
                        output.PrintMultipleRows(["#", "hist", "Δ", $"estim."], indices, histogram, deltas, c_row);
                    }
                    else
                    {
                        output.PrintMultipleRows(["#", "hist"], indices, histogram);
                    }
                    Application.DoEvents();
                    output.AppendLine();
                }
            }
            _calcGrowthButton.Visible = true;
            processor.Order = oldOrder;
            _outputTextBox.Text = output.ToString();
        }

        private void _testButton_Click(object sender, EventArgs e)
        {
            var processor = _processorComboBox.SelectedItem as UlongProcessorBase;
            if (processor == null) return;
            _outputTextBox.Text = processor.TestProduct();
        }

        private async void _getWalkChart_Click(object sender, EventArgs e)
        {
            var processor = _processorComboBox.SelectedItem as UlongProcessorBase;
            if (processor == null) return;
            var distances = await GetDistanceTableAsync(processor, processor.Identity);
            var maxDist = distances.Max();
            var basis = processor.GetStandardBasis();
            ulong[] walks = new ulong[1000];
            var random = new Random();

            int seriesCount = 100000;
            var plotModel = new PlotModel { Title = $"{seriesCount} Random Walks (mean): " + processor.DisplayName };
            OxyColor GetRandomColor() => OxyColor.FromRgb((byte)random.Next(255), (byte)random.Next(255), (byte)random.Next(255));


            var newSeries = new LineSeries
            {
                Title = "Distance",
                Color = OxyColor.FromRgb(255, 0, 0),
                MarkerType = MarkerType.Circle,
                MarkerSize = 0,
                StrokeThickness = 1,
                MarkerStrokeThickness = 0,
            };
            newSeries.Points.Add(new DataPoint(0, 0));
            ulong[] states = new ulong[seriesCount];
            int maxStep = 166;

            for (int step = 1; step < maxStep; step++)
            {
                long totalDist = 0;
                double averageDistance = 0;
                Parallel.For(0, seriesCount, walk =>
                {
                    states[walk] = processor.Product(states[walk], basis[random.Next(basis.Length)]);
                    Interlocked.Add(ref totalDist, distances[states[walk]]);
                });
                averageDistance = (double)totalDist / seriesCount;
                newSeries.Points.Add(new DataPoint(step, averageDistance));
            }
            // newSeries.SeriesGroupName = "Measured Mean";
            plotModel.Series.Add(newSeries);

            var estimatedCeiling = Enumerable.Reverse(newSeries.Points).Take(10).Average(x => x.Y);
            //Math.Round(newSeries.Points.Last().Y * 2) / 2;

            double GetExpConstByLeastSquaresBisect(double from, double to, double tolerance = 0.000000001)
            {
                double Func(double x, double c) => estimatedCeiling * (1 - Math.Exp(-x * c));
                double ErrorSumFromSeries(double c)
                {
                    int controlPoint = 8;
                    return newSeries.Points[controlPoint].Y - Func(controlPoint, c);
                }
                double mid = (from + to) / 2;
                while (to - from > tolerance)
                {
                    if (ErrorSumFromSeries(mid) < 0)
                    {
                        to = mid;
                    }
                    else
                    {
                        from = mid;
                    }
                    mid = (from + to) / 2;
                }
                return mid;
            }
            double c = GetExpConstByLeastSquaresBisect(0.1, 20);

            double expMinusC = Math.Exp(c);

            // D[a^x] = a^x * ln(a) * D[x]
            plotModel.Series.Add(new FunctionSeries(x =>
                estimatedCeiling * (1 - Math.Exp(-x * c)), 0, maxStep - 1, maxStep - 1));

            var viewer = new ChartForm
            {
                X0 = 0,
                X1 = maxStep - 1,
                PointCount = maxStep,
                ExpFactor = c,
                Asymptote = estimatedCeiling,
                Text = $"Found Exp = {expMinusC:0.0000000000000} (diameter = {maxDist})"
            };
            viewer._dataPointsTextBox.Text = newSeries.Points.Select(x => $"{x.Y:0.0}").Aggregate((a, b) => a + " " + b);
            viewer.PlotView.Model = plotModel;
            viewer.ShowDialog();
        }

        private async void _elementPairStats_Click(object sender, EventArgs e)
        {
            var processor = _processorComboBox.SelectedItem as UlongProcessorBase;
            if (processor == null) return;
            List<sbyte> diameters = new();
            for(ulong a = 1; a < processor.UlongCount; a++)
            {
                for (ulong b = a + 1; b < processor.UlongCount; b++)
                {
                    var dists = _distanceStatsHelper.GetPartialDistanceTableForBasis(processor, processor.Identity, 
                        [processor.GetIth((int)a), processor.GetIth((int)b)],
                        (i, n) => { });
                    diameters.Add(dists.Max());
                }
            } 
            var histogram = _distanceStatsHelper.GetHistogram(diameters.ToArray());
            var model = new PlotModel
            {
                Title = "Diameter Distribution",
                Series =
                {
                    new BarSeries
                    {
                        ItemsSource = histogram,
                        ValueField = "Value",
                    }, 
                }
            };
            var plainChartForm = new Form
            {
                Controls =
                {
                    new PlotView
                    {
                        Dock = DockStyle.Fill,
                        Model =model
                    }
                }
            };
            plainChartForm.Show();
        }
    }
}
