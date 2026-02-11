using UnityEngine;

public class CarController : MonoBehaviour
{
    [Header("Setting")]
    [SerializeField] private float moveSpeed = 50f;
    [SerializeField] private float turnSpeed = 100f;
    [SerializeField] private float groundDrag = 3f;
    [SerializeField] private float airDrag = 0.1f;
    [SerializeField] private float gravityForce = 10f;
    [Header("References")]
    [SerializeField] private Transform carModel;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Rigidbody sphereRB;

    private float moveInput;
    private float turnInput;
    private bool isGrounded;

    // Update is called once per frame
    void Update()
    {
        moveInput = Input.GetAxis("Vertical");
        turnInput = Input.GetAxis("Horizontal");

        // 2. Xử lý Visual: Xoay mô hình xe theo hướng rẽ
        // Lưu ý: Chúng ta chỉ xoay model, còn khối cầu vật lý vẫn giữ nguyên hướng rotation để ổn định
        //  Mathf.Sign(moveInput) trả về dáu của moveInput
        if (moveInput != 0)
        {
            transform.Rotate(0, turnInput * turnSpeed * Time.deltaTime * Mathf.Sign(moveInput), 0);
        }
        if (turnInput != 0 && moveInput != 0)
        {
            // Cho phép xoay tại chỗ (tùy chọn)
            transform.Rotate(0, turnInput * turnSpeed * Time.deltaTime, 0);
        }
        
        // Cập nhật vị trí model bám theo sphere
        carModel.position = sphereRB.transform.position - new Vector3(0, 0f, 0); // Offset xuống 1 chút
    }

    void FixedUpdate()
    {
        // 3. Kiểm tra chạm đất (Ground Check)
        // Bắn Raycast từ tâm xe xuống dưới
        isGrounded = Physics.Raycast(transform.position, -transform.up, out RaycastHit hit, 1.2f, groundLayer);

        // 4. Xử lý di chuyển
        if (isGrounded)
        {
            Debug.Log("Is Grounded");
            sphereRB.linearDamping = groundDrag;

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
