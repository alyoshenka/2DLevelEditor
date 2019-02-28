using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.IO;
using Newtonsoft.Json;
//using System.Runtime.CompilerServices;
//using System.Web.Helpers;
//using System;

// single player validation should be done in editor
// level chaining in editor
// maually set win cond
// exe loads from file
// silent launch?
// delete files from editor

public class GameManager : MonoBehaviour {

    class LevelData
    {
        public List<string> winConds;
        public bool isLastLevel;
        public int[,] data;
        public Vector2 size;
    }

    List<LevelData> levels;

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
        levels = new List<LevelData>();
        // key = new Dictionary<int, string>();

        // get args
        string[] args = System.Environment.GetCommandLineArgs();
        // search for level data
        string path = "";
        foreach(string s in args) { if (s.Contains("LevelData")){ path = s; break; }}
        if(path == "") { /*error*/ }

        SetupLevel();

        // Cursor.SetCursor(Resources.Load("cursor") as Texture2D, Vector2.zero, CursorMode.Auto);
    }

    // loads data from file into list
    void LoadData(string path)
    {
        // reset path
        path = "C:/Users/s189076/source/repos/LevelEditor/LevelEditor/bin/Debug/Levels/unityTest.txt";
        // verify path and make relative

        try
        {
            
            List<int> line2 = new List<int>();
            
            using (StreamReader sr = new StreamReader(path))
            {
                string line = sr.ReadLine();
                while ((line = sr.ReadLine()) != null) { levels.Add(JsonConvert.DeserializeObject<LevelData>(line)); }
            }
        }
        catch { Debug.Log("fail :("); return; }
    }

    void SetupLevel() // make this function better
    {
        Vector3 pos = new Vector3();
        Quaternion rot = new Quaternion();
        // GO LEVEL BY LEVEL    
        for (int i = 0; i < levels[0].data.Length; i++)
        {
            for(int j = 0; j < levels.Count; j++)
            {
                // better way to do it!
                pos = new Vector3(i, j, 0); // check these vals
                //if (data[i][j] == TileType.player) { Instantiate(Resources.Load("Player"), pos, rot); }
                //if (data[i][j] == TileType.wall  ) { Instantiate(Resources.Load("Wall"),   pos, rot); }
                //if (data[i][j] == TileType.pickup) { Instantiate(Resources.Load("Pickup"), pos, rot); }
                //if (data[i][j] == TileType.enemy ) { Instantiate(Resources.Load("Enemy"),  pos, rot); }
                //if (data[i][j] == TileType.goal  ) { Instantiate(Resources.Load("Goal"),   pos, rot); }
                //if (data[i][j] == TileType.floor)  { Instantiate(Resources.Load("Floor"),  pos, rot); }
                // Instantiate(Resources.Load("Floor"), pos, rot); // always add floor
            }
        }
    }



    public void TryToAdvance()
    {
        // if win cond, go to win screen or next level
        // else restart level (?) -> this should be a setting
 
    }
    
}
