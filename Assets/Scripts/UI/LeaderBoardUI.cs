using UnityEngine;
using System.Collections.Generic;

public class LeaderBoardUI : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Kéo GameObject chứa tất cả các xe 3D vào đây")]
    [SerializeField] private GameObject player_Kart_UI;
    [SerializeField] private GameObject npc_Kart_UI;
    
    [Header("Setting")]
    [SerializeField] private float spacing = 70f;
    [SerializeField] private float startPos = -20f;
    [SerializeField] private float moveSpeed = 10f; // Tốc độ trượt lướt của thanh UI khi đổi hạng

    // Từ điển liên kết: 1 chiếc xe (CarProgress) sẽ đi kèm 1 thanh UI (LeaderboardSlotUI)
    private Dictionary<CarProgress, LeaderboardSlotUI> uiMap = new Dictionary<CarProgress, LeaderboardSlotUI>();

    public void Setup(List<CarProgress> allCars)
    {
        // Lặp qua tất cả các xe (Transform con) nằm trong kartHolder
        foreach (CarProgress carProgress in allCars)
        {
            // Lấy script CarProgress của chiếc xe này
            if (carProgress == null) continue;

            // 1. Chọn Prefab
            GameObject prefabToSpawn = carProgress.gameObject.CompareTag("Player") ? player_Kart_UI : npc_Kart_UI;

            // 2. Sinh ra UI và ép nó làm con của chính GameObject chứa script này (this.transform)
            GameObject spawnedUI = Instantiate(prefabToSpawn, this.transform);

            // 3. Lấy script Slot và nạp vào Từ điển để quản lý
            LeaderboardSlotUI slotUI = spawnedUI.GetComponent<LeaderboardSlotUI>();
            uiMap.Add(carProgress, slotUI);
        }
    }

    void Update()
    {
        // Nếu RaceManager chưa sẵn sàng thì chưa làm gì cả
        if (RaceManager.Instance == null) return;

        // Quét qua từng cặp (Xe - Thanh UI) trong Từ điển
        foreach (var kvp in uiMap)
        {
            CarProgress racer = kvp.Key;
            LeaderboardSlotUI slotUI = kvp.Value;

            // 1. Hỏi RaceManager xem xe này đang đứng hạng mấy
            int rank = RaceManager.Instance.GetCarRank(racer);

            // Lấy tên xe (Nếu xe đẻ ra từ Code có chữ (Clone), hàm Replace sẽ xóa chữ đó đi cho đẹp)
            string displayName = racer.CompareTag("Player") ? "The Dinh" : racer.gameObject.name.Replace("(Clone)", "").Trim();

            // 2. Cập nhật số và tên lên Text
            slotUI.Setup(rank, displayName);

            // 3. TÍNH TOÁN TỌA ĐỘ Y: Hạng 1 nằm trên cùng, Hạng 2 tụt xuống 1 khoảng spacing...
            float targetY = startPos - spacing * (rank - 1);
            Vector2 targetPosition = new Vector2(0, targetY);

            // 4. Di chuyển thanh UI cực mượt bằng Lerp 
            RectTransform uiRect = slotUI.GetComponent<RectTransform>();
            uiRect.anchoredPosition = Vector2.Lerp(uiRect.anchoredPosition, targetPosition, Time.deltaTime * moveSpeed);
        }
    }
}