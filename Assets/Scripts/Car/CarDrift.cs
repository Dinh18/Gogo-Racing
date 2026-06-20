using System;
using UnityEngine;

public class CarDrift : MonoBehaviour
{
    [Header("Drift Setting")]
    [SerializeField] private float steerForce = 50f;
    [SerializeField] private float maxBoostCharge = 100f;
    [SerializeField] private float chargeRate = 25f;
    [SerializeField]private float currentBoostCharge = 0f;
    public bool isDrifting = false;
    private float driftDirection = 0;
    public event Action<bool> OnDriftStateChange;
    [SerializeField] private ItemIngameDataSO nitroItemData;
    // private bool isGrounded;

    public void ApplyStats(float handlingStat, float driftStat)
    {
        steerForce = Mathf.Lerp(30f, 70f, handlingStat / 100f);
        chargeRate = Mathf.Lerp(15f, 35f, driftStat / 100f);
    }

    public void HandleInput(float turnInput, bool isDrifting, bool isGrounded, CarMovement carMovement)
    {
        // Debug.Log("Handle Drift Input: " + turnInput + ", " + isDrifting + ", " + isGrounded);

        if(isDrifting && isGrounded && turnInput != 0)
        {
            this.isDrifting = true;
            OnDriftStateChange?.Invoke(this.isDrifting);
            if(driftDirection == 0) driftDirection = Mathf.Sign(turnInput);
        }
        else if(!isDrifting || !isGrounded || turnInput == 0)
        {
            this.isDrifting = false;
            OnDriftStateChange?.Invoke(this.isDrifting);
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
                
                // Khi vừa nạp đầy
                if (currentBoostCharge >= maxBoostCharge)
                {
                    itemController.AddItem(nitroItemData); // Thêm Nitro vào túi đồ
                    currentBoostCharge = 0f; // Reset lại vạch sạc từ đầu
                }
            }
        }
        else
        {
            // Nếu ngừng drift giữa chừng hoặc sau khi đã nhận item thì reset vạch sạc
            currentBoostCharge = 0f; 
        }
    }

    public float GetDiftDirection()
    {
        return driftDirection;
    }
}
