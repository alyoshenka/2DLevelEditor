﻿using System.Windows;
using System.Windows.Controls;
using System.IO;
using Newtonsoft.Json;

namespace LevelEditor
{
    // holds functions that handle file io
    public partial class MainWindow : Window
    {
        // saves level data to file
        void SaveData(string path)
        {
            if (path == "") { outputBlock.Text = "cannot save to empty file"; return; }
            if(! ValidateLevel()) { return; }
            path = "Levels/" + path + ".txt";

            using (StreamWriter file = File.CreateText(path)) { file.WriteLine(JsonConvert.SerializeObject(ToLevelData())); }

            ShowLevelOrder(); // update
        }

        // saves level chain to file
        void SaveGameButton(object sender, RoutedEventArgs e)
        {
            string path = "Games/Game1.txt";
            outputBlock.Text = "Saved game";
            using (StreamWriter sw = File.CreateText(path))
            {
                sw.WriteLine(JsonConvert.SerializeObject(key)); // save key
                foreach (ListBoxItem i in lLevels.Items) { sw.WriteLine(JsonConvert.SerializeObject(LoadData(i.Content.ToString()))); } // save levels
            }
        }

        // loads data from file
        LevelData LoadData(string path)
        {
            path = "Levels/" + path + ".txt";

            LevelData level;
            try { using (StreamReader sr = new StreamReader(path)) { level = JsonConvert.DeserializeObject<LevelData>(sr.ReadToEnd()); } }
            catch { level = ToLevelData(); outputBlock.Text = "error loading data from " + path; }  // return current level                   
            return level;
        }
    }
}
