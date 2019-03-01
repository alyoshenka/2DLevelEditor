using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.IO;
using System.Collections.ObjectModel;
using System.Numerics;

// to do
// dynamically *move* buttons instead of reinitializing them
// add instructions
// ButtonAt(Index) function
// add key for unity ^
// listbox (?) for level chaining
// optimization of data
// dark theme
// organize
// prev / temp save file
// parent class for ui args?
// overlay instructions
// index -> vector2
// dont let arrow keys access slider
// border on selected item?
// show if there is more level off screen
// fix mousewheelhandler
// fix level scroll
// handledeventstoo
// error output
// load games
// flip sprite
// close editor then open player?
// add default levels
// better error handling and messages
// currently only support for 1 game


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
      
        // puts data into grid
        void DisplayData(LevelData level)
        {
            // init data
            for (int y = 0; y < level.height; y++)
            {
                for (int x = 0; x < level.width; x++)
                {
                    data[y, x] = (TileType)level.data[y, x];
                }
            }
            // IS THIS NEEDED
            rows = level.height + level.height % stepVal;
            cols = level.width + level.width % stepVal;
            rows = rows.Clamp(minRows, maxRows);
            cols = cols.Clamp(minCols, maxCols);
            // set to same size (square grid)
            if (cols > rows) { rows = cols; }
            if (rows > cols) { cols = rows; }

            // checkboxes
            ecb.IsChecked = level.winConds.Contains(0);
            pcb.IsChecked = level.winConds.Contains(1);
            gcb.IsChecked = level.winConds.Contains(2);
            tcb.IsChecked = level.winConds.Contains(3);

            InitLevel(rows, cols);
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
        
        // custom dependancy property to get parent
        // static readonly DependencyProperty GetParentProperty = DependencyProperty.Register("Parent", typeof(ListBox), typeof(ListBoxItem));
        
        static T GetParent<T>(DependencyObject depObj) where T : class
        {
            DependencyObject target = depObj;
            // do { target = VisualTreeHelper.GetParent(target); }
            while (target != null && ! (target is T)) { target = VisualTreeHelper.GetParent(target); }
            return target as T;
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
            // only if tbs not selected
            if(tileGrid.IsMouseOver == false) { return; }

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
            level.height = (int)max.Y+1;
            level.width = (int)max.X+1;
            level.data = new int[level.height, level.width];
            for (int y = 0; y < level.height; y++)
            {
                for (int x = 0; x < level.width; x++) { level.data[y, x] = (int)data[y, x]; }           
            }
            // winconds            
            level.winConds = new List<int>(); // change logic later
            if (gcb.IsChecked == true) { level.winConds.Add((int)WinCondition.goal); }
            if (ecb.IsChecked == true) { level.winConds.Add((int)WinCondition.enemies); }
            if (pcb.IsChecked == true) { level.winConds.Add((int)WinCondition.pickups); }
            if (tcb.IsChecked == true) { level.winConds.Add((int)WinCondition.time); }
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

        // checks if level is compatable with win conds
        bool ValidateLevel()
        {
            LevelData current = ToLevelData();
            if (!current.data.Contains((int)TileType.player)) { outputBlock.Text = "Need player in level"; return false; }
            if (gcb.IsChecked == true && !current.data.Contains((int)TileType.goal)) { outputBlock.Text = "Need goal in level"; return false; }
            if (ecb.IsChecked == true && !current.data.Contains((int)TileType.enemy)) { outputBlock.Text = "Need enemy in level"; return false; }
            if (pcb.IsChecked == true && !current.data.Contains((int)TileType.pickup)) { outputBlock.Text = "Need pickup in level"; return false; }
            return true;
        }

        private void loadedLevels_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        //private void uLevels_MouseWheel(object sender, MouseWheelEventArgs e)
        //{

        //}
    }   
}
