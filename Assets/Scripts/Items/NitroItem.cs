using System.Collections;
using UnityEngine;

public class NitroItem : ItemBase
{
    public override void UseItem(GameObject target)
    {
        StartCoroutine(ActiveNitroRoutine(target));
    }

    private IEnumerator ActiveNitroRoutine(GameObject target)
    {
        PlayerMovement playerMovement = target.GetComponent<PlayerMovement>();
        if(playerMovement != null)
        {
            playerMovement.SetMoveSpeed(1.5f);
            yield return new WaitForSeconds(2f); // Wait for 5 seconds
            playerMovement.SetMoveSpeed(1f); // Reset to default speed
        }
    }
}
