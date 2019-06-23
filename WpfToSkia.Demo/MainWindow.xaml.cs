using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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
using System.Windows.Threading;

namespace WpfToSkia.Demo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ObservableCollection<DataItem> Items
        {
            get { return (ObservableCollection<DataItem>)GetValue(ItemsProperty); }
            set { SetValue(ItemsProperty, value); }
        }
        public static readonly DependencyProperty ItemsProperty =
            DependencyProperty.Register("Items", typeof(ObservableCollection<DataItem>), typeof(MainWindow), new PropertyMetadata(null));

        public PointCollection Points
        {
            get { return (PointCollection)GetValue(PointsProperty); }
            set { SetValue(PointsProperty, value); }
        }
        public static readonly DependencyProperty PointsProperty =
            DependencyProperty.Register("Points", typeof(PointCollection), typeof(MainWindow), new PropertyMetadata(null));

        private int counter = 1;
        private DispatcherTimer _timer;
        private Random _rnd;

        public MainWindow()
        {
            _rnd = new Random();

            InitializeComponent();

            ContentRendered += MainWindow_ContentRendered;
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromMilliseconds(50);
            _timer.Tick += _timer_Tick;
        }

        private void MainWindow_ContentRendered(object sender, EventArgs e)
        {
            var items = new ObservableCollection<DataItem>();

            for (int i = 0; i < 1000; i++)
            {
                items.Add(new DataItem()
                {
                    Name = "Data Item " + counter++,
                });
            }

            Items = items;

            _timer.Start();
        }

        private void _timer_Tick(object sender, EventArgs e)
        {
            if (tabControl.SelectedItem == polygonTabItem)
            {
                Points = CreatePolygonPoints();
            }
        }

        private PointCollection CreatePolygonPoints()
        {
            List<Point> points = new List<Point>();

            double pointsCount = _rnd.Next(100, 1000);

            for (double i = 0; i < pointsCount; i++)
            {
                double x = (i / pointsCount) * graphHost.ActualWidth;
                double y = _rnd.Next(0, (int)graphHost.ActualHeight);

                points.Add(new Point(x, y));
            }

            points.Add(new Point(graphHost.ActualWidth, 0));
            points.Add(new Point(0, 0));

            return new PointCollection(points);
        }
    }
}
