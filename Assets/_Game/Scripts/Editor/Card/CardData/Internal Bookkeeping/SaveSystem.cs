using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace DuelMasters.Editor.Data.InternalBookkeeping
{
    public static class SaveSystem
    {
        private static readonly string DATA_FOLDER =
            Application.dataPath + "/Editor Default Resources/Editor Internal Bookkeeping/";

        [UnityEditor.Callbacks.DidReloadScripts]
        public static void Initialize()
        {
            if (!Directory.Exists(DATA_FOLDER))
            {
                Directory.CreateDirectory(DATA_FOLDER);
            }
        }

        public static void Save(string filename, string saveString)
        {
            File.WriteAllText(DATA_FOLDER + $"/{filename}.json", saveString);
        }

        public static string Load(string filename)
        {
            if (File.Exists(DATA_FOLDER + $"/{filename}.json"))
                return File.ReadAllText(DATA_FOLDER + $"/{filename}.json");
            
            return null;
        }
    }
}