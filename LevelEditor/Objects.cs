using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Numerics;

namespace LevelEditor
{
    // contains helper enums, structs, classes

    public partial class MainWindow : Window
    {
        enum TileType { empty, player, enemy, wall, floor, pickup, goal, random };
        enum WinCondition { enemies, pickups, goal, time };

        // holds tile data and allows type conversion given one input
        struct TileData
        {
            public TileType type;
            public int storeNum;
            public Brush color;
            public string displayS;
            public TileData(TileType t, int i, Brush b, string s)
            {
                type = t;
                storeNum = i;
                color = b;
                displayS = s;
            }
            public static explicit operator TileData(TileType t) // can i do this using reflection?
            {
                for (int i = 0; i < defaultTiles.Length; i++) { if (defaultTiles[i].type == t) { return defaultTiles[i]; } }
                return defaultTiles[0];
            }
            public static explicit operator TileData(int num)
            {
                for (int i = 0; i < defaultTiles.Length; i++) { if (defaultTiles[i].storeNum == num) { return defaultTiles[i]; } }
                return defaultTiles[0];
            }
            public static explicit operator TileData(Brush b)
            {
                for (int i = 0; i < defaultTiles.Length; i++) { if (defaultTiles[i].color == b) { return defaultTiles[i]; } }
                return defaultTiles[0];
            }
            public static explicit operator TileData(string s)
            {
                for (int i = 0; i < defaultTiles.Length; i++) { if (defaultTiles[i].displayS == s) { return defaultTiles[i]; } }
                return defaultTiles[0];
            }
        }

        class LevelData
        {
            public List<int> winConds;
            public bool isLastLevel;
            public int[,] data;
            public int width;
            public int height;
        }
    }
}
