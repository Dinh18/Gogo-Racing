using UnityEngine;

public class PlayerDrift : MonoBehaviour
{
    [Header("Drift Setting")]
    [SerializeField] private float steerForce = 50f;
    [SerializeField] private float maxBoostCharge = 100f;
    [SerializeField] private float chargeRate = 25f;
    [SerializeField]private float currentBoostCharge = 0f;
    public bool isDrifting = false;
    private float driftDirection = 0;
    // private bool isGrounded;

    public void HandleInput(float turnInput, bool isDrifting, bool isGrounded)
    {
        // Debug.Log("Handle Drift Input: " + turnInput + ", " + isDrifting + ", " + isGrounded);

        if(isDrifting && isGrounded && turnInput != 0)
        {
            this.isDrifting = true;
            if(driftDirection == 0) driftDirection = Mathf.Sign(turnInput);
        }
        else if(!isDrifting || !isGrounded || turnInput == 0)
        {
            this.isDrifting = false;
            driftDirection = 0;
        }
    }

    public void HandleDriftCharge(PlayerItemController itemController)
    {
        if(isDrifting)
        {
            if(currentBoostCharge < maxBoostCharge)
            {
                currentBoostCharge += chargeRate * Time.deltaTime;
                currentBoostCharge = Mathf.Clamp(currentBoostCharge, 0, maxBoostCharge);
            }
            else
            {
                GameObject nitroItemPrefab = Resources.Load<GameObject>(Constants.NITRO_ITEM_PREFAB_PATH);
                Instantiate(nitroItemPrefab, itemController.ItemsHolder.transform);
                itemController.AddItem(nitroItemPrefab);
            }
        }
    }

    public float GetDiftDirection()
    {
        return driftDirection;
    }
}
