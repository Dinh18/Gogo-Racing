using System;
using UnityEngine;

public class CarController : MonoBehaviour
{
    [Header("Movement Setting")]
    [SerializeField] private float moveSpeed = 50f;
    [SerializeField] private float turnSpeed = 100f;
    [SerializeField] private float groundDrag = 3f;
    [SerializeField] private float airDrag = 0.1f;
    [SerializeField] private float gravityForce = 10f;
    [Header("Drift Setting")]
    [SerializeField] private float steerForce = 50f;
    [SerializeField] private float driftSlipAngle = 30f;
    private bool isDrifting = false;
    private float driftDirection = 0;
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
        RotateCar();
        if(Input.GetKey(KeyCode.LeftControl) && isGrounded && turnInput != 0)
        {
            isDrifting = true;
            if(driftDirection == 0) driftDirection = Mathf.Sign(turnInput);
        }
        else if(Input.GetKeyUp(KeyCode.LeftControl))
        {
            isDrifting = false;
            driftDirection = 0;
        }
    }

    void FixedUpdate()
    {
        Move();

    }

    private void RotateCar()
    {
        moveInput = Input.GetAxis("Vertical");
        turnInput = Input.GetAxis("Horizontal");

        if(moveInput == 0) return;

        float currentTurnSpeed = isDrifting ? (turnSpeed * 0.5f) : turnSpeed;
        transform.Rotate(0, turnInput * currentTurnSpeed * Time.deltaTime * Mathf.Sign(moveInput),0);

        // Debug.Log(moveInput);

        // if(moveInput == 0){
        //     Debug.Log("No Move Input");
        //     return;
        // }

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

    private void Move()
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

    // private void Drift()
    // {
    //     float currenSpeed = isDrifting ? (turnInput * 0.5f) : turnInput;
    // }
}
