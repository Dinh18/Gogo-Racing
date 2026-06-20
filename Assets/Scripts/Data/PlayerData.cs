using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class MapProgress
{
    public int mapID;
    public int lastRank;
    public int bestRank;
}

[System.Serializable]
public class PlayerData
{
    public string namePlayer = "";
    public int kartID;
    public int characterID;
    public int unlockedMapIndex = 0;
    public int unlockedKartIndex = 0;
    public int unlockedCharacterIndex = 0;
    public List<MapProgress> mapProgresses = new List<MapProgress>();

    public PlayerData(string namePlayer, int kartID, int charID)
    {
        this.namePlayer = namePlayer;
        this.kartID = kartID;
        this.characterID = charID;
        this.unlockedMapIndex = 0;
        this.unlockedKartIndex = 0;
        this.unlockedCharacterIndex = 0;
        this.mapProgresses = new List<MapProgress>();
    }
}
