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
// dynamically *move* buttons instead of reinitializing them
// don't save blank data
// way to get rows and cols from file
// add instructions
// ButtonAt(Index) function
// save in JSON
// add key for unity ^
// bool for isplacing vs isremoving?
// listbox (?) for level chaining
// index vs vec2
// connect levels in class/ file?

namespace LevelEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {         
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
        TileType[,] data;
        // singletons
        Index? playerIdx;
        Index? goalIdx;
        static TileData[] defaultTiles;

        Button selectedB;

        // buttons
        TileType currentTile;

        public MainWindow()
        {
            // create levels folder if it doesnt't exist
            Directory.CreateDirectory("Levels");

            InitializeComponent();

            InitFeilds();
            InitTileData();
            InitLevel(rows, cols);
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
                    // what happens here
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
                        for (int x = 0; x < rows; x++) { row += ((TileData)data[x, y]).storeNum + "-"; } // CHANGE
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
                        for (int i = 0; i < tiles.Length - 1; i++)
                        {
                            int j = Int32.Parse(tiles[i]);
                            TileType k = ((TileData)(j)).type;
                            data[c, r] = k; // ?
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
            for (int r = 0; r < rows; r++) { for (int c = 0; c < cols; c++) { data[c, r] = TileType.empty; } }       
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
            selectedB.ClearValue(BackgroundProperty);
            selectedB = b;
            TileType prev = currentTile;
            currentTile = ((TileData)selectedB.Content.ToString()).type;
            //if (prev == currentTile) { currentTile = TileType.empty; }
            //else { selectedB.Background = Brushes.LightBlue; }   
            // make clear selected on double click?
            selectedB.Background = Brushes.LightBlue;
        }

        // places tile
        public void PlaceTile(object sender, RoutedEventArgs e)
        {
            Button b = (Button)sender;
            b.Content = selectedB.Content; // set content
            b.Background = selectedB.BorderBrush; // set color           
            Index i = GetIndex(b);
            if (currentTile == TileType.player)
            {               
                if(playerIdx != null)
                {
                    Index p = (Index)playerIdx;
                    data[p.y, p.x] = TileType.empty;
                }
                playerIdx = i;
            }
            if (currentTile == TileType.goal)
            {
                if (goalIdx != null)
                {
                    Index p = (Index)goalIdx;
                    data[p.y, p.x] = TileType.empty;
                }
                goalIdx = i;
            }
            data[i.y, i.x] = data[i.y, i.x] == currentTile ? TileType.empty : currentTile;
            InitLevel(rows, cols);
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
            if (b.IsMouseOver && Mouse.RightButton == MouseButtonState.Pressed) { PlaceTile(sender, e); }
        }

        void WinCond(object sender, RoutedEventArgs e)
        {
            // find how many boxes are checked
            int count = 0;
            if (gcb.IsChecked == true) { count++; }
            if (ecb.IsChecked == true) { count++; }
            if (pcb.IsChecked == true) { count++; }
            if (tcb.IsChecked == true) { count++; }
            if(count == 0) { ((CheckBox)sender).IsChecked = true; }
        }

        // the point of this program
        void OpenInUnity(object sender, RoutedEventArgs e)
        {
            string path = "C:\\Users\\s189076\\source\\repos\\LevelEditor\\LevelPlayer\\Builds\\LevelPlayer.exe"; // make relative           
            // process.waitforexit
            System.Diagnostics.ProcessStartInfo s = new System.Diagnostics.ProcessStartInfo();
            s.WorkingDirectory = path;
            // System.Diagnostics.Process.Start(s); // THIS
        }

        // handles keyboard input
        void KeyDownHandler(object sender, KeyEventArgs e)
        {
            Vector2 input = new Vector2();
            input.x = e.Key == Key.Left ? -1 : e.Key == Key.Right ? 1 : 0;
            input.y = e.Key == Key.Up ? -1 : e.Key == Key.Down ? 1 : 0;
            ShiftGrid(input);
        }

        // moves grid in direction of arrow keys
        void ShiftGrid(Vector2 dir)
        {
            // look for patterns to optimize code

            // down
            if(dir.y == 1)
            {
                for(int r = rows - 1; r > 0; r--)
                {
                    for(int c = 0; c < cols; c++)
                    {
                        // data[c, 0] = TileType.empty;
                        data[c, r] = data[c, r-1];
                    }
                }
                for(int c = 0; c < cols; c++)
                {
                    data[c,0] = TileType.empty;
                }
            }
            // up
            if(dir.y == -1)
            {
                for(int r = 0; r < rows - 1; r++)
                {
                    for(int c = 0; c < cols; c++)
                    {
                        data[c, r] = data[c, r + 1];
                    }
                }
                for (int c = cols - 1; c >= 0; c--)
                {
                    data[c, rows - 1] = TileType.empty;
                }
            }
            // right
            if (dir.x == 1)
            {
                for (int r = 0; r < rows; r++)
                {
                    for (int c = cols - 1; c > 0; c--)
                    {
                        data[c, r] = data[c - 1, r];
                    }
                }
                for(int r = 0; r < rows; r++) { data[0, r] = TileType.empty; }
            }
            // left
            if(dir.x == -1)
            {
                for (int r = 0; r < rows; r++)
                {
                    for (int c = 0; c < cols - 1; c++)
                    {
                        data[c, r] = data[c + 1, r];
                    }
                }
                for(int r = 0; r < rows; r++) { data[cols - 1, r] = TileType.empty; }
            }          
            InitLevel(rows, cols);
        }

        // gets the index of highest row and col
        Index GetMaxSize()
        {
            Index max = new Index(0, 0);
            for(int y = 0; y < cols; y++)
            {
                for(int x = 0; x < rows; x++)
                {
                    if(max.x > x) { max.x = x; }
                    if(max.y > y) { max.y = y; }
                }
            }
            return max;
        }

        // puts data into json format for serialization
        
    }   
}
