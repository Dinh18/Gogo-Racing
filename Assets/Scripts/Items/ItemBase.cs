using UnityEngine;

public abstract class ItemBase : MonoBehaviour
{
    // Kích hoạt item. Truyền vào GameObject người dùng (xe) để xác định vị trí thả, bắn...
    public abstract void UseItem(GameObject user);
}
