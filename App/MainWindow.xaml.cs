using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using KiTPO.Helpers;
using Microsoft.Win32;

namespace KiTPO
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public double RootX = 43;
        public double RootY = 228;
        public double ScaleValue = 31;
        public string FilePath = "out" + DateTime.UtcNow.Millisecond + ".txt";


        public ObservableCollection<string> OutputList { get; set; } = new();

        public MainWindow()
        {
            InitializeComponent();
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
            OutputListView.ItemsSource = OutputList;
        }


        private async void PushToOutput(string v)
        {
            OutputList.Add(v);
            OutputListView.SelectedIndex = OutputList.Count - 1;
            OutputListView.ScrollIntoView(OutputListView.SelectedItem);

            await using StreamWriter file = new(FilePath, append: true);
            await file.WriteLineAsync(v);
        }

        private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var pos = e.GetPosition(MainImage);
            ProcessPoint(pos.X, pos.Y);
        }

        private void ProcessPoint(double x, double y, bool append = false)
        {
            if (!append) ResetCanvas();

            var newEllipse = new Ellipse
            {
                Fill = new SolidColorBrush(Colors.Red),
                Width = 5,
                Height = 5
            };

            Canvas.Children.Add(newEllipse);
            Canvas.SetLeft(newEllipse, x);
            Canvas.SetTop(newEllipse, y);
            var (position, xFlatten, yFlatten, areaNum) =
                CoordinatesProcessing.GetPointPosition(x, y, RootX, RootY, ScaleValue);
            PushToOutput(CoordinatesProcessing.GenerateMessage(position, xFlatten, yFlatten, areaNum));
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            var x = XInput.Value;
            var y = YInput.Value;
            if (x != null && y != null)
            {
                (x, y) = CoordinatesProcessing.UnFlattenCoordinates(x.Value, y.Value, RootX, RootY, ScaleValue);
                ProcessPoint(x.Value, y.Value);
                XInput.Value = null;
                YInput.Value = null;
            }
            else PushToOutput("Ошибка ввода данных");
        }

        private void ResetCanvas()
        {
            var toRemove = new List<UIElement>();
            foreach (UIElement el in Canvas.Children)
                if (el is Ellipse)
                    toRemove.Add(el);
            foreach (var el in toRemove)
                Canvas.Children.Remove(el);
        }

        private async void MenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog {DefaultExt = ".txt", Filter = "Text Files (*.txt)|*.txt"};
            var result = dlg.ShowDialog();

            if (result == true)
            {
                ResetCanvas();
                var filename = dlg.FileName;
                var (err, list) = await FileProcessing.GetPointsFromFile(filename);
                PushToOutput(FileProcessing.GenerateMessage((err, list)));
                if (list != null)
                {
                    foreach (var point in list)
                    {
                        var (x, y) =
                            CoordinatesProcessing.UnFlattenCoordinates(point.Item1, point.Item2, RootX,
                                RootY, ScaleValue);
                        ProcessPoint(x, y, true);
                    }
                }
            }
        }
    }
}