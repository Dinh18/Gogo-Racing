using DG.Tweening;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    [SerializeField] private AudioMixer audioMixer;
    private const string MASTER_KEY = "MasterVolume";
    private const string BGM_KEY = "BGMVolume";
    private const string SFX_KEY = "SFXVolume";
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); 
        }
        else
        {
            Destroy(gameObject);
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        LoadVolumeSettings();
    }

    public void SetMasterVolume(float value)
    {
        SetVolume(MASTER_KEY, "MasterVol", value);
    }

    public void SetBGMVolume(float value)
    {
        SetVolume(BGM_KEY, "BGMVol", value);
    }

    public void SetSFXVolume(float value)
    {
        SetVolume(SFX_KEY, "SFXVol", value);
    }

    private void SetVolume (string prefKey, string mixerParam, float sliderValue)
    {
        // Khi sliderValue tiến về 0, đặt mức dB cực nhỏ (-80dB) để tắt hẳn tiếng.
        // Ngược lại, tính toán theo công thức Log10
        float decibelValue = sliderValue <= 0.0001f ? -80f : Mathf.Log10(sliderValue) * 20f;

        // Gán trực tiếp vì khi kéo Slider thì giá trị thay đổi liên tục
        bool success = audioMixer.SetFloat(mixerParam, decibelValue);
        
        if (!success)
        {
            Debug.LogError($"[AudioManager] LỖI: Không thể set tham số '{mixerParam}'! Hãy kiểm tra lại xem đã Expose và đổi tên đúng chính xác chưa.");
        }
        else
        {
            Debug.Log($"[AudioManager] Thành công set {mixerParam} thành {decibelValue} dB (Slider: {sliderValue})");
        }
        
        // Nên lưu trực tiếp sliderValue thay vì clampedValue để khi mở lại Setting, thanh slider nằm đúng ở số 0
        PlayerPrefs.SetFloat(prefKey, sliderValue);
    }

    private void LoadVolumeSettings()
    {
        SetMasterVolume(PlayerPrefs.GetFloat(MASTER_KEY, 1f));
        SetBGMVolume(PlayerPrefs.GetFloat(BGM_KEY, 1f));
        SetSFXVolume(PlayerPrefs.GetFloat(SFX_KEY, 1f));
    }
}
