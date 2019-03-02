using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Numerics;
using System.Collections.ObjectModel;
using System.IO;

namespace LevelEditor
{
    // holds functions that deal exclusively with wpf controls

    public partial class MainWindow : Window
    {

        // the point of this program
        void OpenInUnity(object sender, RoutedEventArgs e)
        {
            // string path = "C:\\Users\\s189076\\source\\repos\\LevelEditor\\LevelPlayer\\Builds\\LevelPlayer.exe"; // make relative    
            string path = Directory.GetCurrentDirectory() + "/levelPlayer.exe";
            string game = "Game=/Games/Game1.txt"; // change

            System.Diagnostics.Process.Start(path, game);

        }

        void SaveLevelButton(object sender, RoutedEventArgs e)
        {
            outputBlock.Text = "Level Saved";
            SaveData(saveText.Text);
        }

        // buttonclick opens level
        void LoadDataButton(object sender, RoutedEventArgs e)
        {
            outputBlock.Text = "Loaded "+ loadText.Text;
            DisplayData(LoadData(loadText.Text));
        }

        // double click opens level
        void LoadDataMouse(object sender, MouseEventArgs e) // CHECK
        {
            ListBoxItem i = (ListBoxItem)sender;
            string s = i.Content.ToString();
            loadText.Text = s;
            saveText.Text = s;
            lvlName.Content = "Level: " + s;
            outputBlock.Text = "Loaded " + loadText.Text;
            DisplayData(LoadData(s));
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
            outputBlock.Text = "Selected " + currentTile + " tile";
            //if (prev == currentTile) { currentTile = TileType.empty; }
            //else { selectedB.Background = Brushes.LightBlue; }   
            // make clear selected on double click?
            selectedB.Background = Brushes.LightBlue;
        }

        // allows click and drag selection+
        void ClickAndDrag(object sender, MouseEventArgs e)
        {
            Button b = (Button)sender;
            if (b.IsMouseOver && Mouse.RightButton == MouseButtonState.Pressed) { PlaceTile(sender, e); }
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
            if (sg) { ShiftGrid(input); outputBlock.Text = "Shifting grid"; }

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
            ObservableCollection<ListBoxItem> parent;
            ObservableCollection<ListBoxItem> other;
            ListBox parentLB;
            if (GetParent<ListBox>(current) == uLevels)
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
                outputBlock.Text = "Swapping levels";

            }
            int idx = parent.IndexOf(current);
            // if within bounds
            if (e.Key == Key.Left && idx > 0)
            {
                parent.Remove(current);
                parent.Insert(idx - 1, current);
                parentLB.SelectedIndex = idx - 1;

            }
            if (e.Key == Key.Right && idx < parent.Count - 1)
            {
                parent.Remove(current);
                parent.Insert(idx + 1, current);
                parentLB.SelectedIndex = idx + 1;
            }

            // delete
            if(e.Key == Key.Delete || e.Key == Key.Back)
            {
                parent.Remove(current);
                parentLB.SelectedIndex = idx;
                outputBlock.Text = "Deleting " + current.Content;
                File.Delete("Levels/" + current.Content + ".txt");
            }
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

        // clears data in grid
        void ClearData(object sender, RoutedEventArgs e)
        {
            for (int r = 0; r < rows; r++) { for (int c = 0; c < cols; c++) { data[c, r] = TileType.empty; } }
            InitLevel(rows, cols); // there is a more efficient way than recreating each time
            saveText.Text = "new level";
            lvlName.Content = "Level: new level";
            outputBlock.Text = "Clearing data, making blank level";
        }

        void timeCB(object sender, RoutedEventArgs e)
        {
            CheckBox cb = (CheckBox)sender;
            if (cb.IsChecked == true ) { timeTB.IsEnabled = true;  }
            if (cb.IsChecked == false) { timeTB.IsEnabled = false; }
        }
    }
}
