using UnityEngine;

public class OutOfBoundsTerrain : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        CarProgress car = collision.gameObject.GetComponentInParent<CarProgress>();
        if (car != null && !car.isEliminated)
        {
            RaceManager.Instance.EliminateCar(car);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        CarProgress car = other.gameObject.GetComponentInParent<CarProgress>();
        if (car != null && !car.isEliminated)
        {
            RaceManager.Instance.EliminateCar(car);
        }
    }
}
