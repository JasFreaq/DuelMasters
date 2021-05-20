using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class CardParams
{
    #region Set

    [SerializeField]
    public enum Set
    {
        BaseSet = 1,
        EvoCrushinatorsOfDoom = 2,
        RampageOfTheSuperWarriors = 3,
        ShadowclashOfBlindingNight = 4,
        SurvivorsOfTheMegapocalypse = 5,
        StompATronsOfInvincibleWrath = 6,
        ThunderchargeOfUltraDestruction = 7,
        EpicDragonsOfHyperchaos = 8,
        FatalBroodOfInfiniteRuin = 9,
        ShockwavesOfTheShatteredRainbow = 10,
        BlastOSplosionOfGiganticRage = 11,
        ThrashOfTheHybridMegacreatures = 12
    }

    public static Set SetFromString(string setStr)
    {
        switch (setStr)
        {
            case "DM-01 Base Set":
                return Set.BaseSet;
            case "DM-02 Evo-Crushinators of Doom":
                return Set.EvoCrushinatorsOfDoom;
            case "DM-03 Rampage of the Super Warriors":
                return Set.RampageOfTheSuperWarriors;
            case "DM-04 Shadowclash of Blinding Night":
                return Set.ShadowclashOfBlindingNight;
            case "DM-05 Survivors of the Megapocalypse":
                return Set.SurvivorsOfTheMegapocalypse;
            case "DM-06 Stomp-A-Trons of Invincible Wrath":
                return Set.StompATronsOfInvincibleWrath;
            case "DM-07 Thundercharge of Ultra Destruction":
                return Set.ThunderchargeOfUltraDestruction;
            case "DM-08 Epic Dragons of Hyperchaos":
                return Set.EpicDragonsOfHyperchaos;
            case "DM-09 Fatal Brood of Infinite Ruin":
                return Set.FatalBroodOfInfiniteRuin;
            case "DM-10 Shockwaves of the Shattered Rainbow":
                return Set.ShockwavesOfTheShatteredRainbow;
            case "DM-11 Blastosplosion of Gigantic Rage":
                return Set.BlastOSplosionOfGiganticRage;
            case "DM-12 Thrash of the Hybrid Megacreatures":
                return Set.ThrashOfTheHybridMegacreatures;
            default:
                return 0;
        }
    }

    public static string StringFromSet(Set set)
    {
        switch (set)
        {
            case Set.BaseSet:
                return "DM-01 Base Set";
            case Set.EvoCrushinatorsOfDoom:
                return "DM-02 Evo-Crushinators of Doom";
            case Set.RampageOfTheSuperWarriors:
                return "DM-03 Rampage of the Super Warriors";
            case Set.ShadowclashOfBlindingNight:
                return "DM-04 Shadowclash of Blinding Night";
            case Set.SurvivorsOfTheMegapocalypse:
                return "DM-05 Survivors of the Megapocalypse";
            case Set.StompATronsOfInvincibleWrath:
                return "DM-06 Stomp-A-Trons of Invincible Wrath";
            case Set.ThunderchargeOfUltraDestruction:
                return "DM-07 Thundercharge of Ultra Destruction";
            case Set.EpicDragonsOfHyperchaos:
                return "DM-08 Epic Dragons of Hyperchaos";
            case Set.FatalBroodOfInfiniteRuin:
                return "DM-09 Fatal Brood of Infinite Ruin";
            case Set.ShockwavesOfTheShatteredRainbow:
                return "DM-10 Shockwaves of the Shattered Rainbow";
            case Set.BlastOSplosionOfGiganticRage:
                return "DM-11 Blastosplosion of Gigantic Rage";
            case Set.ThrashOfTheHybridMegacreatures:
                return "DM-12 Thrash of the Hybrid Megacreatures";
            default:
                return null;
        }
    }

    #endregion

    #region Civilization

    [System.Serializable]
    public enum Civilization
    {
        Darkness = 1,
        Fire = 2,
        Nature = 3,
        Light = 4,
        Water = 5
    }
    
    public static Civilization CivilizationFromString(string civStr)
    {
        switch (civStr)
        {
            case "Darkness":
                return Civilization.Darkness;
            case "Fire":
                return Civilization.Fire;
            case "Nature":
                return Civilization.Nature;
            case "Light":
                return Civilization.Light;
            case "Water":
                return Civilization.Water;
            default:
                return 0;
        }
    }

    public static bool IsCivilizationEqual(Civilization[] civilizationA, Civilization[] civilizationB)
    {
        if (civilizationA.Length == civilizationB.Length)
        {
            for (int i = 0, n = civilizationA.Length; i < n; i++)
            {
                if (civilizationA[i] != civilizationB[i])
                {
                    return false;
                }
            }
        }
        else
            return false;

        return true;
    }

    #endregion

    #region Rarity

    [System.Serializable]
    public enum Rarity
    {
        Common = 1,
        Uncommon = 2,
        Rare = 3,
        VeryRare = 4,
        SuperRare = 5
    }

    public static Rarity RarityFromString(string rarityStr)
    {
        switch (rarityStr)
        {
            case "Common":
                return Rarity.Common;
            case "Uncommon":
                return Rarity.Uncommon;
            case "Rare":
                return Rarity.Rare;
            case "Very Rare":
                return Rarity.VeryRare;
            case "Super Rare":
                return Rarity.SuperRare;
            default:
                return 0;
        }
    }

    public static string StringFromRarity(Rarity rarity)
    {
        switch (rarity)
        {
            case Rarity.Common:
            case Rarity.Uncommon:
            case Rarity.Rare:
                return rarity.ToString();
            case Rarity.VeryRare:
                return "Very Rare";
            case Rarity.SuperRare:
                return "Super Rare";
            default:
                return null;
        }
    }

    #endregion

    #region CardType

    [System.Serializable]
    public enum CardType
    {
        Spell = 1,
        Creature = 2,
        EvolutionCreature = 3
    }

    public static CardType CardTypeFromString(string typeStr)
    {
        switch (typeStr)
        {
            case "Spell":
                return CardType.Spell;
            case "Creature":
                return CardType.Creature;
            case "Evolution Creature":
                return CardType.EvolutionCreature;
            default:
                return 0;
        }
    }

    public static string StringFromCardType(CardType cardType)
    {
        switch (cardType)
        {
            case CardType.Spell:
            case CardType.Creature:
                return cardType.ToString();
            case CardType.EvolutionCreature:
                return "Evolution Creature";
            default:
                return null;
        }
    }

    #endregion

    #region Races

    [System.Serializable]
    public enum Race
    {
        //Darkness
        BrainJacker = 1,
        Chimera = 2,
        DarkLord = 3,
        DeathPuppet = 4,
        DemonCommand = 5,
        DevilMask = 6,
        Ghost = 7,
        Hedrian = 8,
        LivingDead = 9,
        PandorasBox = 10,
        ParasiteWorm = 11,
        ZombieDragon = 12,

        //Fire
        ArmoredDragon = 13,
        ArmoredWyvern = 14,
        Armorloid = 15,
        Dragonoid = 16,
        DuneGecko = 17,
        FireBird = 18,
        Human = 19,
        MachineEater = 20,
        MeltWarrior = 21,
        RockBeast = 22,
        VolcanoDragon = 23,
        Xenoparts = 24,

        //Nature
        BalloonMushroom = 25,
        BeastFolk = 26,
        ColonyBeetle = 27,
        EarthDragon = 28,
        Giant = 29,
        GiantInsect = 30,
        HornedBeast = 31,
        MysteryTotem = 32,
        SnowFaerie = 33,
        TreeFolk = 34,
        WildVeggies = 35,

        //Light
        AngelCommand = 36,
        Berserker = 37,
        Gladiator = 38,
        Guardian = 39,
        Initiate = 40,
        LightBringer = 41,
        MechaDelSol = 42,
        MechaThunder = 43,
        RainbowPhantom = 44,
        Soltrooper = 45,
        StarlightTree = 46,

        //Water
        CyberCluster = 47,
        CyberLord = 48,
        CyberMoon = 49,
        CyberVirus = 50,
        EarthEater = 51,
        Fish = 52,
        GelFish = 53,
        Leviathan = 54,
        LiquidPeople = 55,
        Merfolk = 56,
        SeaHacker = 57,

        //Multi
        Naga = 58,
        Pegasus = 59,
        Phoenix = 60,
        Starnoid = 61,
        SpiritQuartz = 62,
        Survivor = 63
    }

    public static Race RaceFromString(string raceStr)
    {
        switch (raceStr)
        {
                case "Brain Jacker":
                    return Race.BrainJacker;
                case "Chimera":
                    return Race.Chimera;
                case "Dark Lord":
                    return Race.DarkLord;
                case "Death Puppet":
                    return Race.DeathPuppet;
                case "Demon Command":
                    return Race.DemonCommand;
                case "Devil Mask":
                    return Race.DevilMask;
                case "Ghost":
                    return Race.Ghost;
                case "Hedrian":
                    return Race.Hedrian;
                case "Living Dead":
                    return Race.LivingDead;
                case "Pandoras Box":
                    return Race.PandorasBox;
                case "Parasite Worm":
                    return Race.ParasiteWorm;
                case "Zombie Dragon":
                    return Race.ZombieDragon;
                case "Armored Dragon":
                    return Race.ArmoredDragon;
                case "Armored Wyvern":
                    return Race.ArmoredWyvern;
                case "Armorloid":
                    return Race.Armorloid;
                case "Dragonoid":
                    return Race.Dragonoid;
                case "Dune Gecko":
                    return Race.DuneGecko;
                case "Fire Bird":
                    return Race.FireBird;
                case "Human":
                    return Race.Human;
                case "Machine Eater":
                    return Race.MachineEater;
                case "Melt Warrior":
                    return Race.MeltWarrior;
                case "Rock Beast":
                    return Race.RockBeast;
                case "Volcano Dragon":
                    return Race.VolcanoDragon;
                case "Xenoparts":
                    return Race.Xenoparts;
                case "Balloon Mushroom":
                    return Race.BalloonMushroom;
                case "Beast Folk":
                    return Race.BeastFolk;
                case "Colony Beetle":
                    return Race.ColonyBeetle;
                case "Earth Dragon":
                    return Race.EarthDragon;
                case "Giant":
                    return Race.Giant;
                case "Giant Insect":
                    return Race.GiantInsect;
                case "Horned Beast":
                    return Race.HornedBeast;
                case "Mystery Totem":
                    return Race.MysteryTotem;
                case "Snow Faerie":
                    return Race.SnowFaerie;
                case "Tree Folk":
                    return Race.TreeFolk;
                case "Wild Veggies":
                    return Race.WildVeggies;
                case "Angel Command":
                    return Race.AngelCommand;
                case "Berserker":
                    return Race.Berserker;
                case "Gladiator":
                    return Race.Gladiator;
                case "Guardian":
                    return Race.Guardian;
                case "Initiate":
                    return Race.Initiate;
                case "Light Bringer":
                    return Race.LightBringer;
                case "Mecha del Sol":
                    return Race.MechaDelSol;
                case "Mecha Thunder":
                    return Race.MechaThunder;
                case "Rainbow Phantom":
                    return Race.RainbowPhantom;
                case "Soltrooper":
                    return Race.Soltrooper;
                case "Starlight Tree":
                    return Race.StarlightTree;
                case "Cyber Cluster":
                    return Race.CyberCluster;
                case "Cyber Lord":
                    return Race.CyberLord;
                case "Cyber Moon":
                    return Race.CyberMoon;
                case "Cyber Virus":
                    return Race.CyberVirus;
                case "Earth Eater":
                    return Race.EarthEater;
                case "Fish":
                    return Race.Fish;
                case "Gel Fish":
                    return Race.GelFish;
                case "Leviathan":
                    return Race.Leviathan;
                case "Liquid People":
                    return Race.LiquidPeople;
                case "Merfolk":
                    return Race.Merfolk;
                case "SeaHacker":
                    return Race.SeaHacker;
                case "Naga":
                    return Race.Naga;
                case "Pegasus":
                    return Race.Pegasus;
                case "Phoenix":
                    return Race.Phoenix;
                case "Starnoid":
                    return Race.Starnoid;
                case "Spirit Quartz":
                    return Race.SpiritQuartz;
                case "Survivor":
                    return Race.Survivor;
                default:
                    return 0;
        }
    }

    public static string StringFromRace(Race race)
    {
        switch (race)
        {
            case Race.PandorasBox:
                return "Pandora's Box";
            case Race.MechaDelSol:
                return "Mecha del Sol";
            default:
                string raceName = race.ToString();
                raceName = string.Concat(raceName.Select(x => Char.IsUpper(x) ? " " + x : x.ToString())).TrimStart(' ');
                return raceName;
        }
    }
    
    public static string PluralStringFromRace(Race race)
    {
        switch (race)
        {
            case Race.Xenoparts:
            case Race.BeastFolk:
            case Race.TreeFolk:
            case Race.WildVeggies:
            case Race.Merfolk:
            case Race.LiquidPeople:
                return StringFromRace(race);
            case Race.PandorasBox:
            case Race.CyberVirus:
            case Race.Fish:
            case Race.GelFish:
                return StringFromRace(race) + "es";
            default:
                return StringFromRace(race) + "s";
        }
    }

    #endregion
}
