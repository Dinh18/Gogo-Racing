using UnityEngine;

[CreateAssetMenu(fileName = "ItemIngameDataSO", menuName = "Scriptable Objects/ItemIngameDataSO")]
public class ItemIngameDataSO : ScriptableObject
{
    public int itemID;
    public string itemName;
    public string description;
    public Sprite icon;
    public GameObject prefab;
}
