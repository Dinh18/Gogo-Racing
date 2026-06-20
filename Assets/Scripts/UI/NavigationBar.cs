using System.Collections.Generic;
using UnityEngine;

public class NavigationBar : MonoBehaviour
{
    [SerializeField] private List<NavButton> buttons = new List<NavButton>();

    private void Start()
    {
        // Quét tự động 100% không cần kéo thả
        foreach (Transform child in this.transform)
        {
            NavButton navBtn = child.GetComponent<NavButton>();
            if (navBtn != null)
            {
                buttons.Add(navBtn);
                
                // Khi một nút bất kỳ bị bấm, truyền chính cái nút đó vào hàm SelectTab
                navBtn.button.onClick.AddListener(() => SelectTab(navBtn));
            }
        }
    }

    // Hàm tổng xử lý logic sáng/tối cho mọi Nav Bar
    public void SelectTab(NavButton clickedButton)
    {
        foreach (var btn in buttons)
        {
            if (btn == clickedButton)
            {
                btn.ButtonSelect(); // Nút được bấm thì sáng lên
            }
            else
            {
                btn.ButtonUnSelect(); // Các nút khác thì tối đi
            }
        }
    }
}