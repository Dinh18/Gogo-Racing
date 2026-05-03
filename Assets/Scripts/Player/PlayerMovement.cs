using System;
using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Setting")]
    [SerializeField] public float defaultMoveSpeed = 50f;
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
    public float amountAccelerate{get; private set;} = 1.5f;
    private float timeAccelerate = 2.8f;
    public static event Action OnStartBoost;
    public void Accelerate()
    {
        moveSpeed = amountAccelerate * defaultMoveSpeed;
        OnStartBoost?.Invoke();
        StartCoroutine(ResetMoveSpeed(timeAccelerate));
    }

    private IEnumerator ResetMoveSpeed(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        moveSpeed = defaultMoveSpeed;
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
        }
    }
}
