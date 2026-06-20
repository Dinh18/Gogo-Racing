using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RaceProgressUI : MonoBehaviour
{
    [Header("References")] 
    [SerializeField] private Text currLapText;
    [SerializeField] private Text maxLapText;
    [SerializeField] private Text timeText;
    
    // Bỏ [SerializeField] đi vì ta sẽ nhận biến này từ Manager truyền vào
    private CarProgress playerProgress; 

    // Hàm này sẽ được gọi từ RaceSetupManager SAU KHI xe đã sinh ra
    public void Setup(CarProgress playerProgress)
    {
        this.playerProgress = playerProgress;
        // Gắn sự kiện (Đảm bảo bỏ đăng ký cũ trước để không bị trùng lặp nếu chạy lại)
        playerProgress.FinishedLap -= UpdateCurrLapText;
        playerProgress.FinishedLap += UpdateCurrLapText;

        // Setup UI ban đầu
        maxLapText.text = "/" + RaceManager.Instance.totalLaps.ToString() + " LAP";
        currLapText.text = "0";
        timeText.text = "00:00.000";
    }

    void OnDestroy()
    {
        // Kiểm tra null trước khi hủy sự kiện để an toàn tuyệt đối
        if (playerProgress != null)
        {
            playerProgress.FinishedLap -= UpdateCurrLapText;
        }
    }

    void Update()
    {
        // 1. Nếu chưa có player (chưa setup xong) thì không làm gì cả
        if (playerProgress == null) return;

        // 2. CHÚ Ý LỖI LOGIC: Thời gian chạy phải đếm lúc ĐANG ĐUA (Racing)
        // chứ không phải lúc ĐẾM NGƯỢC (CountDown)
        if (RaceManager.Instance.GetCurrState() != RaceState.Racing) return;
        
        UpdateTimerDisplay();
    }

    private void UpdateTimerDisplay()
    {
        int minutes = Mathf.FloorToInt(playerProgress.totalTime / 60f);
        int seconds = Mathf.FloorToInt(playerProgress.totalTime % 60f);
        int milliseconds = Mathf.FloorToInt((playerProgress.totalTime % 1f) * 1000f);

        timeText.text = $"{minutes:00}:{seconds:00}.{milliseconds:000}";
    }

    private void UpdateCurrLapText(int currLap)
    {
        currLapText.text = (currLap - 1).ToString();
    }
}