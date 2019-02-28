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
using System.Collections.ObjectModel;

// to do
// dynamically *move* buttons instead of reinitializing them
// add instructions
// ButtonAt(Index) function
// add key for unity ^
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
// dont let arrow keys access slider
// border on selected item?
// show if there is more level off screen
// fix mousewheelhandler
// fix level scroll
// handledeventstoo
// levels dont move correctly
// double click

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
        Vector2? playerIdx;
        Vector2? goalIdx;

        static TileData[] defaultTiles;
        Button selectedB;
        TileType currentTile;
        ObservableCollection<ListBoxItem> savedLevels;
        ObservableCollection<ListBoxItem> loadedLevels;
        static Dictionary<int, string> key; // unity int to string key


        public MainWindow()
        {
            // create levels folder if it doesnt't exist
            Directory.CreateDirectory("Levels");
            Directory.CreateDirectory("Games");

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
            foreach (var r in tileGrid.RowDefinitions)
            {
                List<string> row = new List<string>();
                foreach (var c in tileGrid.ColumnDefinitions)
                {
                    // what happens here
                }
                dat.Add(row);
            }
            return dat;
        }

        void SaveLevelButton(object sender, RoutedEventArgs e)
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

        // saves level chain to file
        void SaveGameButton(object sender, RoutedEventArgs e)
        {
            string path = "Games/" + saveGame.Text + ".txt";           
            using (StreamWriter sw = File.CreateText(path))
            {
                sw.WriteLine(JsonConvert.SerializeObject(key)); // save key
                foreach (ListBoxItem i in lLevels.Items) { sw.WriteLine(JsonConvert.SerializeObject(LoadData(i.Content.ToString()))); } // save levels
            }
        }

        // buttonclick opens level
        void LoadDataButton(object sender, RoutedEventArgs e)
        {
            DisplayData(LoadData(loadText.Text));            
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
        LevelData LoadData(string path)
        {
            path = "Levels/" + path + ".txt";

            LevelData level;
            using (StreamReader sr = new StreamReader(path)) { level = JsonConvert.DeserializeObject<LevelData>(sr.ReadToEnd()); }
            
            return level;
        }

        // puts data into grid
        void DisplayData(LevelData level)
        {
            // init data
            for (int y = 0; y < level.size.Y; y++)
            {
                for (int x = 0; x < level.size.X; x++)
                {
                    data[y, x] = (TileType)level.data[y, x];
                }
            }
            // IS THIS NEEDED
            rows = (int)level.size.Y + (int)level.size.Y % stepVal;
            cols = (int)level.size.X + (int)level.size.X % stepVal;
            rows = rows.Clamp(minRows, maxRows);
            cols = cols.Clamp(minCols, maxCols);
            // set to same size (square grid)
            if (cols > rows) { rows = cols; }
            if (rows > cols) { cols = rows; }

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
            Vector2 i = GetIndex(b);
            if (currentTile == TileType.player)
            {
                if (playerIdx != null)
                {
                    Vector2 p = (Vector2)playerIdx;
                    data[(int)p.Y, (int)p.X] = TileType.empty;
                }
                playerIdx = i;
            }
            if (currentTile == TileType.goal)
            {
                if (goalIdx != null)
                {
                    Vector2 p = (Vector2)goalIdx;
                    data[(int)p.Y, (int)p.X] = TileType.empty;
                }
                goalIdx = i;
            }
            if (data[(int)i.Y, (int)i.X] == currentTile && Mouse.RightButton != MouseButtonState.Pressed) { data[(int)i.Y, (int)i.X] = TileType.empty; }
            else { data[(int)i.Y, (int)i.X] = currentTile; }
            // data[i.y, i.x] = data[i.y, i.x] == currentTile ? TileType.empty : currentTile;
            InitLevel(rows, cols);
        }

        // gets index of button in grid // VECTOR2
        Vector2 GetIndex(Button b)
        {
            // terry help me do this better pls

            Vector2 idx = new Vector2();
            int val;

            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    val = r * cols + c; // gets index of child
                    if (tileGrid.Children[val] == b)
                    {
                        idx.X = r;
                        idx.Y = c;
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
            if (count == 0) { ((CheckBox)sender).IsChecked = true; }
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
            if (e.Key == Key.A) { input.X = -1; sg = true; }
            if (e.Key == Key.D) { input.X = 1; sg = true; }
            if (e.Key == Key.W) { input.Y = -1; sg = true; }
            if (e.Key == Key.S) { input.Y = 1; sg = true; }
            if (sg) { ShiftGrid(input); }

            // move levels
            LevelShiftHandler(sender, e);
        }

        // deals with lb selection
        void SelectionChangedHandler(object sender, SelectionChangedEventArgs e)
        {
            if ((ListBox)sender == uLevels) { lLevels.UnselectAll(); }
            if ((ListBox)sender == lLevels) { uLevels.UnselectAll(); }
        }

        // moves level order
        void LevelShiftHandler(object sender, KeyEventArgs e)
        {
            ListBoxItem current = GetSelectedItem();
            //ListBox parent;
            //ListBox other;
            //parent = GetParent<ListBox>(current);
            //other = parent == uLevels ? uLevels : lLevels;
            ObservableCollection<ListBoxItem> parent;
            ObservableCollection<ListBoxItem> other;
            ListBox parentLB;
            ListBox otherLB;
            if(GetParent<ListBox>(current) == uLevels)
            {
                parent = savedLevels;
                other = loadedLevels;
                parentLB = uLevels;
            }
            else
            {
                parent = loadedLevels;
                other = savedLevels;
                parentLB = lLevels;
            }

            // if enter, swap
            // if up/down -> up/down (wrapping)
            if (e.Key == Key.Enter)
            {
                // swap       
                parent.Remove(current);
                other.Add(current);
                
            }
            int idx = parent.IndexOf(current);
            // if within bounds
            if (e.Key == Key.Left && idx > 0)
            {
                parent.Remove(current);
                parent.Insert(idx - 1, current);
                parentLB.SelectedIndex = idx - 1;
                
            }
            if(e.Key == Key.Right && idx < parent.Count - 1)
            {
                parent.Remove(current);
                parent.Insert(idx + 1, current);
                parentLB.SelectedIndex = idx + 1;
            }
        }

        // custom dependancy property to get parent
        // static readonly DependencyProperty GetParentProperty = DependencyProperty.Register("Parent", typeof(ListBox), typeof(ListBoxItem));
        
        static T GetParent<T>(DependencyObject depObj) where T : class
        {
            DependencyObject target = depObj;
            // do { target = VisualTreeHelper.GetParent(target); }
            while (target != null && ! (target is T)) { target = VisualTreeHelper.GetParent(target); }
            return target as T;
        }

        void SwapListBoxControl(object sender, RoutedEventArgs e)
        {
            if ((ListBox)sender == lLevels) { uLevels.UnselectAll(); }
            if ((ListBox)sender == uLevels) { lLevels.UnselectAll(); }
        }

        // handles mouse wheel input
        void MouseWheelHandler(object sender, MouseWheelEventArgs e)
        {
            ListBoxItem selected = GetSelectedItem();
            if (null == selected) { return; }
            // ListBox parent = (ListBox)selected.Parent;
            // this doesn't work, look into dependancy property davis was talking about
            ListBox parent = GetParent<ListBox>(selected);

            int idx = GetIndexOfItem(parent, selected);

            try
            {
                if (e.Delta >= 1)
                {
                    if (idx > 0)
                    {
                        // shift
                        ListBoxItem temp = savedLevels[idx - 1];
                        savedLevels[idx - 1] = savedLevels[idx];
                        // savedLevels.Insert(idx, temp);
                        savedLevels[idx] = temp;
                        parent.SelectedIndex = idx - 1;
                    }
                }
                if (e.Delta <= -1)
                {
                    if (idx < savedLevels.Count - 1)
                    {
                        // shift
                        ListBoxItem temp = savedLevels[idx];
                        savedLevels[idx] = savedLevels[idx + 1];
                        // savedLevels.Insert(idx + 1, temp); // why is this breaking everything
                        savedLevels[idx + 1] = temp;
                        parent.SelectedIndex = idx + 1;
                    }
                }
            }
            catch { };
            
            // selected.DataContext = savedLevels;
            
        }

        // returns index of item in listbox MAY NOT NEED
        int GetIndexOfItem(ListBox lb, ListBoxItem i)
        {
            for(int j = 0; j < lb.Items.Count; j++)
            {
                if(lb.Items[j] == i) { return j; }
            }
            return -1;
        }

        // gets selected listboxitem
        ListBoxItem GetSelectedItem()
        {
            if(lLevels.SelectedItem != null) { return (ListBoxItem)lLevels.SelectedItem; }
            if(uLevels.SelectedItem != null) { return (ListBoxItem)uLevels.SelectedItem; }
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
        Vector2 GetMaxSize()
        {
            Vector2 max = new Vector2(0, 0);
            for(int y = 0; y < cols; y++)
            {
                for(int x = 0; x < rows; x++)
                {
                    if(x > max.X && data[y,x] != TileType.empty) { max.X = x; }
                    if(y > max.Y && data[y,x] != TileType.empty) { max.Y = y; }
                }
            }
            return max;
        }

        // puts data into level data
        LevelData ToLevelData()
        {
            LevelData level = new LevelData();
            // tile data
            Vector2 max = GetMaxSize();
            // convert to count
            max.X++;
            max.Y++;
            level.size = max;
            level.data = new int[(int)max.Y, (int)max.X];
            for (int y = 0; y < max.Y; y++)
            {
                for (int x = 0; x < max.X; x++) { level.data[y, x] = (int)data[y, x]; }           
            }
            // winconds            
            level.winConds = new List<string>(); // change logic later
            if (gcb.IsChecked == true) { level.winConds.Add(WinCondition.goal.ToString()); }
            if (ecb.IsChecked == true) { level.winConds.Add(WinCondition.enemies.ToString()); }
            if (pcb.IsChecked == true) { level.winConds.Add(WinCondition.pickups.ToString()); }
            if (tcb.IsChecked == true) { level.winConds.Add(WinCondition.time.ToString()); }
            // next level
            level.isLastLevel = true; // MODIFY HERE

            return level;
        }

        // show all files in Level in listbox
        void ShowLevelOrder()
        {
            // unloadedLevels.Items.Clear(); // optimize
            DirectoryInfo d = new DirectoryInfo("Levels");
            FileInfo[] files = d.GetFiles();
            savedLevels.Clear(); // should this be here
            foreach (FileInfo f in files)
            {
                ListBoxItem i = new ListBoxItem();
                i.Content = f.ToString().Replace(".txt", "");
                i.MouseDoubleClick += new MouseButtonEventHandler(LoadDataMouse);
                savedLevels.Add(i);
            }
            uLevels.ItemsSource = savedLevels; 
        }

        private void loadedLevels_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        //private void uLevels_MouseWheel(object sender, MouseWheelEventArgs e)
        //{

        //}
    }   
}
