using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace LevelEditor
{
    // contains helper enums and structs
    public partial class MainWindow : Window
    {
        enum TileType { empty, player, enemy, wall, floor, pickup, goal, random };
        enum WinCondition { enemies, pickups, goal, time };

        struct Index { public int y, x; }

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

        struct Vector2 { public float x, y; }
    }
}
