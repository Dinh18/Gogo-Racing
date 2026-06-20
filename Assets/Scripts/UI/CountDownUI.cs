using System;
using System.Collections;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CountDownUI : MonoBehaviour
{
    [SerializeField] private Text countdownText;
    public static event Action StartRacing;
    
    public IEnumerator HandleCountdown(int time)
    {
        countdownText.gameObject.SetActive(true);
        int count = time;
        while(count > 0)
        {
            countdownText.text = count.ToString();
            countdownText.transform.localScale = Vector3.zero;
            countdownText.transform.transform.DOScale(1.5f,0.5f).SetEase(Ease.OutBack);

            yield return new WaitForSeconds(1);
            count --;
        }

        countdownText.text = "GO!";
        countdownText.color = Color.green; // Đổi sang màu xanh cho ngầu
        countdownText.transform.localScale = Vector3.zero;
        countdownText.transform.DOScale(2f, 0.5f).SetEase(Ease.OutElastic); // Hiệu ứng GO nảy mạnh hơn

        // Đợi một chút cho người chơi đọc được chữ GO!
        yield return new WaitForSeconds(1f);

        // Hiệu ứng mờ dần (Fade out) rồi tắt hẳn
        countdownText.DOFade(0f, 0.5f).OnComplete(() => {
            countdownText.gameObject.SetActive(false);
        });
        StartRacing?.Invoke();
    }
}
