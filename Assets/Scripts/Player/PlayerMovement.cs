using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Setting")]
    [SerializeField] private float defaultMoveSpeed = 50f;
    [SerializeField] private float moveSpeed = 50f;
    [SerializeField] private float groundDrag = 3f;
    [SerializeField] private float airDrag = 0.1f;
    [SerializeField] private float gravityForce = 10f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Rigidbody sphereRB;
    
    private float moveInput;
    public bool isGrounded;
    public void SetMoveSpeed(float time)
    {
        moveSpeed = defaultMoveSpeed * time;
    }

    public void Move(ICarInput inputController)
    
    {
        moveInput = inputController.MoveInput;
        // 3. Kiểm tra chạm đất (Ground Check)
        // Bắn Raycast từ tâm xe xuống dưới
        isGrounded = Physics.Raycast(transform.position, -transform.up, out RaycastHit hit, 1.2f, groundLayer);

        // 4. Xử lý di chuyển
        if (isGrounded)
        {
            // Debug.Log("Is Grounded");
            sphereRB.linearDamping = groundDrag;

            // Debug.Log(moveInput);

            if (moveInput != 0)
            {
                // Chỉ dùng AddForce để đẩy xe đi về phía trước của Model
                sphereRB.AddForce(transform.forward * moveInput * moveSpeed, ForceMode.Acceleration);
            }
            
            // Align to ground: Xoay xe theo độ dốc mặt đường
            Quaternion targetRotation = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 0.1f);
        }
        else
        {
            sphereRB.linearDamping = airDrag;
            // Thêm trọng lực thủ công để xe rơi nhanh hơn (cảm giác đầm hơn)
            sphereRB.AddForce(Vector3.down * gravityForce, ForceMode.Acceleration);
        }
    }
}
