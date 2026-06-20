using UnityEngine;
using UnityEngine.UI;

public class MainMenuUIManager : MonoBehaviour
{
    [SerializeField] private GameObject[] allPanels;
    [SerializeField] private Transform charHolder;
    [SerializeField] private Transform kartHolder;
    [SerializeField] private Button tutorialBtn;
    [SerializeField] private Panel[] panels;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        CharacterSO currChar = DataManager.Instance.GetCurrentItem<CharacterSO>(DataManager.Instance.GetAllCharacters(), DataManager.Instance.GetCurrentCharacterID());
        KartDataSO currKart = DataManager.Instance.GetCurrentItem<KartDataSO>(DataManager.Instance.GetAllKarts(), DataManager.Instance.GetCurrentKartID());

        ShowItem(currChar, currKart);
        GaragePanel.OnChangeItem += ShowItem;
        tutorialBtn.onClick.AddListener(StartTutorial);

        foreach(Panel panel in panels)
        {
            panel.Setup();
        }
        UIManager.Instance.HideLoading();
    }

    void OnDestroy()
    {
        GaragePanel.OnChangeItem -= ShowItem;
        tutorialBtn.onClick.RemoveListener(StartTutorial);
    }

    // Update is called once per frame

    private void ShowItem(CharacterSO charSO, KartDataSO kartSO)
    {
        foreach(Transform child in charHolder)
        {
            Destroy(child.gameObject);
        }
        foreach(Transform child in kartHolder)
        {
            Destroy(child.gameObject);
        }

        Instantiate(charSO.mainMenuPrefab,charHolder);
        Instantiate(kartSO.mainMenuPrefab,kartHolder);
    }

    public void OpenPanel(GameObject panelToOpen)
    {
        foreach (GameObject panel in allPanels)
        {
            if(panelToOpen != null) panel.SetActive(panel == panelToOpen); 
        }
    }

    public void ClosePanel()
    {
        foreach (GameObject panel in allPanels)
        {
            panel.SetActive(false); 
        }
    }

    private void StartTutorial()
    {
        GameManager.Instance.ChangeGameState(GameState.Tutorial);
    }
    
}
