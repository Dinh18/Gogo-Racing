using UnityEngine;

public class NitroItem : ItemBase
{
    public override void UseItem(GameObject user)
    {
        user.GetComponent<CarMovement>().Accelerate();
    }

}
