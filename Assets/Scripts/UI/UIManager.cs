using System.Collections;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    [SerializeField] private GameObject loadingImage;
    void Awake()
    {
        Instance = this;
    }
    public void ShowLoading()
    {
        loadingImage.SetActive(true);
    }

    public void HideLoading()
    {
        loadingImage.SetActive(false);
    }
}
