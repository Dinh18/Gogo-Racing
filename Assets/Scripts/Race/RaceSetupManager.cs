using System.Collections.Generic;
using UnityEngine;

public class RaceSetupManager : MonoBehaviour
{
    [SerializeField] private List<Transform> spawnsPoints;
    // [SerializeField] private GameObject npcBasPrefab;

    public void Setup(int mapID)
{
    // Tạo biến tham chiếu cục bộ để code không bị dài dòng
    var dataMgr = DataManager.Instance;
    var allKarts = dataMgr.GetAllKarts();
    var allChars = dataMgr.GetAllCharacters();
    
    int lastRank = dataMgr.GetLastRank(mapID);
    int playerSpawnIndex = 0;

    if (lastRank == -1) 
    {
        playerSpawnIndex = spawnsPoints.Count - 1; // Lần đầu chơi thì ở vị trí cuối
    }
    else 
    {
        // Rank 1 -> index 0, Rank 2 -> index 1...
        playerSpawnIndex = Mathf.Clamp(lastRank - 1, 0, spawnsPoints.Count - 1);
    }
    
    int countNPC = 0;

    for (int i = 0; i < spawnsPoints.Count; i++)
    {
        bool isPlayer = (i == playerSpawnIndex);

        int kartID = isPlayer ? dataMgr.GetCurrentKartID() : dataMgr.GetNPCKart(mapID, countNPC);
        int charID = isPlayer ? dataMgr.GetCurrentCharacterID() : dataMgr.GetNPCCharacter(mapID, countNPC);

        KartDataSO kartData = dataMgr.GetCurrentItem(allKarts, kartID);
        CharacterSO charSO = dataMgr.GetCurrentItem(allChars, charID);

        GameObject carObj = Instantiate(kartData.ingamePrefab, spawnsPoints[i].position, spawnsPoints[i].rotation);
        CarController controller = carObj.GetComponent<CarController>();

        if (isPlayer)
        {
            carObj.tag = "Player";
            carObj.AddComponent<PlayerInputController>();
        }
        else
        {
            carObj.tag = "NPC";
            carObj.AddComponent<AIInputController>();
            countNPC++;
        }

        controller.Setup(charSO, kartData);
    }
}
}
