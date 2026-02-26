using UnityEngine;

public class PlayerItemController : MonoBehaviour
{
    [SerializeField] public GameObject ItemsHolder;
    [SerializeField] private GameObject itemHolder1;
    [SerializeField] private GameObject itemHolder2;
    [SerializeField] private GameObject item1;
    [SerializeField] private GameObject item2;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddItem(GameObject itemPrefab)
    {
        if(item1 == null)
        {
            item1 = Instantiate(itemPrefab, itemHolder1.transform);
        }
        else if(item2 == null)
        {
            item2 = Instantiate(itemPrefab, itemHolder2.transform);
        }
    }

    public void UseItem()
    {
        if(item1 != null)
        {
            ItemBase item = item1.GetComponent<ItemBase>();
            if(item != null)
            {
                item.UseItem(this.gameObject);
                Destroy(item1);
                item1 = null;
            }
        }
    }
}
