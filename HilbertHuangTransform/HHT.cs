using System.Collections.Generic;
using System.Linq;

namespace HilbertHuangTransform
{
    public static class HHT
    {
        public static List<List<PointD>> SplitToIMF(double[] xValues, double[] yValues, int? maxIMF = null)
        {
            return SplitToIMF(GetPoints(xValues, yValues), maxIMF);
        }

        public static List<List<PointD>> SplitToIMF(List<PointD> points, int? maxIMF = null)
        {
            var items = new List<List<PointD>>();

            while (maxIMF == null ? true : maxIMF-- > 0)
            {
                var middle = GetMiddle(points);

                if (!middle.Any())
                    break;

                var imf = Substract(points, middle);
                items.Add(imf);

                points = middle;
            }

            return items;
        }

        private static List<PointD> GetMiddle(List<PointD> points)
        {
            var size = points.Count;
            var middle = new List<PointD>(size);

            var maxPoints = GetLocalMaximums(points);
            var minPoints = GetLocalMinimums(points);

            if (maxPoints.Count > 1 && minPoints.Count > 1)
            {
                alglib.spline1dinterpolant splineMax = new alglib.spline1dinterpolant();
                alglib.spline1dinterpolant splineMin = new alglib.spline1dinterpolant();

                alglib.spline1dbuildcubic(
                    maxPoints.Select(p => p.X).ToArray(),
                    maxPoints.Select(p => p.Y).ToArray(),
                    out splineMax);

                alglib.spline1dbuildcubic(
                    minPoints.Select(p => p.X).ToArray(),
                    minPoints.Select(p => p.Y).ToArray(),
                    out splineMin);

                for (int i = 0; i < size; i++)
                {
                    var xValue = points[i].X;
                    var yMaxValue = alglib.spline1dcalc(splineMax, xValue);
                    var yMinValue = alglib.spline1dcalc(splineMin, xValue);

                    middle.Add(new PointD
                    {
                        X = xValue,
                        Y = (yMaxValue + yMinValue) / 2.0
                    });
                }
            }

            return middle;
        }

        private static List<PointD> Substract(List<PointD> points1, List<PointD> points2)
        {
            var size = points1.Count;
            var items = new List<PointD>();

            for (int i = 0; i < size; i++)
                items.Add(new PointD
                {
                    X = points1[i].X,
                    Y = points1[i].Y - points2[i].Y
                });

            return items;
        }

        private static List<PointD> GetPoints(double[] xValues, double[] yValues)
        {
            var count = xValues.Length;
            var points = new List<PointD>(count);

            for (int i = 0; i < count; i++)
                points.Add(new PointD
                {
                    X = xValues[i],
                    Y = yValues[i]
                });

            return points;
        }

        private static List<PointD> GetLocalMaximums(List<PointD> points)
        {
            var items = new List<PointD>(points.Capacity / 2);

            for (int i = 1; i < points.Count - 1; i++)
                if (points[i - 1].Y < points[i].Y && points[i].Y >= points[i + 1].Y)
                    items.Add(points[i]);

            return items;
        }

        private static List<PointD> GetLocalMinimums(List<PointD> points)
        {
            var items = new List<PointD>(points.Capacity / 2);

            for (int i = 1; i < points.Count - 1; i++)
                if (points[i - 1].Y > points[i].Y && points[i].Y <= points[i + 1].Y)
                    items.Add(points[i]);

            return items;
        }
    }
}
