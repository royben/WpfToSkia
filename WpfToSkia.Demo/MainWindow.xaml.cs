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

        int counter = 1;

        public MainWindow()
        {
            InitializeComponent();

            ContentRendered += MainWindow_ContentRendered;
        }

        private void MainWindow_ContentRendered(object sender, EventArgs e)
        {
            var items = new ObservableCollection<DataItem>();

            for (int i = 0; i < 10000; i++)
            {
                items.Add(new DataItem()
                {
                    Name = "Roy Ben Shabat " + counter++,
                    Y = i * 70,
                    X = 0,
                });
            }

            Items = items;

            //StackPanel panel = new StackPanel() { HorizontalAlignment = HorizontalAlignment.Left, VerticalAlignment = VerticalAlignment.Top };

            //for (int i = 0; i < 10000; i++)
            //{
            //    ItemControl control = new ItemControl();
            //    panel.Children.Add(control);
            //}

            //border.Child = panel;

            VirtualizingStackPanel s = new VirtualizingStackPanel();
            s.
        }

        private void Border1_MouseEnter(object sender, MouseEventArgs e)
        {
            //border1.Opacity = 0;
            //Debug.WriteLine("Border 1 Mouse Enter");
        }

        private void Border1_MouseLeave(object sender, MouseEventArgs e)
        {
            //border1.Opacity = 1;
            //Debug.WriteLine("Border 1 Mouse Leave");
        }

        private void Border2_MouseEnter(object sender, MouseEventArgs e)
        {
            //border2.Opacity = 0;
            //Debug.WriteLine("Border 2 Mouse Enter");
        }

        private void Border2_MouseLeave(object sender, MouseEventArgs e)
        {
            //border2.Opacity = 1;
            //Debug.WriteLine("Border 2 Mouse Leave");
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Items.Add(new DataItem()
            {
                Name = "Roy Ben Shabat " + counter++,
            });
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Items.Remove(Items.Last());
        }
    }
}
