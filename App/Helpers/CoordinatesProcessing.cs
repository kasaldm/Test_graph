using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Documents;
using KiTPO.Enums;

namespace KiTPO.Helpers
{
    public static class CoordinatesProcessing
    {
        public static Dictionary<int, List<List<(double, double)>>> Areas = new()
        {
            {
                1, new()
                {
                    new() {(0, 0), (1, 0), (0, 4)},
                    new() {(1, 0), (0, 4), (4, 6)}
                }
            },
            {
                2, new()
                {
                    new() {(1, 0), (4, 6), (10, 0)},
                    new() {(4, 6), (7, 4), (10, 0)}
                }
            },
            {
                3, new()
                {
                    new() {(6, 7), (10, 7), (7, 4)},
                    new() {(7, 4), (10, 7), (10, 0)}
                }
            },
            {
                4, new()
                {
                    new() {(0, 4), (0, 7), (6, 7)},
                    new() {(4, 6), (6, 7), (7, 4)}
                }
            }
        };

        public const double Accuracy = 0.02;
        public const double TriangleAccuracy = 0.05;

        public static (double, double) FlattenCoordinates(double x, double y, double offsetX, double offsetY,
            double scale) =>
            ((x - offsetX) / scale, ((y - offsetY) / scale) * -1);

        public static (double, double) UnFlattenCoordinates(double x, double y, double offsetX, double offsetY,
            double scale) => (
            (x * scale) + offsetX, ((-y * scale) + offsetY));


        public static (PointPosition, double, double, int?) GetPointPosition(double x, double y, double offsetX,
            double offsetY, double scale)
        {
            (x, y) = FlattenCoordinates(x, y, offsetX, offsetY, scale);
            if (!(x >= 0 - Accuracy && x <= 10 + Accuracy) || !(y >= 0 - Accuracy && y <= 7 + Accuracy))
                return (PointPosition.Outside, x, y, null);

            foreach (var area in Areas)
            foreach (var tr in area.Value)
            {
                var result = GetPointPositionInTriangle((x, y), tr);
                if (result != PointPosition.Outside) return (result, x, y, area.Key);
            }

            return (PointPosition.Outside, x, y, null);
        }

        public static PointPosition GetPointPositionInTriangle((double, double) rootPoint,
            List<(double, double)> trPoints)
        {
            if (trPoints.Count != 3) return PointPosition.Outside;
            var rootSquare = CalcSquare(trPoints[0], trPoints[1], trPoints[2]);
            var trSquares = new List<List<(double, double)>>
            {
                new() {rootPoint, trPoints[0], trPoints[1]},
                new() {rootPoint, trPoints[1], trPoints[2]},
                new() {rootPoint, trPoints[2], trPoints[0]},
            }.Select(x => CalcSquare(x[0], x[1], x[2])).ToList();
            var trSquaresSum = trSquares.Aggregate(0.0, (acc, x) => acc + x);
            if (Math.Abs(trSquaresSum - rootSquare) > TriangleAccuracy * 3) return PointPosition.Outside;

            return trSquares.Any(x => x <= TriangleAccuracy) ? PointPosition.OnTheBorder : PointPosition.Inside;
        }

        public static double CalcSquare((double, double) x1, (double, double) x2, (double, double) x3)
            => Math.Abs(0.5 * (((x1.Item1 - x3.Item1) * (x2.Item2 - x3.Item2)) -
                               ((x2.Item1 - x3.Item1) * (x1.Item2 - x3.Item2))));

        public static string GenerateMessage(PointPosition p, double x, double y, int? num)
            => "Точка с координатами x: " + x.ToString("0.##") + ", y: " + y.ToString("0.##") + " находится " + p switch
            {
                PointPosition.Inside => "внутри зоны #" + num,
                PointPosition.OnTheBorder => " на границе зоны #" + num,
                PointPosition.Outside => "за зоной",
                _ => ""
            };
    }
}