using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.IO;

// single player validation should be done in editor

public class GameManager : MonoBehaviour {

    enum TileType { player, enemy, wall, floor, pickup, goal, empty };

    List<List<TileType?>> data;

    // change how this is done
    GameObject playerObject;
    GameObject goalObject;
    GameObject enemyObject;
    GameObject wallObject;
    GameObject floorObject;
    GameObject pickupObject;
   

    // Use this for initialization
    void Start () {

        //playerObject = GameObject.FindGameObjectWithTag("Player");
        //goalObject = GameObject.FindGameObjectWithTag("Goal");
        GameObject test = Instantiate((GameObject)Resources.Load("Player"));

        // load file
        data = new List<List<TileType?>>();        
        LoadData("blah");
        SetupLevel();
	}	

    // loads data from file into list
    void LoadData(string path)
    {
        // reset path
        path = "C:\\Users\\s189076\\source\\repos\\2dleveleditor\\LevelEditor\\bin\\Debug\\Levels\\unityTest1.txt";
        Debug.Log(Directory.GetCurrentDirectory());

        // if (path.Substring(path.Length - 5) != ".txt") { path += ".txt"; }

        try
        {
            string line;
            List<TileType?> line2 = new List<TileType?>();
            
            using (StreamReader sr = new StreamReader(Directory.GetCurrentDirectory() + "/unityTest1.txt"))
            {                
                while ((line = sr.ReadLine()) != null)
                {
                    string[] tiles = line.Split('-');
                    for (int i = 0; i < tiles.Length-1; i++)
                    {
                        //if(tiles[i] == "player") { line2.Add(TileType.player); } // do better
                        //else { line2.Add(TileType.floor); }
                        line2.Add(StringToTileType(tiles[i]));
                    }                    
                    data.Add(new List<TileType?>(line2));
                    line2.Clear();
                }
            }
        }
        catch { Debug.Log("fail :("); return; }
    }

    void SetupLevel()
    {
        Vector3 pos = new Vector3();
        Quaternion rot = new Quaternion();
        for (int i = 0; i < data.Count; i++)
        {
            for(int j = 0; j < data[i].Count; j++)
            {
                // better way to do it!
                pos = new Vector3(i, j, 0);
                if (data[i][j] == TileType.player) { Instantiate(Resources.Load("Player"), pos, rot); }
                if (data[i][j] == TileType.wall  ) { Instantiate(Resources.Load("Wall"),   pos, rot); }
                if (data[i][j] == TileType.pickup) { Instantiate(Resources.Load("Pickup"), pos, rot); }
                if (data[i][j] == TileType.enemy ) { Instantiate(Resources.Load("Enemy"), pos, rot); }
                // if (data[i][j] == TileType.floor) { Instantiate(enemyObject, new Vector3(i, j, 0), rot); }
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
        catch
        {
            // try reading through string
            switch (s)
            {
                case "player":
                    return TileType.player;
                default:
                    // return "nothing"
                    return TileType.empty;
            }
        }

    }
    
}
