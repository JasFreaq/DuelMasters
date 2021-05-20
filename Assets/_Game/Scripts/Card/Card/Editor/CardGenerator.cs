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
                                Spell spell = ScriptableObject.CreateInstance<Spell>();
                                spell = (Spell) SetupCard(spell, cardData, setName);
                                AssetDatabase.CreateAsset(spell, CardObjectsPath + $"{setName}/{cardData.name}.asset");
                                break;
                            case "Creature":
                            case "Evolution Creature":
                                Creature creature = ScriptableObject.CreateInstance<Creature>();
                                creature = SetupCreatureCard(creature, cardData, setName);
                                AssetDatabase.CreateAsset(creature, CardObjectsPath + $"{setName}/{cardData.name}.asset");
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

    private static Card SetupCard(Card card, CardReadDataFormat cardReadDataFormat, string setName)
    {
        card.Name = cardReadDataFormat.name;
        
        card.Set = CardParams.SetFromString(cardReadDataFormat.set);
        
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
        card.Civilization = civilization;

        card.Rarity = CardParams.RarityFromString(cardReadDataFormat.rarity);

        card.CardType = CardParams.CardTypeFromString(cardReadDataFormat.type);

        card.Cost = int.Parse(cardReadDataFormat.cost);
        
        card.ArtworkImage = Resources.Load<Sprite>($"{setName}/{cardReadDataFormat.id}");

        card.RulesText = cardReadDataFormat.text;

        card.FlavorText = cardReadDataFormat.flavor;

        return card;
    }
    
    private static Creature SetupCreatureCard(Creature creature, CardReadDataFormat cardReadDataFormat, string setName)
    {
        creature = (Creature) SetupCard(creature, cardReadDataFormat, setName);

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
        creature.Race = race;

        string power = cardReadDataFormat.power;
        if (power[power.Length - 1] == '+')
        {
            power = power.Substring(0, power.Length - 1);
        }
        creature.Power = int.Parse(power);

        return creature;
    }
}
