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
using KiTPO.Extensions;
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
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
            InitializeComponent();
            OutputListView.ItemsSource = OutputList;
            SetInputValues(0,0);
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
            PushToOutput("Ввод точки через график");
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
                
                var z = x + y;
                PushToOutput("Точка вводится через клавиатуру");
                ProcessPoint(x.Value, y.Value);
                
            }
            
            SetInputValues(0,0);
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
        
        private void SetInputValues(double x, double y)
        {
            XInput.Text = x.ToString(CultureInfo.InvariantCulture);
            YInput.Text = y.ToString(CultureInfo.InvariantCulture);
        }

        private void ButtonBase_OnClick2(object sender, RoutedEventArgs e)
            => SetInputValues(NumberExtensions.RandomInRange(NumberExtensions.Min, NumberExtensions.Max),
                NumberExtensions.RandomInRange(NumberExtensions.Min, NumberExtensions.Max));
    }
}