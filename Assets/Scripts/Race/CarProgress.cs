using UnityEngine;
using UnityEngine.Splines;
using Unity.Mathematics;

public class CarProgress : MonoBehaviour
{
    [Header("Race Info")]
    public int currentLap = 1;
    public float splineProgress = 0f; // Vị trí trên track (0.0 đến 1.0)
    public float totalDistance = 0f;  // Dùng để xếp hạng

    [Header("References")]
    public SplineContainer trackSpline;

    private float previousProgress = 0f;
    private bool passedHalfway = false; // Chống cheat (đi lùi qua vạch đích)
    private float splineLength = 0f;

    public bool HasFinished { get; private set; } = false;

    void Start()
    {
        if (trackSpline == null)
        {
            trackSpline = Object.FindFirstObjectByType<SplineContainer>();
        }

        if (trackSpline != null)
        {
            splineLength = trackSpline.CalculateLength();
        }
    }

    void Update()
    {
        if (trackSpline == null || HasFinished) return;

        UpdateProgress();
    }

    private void UpdateProgress()
    {
        // 1. Tính toán vị trí t (0 -> 1) trên spline
        float3 localPos = trackSpline.transform.InverseTransformPoint(transform.position);
        SplineUtility.GetNearestPoint(trackSpline.Spline, localPos, out _, out float t);

        splineProgress = t;

        // 2. Chống Cheat: Xác nhận xe đã đi qua nửa đoạn đường
        if (splineProgress > 0.4f && splineProgress < 0.6f)
        {
            passedHalfway = true;
        }

        // 3. Xử lý qua Vạch đích (Finish Line)
        // Khi t chuyển từ gần 1.0 về gần 0.0 (đi tiến)
        if (previousProgress > 0.8f && splineProgress < 0.2f)
        {
            if (passedHalfway) // Đảm bảo đã chạy hết vòng, không phải đứng tại vạch đích lùi rồi tiến
            {
                currentLap++;
                passedHalfway = false; // Reset cho vòng tiếp theo
            }
        }
        
        previousProgress = splineProgress;

        // 4. Tính tổng quãng đường để phục vụ xếp hạng
        // Tính bằng: (Số vòng đã hoàn thành * độ dài vòng) + Quãng đường vòng hiện tại
        int completedLaps = currentLap - 1;
        totalDistance = (completedLaps * splineLength) + (splineProgress * splineLength);
    }

    public void FinishRace()
    {
        HasFinished = true;
        
        // Tắt AI hoặc Input nếu muốn xe tự phanh lại
        var aiController = GetComponent<AIInputController>();
        if (aiController != null) aiController.enabled = false;
    }
}
