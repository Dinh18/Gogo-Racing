using UnityEngine;
using UnityEngine.EventSystems; // Bắt buộc phải có thư viện này để dùng các sự kiện UI

// Thêm IDragHandler để Unity biết script này dùng để bắt sự kiện vuốt/kéo
public class UIRotateObject : MonoBehaviour, IDragHandler
{
    [Header("Vật thể 3D cần xoay (Kéo xe vào đây)")]
    [SerializeField] private Transform targetObject;

    [Header("Tốc độ xoay")]
    [SerializeField] private float rotateSpeed = 0.5f;

    [Header("Vuốt trái -> xe quay trái? (Tick để đảo chiều)")]
    [SerializeField] private bool invertX = false;

    // Hàm này sẽ tự động chạy liên tục KHI BẠN GIỮ VÀ KÉO CHUỘT trên tấm UI
    public void OnDrag(PointerEventData eventData)
    {
        if (targetObject != null)
        {
            // Lấy khoảng cách di chuyển của chuột/ngón tay theo chiều ngang (trục X)
            float dragDelta = eventData.delta.x;

            // Đảo chiều xoay nếu cần
            float direction = invertX ? 1f : -1f;

            // Xoay chiếc xe quanh trục Y (trục thẳng đứng hướng lên trời)
            // Dùng Space.World để đảm bảo xe luôn xoay quanh trục thẳng đứng của thế giới, không bị lật nghiêng
            targetObject.Rotate(Vector3.up, dragDelta * rotateSpeed * direction, Space.World);
        }
    }
}