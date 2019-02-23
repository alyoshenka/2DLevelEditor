using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.IO;

// single player validation should be done in editor
// level chaining in editor
// maually set win cond
// exe loads from file

public class GameManager : MonoBehaviour {

    enum TileType { empty=0, player, enemy, wall, floor, pickup, goal };

    List<List<TileType?>> data;

    // change how this is done
    GameObject playerObject;
    GameObject goalObject;
    GameObject enemyObject;
    GameObject wallObject;
    GameObject floorObject;
    GameObject pickupObject;

    // Use this for initialization
    void Awake () {
        
        // load file
        data = new List<List<TileType?>>();        
        LoadData("blah");
        SetupLevel();

        Cursor.SetCursor(Resources.Load("cursor") as Texture2D, Vector2.zero, CursorMode.Auto);
    }

    // loads data from file into list
    void LoadData(string path)
    {
        // reset path
        path = "C:\\Users\\s189076\\source\\repos\\LevelEditor\\LevelPlayer\\Builds\\unityTest.txt";
        Debug.Log(Directory.GetCurrentDirectory());

        // if (path.Substring(path.Length - 5) != ".txt") { path += ".txt"; }

        try
        {
            string line;
            List<TileType?> line2 = new List<TileType?>();
            
            using (StreamReader sr = new StreamReader("unityTest.txt"))
            {                
                while ((line = sr.ReadLine()) != null)
                {
                    string[] tiles = line.Split('-');
                    for (int i = 0; i < tiles.Length-1; i++)
                    {                        
                        line2.Add(StringToTileType(tiles[i]));
                    }                    
                    data.Add(new List<TileType?>(line2));
                    line2.Clear();
                }
            }
        }
        catch { Debug.Log("fail :("); return; }
    }

    void SetupLevel() // make this function better
    {
        Vector3 pos = new Vector3();
        Quaternion rot = new Quaternion();
        for (int i = 0; i < data.Count; i++)
        {
            for(int j = 0; j < data[i].Count; j++)
            {
                // better way to do it!
                pos = new Vector3(i, j, 0); // check these vals
                if (data[i][j] == TileType.player) { Instantiate(Resources.Load("Player"), pos, rot); }
                if (data[i][j] == TileType.wall  ) { Instantiate(Resources.Load("Wall"),   pos, rot); }
                if (data[i][j] == TileType.pickup) { Instantiate(Resources.Load("Pickup"), pos, rot); }
                if (data[i][j] == TileType.enemy ) { Instantiate(Resources.Load("Enemy"),  pos, rot); }
                if (data[i][j] == TileType.goal  ) { Instantiate(Resources.Load("Goal"),   pos, rot); }
                if (data[i][j] == TileType.floor) { Instantiate(Resources.Load("Floor"), pos, rot); }
                Instantiate(Resources.Load("Floor"), pos, rot); // always add floor
            }
        }
    }

    // returns tiletype from string
    TileType StringToTileType(string s)
    {
        try
        {
            // try parsing
            TileType t = (TileType)System.Enum.Parse(typeof(TileType), s);
            return t;
        }
        catch { return TileType.empty; }
    }

    public void TryToAdvance()
    {
        // if win cond, go to win screen
 
    }
    
}
