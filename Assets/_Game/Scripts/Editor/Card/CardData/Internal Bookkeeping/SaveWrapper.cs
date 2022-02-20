using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DuelMasters.Editor.Data.InternalBookkeeping
{
    public static class SaveWrapper
    {
        public static void Save<TKey, TValue>(string filename, Dictionary<TKey, TValue> dataDict)
        {
            List<SerializableKeyValuePair<TKey, TValue>> dataPairs = new List<SerializableKeyValuePair<TKey, TValue>>();

            foreach (KeyValuePair<TKey, TValue> keyValuePair in dataDict)
            {
                SerializableKeyValuePair<TKey, TValue> dataPair = new SerializableKeyValuePair<TKey, TValue>
                {
                    Key = keyValuePair.Key,
                    Value = keyValuePair.Value
                };

                dataPairs.Add(dataPair);
            }

            SerializableList<SerializableKeyValuePair<TKey, TValue>> dataList =
                new SerializableList<SerializableKeyValuePair<TKey, TValue>>
                {
                    List = dataPairs
                };
            string saveFile = JsonUtility.ToJson(dataList);
            SaveSystem.Save(filename, saveFile);
        }

        public static Dictionary<TKey, TValue> Load<TKey, TValue>(string filename)
        {
            string saveFile = SaveSystem.Load(filename);

            if (!string.IsNullOrWhiteSpace(saveFile))
            {
                SerializableList<SerializableKeyValuePair<TKey, TValue>> dataPairs =
                    JsonUtility.FromJson<SerializableList<SerializableKeyValuePair<TKey, TValue>>>(saveFile);

                Dictionary<TKey, TValue> dataDict = new Dictionary<TKey, TValue>();

                foreach (SerializableKeyValuePair<TKey, TValue> dataPair in dataPairs.List)
                {
                    dataDict.Add(dataPair.Key, dataPair.Value);
                }
                
                Debug.Log($"Loaded data file: {filename}");
                return dataDict;
            }

            Debug.LogWarning($"Missing data file: {filename}");
            return new Dictionary<TKey, TValue>();
        }
    }
}