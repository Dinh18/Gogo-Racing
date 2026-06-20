using UnityEngine;

public class CarAudioController : MonoBehaviour
{
    [Header("Liên kết")]
    [SerializeField] private CarMovement carMovement;
    [SerializeField] private CarDrift playerDrift;
    [Header("Audio Sources")]
    [SerializeField] private AudioSource engineAudioSource;
    [SerializeField] private AudioSource driftAudioSource;
    [SerializeField] private AudioSource boostAudioSource;
    [Header("Cài đặt Âm lượng (Volume)")]
    public float minVolume = 0.3f; // Âm lượng lúc xe đứng im (Garanti)
    public float maxVolume = 1.0f; // Âm lượng lúc lút ga

    [Header("Cài đặt Cao độ (Pitch)")]
    public float minPitch = 0.8f;  // Cao độ trầm lúc xe đứng im
    public float maxPitch = 2.0f;  // Cao độ gầm rú lúc max tốc độ

    void OnEnable()
    {
        if(playerDrift != null) playerDrift.OnDriftStateChange += PlayDriftAudio;
        // if(carMovement != null) carMovement.OnStartBoost += PlayBoostAudio;
    }
    void OnDisable()
    {
        if(playerDrift != null) playerDrift.OnDriftStateChange -= PlayDriftAudio;
        if(carMovement != null) carMovement.OnStartBoost -= PlayBoostAudio;

    }

    void Start()
    {
        
        // Đảm bảo file âm thanh được lặp lại liên tục (Loop)
        engineAudioSource.loop = true;
        driftAudioSource.loop = true;

        // driftAudioSource.Stop();
        
        // Nếu chưa bật thì tự động bật lên phát tiếng
        if (!engineAudioSource.isPlaying)
        {
            engineAudioSource.Play();
        }
    }

    void Update()
    {
        PlayEngineAudio();   
        if (driftAudioSource.isPlaying)
        {
            // Liên tục thay đổi Pitch ngẫu nhiên một chút xíu (từ 0.9 đến 1.1)
            // Giúp xóa bỏ cảm giác lặp đi lặp lại của file âm thanh
            driftAudioSource.pitch = Random.Range(0.9f, 1.1f);
            
            // Có thể làm nhiễu thêm cả Volume để giống mặt đường lồi lõm
            driftAudioSource.volume = Random.Range(0.8f, 1.0f);
        }
        if(boostAudioSource.isPlaying)
        {
            float currSpeed = carMovement.currSpeed - carMovement.GetDefaultMoveSpeed();
            float maxSpeed = carMovement.GetDefaultMoveSpeed() * carMovement.amountAccelerate - carMovement.GetDefaultMoveSpeed();
            float speedRatio;
            if(currSpeed <= 0)
            {
                speedRatio = 0;
            }
            else
            {
                speedRatio = Mathf.Clamp01(currSpeed / maxSpeed);;
            }

            boostAudioSource.volume = Mathf.Lerp(minVolume, maxVolume, speedRatio);
            boostAudioSource.pitch = Mathf.Lerp(minPitch, maxPitch, speedRatio);
        }
    }
    private void PlayEngineAudio()
    {
        // Lấy vận tốc hiện tại từ script vật lý
        if (carMovement == null) return;
        float currentSpeed = carMovement.currSpeed;
        float maxSpeed = (carMovement.GetDefaultMoveSpeed() * carMovement.amountAccelerate)/carMovement.GetGroundDrag();

        // Tính tỷ lệ % tốc độ (từ 0.0 đến 1.0)
        float speedRatio = Mathf.Clamp01(currentSpeed / maxSpeed);

        // NỘI DUNG CHÍNH: Thay đổi thuộc tính âm thanh theo % tốc độ
        engineAudioSource.volume = Mathf.Lerp(minVolume, maxVolume, speedRatio);
        engineAudioSource.pitch = Mathf.Lerp(minPitch, maxPitch, speedRatio);
    }
    private void PlayDriftAudio(bool isDrifting)
    {
        if(isDrifting)
        {
            if (!driftAudioSource.isPlaying)
            {
                driftAudioSource.Play();
            }
        }
        else
        {
            driftAudioSource.Stop();
        }
    }

    private void PlayBoostAudio()
    {
        if (!boostAudioSource.isPlaying)
        {
            boostAudioSource.Play();
        }
    }
}