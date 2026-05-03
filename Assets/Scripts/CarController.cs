using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(ICarInput))]
public class CarController : MonoBehaviour
{
    
    private PlayerItemController itemController;
    private PlayerMovement playerMovement;
    private CarVisuals carVisuals;
    private ICarInput carInput;
    private PlayerDrift playerDrift;
    void Start()
    {
        itemController = GetComponent<PlayerItemController>();
        carInput = GetComponent<ICarInput>();

        playerMovement = GetComponent<PlayerMovement>();
        playerDrift = GetComponent<PlayerDrift>();

        carVisuals = GetComponent<CarVisuals>();
    }


    // Update is called once per frame
    void Update()
    {
        if(playerDrift != null)
        {
            playerDrift.HandleInput(carInput.TurnInput, carInput.IsDrifting, carInput.IsBoosting, playerMovement.isGrounded, playerMovement);
            playerDrift.HandleDriftCharge(itemController);
        }
        if(carInput.IsUsingItem)
        {
            itemController.UseItem();
        }
        if(carVisuals != null)
        {
            carVisuals.HandleVisualRotation(carInput.MoveInput, carInput.TurnInput, carInput.IsDrifting, playerDrift.GetDiftDirection(), playerMovement.isSpinning);
        }
    }
    void FixedUpdate()
    {
        if(playerMovement != null) playerMovement.Move(carInput);
    }

    public void HitBanana(float spinDuration)
    {
        if(playerMovement != null && !playerMovement.isSpinning)
        {
            StartCoroutine(SpinPOutRoutine(spinDuration));
        }
    }

    private IEnumerator SpinPOutRoutine(float duration)
    {
        playerMovement.isSpinning = true;
        yield return new WaitForSeconds(duration);
        playerMovement.isSpinning = false;
    }




    

    
}
