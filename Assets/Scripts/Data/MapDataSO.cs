using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MapData_", menuName = "Scriptable Objects/MapData")]
public class MapDataSO : ScriptableObject
{
    [Header("Thông tin cơ bản (Dành cho UI)")]
    public int mapID;
    public string mapName;
    public Sprite mapThumnail;
    [TextArea] public string description; 
    [Header("Thông số hệ thống (Dành cho Code)")]
    public string sceneName;
    public int totalLaps = 3;
    [Header("NPC Settings")]
    public List<NPCSetting> npcSettings;
    [Header("Âm thanh & Hiệu ứng")]
    public AudioClip mapBGM;
}
[System.Serializable]
public class NPCSetting
{
    public int kartID;
    public int characterID;
}
