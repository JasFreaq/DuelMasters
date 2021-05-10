using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class CardGenerator : EditorWindow
{
    private static string CardsDatabasePath = "Assets/_Game/Data/cardsDatabase.json";
    private static string CardObjectsPath = "Assets/_Game/Cards/Resources/";

    [MenuItem("Window/Card Generator")]
    public static void ShowEditorWindow()
    {
        GetWindow(typeof(CardGenerator), true, "Card Generator");

        //CardsDatabasePath = Application.dataPath + "/_Game/Data/cardsDatabase.json";
        //CardObjectsPath = Application.dataPath + "/_Game/Cards/Resources/";
    }

    private void OnGUI()
    {
        if (GUILayout.Button("Generate"))
        {
            if (File.Exists(CardsDatabasePath))
            {
                string database = File.ReadAllText(CardsDatabasePath);
                if (!string.IsNullOrWhiteSpace(database))
                {
                    CardReadDataList cardReadDataList = JsonUtility.FromJson<CardReadDataList>(database);
                    
                    string setName = "";

                    foreach (CardReadDataFormat cardData in cardReadDataList.cards)
                    {
                        if (cardData.set != setName)
                        {
                            setName = cardData.set;

                            string setPath = CardObjectsPath + $"{setName}/";
                            if (!Directory.Exists(setPath))
                            {
                                Directory.CreateDirectory(setPath);
                            }
                        }

                        switch (cardData.type)
                        {
                            case "Spell":
                                SpellData spellData = ScriptableObject.CreateInstance<SpellData>();
                                spellData = (SpellData) SetupCard(spellData, cardData, setName);
                                AssetDatabase.CreateAsset(spellData, CardObjectsPath + $"{setName}/{cardData.name}.asset");
                                break;
                            case "Creature":
                            case "Evolution Creature":
                                CreatureData creatureData = ScriptableObject.CreateInstance<CreatureData>();
                                creatureData = SetupCreatureCard(creatureData, cardData, setName);
                                AssetDatabase.CreateAsset(creatureData, CardObjectsPath + $"{setName}/{cardData.name}.asset");
                                break;
                        }
                    }

                    AssetDatabase.SaveAssets();
                }
            }
            else
                Debug.LogError("Missing 'cardsDatabase' at path location.");
        }
    }

    private static CardData SetupCard(CardData cardData, CardReadDataFormat cardReadDataFormat, string setName)
    {
        cardData.Name = cardReadDataFormat.name;
        
        cardData.Set = CardParams.SetFromString(cardReadDataFormat.set);
        
        CardParams.Civilization[] civilization;
        if (!String.IsNullOrWhiteSpace(cardReadDataFormat.civilization))
        {
            civilization = new CardParams.Civilization[1];
            civilization[0] = CardParams.CivilizationFromString(cardReadDataFormat.civilization);
        }
        else
        {
            civilization = new CardParams.Civilization[2];
            for (int i = 0; i < 2; i++)
            {
                civilization[i] = CardParams.CivilizationFromString(cardReadDataFormat.civilizations[i]);
            }
        }
        cardData.Civilization = civilization;

        cardData.Rarity = CardParams.RarityFromString(cardReadDataFormat.rarity);

        cardData.CardType = CardParams.CardTypeFromString(cardReadDataFormat.type);

        cardData.Cost = int.Parse(cardReadDataFormat.cost);
        
        cardData.ArtworkImage = Resources.Load<Sprite>($"{setName}/{cardReadDataFormat.id}");

        cardData.RulesText = cardReadDataFormat.text;

        cardData.FlavorText = cardReadDataFormat.flavor;

        return cardData;
    }
    
    private static CreatureData SetupCreatureCard(CreatureData creatureData, CardReadDataFormat cardReadDataFormat, string setName)
    {
        creatureData = (CreatureData) SetupCard(creatureData, cardReadDataFormat, setName);

        CardParams.Race[] race;
        if (!String.IsNullOrWhiteSpace(cardReadDataFormat.race))
        {
            race = new CardParams.Race[1];
            race[0] = CardParams.RaceFromString(cardReadDataFormat.race);
        }
        else
        {
            race = new CardParams.Race[2];
            for (int i = 0; i < 2; i++)
            {
                race[i] = CardParams.RaceFromString(cardReadDataFormat.races[i]);
            }
        }
        creatureData.Race = race;

        string power = cardReadDataFormat.power;
        if (power[power.Length - 1] == '+')
        {
            power = power.Substring(0, power.Length - 1);
        }
        creatureData.Power = int.Parse(power);

        return creatureData;
    }
}
