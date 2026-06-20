using System;
using UnityEngine;
using UnityEngine.UI;


public class GaragePanel : Panel
{
    [Header("Hierarchy References")]
    // [SerializeField] private GameObject kart;
    // [SerializeField] private GameObject character;
    [SerializeField] private GameObject[] allContents;
    [SerializeField] private Transform kartList;
    [SerializeField] private Transform characterList;
    [SerializeField] private GameObject currentKartModel;
    [SerializeField] private GameObject currentCharacterModel;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private GarageItemList kartItemList;
    [SerializeField] private GarageItemList characterItemList;
    [SerializeField] private Button confirmButton;
    [Header("Prefabs References")]
    [SerializeField] private GarageItemUI kartListItemPrefab;
    [SerializeField] private GarageItemUI characterListItemPrefab;
    private ItemDataSO kartSelected;
    private ItemDataSO charSelected;
    public static event Action<CharacterSO, KartDataSO> OnChangeItem;

    [Header("Kart Stats UI")]
    [SerializeField] private Text speedText;
    [SerializeField] private Text accelerationText;
    [SerializeField] private Text handlingText;
    [SerializeField] private Text driftText;

    public override void Setup()
    {
        GenerateKartList();
        GenerateCharacterList();
    }

    void OnEnable()
    {
        KartDataSO currentKartData = DataManager.Instance.GetCurrentItem(DataManager.Instance.GetAllKarts(), DataManager.Instance.GetCurrentKartID());
        CharacterSO currentCharacterData = DataManager.Instance.GetCurrentItem(DataManager.Instance.GetAllCharacters(), DataManager.Instance.GetCurrentCharacterID());

        ChangeKart(currentKartData);
        ChangeCharacter(currentCharacterData);

        kartSelected = currentKartData;
        charSelected = currentCharacterData;

        // Chọn item đang trang bị trong danh sách
        if (kartItemList != null) kartItemList.Setup(currentKartData);
        if (characterItemList != null) characterItemList.Setup(currentCharacterData);

        confirmButton.onClick.AddListener(OnClickConfirm);
        ShowKart();

        // Chuyển sang tab Kart
        if (allContents != null && allContents.Length > 0)
        {
            ShowContent(allContents[0]);
        }

        // Cập nhật giao diện của các nút NavigationBar (nếu có)
        NavigationBar navBar = GetComponentInChildren<NavigationBar>();
        if (navBar != null)
        {
            NavButton[] navButtons = navBar.GetComponentsInChildren<NavButton>();
            if (navButtons.Length > 0)
            {
                foreach (var btn in navButtons)
                {
                    if (btn == navButtons[0]) btn.ButtonSelect();
                    else btn.ButtonUnSelect();
                }
            }
        }

        UpdateKartStatsUI(currentKartData);
    }

    void OnDisable()
    {
        confirmButton.onClick.RemoveListener(OnClickConfirm);
    }


    public void ShowKart()
    {
        currentKartModel.SetActive(true);
        currentCharacterModel.SetActive(false);
    }
    public void ShowCharacter()
    {
        currentKartModel.SetActive(false);
        currentCharacterModel.SetActive(true);
    }
    public void ShowContent(GameObject contentToShow)
    {
        foreach(var content in allContents)
        {
            content.SetActive(content == contentToShow);
        }
    }
    private void GenerateKartList()
    {
        var allKarts = DataManager.Instance.GetAllKarts();
        for (int i = 0; i < allKarts.Length; i++)
        {
            var kartData = allKarts[i];
            // CÁCH CHUẨN: Instantiate thẳng vào Parent và ép giữ nguyên Scale (false)
            GarageItemUI itemUI = Instantiate(kartListItemPrefab, kartList, false);
            
            bool isUnlocked = (i <= DataManager.Instance.GetUnlockedKartIndex());
            // Lấy tham chiếu an toàn tuyệt đối
            itemUI.Setup(kartData.avatarSprite,kartData.itemName, kartData,OnKartItemClicked, isUnlocked);
        }
    }
    private void GenerateCharacterList()
    {
        var allCharacters = DataManager.Instance.GetAllCharacters();
        for (int i = 0; i < allCharacters.Length; i++)
        {
            var charData = allCharacters[i];
            GarageItemUI itemUI = Instantiate(characterListItemPrefab, characterList, false);

            bool isUnlocked = (i <= DataManager.Instance.GetUnlockedCharacterIndex());
            itemUI.Setup(charData.avatarSprite,charData.itemName, charData, OnCharacterItemClicked, isUnlocked);
        }
    }
    private void ChangeKart(KartDataSO newKart)
    {
        if(currentKartModel != null)
        {
            Destroy(currentKartModel);
        }

        currentKartModel = Instantiate(newKart.mainMenuPrefab, spawnPoint);

        currentKartModel.transform.localPosition = Vector3.zero;
        currentKartModel.transform.localRotation = Quaternion.identity;
    }

    private void ChangeCharacter(CharacterSO newCharacter)
    {
        if(currentCharacterModel != null)
        {
            Destroy(currentCharacterModel);
        }

        currentCharacterModel = Instantiate(newCharacter.mainMenuPrefab, spawnPoint);

        currentCharacterModel.transform.localPosition = Vector3.zero;
        currentCharacterModel.transform.localRotation = Quaternion.identity;
    }

    private void OnKartItemClicked(ItemDataSO clickedData)
    {
        // Ép kiểu Data trả về thành KartDataSO
        KartDataSO kartData = clickedData as KartDataSO;
        
        if (kartData != null)
        {
            // Gọi hàm sinh xe 3D ra màn hình để Xem trước
            ChangeKart(kartData);

            kartSelected = kartData;
            
            // Nếu bạn muốn cái viền xanh ngọc nhảy sang chiếc xe vừa bấm, gọi thêm dòng này:
            kartItemList.Setup(kartData); 

            UpdateKartStatsUI(kartData);
        }
    }

    private void OnCharacterItemClicked(ItemDataSO clickedData)
    {
        // Ép kiểu Data trả về thành CharacterSO
        CharacterSO charData = clickedData as CharacterSO;
        
        if (charData != null)
        {
            // Gọi hàm sinh nhân vật 3D ra màn hình để Xem trước
            ChangeCharacter(charData);

            charSelected = charData;
            
            // Cập nhật viền sáng cho list nhân vật
            characterItemList.Setup(charData);
        }
    }

    private void OnClickConfirm()
    {
        DataManager.Instance.ChangeKart(kartSelected as KartDataSO);
        DataManager.Instance.ChangeChar(charSelected as CharacterSO);

        OnChangeItem?.Invoke(charSelected as CharacterSO, kartSelected as KartDataSO);
    }
    
    private void UpdateKartStatsUI(KartDataSO kartData)
    {
        if (kartData == null) return;
        
        // Cập nhật giá trị lên Text
        if (speedText != null) speedText.text = kartData.speedStat.ToString("F0");
        if (accelerationText != null) accelerationText.text = kartData.accelerationStat.ToString("F0");
        if (handlingText != null) handlingText.text = kartData.handlingStat.ToString("F0");
        if (driftText != null) driftText.text = kartData.driftStat.ToString("F0");
    }
}
