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
    List<CardBaseData> cards = new List<CardBaseData>();

    void Start()
    {
        SaveSystem.Initialize();
        
        for (int i = 1; i <= 12; i++)
        {
            FileInfo theSourceFile = new FileInfo($"Assets/_Game/Data/Cards/{i}/set.txt");
            StreamReader reader = theSourceFile.OpenText();

            string text;
            do
            {
                text = reader.ReadLine();
                if (!String.IsNullOrWhiteSpace(text) && text.Length > 10)
                {
                    string sub = text.Substring(1, 9);
                    if (sub == "card name")
                    {
                        int idIndex = text.IndexOf("id=");
                        int nameLen = idIndex - 14;
                        string Name = text.Substring(12, nameLen);

                        string id = text.Substring(idIndex + 4, 36);

                        CardBaseData cardBase = new CardBaseData();
                        cardBase.name = Name;
                        cardBase.id = id;

                        cards.Add(cardBase);
                    }
                }
            } while (text != null);
        }

        Load();
    }

    public void Save(List<Card> cards)
    {
        CardList cardList = new CardList();
        cardList.cards = cards;
        
        

        Debug.Log("Saved");
    }

    public void Load()
    {
        string data = SaveSystem.Load("DuelMastersCards");

        if (!string.IsNullOrWhiteSpace(data))
        {
            CardList cardList = JsonUtility.FromJson<CardList>(data);

            foreach (Card card in cardList.cards)
            {
                foreach (CardBaseData cardBase in cards)
                {
                    if (card.name == cardBase.name)
                    {
                        card.id = cardBase.id;
                        break;
                    }
                }
            }

            string saveFile = JsonUtility.ToJson(cardList);
            SaveSystem.Save(saveFile, "cardsDatabase");
        }
    }
}