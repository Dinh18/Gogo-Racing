using System.Collections.Generic;
using UnityEngine;

public class ItemBox : MonoBehaviour
{
    public List<GameObject> items;
    public GameObject item;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        int randomIndex = Random.Range(0, items.Count);
        item = items[randomIndex];
    }
    void OnTriggerEnter(Collider other)
    {
        PlayerItemController itemController = other.GetComponentInParent<PlayerItemController>();
        if (itemController != null)
        {
            itemController.AddItem(item);
            Destroy(gameObject);
        }
    }
}
