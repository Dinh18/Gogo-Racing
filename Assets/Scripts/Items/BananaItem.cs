using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class BananaItem : ItemBase
{
    [SerializeField] private GameObject bananaPrefab;
    private Transform rearSpawnPoint;
    private IEnumerator Start()
    {
        yield return null; // Đợi một frame để đảm bảo mọi thứ đã được khởi tạo
        RearSpawnMarker marker = transform.root.GetComponentInChildren<RearSpawnMarker>();

        if (marker != null)
        {
            rearSpawnPoint = marker.transform;
        }
        else
        {
            Debug.LogError("Chưa gắn RearSpawnMarker cho đuôi xe này!");
        }
    }
    public override void UseItem()
    {
        if(Physics.Raycast(rearSpawnPoint.position, Vector3.down, out RaycastHit hit, 5f))
        {
            Instantiate(bananaPrefab, hit.point, rearSpawnPoint.rotation);
            Debug.Log("Đã đặt vỏ chuối an toàn lên mặt đường!");
        }
        else
            {
                // Nếu tia laser không chạm đất (ví dụ xe đang bay trên không)
                // Thì cứ sinh ra bình thường ở đuôi xe, chấp nhận việc lơ lửng 
                Instantiate(bananaPrefab, rearSpawnPoint.position, rearSpawnPoint.rotation);
            }
        
    }
}
