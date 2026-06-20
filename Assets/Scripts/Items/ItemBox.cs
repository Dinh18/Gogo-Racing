using System.Collections.Generic;
using UnityEngine;

public class ItemBox : MonoBehaviour
{
    public ItemIngameDataSO item;
    
    void Start()
    {
        if (DataManager.Instance != null && DataManager.Instance.GetAllItems().Length > 0)
        {
            int randomIndex = Random.Range(0, DataManager.Instance.GetAllItems().Length);
            item = DataManager.Instance.GetAllItems()[randomIndex];
        }
    }

    void OnTriggerEnter(Collider other)
    {
        PlayerItemController itemController = other.GetComponentInParent<PlayerItemController>();
        if (itemController != null)
        {
            if (item != null)
            {
                itemController.AddItem(item);
            }
            Destroy(gameObject);
        }
    }
}
