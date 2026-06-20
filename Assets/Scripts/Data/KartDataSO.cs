using UnityEngine;

[CreateAssetMenu(fileName = "Kart_", menuName = "Scriptable Objects/KartDataSO")]
[System.Serializable]
public class KartDataSO : ItemDataSO
{
    // public int kartID;
    // public string nameKart;
    // public GameObject mainMenuPrefab;
    // public GameObject ingamePrefab;
    // public Sprite avatarSprite;

    [Header("Kart Stats")]
    [Tooltip("Tốc độ tối đa của xe")]
    [Range(0f, 200f)] public float speedStat = 50f;

    [Tooltip("Gia tốc, khả năng đạt tốc độ tối đa")]
    [Range(0f, 100f)] public float accelerationStat = 50f;

    [Tooltip("Khả năng bẻ lái, độ nhạy khi rẽ")]
    [Range(0f, 100f)] public float handlingStat = 50f;

    [Tooltip("Tốc độ nạp Nitro khi Drift")]
    [Range(0f, 100f)] public float driftStat = 50f;
}
