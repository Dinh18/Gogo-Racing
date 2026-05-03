using UnityEngine;

public class BananaTrap : MonoBehaviour
{
    [Header("Banana Trap Settings")]
    [SerializeField] private float spinDuration = 1.5f;
    void OnTriggerEnter(Collider other)
    {
        CarController carController = other.GetComponentInParent<CarController>();
        if(carController != null)
        {
            carController.HitBanana(spinDuration);
            Destroy(gameObject);
        }
    }
}
