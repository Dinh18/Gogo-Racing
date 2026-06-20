using UnityEngine;

public class BananaItem : ItemBase
{
    [SerializeField] private GameObject bananaPrefab;

    public override void UseItem(GameObject user)
    {
        RearSpawnMarker marker = user.GetComponentInChildren<RearSpawnMarker>();
        if (marker == null)
        {
            Debug.LogError("Chưa gắn RearSpawnMarker cho xe này!");
            return;
        }

        Transform rearSpawnPoint = marker.transform;

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
