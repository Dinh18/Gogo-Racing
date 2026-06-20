using UnityEngine;
using UnityEngine.UI;

public class ItemUI : MonoBehaviour
{
    [Header("UI Elements")]
    public Image item1Image;
    public Image item2Image;

    [Header("Player Reference")]
    [Tooltip("Kéo thả chiếc xe của người chơi vào đây trong Editor, hoặc gán qua code")]
    private PlayerItemController playerItemController;

    private void Start()
    {
        // Ẩn icon ban đầu
        // UpdateSlot(1, null);
        // UpdateSlot(2, null);

        // if (playerItemController != null)
        // {
        //     // Đăng ký lắng nghe sự kiện nhặt/sử dụng Item
        //     playerItemController.OnItemChanged += OnItemChangedHandler;
            
        //     // Cập nhật trạng thái hiện tại (nếu người chơi đã có đồ từ trước)
        //     UpdateSlot(1, playerItemController.item1);
        //     UpdateSlot(2, playerItemController.item2);
        // }

    }

    public void Setup(PlayerItemController controller)
    {   
        if (controller == null)
        {
            Debug.LogError("PlayerItemController is null when setting up ItemUI!");
            return;
        }
        this.playerItemController = controller;
        UpdateSlot(1, null);
        UpdateSlot(2, null);


        if (playerItemController != null)
        {
            // Đăng ký lắng nghe sự kiện nhặt/sử dụng Item
            playerItemController.OnItemChanged += OnItemChangedHandler;
            
            // Cập nhật trạng thái hiện tại (nếu người chơi đã có đồ từ trước)
            UpdateSlot(1, playerItemController.item1);
            UpdateSlot(2, playerItemController.item2);
        }
    }

    private void OnDestroy()
    {
        if (playerItemController != null)
        {
            // Hủy đăng ký sự kiện để tránh lỗi bộ nhớ (memory leak)
            playerItemController.OnItemChanged -= OnItemChangedHandler;
        }
    }

    private void OnItemChangedHandler(int slot, ItemIngameDataSO itemData)
    {
        UpdateSlot(slot, itemData);
    }

    private void UpdateSlot(int slot, ItemIngameDataSO itemData)
    {
        Image targetImage = (slot == 1) ? item1Image : item2Image;
        if (targetImage == null) return;

        if (itemData != null && itemData.icon != null)
        {
            targetImage.sprite = itemData.icon;
            targetImage.enabled = true; // Hiện icon
        }
        else
        {
            targetImage.sprite = null;
            targetImage.enabled = false; // Ẩn icon nếu không có đồ
        }
    }
}
