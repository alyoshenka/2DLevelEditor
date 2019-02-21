using System;
using System.Collections.Generic;
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

// to do
// save data before resizing

namespace LevelEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        enum tileType
        {
            player, enemy, wall, floor, pickup, goal
        };

        // initial values
        int rows;
        int cols;
        RowDefinition row;
        ColumnDefinition col;

        // buttons
        Button playerB;
        Button enemyB;
        Button wallB;
        Button floorB;
        Button pickupB;
        Button goalB;

        public MainWindow()
        {         
            InitializeComponent();
            
            rows = (int)slider.Value;
            cols = rows;

            InitLevel(rows, cols);
        }

        // initialize level feild
        void InitLevel(int r, int c)
        {
            if(null == tileGrid) { return; } // so no run on init

            // reset
            tileGrid.Children.Clear();
            tileGrid.RowDefinitions.Clear();
            tileGrid.ColumnDefinitions.Clear();

            row = new RowDefinition();
            row.Height = new GridLength(tileGrid.Height / r);
            col = new ColumnDefinition();
            col.Width = new GridLength(tileGrid.Width / c);

            // initialize rows and cols           
            for (int i = 0; i < r; i++) { tileGrid.RowDefinitions.Add(new RowDefinition() { Height = row.Height }); }
            for (int i = 0; i < c; i++) { tileGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = col.Width }); }
            tileGrid.ShowGridLines = true;

            // place buttons
            for (int y = 0; y < r; y++)
            {
                for (int x = 0; x < c; x++)
                {
                    Button b = new Button();
                    Grid.SetRow(b, y);
                    Grid.SetColumn(b, x);
                    tileGrid.Children.Add(b);
                }
            }            
        }

        // resizes level feild
        public void ResizeLevel(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            rows = (int)slider.Value;
            cols = rows;
           
            InitLevel(rows, cols);
        }

        // saves level data to file
        public void SaveData(object sender, RoutedEventArgs e)
        {

        }

        // loads data from file
        public void LoadData(object sender, RoutedEventArgs e)
        {

        }
    }
}
