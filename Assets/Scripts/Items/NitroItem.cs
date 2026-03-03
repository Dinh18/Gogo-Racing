using System.Collections;
using UnityEngine;

public class NitroItem : ItemBase
{
    public override void UseItem(GameObject target)
    {
        ActiveNitro(target);
        Destroy(this.gameObject);
    }

    private void ActiveNitro(GameObject target)
    {
        PlayerMovement playerMovement = target.GetComponent<PlayerMovement>();
        if(playerMovement != null)
        {
            playerMovement.Accelerate(1.5f, 2f); // 1.5x speed for 2 seconds
        }
    }
}
