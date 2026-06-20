using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MapItemUI : MonoBehaviour
{
    private MapDataSO mapData;
    [SerializeField] private Image thumbnailImage;
    [SerializeField] private Text mapText;
    [SerializeField] private Button selectButton;
    void OnEnable()
    {
        selectButton.onClick.AddListener(OnClickSelect);
    }

    void OnDisable()
    {
        selectButton.onClick.RemoveListener(OnClickSelect);
    }
    public void Setup(MapDataSO mapData, bool isUnlocked = true)
    {
        this.mapData = mapData;
        thumbnailImage.sprite = mapData.mapThumnail;
        mapText.text = mapData.mapName;

        selectButton.interactable = isUnlocked;
        if (!isUnlocked)
        {
            thumbnailImage.color = Color.gray;
        }
        else
        {
            thumbnailImage.color = Color.white;
        }
    }

    public void OnClickSelect()
    {
        GameManager.Instance.ChangeGameState(GameState.InGame);
        GameManager.Instance.StartLoadScene(mapData.sceneName);
    }
}
