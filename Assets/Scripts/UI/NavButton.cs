using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events; // Thêm thư viện này để dùng UnityEvent

[RequireComponent(typeof(Button))]
public class NavButton : MonoBehaviour
{
    [HideInInspector] public Button button;
    
    [Header("UI References")]
    [SerializeField] private GameObject bg;
    [SerializeField] private Text nameText;
    
    [Header("Sự kiện khi bấm nút (Tùy chọn)")]
    // Cái này giúp bạn kéo thả hàm mở Panel trực tiếp ngoài Inspector cực kỳ tiện
    public UnityEvent onTabSelected; 

    private string selectColorHex = "#0100B9";
    private Color selectColor;

    private void Awake()
    {
        button = GetComponent<Button>();
        ColorUtility.TryParseHtmlString(selectColorHex, out selectColor);
    }

    public void ButtonSelect()
    {
        bg.SetActive(true);
        nameText.color = Color.white;
        
        // Kích hoạt sự kiện để mở Panel tương ứng
        onTabSelected?.Invoke(); 
    }
    
    public void ButtonUnSelect()
    {
        bg.SetActive(false);
        nameText.color = selectColor;
    }
}