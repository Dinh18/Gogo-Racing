using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputController : MonoBehaviour, ICarInput
{
    public float MoveInput {get; private set;}

    public float TurnInput {get; private set;}

    public bool IsDrifting {get; private set;}

    public bool IsUsingItem {get; private set;}

    public bool IsBoosting {get; private set;}

    // Update is called once per frame
    void Update()
    {
        MoveInput = Input.GetAxis("Vertical");
        TurnInput = Input.GetAxis("Horizontal");
        IsDrifting = Input.GetKey(KeyCode.LeftControl);
        IsUsingItem = Input.GetKey(KeyCode.U);
        IsBoosting = Input.GetKey(KeyCode.Space);
    }
}
