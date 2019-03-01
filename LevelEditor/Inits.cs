using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Collections.Generic;

namespace LevelEditor

    // holds initialization functions
{
    public partial class MainWindow : Window
    {
        // inits feilds
        void InitFeilds()
        {
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
            data = new TileType[maxRows, maxCols];
            selectedB = new Button();
            playerIdx = null;
            goalIdx = null;
            gcb.IsChecked = true;
            currentTile = TileType.empty;
            savedLevels = new System.Collections.ObjectModel.ObservableCollection<ListBoxItem>();
            loadedLevels = new System.Collections.ObjectModel.ObservableCollection<ListBoxItem>();
            uLevels.DataContext = savedLevels;
            lLevels.DataContext = loadedLevels;
            //uLevels.MouseWheel += new MouseWheelEventHandler(MouseWheelHandler);
            //lLevels.MouseWheel += new MouseWheelEventHandler(MouseWheelHandler);
            uLevels.SelectionChanged += new SelectionChangedEventHandler(SelectionChangedHandler);
            lLevels.SelectionChanged += new SelectionChangedEventHandler(SelectionChangedHandler);
            key = new Dictionary<int, string>();
        }

        // initialize level feild
        void InitLevel(int r, int c)
        {
            if (null == tileGrid) { return; } // so no run on init

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

            // place buttons
            for (int y = 0; y < r; y++)
            {
                for (int x = 0; x < c; x++)
                {
                    Button b = new Button();
                    b.Click += new RoutedEventHandler(PlaceTile);
                    b.MouseEnter += new MouseEventHandler(ClickAndDrag);
                    b.Background = Brushes.LightGray;
                    Grid.SetRow(b, y);
                    Grid.SetColumn(b, x);
                    // put data back in
                    if (data != null)
                    {
                        if (data[x, y] != TileType.empty) { b.Content = (string)data[x, y].ToString(); }
                        b.BorderBrush = ((TileData)data[x, y]).color;
                        b.FontSize = tileGrid.Height / rows / 4;
                    }
                    tileGrid.Children.Add(b);
                }
            }
        }

        void InitTileData()
        {
            defaultTiles = new TileData[8];
            defaultTiles[0] = new TileData(TileType.empty, 0, Brushes.White, "");
            defaultTiles[1] = new TileData(TileType.player, 1, Brushes.Blue, "Player");
            defaultTiles[2] = new TileData(TileType.enemy, 2, Brushes.Red, "Enemy");
            defaultTiles[3] = new TileData(TileType.floor, 3, Brushes.Orange, "Floor");
            defaultTiles[4] = new TileData(TileType.wall, 4, Brushes.Green, "Wall");
            defaultTiles[5] = new TileData(TileType.goal, 5, Brushes.Purple, "Goal");
            defaultTiles[6] = new TileData(TileType.pickup, 6, Brushes.Yellow, "Pickup");
            defaultTiles[7] = new TileData(TileType.random, 7, Brushes.Black, "Random");
        }
    }
}
