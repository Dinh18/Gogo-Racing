using Unity.Cinemachine;
using UnityEngine;
using System.Collections;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class CameraController : MonoBehaviour
{
    [SerializeField] private CinemachineCamera virtualCamera;
    [Header("FOV Settings")]
    [SerializeField] private float fovIncrease = 15f; // Đã giảm xuống một nửa từ 30f
    [SerializeField] private float fovTransitionSpeed = 8f; // Tăng tốc độ chuyển đổi cho cảm giác giật mạnh hơn
    
    [Header("Speed Effect (Post Processing)")]
    [SerializeField] private float maxLensDistortion = -0.85f; // Tăng thêm méo viền
    [SerializeField] private float maxChromaticAberration = 1.2f; // Tăng thêm nhòe viền
    [SerializeField] private float maxVignetteIntensity = 0.35f; // Thêm tối góc để tập trung ánh nhìn

    private CarMovement targetCar;
    private float defaultFOV;
    private Coroutine fovCoroutine;

    private Volume speedVolume;
    private LensDistortion lensDistortion;
    private ChromaticAberration chromaticAberration;
    private Vignette vignette;

    private void Start()
    {
        if (virtualCamera != null)
        {
            defaultFOV = virtualCamera.Lens.FieldOfView;
        }

        SetupSpeedEffectVolume();
    }

    private void SetupSpeedEffectVolume()
    {
        // Tạo một Global Volume ẩn để xử lý riêng hiệu ứng tốc độ
        speedVolume = gameObject.AddComponent<Volume>();
        speedVolume.isGlobal = true;
        speedVolume.priority = 100; // Độ ưu tiên cao nhất để đè lên các Volume khác
        speedVolume.weight = 0f;    // Bắt đầu với weight = 0 (tắt hiệu ứng)

        VolumeProfile profile = ScriptableObject.CreateInstance<VolumeProfile>();
        speedVolume.profile = profile;

        // Cấu hình Lens Distortion
        lensDistortion = profile.Add<LensDistortion>();
        lensDistortion.intensity.overrideState = true;
        lensDistortion.intensity.value = maxLensDistortion;

        // Cấu hình Chromatic Aberration
        chromaticAberration = profile.Add<ChromaticAberration>();
        chromaticAberration.intensity.overrideState = true;
        chromaticAberration.intensity.value = maxChromaticAberration;

        // Cấu hình Vignette
        vignette = profile.Add<Vignette>();
        vignette.intensity.overrideState = true;
        vignette.intensity.value = maxVignetteIntensity;
    }

    public void Setup(Transform playerTransform)
    {
        virtualCamera.Target.TrackingTarget = playerTransform;
        
        if (targetCar != null)
        {
            targetCar.OnStartBoost -= HandleBoostStart;
            targetCar.OnEndBoost -= HandleBoostEnd;
        }

        targetCar = playerTransform.GetComponent<CarMovement>();
        if (targetCar == null)
        {
            targetCar = playerTransform.GetComponentInParent<CarMovement>();
        }

        if (targetCar != null)
        {
            targetCar.OnStartBoost += HandleBoostStart;
            targetCar.OnEndBoost += HandleBoostEnd;
        }
    }

    private void OnDestroy()
    {
        if (targetCar != null)
        {
            targetCar.OnStartBoost -= HandleBoostStart;
            targetCar.OnEndBoost -= HandleBoostEnd;
        }
    }

    private void HandleBoostStart()
    {
        if (fovCoroutine != null) StopCoroutine(fovCoroutine);
        fovCoroutine = StartCoroutine(ChangeEffect(defaultFOV + fovIncrease, 1f));
    }

    private void HandleBoostEnd()
    {
        if (fovCoroutine != null) StopCoroutine(fovCoroutine);
        fovCoroutine = StartCoroutine(ChangeEffect(defaultFOV, 0f));
    }

    private IEnumerator ChangeEffect(float targetFOV, float targetVolumeWeight)
    {
        while (Mathf.Abs(virtualCamera.Lens.FieldOfView - targetFOV) > 0.1f || Mathf.Abs(speedVolume.weight - targetVolumeWeight) > 0.01f)
        {
            // Smoothly change FOV
            virtualCamera.Lens.FieldOfView = Mathf.Lerp(virtualCamera.Lens.FieldOfView, targetFOV, Time.deltaTime * fovTransitionSpeed);
            
            // Smoothly change Post Processing Weight
            speedVolume.weight = Mathf.Lerp(speedVolume.weight, targetVolumeWeight, Time.deltaTime * fovTransitionSpeed);
            
            yield return null;
        }
        virtualCamera.Lens.FieldOfView = targetFOV;
        speedVolume.weight = targetVolumeWeight;
    }
}
