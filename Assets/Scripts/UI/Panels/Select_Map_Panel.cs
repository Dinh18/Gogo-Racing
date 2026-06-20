using UnityEngine;

public class Select_Map_Panel : Panel
{
    [SerializeField] private Transform contentHolder;
    [SerializeField] private MapItemUI mapItemUI;

    public override void Setup()
    {
        // Xóa các UI rác (hoặc placeholder) cũ
        foreach (Transform child in contentHolder)
        {
            Destroy(child.gameObject);
        }

        var allMaps = DataManager.Instance.GetAllMaps();
        var sortedMaps = System.Linq.Enumerable.ToList(System.Linq.Enumerable.OrderBy(allMaps, m => m.mapID));

        for(int i = 0; i < sortedMaps.Count; i++)
        {
            var map = sortedMaps[i];
            Debug.Log(map.mapName);
            MapItemUI mapItem = Instantiate(mapItemUI,contentHolder,false);
            
            bool isUnlocked = (i <= DataManager.Instance.GetUnlockedMapIndex());
            
            mapItem.Setup(map, isUnlocked);
        }
    }
}
        
