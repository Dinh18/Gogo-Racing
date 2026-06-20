using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Speedometer_Dial : MonoBehaviour
{
    [SerializeField] private CarMovement playerMovement;
    [SerializeField] private RectTransform speedometer_Needle;
    [SerializeField] private Text speedText;
    private float minSpeedAngle = 180f;
    private float maxSpeedAngle = -90f;

    public void Setup(CarMovement playerMovement)
    {
        this.playerMovement = playerMovement;
        speedometer_Needle.localEulerAngles = new Vector3(
        speedometer_Needle.localEulerAngles.x, 
        speedometer_Needle.localEulerAngles.y, 
        minSpeedAngle);
    }

    // Update is called once per frame
    void Update()
    {
        if (playerMovement == null || speedometer_Needle == null) return;

        // Bước 1: Tính toán tốc độ tối đa của xe (giống hệt cách tính bên Audio)
        float carMaxPhysicalSpeed = (playerMovement.GetDefaultMoveSpeed() * playerMovement.amountAccelerate)/playerMovement.GetGroundDrag();

        float visualMaxSpeed = carMaxPhysicalSpeed * 1.1f;

        // Bước 2: Tính tỷ lệ % tốc độ hiện tại (Ra một số từ 0.0 đến 1.0)
        // Clamp01 để đảm bảo khi xe lùi (số âm) hoặc lỗi chạy quá nhanh, tỷ lệ không bị vượt quá giới hạn 0-1
        float speedRatio = Mathf.Clamp01(playerMovement.currSpeed / visualMaxSpeed);

        // Bước 3: Nội suy góc quay (Nếu ratio = 0 -> góc 180, Nếu ratio = 1 -> góc -90)
        float angle = Mathf.Lerp(minSpeedAngle, maxSpeedAngle, speedRatio);

        // Nhân 3.6 để giả lập vận tốc km/h cho đồng hồ hiển thị thực tế hơn
        speedText.text = ((int)(playerMovement.currSpeed * 3.6f)).ToString();

        // Bước 4: Xoay kim
        speedometer_Needle.localEulerAngles = new Vector3(
            speedometer_Needle.localEulerAngles.x, 
            speedometer_Needle.localEulerAngles.y, 
            angle);

    }


}
