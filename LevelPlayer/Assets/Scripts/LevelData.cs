using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

[System.Serializable]
public class LevelData
{
    public int[] winConds { get; set; }
    public int[][] data { get; set; }
    public int width { get; set; }
    public int height { get; set; }
    public int timer { get; set; }
    public static LevelData JsonToLevelData(string s) { return JsonConvert.DeserializeObject<LevelData>(s); }
    }

