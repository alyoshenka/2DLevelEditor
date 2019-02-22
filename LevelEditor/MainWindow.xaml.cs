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

using System.IO;

// to do
// save data before resizing
// dynamically *move* buttons instead   of reinitializing them
// don't save blak data
// way to get rows and cols from file
// save enums as ints
// make player singleton
// right click to deselect/ make null
// add instructions
// ButtonAt(Index) function

namespace LevelEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        enum TileType
        {
            player, enemy, wall, floor, pickup, goal
        };

        struct Index { public int y, x; }
   
        // initial values
        int rows;
        int cols;
        int minRows;
        int minCols;
        int maxRows;
        int maxCols;
        int stepVal;
        RowDefinition row;
        ColumnDefinition col;
        TileType?[,] data;
        // singletons
        Index? playerIdx;
        Index? goalIdx;

        Button selectedB;

        // buttons
        TileType? currentTile;

        public MainWindow()
        {
            // create levels folder if it doesnt't exist
            Directory.CreateDirectory("Levels");

            InitializeComponent();

            minRows = 10;
            minCols = minRows;
            maxRows = 30;
            maxCols = maxRows;
            rows = (int)slider.Value;
            cols = rows;
            stepVal = 5;
            slider.Minimum = minRows;
            slider.Maximum = maxRows;
            slider.TickFrequency = stepVal;
            data = new TileType?[maxRows, maxCols];
            selectedB = new Button();
            playerIdx = null;
            goalIdx = null;

            InitLevel(rows, cols);
        }

        // initialize level feild
        void InitLevel(int r, int c)
        {
            if(null == tileGrid) { return; } // so no run on init

            // EventHandler ev = new EventHandler(this.PlaceTile);

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

            // tileGrid.ShowGridLines = true;

            // place buttons
            for (int y = 0; y < r; y++)
            {
                for (int x = 0; x < c; x++)
                {
                    Button b = new Button();
                    b.Click += new RoutedEventHandler(PlaceTile);
                    b.MouseEnter += new MouseEventHandler(ClickAndDrag);
                    Grid.SetRow(b, y);
                    Grid.SetColumn(b, x);
                    // put data back in
                    if (data != null) { b.Content = data[x, y].ToString(); }
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

        // gets current level data
        List<List<string>> GetDataFromGrid()
        {
            List<List<string>> dat = new List<List<string>>();
            foreach(var r in tileGrid.RowDefinitions)
            {
                List<string> row = new List<string>();
                foreach(var c in tileGrid.ColumnDefinitions)
                {
                    
                }
                dat.Add(row);
            }

            return dat;
        }

        // saves level data to file
        public void SaveData(object sender, RoutedEventArgs e)
        {
            string path = saveText.Text;
            path = "Levels/" + path;
            if(path.Substring(path.Length - 5) != ".txt") { path += ".txt"; }

            try
            {
                using (StreamWriter sw = new StreamWriter(path))
                {
                    string row = "";
                    for (int y = 0; y < cols; y++)
                    {
                        for (int x = 0; x < rows; x++)
                        {
                            row += data[x, y].ToString() + "-";
                        }
                        if (row.Length > 0) { sw.WriteLine(row); }
                        row = "";
                    }
                }
            }
            catch { return; }
            
        }

        // loads data from file
        void LoadData(object sender, RoutedEventArgs e)
        {
            string path = loadText.Text;
            path = "Levels/" + path;
            if (path.Substring(path.Length - 5) != ".txt") { path += ".txt"; }

            try
            {
                using (StreamReader sr = new StreamReader(path))
                {
                    string line;
                    int r = 0;
                    int c = 0;
                    while ((line = sr.ReadLine()) != null)
                    {
                        string[] tiles = line.Split('-');
                        for (int i = 0; i < tiles.Length; i++)
                        {
                            TileType current;
                            if (Enum.TryParse(tiles[i], out current)) { data[c, r] = current; }
                            else { data[c, r] = null; }
                            c++;
                        }
                        if (c > cols + 1) { cols = c; } // TEST RESIZING
                        c = 0;
                        r++;
                    }
                    if (r > rows + 1) { rows = r; }
                }
                // round to nearest step value
                cols += cols % stepVal;
                rows += rows % stepVal;
                // set to same val, whichever is bigger
                cols = cols < rows ? rows : cols;
                rows = maxCols < rows ? rows : cols;

                InitLevel(rows, cols);
            }
            catch { return; }           
        }

        // clears data in grid
        void ClearData(object sender, RoutedEventArgs e)
        {
            for (int r = 0; r < rows; r++) { for (int c = 0; c < cols; c++) { data[c, r] = null; } }       
            InitLevel(rows, cols); // there is a more efficient way than recreating each time
        }

        // clears textbox
        public void TBFocus(object sender, RoutedEventArgs e)
        {
            TextBox t = (TextBox)sender;
            t.Text = "";
        }

        // selects new tile
        public void SelectTile(object sender, RoutedEventArgs e)
        {
            Button b = (Button)sender;
            selectedB = b;
            string s = b.Content.ToString();

            switch (s)
            {
                case "Player":
                    currentTile = TileType.player;
                    break;
                case "Enemy":
                    currentTile = TileType.enemy;
                    break;
                case "Pickup":
                    currentTile = TileType.pickup;
                    break;
                case "Wall":
                    currentTile = TileType.wall;
                    break;
                case "Floor":
                    currentTile = TileType.floor;
                    break;
                case "Goal":
                    currentTile = TileType.goal;
                    break;
                default:
                    currentTile = null;
                    break;
            }
        }

        // places tile
        public void PlaceTile(object sender, RoutedEventArgs e)
        {
            Button b = (Button)sender;
            b.Content = selectedB.Content; // set content
            b.Background = selectedB.BorderBrush; // set color           
            Index i = GetIndex(b);
            if ((string)selectedB.Content == "Player")
            {               
                if(playerIdx != null)
                {
                    Index p = (Index)playerIdx;
                    data[p.y, p.x] = null;
                }
                playerIdx = i;
                // InitLevel(rows, cols); // reload
            }
            data[i.y, i.x] = currentTile;
        }

        // gets index of button in grid
        Index GetIndex(Button b)
        {
            // terry help me do this better pls

            Index idx = new Index();
            int val;

            for(int r = 0; r < rows; r++)
            {
                for(int c = 0; c < cols; c++)
                {
                    val = r * cols + c; // gets index of child
                    if(tileGrid.Children[val] == b)
                    {
                        idx.x = r;
                        idx.y = c;
                        return idx;
                    }
                }
            }
            return idx;
        }

        // allows click and drag selection+
        void ClickAndDrag(object sender, MouseEventArgs e)
        {
            Button b = (Button)sender;
            if(b.IsMouseOver && Mouse.RightButton == MouseButtonState.Pressed)
            {
                saveText.Text = "working";
                PlaceTile(sender, e);
            }
        }
    }   
}
