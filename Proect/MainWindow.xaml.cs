using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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

namespace Proect
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            // Output2.ItemsSource = Logic;
        }

        protected ObservableCollection<int> Logic { get; set; }= new()
        {
            301
        };

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)

        {
            Logic.Add(19);
        }


        private void UIElement_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            var elips = new Ellipse
            {
                Height = 5,
                Width = 5,
                Fill    = new SolidColorBrush(Colors.Green)
            };

            Canvas.Children.Add(elips);
            var x = e.GetPosition(Canvas).X;
            Canvas.SetLeft(elips, x);
            var y = e.GetPosition(Canvas).Y;
            Canvas.SetTop(elips, y);
            
        }
    }    
}