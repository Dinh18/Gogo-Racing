using System;
using System.Collections;
using UnityEngine;

public class CarMovement : MonoBehaviour
{
    [Header("Movement Setting")]
    [SerializeField] private float defaultMoveSpeed = 50f;
    [SerializeField] private float moveSpeed = 50f;
    [SerializeField] private float groundDrag = 3f;
    [SerializeField] private float airDrag = 0.1f;
    [SerializeField] private float gravityForce = 10f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Rigidbody sphereRB;
    public float currSpeed => sphereRB.linearVelocity.magnitude;
    private float moveInput;
    public bool isSpinning = false;
    public bool isGrounded;
    [SerializeField] public float amountAccelerate = 1.5f;
    private float timeAccelerate = 2.8f;
    public event Action OnStartBoost;
    public event Action OnEndBoost;
    private Coroutine boostCoroutine;
    public float GetGroundDrag() => groundDrag;
    public float GetDefaultMoveSpeed() => defaultMoveSpeed;

    public void ApplyStats(float speedStat, float accelerationStat)
    {
        // Chuyển đổi từ thang điểm 0-100 sang giá trị thực tế
        // Gấp rưỡi tốc độ (x1.5) và dùng LerpUnclamped để xe có speedStat > 100 không bị giới hạn
        defaultMoveSpeed = Mathf.LerpUnclamped(30f * 1.5f, 70f * 1.5f, speedStat / 100f);
        moveSpeed = defaultMoveSpeed;
        amountAccelerate = Mathf.LerpUnclamped(1.1f, 1.9f, accelerationStat / 100f);
    }

    public void Accelerate()
    {
        moveSpeed = amountAccelerate * defaultMoveSpeed;
        OnStartBoost?.Invoke();
        if (boostCoroutine != null) StopCoroutine(boostCoroutine);
        boostCoroutine = StartCoroutine(ResetMoveSpeed(timeAccelerate));
    }

    public void CarBroke()
    {
        moveSpeed-=10;
    }

    private IEnumerator ResetMoveSpeed(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        moveSpeed = defaultMoveSpeed;
        OnEndBoost?.Invoke();
    }

    public void Move(ICarInput inputController)
    {
        moveInput = inputController.MoveInput;
        // 3. Kiểm tra chạm đất (Ground Check)
        // Bắt đầu Raycast từ vị trí cao hơn 1 chút để tránh việc tâm xe sát đất quá không bắn được
        Vector3 rayOrigin = transform.position + transform.up * 0.5f;
        float rayDistance = 1.5f;
        isGrounded = Physics.Raycast(rayOrigin, -transform.up, out RaycastHit hit, rayDistance, groundLayer);

        // Debug vẽ tia Raycast để thấy được trong Scene
        Debug.DrawRay(rayOrigin, -transform.up * rayDistance, isGrounded ? Color.green : Color.red);

        // 4. Xử lý di chuyển
        if (isGrounded)
        {
            sphereRB.linearDamping = groundDrag;

            if (moveInput != 0)
            {
                // Log để kiểm tra xem lực có đang được add không
                // Debug.Log($"[Movement] Adding Force: {moveInput * moveSpeed} to {gameObject.name}");
                sphereRB.AddForce(transform.forward * moveInput * moveSpeed, ForceMode.Acceleration);
            }
            
            // Align to ground: Xoay xe theo độ dốc mặt đường
            Quaternion targetRotation = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 0.1f);
        }
        else
        {
            // Debug.Log($"[Movement] {gameObject.name} is NOT grounded!");
            sphereRB.linearDamping = airDrag;
            sphereRB.AddForce(Vector3.down * gravityForce, ForceMode.Acceleration);

            // Tự động cân bằng xe khi ở trên không để không bị lật ngửa hay chúi đầu
            Quaternion targetAirRot = Quaternion.FromToRotation(transform.up, Vector3.up) * transform.rotation;
            transform.rotation = Quaternion.Slerp(transform.rotation, targetAirRot, 0.05f);
        }
    }
}
