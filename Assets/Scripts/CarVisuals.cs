using UnityEngine;

public class CarVisuals : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private float driftSlipAngle = 30f;
    [SerializeField] private float turnSpeed = 100f;
    [SerializeField] private Transform carModel;
    [SerializeField] private Rigidbody sphereRB;
    [SerializeField] private Transform wheelLeft;
    [SerializeField] private Transform wheelRight;
    [SerializeField] private Transform character;
    [SerializeField] private float steerAngle = 30f;

    [Header("Character Inertia (Quán tính nhân vật)")]
    [SerializeField] private float maxSideLeanAngle = 25f;    // Góc nghiêng tối đa sang 2 bên khi cua
    [SerializeField] private float maxForwardLeanAngle = 15f; // Góc ngả trước/sau khi tăng tốc/phanh
    [SerializeField] private float leanSmoothness = 8f;       // Tốc độ phản hồi của quán tính
    
    private Quaternion initialCharacterRot;
    private float previousSpeed;

    private void Start()
    {
        if (character != null)
        {
            initialCharacterRot = character.localRotation;
        }
    }

    public void HandleVisualRotation(float moveInput, float turnInput, bool isDrifting, float driftDirection, bool isSpinning)
    {
        carModel.position = sphereRB.transform.position;

        if(isSpinning)
        {
            carModel.Rotate(0, 720f * Time.deltaTime, 0);
            return;
        }

        if(moveInput == 0 && sphereRB.linearVelocity.magnitude < 0.1f) return;

        float currentSteerAngle = steerAngle * turnInput;

        // Xoay bánh xe
        wheelLeft.localEulerAngles = new Vector3(
            wheelLeft.localEulerAngles.x, 
            currentSteerAngle, 
            wheelLeft.localEulerAngles.z
        );

        wheelRight.localEulerAngles = new Vector3(
            wheelRight.localEulerAngles.x, 
            currentSteerAngle, 
            wheelRight.localEulerAngles.z
        );

        // Tính tốc độ hiện tại và gia tốc
        float currentSpeed = sphereRB.linearVelocity.magnitude;
        float normalizedSpeed = Mathf.InverseLerp(0, 25f, currentSpeed); // Giả sử tốc độ max tầm 25
        
        float acceleration = (currentSpeed - previousSpeed) / Time.deltaTime;
        previousSpeed = currentSpeed;

        // --- MÔ PHỎNG QUÁN TÍNH NHÂN VẬT ---
        if (character != null)
        {
            // 1. Quán tính văng sang hai bên khi rẽ (Lực ly tâm)
            // Đảo dấu để nhân vật nghiêng ngược chiều với hướng rẽ tùy thuộc vào trục Z của model
            float targetSideLean = turnInput * maxSideLeanAngle * normalizedSpeed;

            // 2. Quán tính ngả trước/sau do gia tốc
            // Tăng tốc đột ngột -> ngửa ra sau (X < 0)
            // Phanh gấp -> chúi tới trước (X > 0)
            float targetForwardLean = -Mathf.Clamp(acceleration * 0.5f, -maxForwardLeanAngle, maxForwardLeanAngle);
            
            // Kết hợp input chân ga để tạo cảm giác chồm lên khi nhấn ga
            targetForwardLean -= moveInput * (maxForwardLeanAngle * 0.3f);

            if (isDrifting)
            {
                // Khi drift, lực văng ngang sẽ gắt hơn
                targetSideLean *= 1.5f;
            }

            // Kết hợp với góc quay ban đầu của nhân vật
            Quaternion inertiaRot = Quaternion.Euler(targetForwardLean, 0, targetSideLean);
            Quaternion targetCharacterRot = initialCharacterRot * inertiaRot;

            // Áp dụng độ nghiêng mượt mà
            character.localRotation = Quaternion.Lerp(character.localRotation, targetCharacterRot, Time.deltaTime * leanSmoothness);
        }

        float currentTurnSpeed = isDrifting ? (turnSpeed * 0.5f) : turnSpeed;
        transform.Rotate(0, turnInput * currentTurnSpeed * Time.deltaTime * Mathf.Sign(moveInput != 0 ? moveInput : 1f),0);

        if(isDrifting)
        {
            // Khi Drift: Model xoay thêm một góc về hướng drift
            float targetAngle = driftDirection * driftSlipAngle;
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
