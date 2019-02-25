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
using Newtonsoft.Json;
using System.Numerics;

// to do
// dynamically *move* buttons instead of reinitializing them
// add instructions
// ButtonAt(Index) function
// add key for unity ^
// bool for isplacing vs isremoving? -> use mousebutton enum
// listbox (?) for level chaining
// index vs vec2
// connect levels in class/ file? list<LevelData>?
// good way to show tiledata in txt file?
// optimization of data
// dark theme
// organize
// prev / temp save file
// parent class for ui args?
// overlay instructions
// save game -> list<LevelData>
// allow arr to go off screen for large level editing
// index -> vector2

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
            ShowLevelOrder();
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

        void SaveDataButton(object sender, RoutedEventArgs e)
        {
            SaveData(saveText.Text);
        }

        // saves level data to file
        void SaveData(string path)
        {
            path = "Levels/" + path + ".txt"; 

            using (StreamWriter file = File.CreateText(path)) { file.WriteLine(JsonConvert.SerializeObject(ToLevelData())); }

            ShowLevelOrder(); // update
        }

        // buttonclick opens level
        void LoadDataButton(object sender, RoutedEventArgs e)
        {
            LoadData(loadText.Text);
        }

        // double click opens level
        void LoadDataMouse(object sender, MouseEventArgs e)
        {
            ListBoxItem i = (ListBoxItem)sender;
            string s = i.Content.ToString();
            loadText.Text = s;
            LoadData(s);
        }

        // loads data from file
        void LoadData(string path)
        {           
            path = "Levels/" + path + ".txt"; 

            LevelData level;
            using (StreamReader sr = new StreamReader(path)) { level = JsonConvert.DeserializeObject<LevelData>(sr.ReadToEnd()); }

            // init data
            for (int y = 0; y < level.size.y; y++)
            {
                for (int x = 0; x < level.size.x; x++)
                {
                    data[y, x] = level.data[y, x];
                }
            }
            // IS THIS NEEDED
            rows = level.size.y + level.size.y % stepVal;
            cols = level.size.x + level.size.x % stepVal;
            rows.Clamp(minRows, maxRows);
            cols.Clamp(minCols, maxCols);
            // set to same size (square grid)
            if(cols > rows) { rows = cols; }
            if(rows > cols) { cols = rows; }

            InitLevel(rows, cols);
        }

        // clears data in grid
        void ClearData(object sender, RoutedEventArgs e)
        {
            for (int r = 0; r < rows; r++) { for (int c = 0; c < cols; c++) { data[c, r] = TileType.empty; } }       
            InitLevel(rows, cols); // there is a more efficient way than recreating each time
            saveText.Text = "new level";
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
            // shift grid
            Vector2 input = new Vector2();
            bool sg = false; // Shifting Grid
            if (e.Key == Key.Left) { input.X = -1; sg = true; }
            if (e.Key == Key.Right) { input.X = 1; sg = true; }
            if (e.Key == Key.Up) { input.Y = -1; sg = true; }
            if (e.Key == Key.Down) { input.Y = 1; sg = true; }
            if (sg) { ShiftGrid(input); }
        }

        // handles mouse wheel input
        void MouseWheelHandler(object sender, MouseWheelEventArgs e)
        {
            ListBoxItem selected = GetSelectedItem();
            if (null == selected) { return; }
            ListBox parent = (ListBox)selected.Parent;

            if(e.Delta >= 1)
            {               
                if(selected.TabIndex < parent.Items.Count - 1)
                {
                    // shift
                    ListBoxItem temp = selected;
                    parent.Items[selected.TabIndex] = parent.Items[selected.TabIndex - 1];
                    parent.Items[selected.TabIndex] = temp;
                }               
            }
            if (e.Delta <= -1)
            {
                if(selected.TabIndex > 0)
                {
                    // shift
                    ListBoxItem temp = selected;
                    parent.Items[selected.TabIndex] = parent.Items[selected.TabIndex + 1];
                    parent.Items[selected.TabIndex] = temp;
                }                
            }
        }

        // gets selected listboxitem
        ListBoxItem GetSelectedItem()
        {
            if(loadedLevels.SelectedItem != null) { return (ListBoxItem)loadedLevels.SelectedItem; }
            if(unloadedLevels.SelectedItem != null) { return (ListBoxItem)unloadedLevels.SelectedItem; }
            return null;
        }

        // moves grid in direction of arrow keys
        void ShiftGrid(Vector2 dir)
        {
            // look for patterns to optimize code

            // down
            if(dir.Y == 1)
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
            if(dir.Y == -1)
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
            if (dir.X == 1)
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
            if(dir.X == -1)
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
                    if(x > max.x && data[y,x] != TileType.empty) { max.x = x; }
                    if(y > max.y && data[y,x] != TileType.empty) { max.y = y; }
                }
            }
            return max;
        }

        // puts data into level data
        LevelData ToLevelData()
        {
            LevelData level = new LevelData();
            // tile data
            Index max = GetMaxSize();
            // convert to count
            max.x++;
            max.y++;
            level.size = max;
            level.data = new TileType[max.y, max.x];
            for (int y = 0; y < max.y; y++)
            {
                for (int x = 0; x < max.x; x++) { level.data[y, x] = data[y, x]; }           
            }
            // winconds            
            level.winConds = new List<WinCondition>(); // change logic later
            if (gcb.IsChecked == true) { level.winConds.Add(WinCondition.goal); }
            if (ecb.IsChecked == true) { level.winConds.Add(WinCondition.enemies); }
            if (pcb.IsChecked == true) { level.winConds.Add(WinCondition.pickups); }
            if (tcb.IsChecked == true) { level.winConds.Add(WinCondition.time); }
            // next level
            level.isLastLevel = true; // MODIFY HERE

            return level;
        }

        // show all files in Level in listbox
        void ShowLevelOrder()
        {
            unloadedLevels.Items.Clear(); // optimize
            DirectoryInfo d = new DirectoryInfo("Levels");
            FileInfo[] files = d.GetFiles();
            foreach (FileInfo f in files)
            {
                ListBoxItem i = new ListBoxItem();
                i.Content = f.ToString().Replace(".txt", "");
                i.MouseDoubleClick += new MouseButtonEventHandler(LoadDataMouse);
                unloadedLevels.Items.Add(i);
            }
        }

    }   
}
