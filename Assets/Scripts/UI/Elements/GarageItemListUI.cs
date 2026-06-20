using UnityEngine;
using UnityEngine.UI;

public class GarageItemList : MonoBehaviour
{
    public void Setup(ItemDataSO currentEquippedItem)
    {
        foreach(Transform child in this.transform)
        {
            GarageItemUI item = child.GetComponent<GarageItemUI>(); 

            if (item != null && item.itemData != null)
            {
                if (item.itemData.itemID == currentEquippedItem.itemID)
                {
                    item.SelectedItem(); 
                }
                else
                {
                    item.UnselectedItem();
                }
            }
        }
    }
}
