using UnityEngine;
using UnityEngine.UI;

public class LeaderboardSlotUI : MonoBehaviour
{
    [SerializeField] private Text topText;
    [SerializeField] private Text nameText;
    public void Setup(int top, string name)
    {
        topText.text = top.ToString();
        nameText.text = name;
    }
}
