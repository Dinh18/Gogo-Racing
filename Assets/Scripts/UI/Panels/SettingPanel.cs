using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SettingPanel : Panel
{
    [SerializeField] private GameObject[] allContents;
    public void ShowContent(GameObject contentToShow)
    {
        foreach (var content in allContents)
        {
            content.SetActive(content == contentToShow);
        }
    }
}
