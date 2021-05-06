using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic; //Needed for Lists
using System.Xml; //Needed for XML functionality
using System.Xml.Serialization; //Needed for XML Functionality
using System.IO;
using System.Xml.Linq;
using UnityEditor; //Needed for XDocument

public class Loader : MonoBehaviour
{
    public void Load()
    {
        string data = SaveSystem.Load("cardsDatabase");

        if (!string.IsNullOrWhiteSpace(data))
        {
            CardDataList cardDataList = JsonUtility.FromJson<CardDataList>(data);

            
        }
    }
}