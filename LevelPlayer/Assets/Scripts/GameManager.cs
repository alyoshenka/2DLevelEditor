using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.IO;
using Newtonsoft.Json;
using UnityEngine.SceneManagement;
//using System.Runtime.CompilerServices;
//using System.Web.Helpers;
//using System;

// single player validation should be done in editor
// level chaining in editor
// maually set win cond
// exe loads from file
// silent launch?
// delete files from editor
// add health
// do i need lastlevel check?
// 2d array rotated

public class GameManager : MonoBehaviour {

    List<LevelData> levels;
    LevelData current;
    int currentIdx;

    public bool atGoal;

    // change how this is done
    GameObject playerObject;
    GameObject goalObject;
    GameObject enemyObject;
    GameObject wallObject;
    GameObject floorObject;
    GameObject pickupObject;

    // Use this for initialization
    void Awake () {

        DontDestroyOnLoad(gameObject);
        // load file
        levels = new List<LevelData>();
        // key = new Dictionary<int, string>();
        atGoal = false;
        currentIdx = -1;

        // get args
        string[] args = System.Environment.GetCommandLineArgs();
        string path = ""; // change
        LoadData(path);
                       
        Cursor.SetCursor(Resources.Load("cursor") as Texture2D, Vector2.zero, CursorMode.Auto);
    }

    public void LoadNextLevel()
    {
        currentIdx++;     
        if (currentIdx >= levels.Count) { SceneManager.LoadScene("Win"); } // win screen
        else
        {
            Debug.Log("jonno");
            current = levels[currentIdx];
            // Debug.Break();
            // SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene().name);
            SceneManager.LoadScene("Main");
            // Debug.Break();
                       
            // SetupLevel();
        }               
    }

    // loads data from file into list
    void LoadData(string path)
    {
        // reset path
        path = "C:/Users/s189076/source/repos/LevelEditor/LevelEditor/bin/Debug/Games/unityGame.txt";
        // verify path and make relative

        using (StreamReader sr = new StreamReader(path))
        {
            string line;
            while ((line = sr.ReadLine()) != null)
            {
                LevelData cur = LevelData.JsonToLevelData(line);
                if (cur.data != null) { levels.Add(cur); }
            }
        }
    }

    public void SetupLevel() // make this function better
    {
        Vector3 pos = new Vector3();
        Quaternion rot = new Quaternion();
        // current.data = RotateArray(current.data, current.width, current.height);
        // change to use key
        for (int i = 0; i < current.height; i++)
        {
            for(int j = 0; j < current.width; j++)
            {
                // better way to do it!
                pos = new Vector3(j, i, 0); // check these vals
                // if (current.data[i][j] == 1) { Instantiate(Resources.Load("Player"), pos, rot); }
                if (current.data[i][j] == 2) { Instantiate(Resources.Load("Enemy"), pos, rot); }
                if (current.data[i][j] == 4) { Instantiate(Resources.Load("Floor"), pos, rot); }
                if (current.data[i][j] == 3) { Instantiate(Resources.Load("Wall"), pos, rot); }
                if (current.data[i][j] == 6) { Instantiate(Resources.Load("Goal"), pos, rot); }
                if (current.data[i][j] == 5) { Instantiate(Resources.Load("Pickup"), pos, rot); }

                if (current.data[i][j] == 1) { GameObject.FindGameObjectWithTag("Player").gameObject.transform.position = pos; }
                
                if (current.data[i][j] != 4) { Instantiate(Resources.Load("Floor"), pos, rot); } // always add floor
            }
        }
    }

    // rotates array
    //int[][] RotateArray(int[][] orig, int w, int h)
    //{

    //}


    public void TryToAdvance()
    {
        // if win cond, go to win screen or next level
        // else restart level (?) -> this should be a setting
        if (CheckWinConds()) { LoadNextLevel(); }
    }

    // checks if win conditions have been met
    bool CheckWinConds()
    {      
        bool ret = true;
        for(int i = 0; i < current.winConds.Length; i++) // use key
        {
            if (current.winConds[i] == 0) { ret = NoEnemies(); }
            if (current.winConds[i] == 1) { ret = NoPickups(); }
            if (current.winConds[i] == 2) { ret = atGoal; }
        }
        return ret;
    }

    // checks if player has defeated all enemies
    bool NoEnemies() { return GameObject.FindGameObjectsWithTag("Enemy").Length == 0; }

    // checks if player has collected all pickups
    bool NoPickups() { return GameObject.FindGameObjectsWithTag("Pickup").Length == 0; }

    // time is different

}
