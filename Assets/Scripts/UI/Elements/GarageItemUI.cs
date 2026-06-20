using System;
using UnityEngine;
using UnityEngine.UI;

public class GarageItemUI : MonoBehaviour
{
    public Image avatarImage;
    public Text nameText;
    public GameObject selectdImage;
    public Button button;
    public ItemDataSO itemData;
    public Color selectColor;
    private Action<ItemDataSO> onItemSelected;
    void Start()
    {
        // Gắn hàm OnClickItem vào nút bấm của Unity
        if (button != null)
        {
            button.onClick.AddListener(OnClickItem);
        }
    }
    
    public void Setup(Sprite avatarSprite, string name, ItemDataSO itemData, Action<ItemDataSO> onItemSelected, bool isUnlocked = true)
    {
        avatarImage.sprite = avatarSprite;
        nameText.text = name;
        this.itemData = itemData;
        this.onItemSelected = onItemSelected;

        if (button != null)
        {
            button.interactable = isUnlocked;
        }

        if (!isUnlocked)
        {
            avatarImage.color = Color.gray;
        }
        else
        {
            avatarImage.color = Color.white;
        }
    }

    public void SelectedItem()
    {
        selectdImage.SetActive(true);
        nameText.color = selectColor;
    }

    public void UnselectedItem()
    {
        selectdImage.SetActive(false);
        nameText.color = Color.white;
    }

    private void OnClickItem()
    {
        onItemSelected?.Invoke(itemData);
    }
    
}
