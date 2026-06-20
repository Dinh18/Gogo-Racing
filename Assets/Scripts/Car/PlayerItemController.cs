using UnityEngine;
using System;

public class PlayerItemController : MonoBehaviour
{
    [SerializeField] public ItemIngameDataSO item1;
    [SerializeField] public ItemIngameDataSO item2;

    // Gọi khi có thay đổi item (nhặt hoặc dùng). int = slot (1 hoặc 2), ItemIngameDataSO = dữ liệu (có thể null)
    public event Action<int, ItemIngameDataSO> OnItemChanged;

    public void AddItem(ItemIngameDataSO itemData)
    {
        if (item1 == null)
        {
            item1 = itemData;
            OnItemChanged?.Invoke(1, item1);
        }
        else if (item2 == null)
        {
            item2 = itemData;
            OnItemChanged?.Invoke(2, item2);
        }
    }

    public void UseItem()
    {
        if (item1 != null)
        {
            if (item1.prefab != null)
            {
                // Tạo ra prefab logic của item
                GameObject itemObj = Instantiate(item1.prefab, transform.position, transform.rotation);
                ItemBase itemBase = itemObj.GetComponent<ItemBase>();
                if (itemBase != null)
                {
                    itemBase.UseItem(this.gameObject);
                    
                    // Nếu item không tự destroy, ta destroy cái base sau 1s hoặc tùy logic. 
                    // Đối với BananaItem, nó sinh ra BananaPrefab rồi. Nên cái itemObj này chỉ là logic, ta destroy nó ngay.
                    Destroy(itemObj, 0.1f);
                }
                else
                {
                    Debug.LogWarning($"Prefab của {item1.itemName} không có script kế thừa từ ItemBase!");
                }
            }

            if(item2 != null)
            {
                item1 = item2;
                item2 = null;
                OnItemChanged?.Invoke(1, item1);
                OnItemChanged?.Invoke(2, null);
            }
            else
            {
                item1 = null;
                OnItemChanged?.Invoke(1, null);
            }
            

            
        }
    }
}
