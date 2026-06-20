using UnityEngine;
using System.Collections;

public class BananaTrap : MonoBehaviour
{
    [Header("Banana Trap Settings")]
    [SerializeField] private float spinDuration = 1.5f;
    [SerializeField] private float armDelay = 0.5f; // Thời gian chờ trước khi chuối có tác dụng

    private Collider trapCollider;

    void Start()
    {
        trapCollider = GetComponent<Collider>();
        if (trapCollider != null)
        {
            trapCollider.enabled = false; // Tắt va chạm lúc mới đẻ ra
            StartCoroutine(ArmTrapRoutine());
        }
    }

    private IEnumerator ArmTrapRoutine()
    {
        yield return new WaitForSeconds(armDelay);
        if (trapCollider != null)
        {
            trapCollider.enabled = true; // Bật lại va chạm sau khi xe đã chạy qua
        }
    }

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
