using System;
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
            carVisuals.HandleVisualRotation(carInput.MoveInput, carInput.TurnInput, playerDrift.isDrifting, playerDrift.GetDiftDirection());
        }
    }
    void FixedUpdate()
    {
        playerMovement.Move(carInput);
    }




    

    
}
