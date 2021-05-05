using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class SaveSystem
{
    static readonly string SAVE_FOLDER = Application.dataPath + "/_Game/Data";

    public static void Initialize()
    {
        if (!Directory.Exists(SAVE_FOLDER))
        {
            Directory.CreateDirectory(SAVE_FOLDER);
        }
    }

    public static void Save(string saveString, string fileName = "saveFile")
    {
        File.WriteAllText(SAVE_FOLDER + $"/{fileName}.json", saveString);
    }

    public static string Load(string fileName = "saveFile")
    {
        if (File.Exists(SAVE_FOLDER + $"/{fileName}.json"))
            return File.ReadAllText(SAVE_FOLDER + $"/{fileName}.json");

        return null;
    }
}
