using UnityEngine;

public abstract class ItemBase : MonoBehaviour
{
    [SerializeField] protected string itemName;
    [SerializeField] protected string itemDescription;
    [SerializeField] protected Sprite itemIcon;

    public abstract void UseItem(GameObject target);
}
