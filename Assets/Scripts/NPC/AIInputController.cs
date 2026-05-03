using UnityEngine;
using UnityEngine.Splines;
using Unity.Mathematics;

public class AIInputController : MonoBehaviour, ICarInput
{
    [Header("Đường đua (Spline)")]
    [Tooltip("Kéo SplineRoad của đường đua vào đây. Nếu để trống, code sẽ tự động tìm.")]
    public SplineContainer trackSpline; 
    public float maxLookAheadDistance = 15f; // Khoảng cách nhìn xa tối đa
    public float trackWidth = 10f; // Bề rộng của mặt đường (để AI biết giới hạn 2 bên)

    [Header("Cài đặt Lái xe & Drift")]
    public float maxSteeringAngle = 45f; // Góc cua tối đa mà xe có thể bẻ
    public float driftAngleThreshold = 20f; // Góc cua lớn hơn mức này sẽ bắt đầu lết bánh (Drift)
    public float brakeAngleThreshold = 30f; // Góc cua quá gắt thì sẽ tự động giảm ga (Brake)
    public float steeringSmoothness = 10f; // Độ mượt khi xoay vô lăng

    [Header("Chống kẹt (Unstuck)")]
    public float stuckSpeedThreshold = 1f; // Vận tốc dưới mức này bị coi là kẹt
    public float stuckTimeLimit = 2f; // Thời gian kẹt tối đa trước khi lùi
    public float reverseTime = 1.5f; // Thời gian lùi xe

    private Rigidbody rb;
    private float stuckTimer = 0f;
    private float reversingTimer = 0f;

    public float MoveInput { get; set; }
    public float TurnInput { get; set; }
    public bool IsDrifting { get; set; }
    public bool IsBoosting { get; set; }
    public bool IsUsingItem { get; set; }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        
        if (trackSpline == null)
        {
            trackSpline = Object.FindFirstObjectByType<SplineContainer>();
        }
    }

    private float logTimer = 0f;

    void Update()
    {
        if (trackSpline == null || trackSpline.Splines == null || trackSpline.Splines.Count == 0)
        {
            MoveInput = 0;
            TurnInput = 0;
            if (trackSpline != null) Debug.LogWarning($"[AI] Spline rỗng hoặc lỗi!");
            return;
        }

        HandleUnstuckLogic();

        if (reversingTimer > 0)
        {
            reversingTimer -= Time.deltaTime;
            MoveInput = -1f; 
            return;
        }

        DriveAlongSpline();

        // In log mỗi 1 giây để không bị trôi Console
        logTimer += Time.deltaTime;
        if (logTimer >= 1f)
        {
            bool grounded = GetComponent<PlayerMovement>().isGrounded;
            Debug.Log($"[AI {gameObject.name}] Speed: {rb.linearVelocity.magnitude:F1} | Move: {MoveInput:F2} | Turn: {TurnInput:F2} | Grounded: {grounded}");
            logTimer = 0f;
        }
    }


    private void HandleUnstuckLogic()
    {
        if (rb == null) return;

        Vector3 rayOrigin = transform.position + transform.up * 0.5f;
        // Thêm cảm biến phía trước để phát hiện đâm trực diện
        bool headOnCollision = Physics.Raycast(rayOrigin, transform.forward, out RaycastHit hitF, 2f);
        if (headOnCollision && !hitF.collider.isTrigger)
        {
            stuckTimer += Time.deltaTime * 2f; // Tăng nhanh hơn nếu đâm trực diện
        }

        // Tăng ngưỡng lên 3m/s để nhận diện kẹt nhạy hơn
        if (MoveInput > 0.1f && rb.linearVelocity.magnitude < 3f)
        {
            stuckTimer += Time.deltaTime;
            if (stuckTimer > stuckTimeLimit)
            {
                reversingTimer = reverseTime;
                stuckTimer = 0f;
            }
        }
        else
        {
            stuckTimer = Mathf.Max(0, stuckTimer - Time.deltaTime);
        }
    }

    private void DriveAlongSpline()
    {
        // 1. Lấy vị trí gần nhất
        float3 localPos = trackSpline.transform.InverseTransformPoint(transform.position);
        float distanceToSpline = SplineUtility.GetNearestPoint(trackSpline.Spline, localPos, out float3 nearestLocalPos, out float t);
        if (distanceToSpline > 50f) return;

        trackSpline.Spline.Evaluate(t, out float3 currentLocalPos, out float3 currentLocalTangent, out float3 currentLocalUp);
        Vector3 currentTangent = trackSpline.transform.TransformDirection(currentLocalTangent).normalized;

        // 2. Tầm nhìn linh hoạt (FIX: GIẢM MẠNH CHO ĐƯỜNG CHỮ S)
        float currentSpeed = rb != null ? rb.linearVelocity.magnitude : 10f;
        
        // Đi nhanh nhìn xa, nhưng KHỐNG CHẾ MAX 20m để không nhìn lố sang khúc cua tiếp theo
        float lookAheadDist = Mathf.Clamp(currentSpeed * 0.5f, 6f, 20f); 

        float splineLength = trackSpline.Spline.GetLength();
        if (splineLength <= 0) return;

        float targetT = t + (lookAheadDist / splineLength);
        if (targetT > 1f) targetT = trackSpline.Spline.Closed ? targetT % 1f : 1f;

        // 3. Dự đoán cua gắt phía trước
        float futureT = t + (lookAheadDist * 1.5f / splineLength);
        if (futureT > 1f) futureT = trackSpline.Spline.Closed ? futureT % 1f : 1f;
        trackSpline.Spline.Evaluate(futureT, out _, out float3 futureLocalTangent, out _);
        Vector3 futureTangent = trackSpline.transform.TransformDirection(futureLocalTangent).normalized;
        float futureCurveAngle = Vector3.Angle(currentTangent, futureTangent);

        // 4. Target Point (FIX: BỎ CẮT GÓC, ÔM GIỮA ĐƯỜNG CHO AN TOÀN)
        trackSpline.Spline.Evaluate(targetT, out float3 targetLocalPos, out float3 targetLocalTangent, out _);
        Vector3 targetWaypoint = trackSpline.transform.TransformPoint(targetLocalPos);
        
        // 5. --- NÉ TƯỜNG ĐỘNG (SPEED-BASED WHISKERS) ---
        float wallAvoidanceSteer = 0f;
        // Đi càng nhanh thì tia cảm biến phóng ra càng dài (max 15m) để né từ xa
        float dynamicRayDist = Mathf.Clamp(currentSpeed * 0.7f, 6f, 15f); 
        Vector3 rayOrigin = transform.position + transform.up * 0.5f;

        Vector3 dirFrontRight = (transform.forward + transform.right * 0.8f).normalized;
        Vector3 dirRight = transform.right;
        Vector3 dirFrontLeft = (transform.forward - transform.right * 0.8f).normalized;
        Vector3 dirLeft = -transform.right;

        // Né bên Phải
        if (Physics.Raycast(rayOrigin, dirFrontRight, out RaycastHit hitFR, dynamicRayDist) && !hitFR.collider.isTrigger) 
            wallAvoidanceSteer -= (2.5f * (1f - (hitFR.distance / dynamicRayDist))); // Lực mạnh hơn

        if (Physics.Raycast(rayOrigin, dirRight, out RaycastHit hitR, dynamicRayDist * 0.6f) && !hitR.collider.isTrigger) 
            wallAvoidanceSteer -= (1.0f * (1f - (hitR.distance / (dynamicRayDist * 0.6f))));

        // Né bên Trái
        if (Physics.Raycast(rayOrigin, dirFrontLeft, out RaycastHit hitFL, dynamicRayDist) && !hitFL.collider.isTrigger) 
            wallAvoidanceSteer += (2.5f * (1f - (hitFL.distance / dynamicRayDist))); // Lực mạnh hơn

        if (Physics.Raycast(rayOrigin, dirLeft, out RaycastHit hitL, dynamicRayDist * 0.6f) && !hitL.collider.isTrigger) 
            wallAvoidanceSteer += (1.0f * (1f - (hitL.distance / (dynamicRayDist * 0.6f))));

        // --- TÍNH TOÁN HƯỚNG LÁI ---
        Vector3 directionToTarget = (targetWaypoint - transform.position).normalized;
        float angleToTarget = Vector3.SignedAngle(transform.forward, directionToTarget, Vector3.up);
        
        float desiredTurnInput = angleToTarget / maxSteeringAngle;
        desiredTurnInput += wallAvoidanceSteer;
        
        // FIX: Ép TurnInput về khoảng -1 đến 1. Bẻ lái > 1 sẽ làm nát logic bánh xe vật lý (WheelCollider)
        desiredTurnInput = Mathf.Clamp(desiredTurnInput, -1f, 1f);

        // Vô lăng bẻ nhanh hơn (từ 5f lên 12f) để kịp phản ứng với đường zíc zắc
        TurnInput = Mathf.Lerp(TurnInput, desiredTurnInput, Time.deltaTime * 12f);

        // --- TÍNH TOÁN CHÂN GA & PHANH (QUAN TRỌNG NHẤT) ---
        float absAngle = Mathf.Abs(angleToTarget);
        float targetMoveInput = 1f;

        // Nếu tia né tường kêu gào (cực gần tường) HOẶC khúc cua quá gắt -> BỎ CHÂN GA, ĐẠP PHANH
        if (Mathf.Abs(wallAvoidanceSteer) > 1.0f || absAngle > 60f || futureCurveAngle > 60f)
        {
            targetMoveInput = 0f; // Nhả ga hoàn toàn
        }
        else if (Mathf.Abs(wallAvoidanceSteer) > 0.4f || absAngle > 35f || futureCurveAngle > 40f)
        {
            targetMoveInput = 0.4f; // Chạy rề rề qua cua
        }

        // Tốc độ phản hồi ga/phanh phải nhanh (8f)
        MoveInput = Mathf.Lerp(MoveInput, targetMoveInput, Time.deltaTime * 8f);

        // --- DRIFT & BOOST ---
        IsDrifting = (absAngle > driftAngleThreshold && currentSpeed > 8f);
        IsBoosting = (absAngle < 5f && futureCurveAngle < 5f && MoveInput > 0.9f);
        IsUsingItem = false;

        // Debug visualization (Bật tab Scene lên để xem tia né tường hoạt động)
        Debug.DrawLine(transform.position, targetWaypoint, Color.green);
        Debug.DrawRay(rayOrigin, dirFrontRight * dynamicRayDist, Color.yellow);
        Debug.DrawRay(rayOrigin, dirFrontLeft * dynamicRayDist, Color.yellow);
    }
}
