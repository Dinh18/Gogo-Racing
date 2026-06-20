using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialController : MonoBehaviour
{
    [SerializeField] private Transform spawnsPoint;
    [SerializeField] private List<GameObject> tutArea;
    [SerializeField] private Text tutText;
    [SerializeField] private GameObject instructionPanel; // Gán Panel chứa Text vào đây nếu có
    [SerializeField] private GameObject mainPanel;
    [SerializeField] private List<string> intruction = new List<string> {"W to move forward","W + A to turn left"};
    [SerializeField] private float triggerDistance = 15f; // Khoảng cách tới tutArea để hiện instruction
    private int tutIndex = 0;
    private bool isShowingInstruction = false;
    private Transform playerTransform;

    void Start()
    {
        Setup();
        StatTut(); // Hiện tutorial đầu tiên
    }

    void Update()
    {
        // Khi đang hiện instruction, nhấn Space để tắt
        if (isShowingInstruction && Input.GetKeyDown(KeyCode.Space))
        {
            CloseInstruction();
        }

        // Kiểm tra khoảng cách để hiện tutorial tiếp theo
        if (!isShowingInstruction && playerTransform != null)
        {
            int nextTutIndex = tutIndex + 1; // Instruction tiếp theo sẽ tương ứng với Area tiếp theo

            // Kiểm tra xem còn instruction nào không và có tutArea tương ứng không
            if (nextTutIndex < intruction.Count && nextTutIndex < tutArea.Count)
            {
                if (tutArea[nextTutIndex] != null)
                {
                    float dist = Vector3.Distance(playerTransform.position, tutArea[nextTutIndex].transform.position);
                    if (dist < triggerDistance)
                    {
                        tutIndex++;
                        StatTut();
                    }
                }
            }
        }
    }

    private void Setup()
    {
        KartDataSO tutorialKart = DataManager.Instance.GetCurrentItem<KartDataSO>(DataManager.Instance.GetAllKarts(), DataManager.Instance.GetCurrentKartID());
        CharacterSO tutorialChar = DataManager.Instance.GetCurrentItem<CharacterSO>(DataManager.Instance.GetAllCharacters(), DataManager.Instance.GetCurrentCharacterID());

        GameObject carObj = Instantiate(tutorialKart.ingamePrefab, spawnsPoint.position, spawnsPoint.rotation);
        carObj.tag = "Player";
        carObj.AddComponent<PlayerInputController>();
        CarController controller = carObj.GetComponent<CarController>();
        controller.Setup(tutorialChar, tutorialKart);
        
        playerTransform = carObj.transform;
    }

    public void StatTut()
    {
        if (tutIndex < intruction.Count)
        {
            tutText.text = intruction[tutIndex];
            
            if (instructionPanel != null) instructionPanel.SetActive(true);
            else tutText.gameObject.SetActive(true);

            if (mainPanel != null) mainPanel.SetActive(false);
            
            Time.timeScale = 0.2f;
            isShowingInstruction = true;
        }
    }

    // Gán hàm này vào sự kiện OnClick của Button tắt Instruction (hoặc có thể dùng phím)
    public void CloseInstruction()
    {
        if (isShowingInstruction)
        {
            Time.timeScale = 1f;
            
            if (instructionPanel != null) instructionPanel.SetActive(false);
            else tutText.gameObject.SetActive(false);

            if (mainPanel != null) mainPanel.SetActive(true);
            
            isShowingInstruction = false;
        }
    }
}
