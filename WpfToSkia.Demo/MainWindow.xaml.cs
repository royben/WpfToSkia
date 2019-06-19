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

            for (int i = 0; i < 1000; i++)
            {
                items.Add(new DataItem()
                {
                    Name = "Data Item " + counter++,
                });
            }

            Items = items;
        }
    }
}
