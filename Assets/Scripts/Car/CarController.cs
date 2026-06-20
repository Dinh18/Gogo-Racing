using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class CarController : MonoBehaviour
{
    
    private PlayerItemController itemController;
    private CarMovement playerMovement;
    private CarVisuals carVisuals;
    private ICarInput carInput;
    private CarDrift playerDrift;
    [SerializeField] private Transform charModel;
    private CharacterSO charSO;
    private KartDataSO kartSO;
    // void Start()
    // {
        
    // }

    public CharacterSO GetCharacterSO() => charSO;
    public KartDataSO GetKartDataSO() => kartSO;


    // Update is called once per frame
    void Update()
    {
        // if (carInput == null) carInput = GetComponent<ICarInput>();
        if (carInput == null) return;

        if(playerDrift != null)
        {
            playerDrift.HandleInput(carInput.TurnInput, carInput.IsDrifting, playerMovement.isGrounded, playerMovement);
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

    public void Setup(CharacterSO charSO, KartDataSO kartSO)
    {
        this.charSO = charSO;
        this.kartSO = kartSO;
        foreach(Transform child in charModel)
        {
            Destroy(child.gameObject);
        }
        GameObject character = Instantiate(charSO.ingamePrefab,charModel);
        // if(carVisuals == null) carVisuals = GetComponent<CarVisuals>();
        itemController = GetComponent<PlayerItemController>();
        carInput = GetComponent<ICarInput>();

        playerMovement = GetComponent<CarMovement>();
        playerDrift = GetComponent<CarDrift>();

        if(carVisuals == null) carVisuals = GetComponent<CarVisuals>();
        carVisuals.Setup(character.transform);

        if (playerMovement != null) playerMovement.ApplyStats(kartSO.speedStat, kartSO.accelerationStat);
        if (carVisuals != null) carVisuals.ApplyStats(kartSO.handlingStat);
        if (playerDrift != null) playerDrift.ApplyStats(kartSO.handlingStat, kartSO.driftStat);
    }
    void FixedUpdate()
    {
        if (carInput == null) carInput = GetComponent<ICarInput>();
        if (carInput == null) return;
        
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
