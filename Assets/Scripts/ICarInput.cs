using UnityEngine;

public interface ICarInput
{
    public float MoveInput { get; }
    public float TurnInput { get; }
    public bool IsDrifting { get; }
    public bool IsBoosting { get; }
    public bool IsUsingItem { get; }
}
