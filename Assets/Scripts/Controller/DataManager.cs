using UnityEngine;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance;
    [SerializeField] private KartDataSO[] allKarts;
    [SerializeField] private CharacterSO[] allCharacters;
    [SerializeField] private MapDataSO[] allMaps;
    [SerializeField] private ItemIngameDataSO[] allItems;
    private PlayerData playerData;
    private string savePath;

    void Awake()
    {
        Instance = this;
        savePath = Application.persistentDataPath + "/playerData.json";
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        allCharacters = Resources.LoadAll<CharacterSO>(Constants.CHARACTERSO_PATH);
        System.Array.Sort(allCharacters, (a, b) => a.itemID.CompareTo(b.itemID));
        allKarts =  Resources.LoadAll<KartDataSO>(Constants.KARTSO_PATH);
        System.Array.Sort(allKarts, (a, b) => a.itemID.CompareTo(b.itemID));
        allMaps = Resources.LoadAll<MapDataSO>(Constants.MAPDATASO_PATH);
        System.Array.Sort(allMaps, (a, b) => a.mapID.CompareTo(b.mapID));
        allItems = Resources.LoadAll<ItemIngameDataSO>(Constants.ITEMINGAMEDATASO_PATH);
        LoadPlayerData();
    }

    public int GetNPCKart(int mapID, int index)
    {
        foreach(MapDataSO map in allMaps)
        {
            if(map.mapID == mapID)
            {
                return map.npcSettings[index].kartID;
            }
        }
        Debug.Log($"Khong tim thay npc kart co ID: {index} cho map ID: {mapID}");
        return 0;
    }

    public int GetNPCCharacter(int mapID, int index)
    {
        foreach(MapDataSO map in allMaps)
        {
            if(map.mapID == mapID)
            {
                return map.npcSettings[index].characterID;
            }
        }
        Debug.Log($"Khong tim thay npc character co ID: {index} cho map ID: {mapID}");
        return 0;
    }

    public KartDataSO[] GetAllKarts() => allKarts;
    public CharacterSO[] GetAllCharacters() => allCharacters;
    public MapDataSO[] GetAllMaps() => allMaps;
    public ItemIngameDataSO[] GetAllItems() => allItems;
    public int GetCurrentKartID() => playerData.kartID;
    public int GetCurrentCharacterID() => playerData.characterID;
    
    public int GetUnlockedMapIndex() => playerData.unlockedMapIndex;
    public int GetUnlockedKartIndex() => playerData.unlockedKartIndex;
    public int GetUnlockedCharacterIndex() => playerData.unlockedCharacterIndex;

    public int GetLastRank(int mapID)
    {
        if (playerData.mapProgresses == null) return -1;
        foreach (var progress in playerData.mapProgresses)
        {
            if (progress.mapID == mapID) return progress.lastRank;
        }
        return -1; // -1 means never played
    }

    public int GetBestRank(int mapID)
    {
        if (playerData.mapProgresses == null) return -1;
        foreach (var progress in playerData.mapProgresses)
        {
            if (progress.mapID == mapID) return progress.bestRank;
        }
        return -1;
    }

    public void SaveMapResult(int mapID, int rank, int mapIndex)
    {
        if (playerData.mapProgresses == null)
            playerData.mapProgresses = new System.Collections.Generic.List<MapProgress>();

        bool found = false;
        foreach (var progress in playerData.mapProgresses)
        {
            if (progress.mapID == mapID)
            {
                progress.lastRank = rank;
                if (rank > 0 && (progress.bestRank <= 0 || rank < progress.bestRank))
                {
                    progress.bestRank = rank;
                }
                found = true;
                break;
            }
        }

        if (!found)
        {
            playerData.mapProgresses.Add(new MapProgress { mapID = mapID, lastRank = rank, bestRank = rank });
        }

        if (rank == 1 && mapIndex >= playerData.unlockedMapIndex)
        {
            if (mapIndex < allMaps.Length - 1)
            {
                playerData.unlockedMapIndex = mapIndex + 1;
            }
            if (playerData.unlockedKartIndex < allKarts.Length - 1)
            {
                playerData.unlockedKartIndex++;
            }
            if (playerData.unlockedCharacterIndex < allCharacters.Length - 1)
            {
                playerData.unlockedCharacterIndex++;
            }
        }

        SavePlayerData();
    }

    public void ChangeKart(KartDataSO kartData)
    {
        if(kartData.itemID == playerData.kartID) return;
        playerData.kartID = kartData.itemID;
        SavePlayerData();
    }

    public void ChangeChar(CharacterSO charData)
    {
        if(charData.itemID == playerData.characterID) return;
        playerData.characterID = charData.itemID;
        SavePlayerData();
    }

    [ContextMenu("Save Player Data")]
    public void SavePlayerData()
    {
        string json = JsonUtility.ToJson(playerData);
        System.IO.File.WriteAllText(savePath, json);
        Debug.Log("Đã lưu dữ liệu người chơi vào: " + savePath);
    }

    [ContextMenu("Delete Player Data")]
    public void DeletePlayerData()
    {
        if (System.IO.File.Exists(savePath))
        {
            System.IO.File.Delete(savePath);
            Debug.Log("Đã xóa file dữ liệu người chơi: " + savePath);
        }
        else
        {
            Debug.Log("Không tìm thấy file dữ liệu người chơi để xóa!");
        }

        // Reset data trên RAM
        playerData = new PlayerData("Player", 0, 0);
        Debug.Log("Đã reset dữ liệu người chơi về mặc định!");
    }

    public void LoadPlayerData()
    {
        if (System.IO.File.Exists(savePath))
        {
            string json = System.IO.File.ReadAllText(savePath);
            playerData = JsonUtility.FromJson<PlayerData>(json);
            Debug.Log("Đã tải dữ liệu người chơi thành công.");
        }
        else
        {
            playerData = new PlayerData("Player", 0, 0);
            SavePlayerData();
        }
    }


    public T GetCurrentItem<T>(T[] allItems, int targetID) where T : ItemDataSO
    {
        foreach(var item in allItems)
        {
            if(item.itemID == targetID) return item;
        }
        Debug.Log($"Khong tim thay item co ID: {targetID}");
        return null;
    }
}
