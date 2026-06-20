using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Playables;

public class InGameUIManage : MonoBehaviour
{
    [SerializeField] private CountDownUI countDownUI;
    [SerializeField] private LeaderBoardUI leaderBoardUI;
    [SerializeField] private RaceProgressUI raceProgressUI;
    [SerializeField] private ItemUI itemUI;
    [SerializeField] private Speedometer_Dial speedometer_Dial;
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject skipButton;
    [SerializeField] private PlayableDirector introDirector;
    [SerializeField] private CinemachineCamera[] allVirtualCameras;
    [SerializeField] private CinemachineCamera awardCam;
    [SerializeField] private CinemachineCamera playerCam;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Setup(List<CarProgress> allCars, CarProgress playerProgress, CarMovement playerMovement, PlayerItemController playerItemController)
    {
        if(playerItemController == null) Debug.LogError("PlayerItemController is null in InGameUIManage.Setup");
        leaderBoardUI.Setup(allCars);
        raceProgressUI.Setup(playerProgress);
        speedometer_Dial.Setup(playerMovement);
        itemUI.Setup(playerItemController);
    }
    public void UpdateUI(RaceState newState, RaceState previousState)
    {
        ToggleUI(false);
        pauseMenu.SetActive(false);
        Time.timeScale = 1;
        switch(newState)
        {
            case RaceState.Preparatation:
                break;
            case RaceState.Cinimatic:
                ToggleUI(false);
                ToggleSkipButton(true);
                StartCoroutine(SetupRaceRoutine());
                break;
            case RaceState.CountDown:
                ToggleUI(true);
                ToggleSkipButton(false);
                countDownUI.gameObject.SetActive(true);
                if(previousState != RaceState.Pause) StartCoroutine(countDownUI.HandleCountdown(3));
                break;
            case RaceState.Racing:
                ToggleUI(true);
                countDownUI.gameObject.SetActive(false);
                ToggleSkipButton(false);
                SwitchCamera(playerCam);
                break;
            case RaceState.Finished:
                ToggleUI(false);
                ToggleSkipButton(true);
                SwitchCamera(awardCam);
                break;
            case RaceState.Scoreboard:
                ToggleUI(false);
                ToggleSkipButton(true);
                break;
            case RaceState.Pause:
                ToggleUI(false);
                ToggleSkipButton(false);
                pauseMenu.SetActive(true);
                Time.timeScale = 0;
                break;
        }
    }

    public IEnumerator SetupRaceRoutine()
    {
        yield return null;

        if(introDirector!=null)
        {
            introDirector.Play();
            yield return new WaitUntil(() =>introDirector.state != PlayState.Playing || Input.GetKeyDown(KeyCode.Space));
            introDirector.Stop();
            RaceManager.Instance.ChangeState(RaceState.CountDown);
        }
    }

    public void SwitchCamera(CinemachineCamera targetCam)
    {
        foreach(CinemachineCamera cam in allVirtualCameras)
        {
            if (cam != null) cam.Priority = 10;
        }
        if (targetCam != null)
        {
            targetCam.Priority = 20;
        }
    }

    public void ToggleUI(bool isActive)
    {
        countDownUI.gameObject.SetActive(isActive);
        leaderBoardUI.gameObject.SetActive(isActive);
        raceProgressUI.gameObject.SetActive(isActive);
        itemUI.gameObject.SetActive(isActive);
        speedometer_Dial.gameObject.SetActive(isActive);
        // skipButton.SetActive(!isActive);
    }

    public void ToggleSkipButton(bool isActive)
    {
        skipButton.SetActive(isActive);
    }
}
