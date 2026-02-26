using UnityEngine;

public class CarVisuals : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private float driftSlipAngle = 30f;
    [SerializeField] private float turnSpeed = 100f;
    [SerializeField] private Transform carModel;
    [SerializeField] private Rigidbody sphereRB;
    public void HandleVisualRotation(float moveInput, float turnInput, bool isDrifting, float driftDirection)
    {
        carModel.position = sphereRB.transform.position;
        if(moveInput == 0) return;

        float currentTurnSpeed = isDrifting ? (turnSpeed * 0.5f) : turnSpeed;
        transform.Rotate(0, turnInput * currentTurnSpeed * Time.deltaTime * Mathf.Sign(moveInput),0);

        Debug.Log(isDrifting);

        if(isDrifting)
        {
            // Khi Drift: Model xoay thêm một góc (ví dụ 30 độ) về hướng drift
            float targetAngle = driftDirection * driftSlipAngle;
            // Dùng Lerp để xoay model mượt mà sang góc drift
            Quaternion driftRot = Quaternion.Euler(0, targetAngle, 0);
            carModel.localRotation = Quaternion.Lerp(carModel.localRotation, driftRot, Time.deltaTime * 5f);
        }
        else
        {
            // Khi KHÔNG Drift: Model quay về thẳng (0 độ)
            carModel.localRotation = Quaternion.Lerp(carModel.localRotation, Quaternion.identity, Time.deltaTime * 5f);
        }
    }
}
