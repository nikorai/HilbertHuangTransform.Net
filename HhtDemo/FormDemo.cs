using HilbertHuangTransform;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace HhtDemo
{
    public partial class FormDemo : Form
    {
        public FormDemo()
        {
            InitializeComponent();
            WindowState = FormWindowState.Maximized;
        }

        private void FormDemo_Load(object sender, EventArgs e)
        {
            var sourceSignal = GenerateExampleSignal();

            var imfList = HHT.SplitToIMF(sourceSignal);

            // draw signals
            chart.ChartAreas.Add(new ChartArea("HHT Demo"));
            chart.Legends.Add("HHT Demo");

            Series seriesSource = GetChartSeriesFromPoints(sourceSignal, $"source");
            seriesSource.Color = System.Drawing.Color.Black;
            chart.Series.Add(seriesSource);

            for (int i = 0; i < imfList.Count; i++)
            {
                Series series = GetChartSeriesFromPoints(imfList[i], $"imf {i}");
                chart.Series.Add(series);
            }
        }

        private List<PointD> GenerateExampleSignal()
        {
            var points = new List<PointD>();

            int N = 1000;
            double xMax = 10 * Math.PI;

            for (int n = 0; n < N; n++)
            {
                var xValue = n / (N - 1.0) * xMax;
                var ySource1 = 1.0 * Math.Sin(0.2 * xValue);
                var ySource2 = 0.5 * Math.Sin(2.0 * xValue);
                var ySource3 = 0.1 * Math.Sin(20.0 * xValue);

                points.Add(new PointD
                {
                    X = xValue,
                    Y = ySource1 + ySource2 + ySource3
                });
            }

            return points;
        }

        private Series GetChartSeriesFromPoints(List<PointD> points, string seriesName)
        {
            var series = new Series(seriesName);
            series.ChartType = SeriesChartType.Line;

            foreach (var point in points)
                series.Points.AddXY(
                    point.X,
                    point.Y);

            return series;
        }
    }
}
