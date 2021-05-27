using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using KiTPO.Enums;

namespace KiTPO.Helpers
{
    public static class FileProcessing
    {
        public static async Task<(FileReadError?, List<(double, double)>)> GetPointsFromFile(string path)
        {
            var content = (await File.ReadAllLinesAsync(path)).ToList();
            content = content.Where(x => x.Replace(" ", "") != "").ToList();

            if (content.Count == 0) return (FileReadError.FileIsEmpty, null);

            var points = new List<(double, double)>();
            foreach (var values in content.Select(line => line.Replace(" ", "").Split(";")))
            {
                if (values.Length != 2) return (FileReadError.WrongFormat, null);
                double? v1 = double.TryParse(values[0], out var x) ? x : null;
                double? v2 = double.TryParse(values[1], out var y) ? y : null;
                
                if (v1 == null || v2 == null) return (FileReadError.WrongFormat, null);
                points.Add((v1.Value, v2.Value));
            }

            return (null, points);
        }
        
        public static string GenerateMessage((FileReadError?, List<(double, double)>) x)
            => x.Item1 switch
            {
                null => "Файл открыт, считано " + x.Item2.Count + " точек",
                FileReadError.WrongFormat => "Файл открыт, неправильный формат",
                FileReadError.FileIsEmpty => "Файл был открыт, но он пуст",
                FileReadError.OutOfRange => "Файл открыт, но некоторые значения оказались за пределами числового диапазона"
            };
    }
}