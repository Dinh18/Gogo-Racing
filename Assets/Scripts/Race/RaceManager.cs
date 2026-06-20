using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Playables;

public enum RaceState
{
    Preparatation,
    Cinimatic,
    CountDown,
    Racing,
    Finished,
    Scoreboard,
    Pause
}

public class RaceManager : MonoBehaviour
{
    public static RaceManager Instance { get; private set; }
    [SerializeField] private InGameUIManage inGameUIManage;
    [SerializeField] private RaceSetupManager raceSetupManager;
    [SerializeField] private CameraController cameraController;
    [Header("Race Settings")]
    public int totalLaps = 3;

    [Header("Race State")]
    public List<CarProgress> allCars = new List<CarProgress>();
    public List<CarProgress> finishedCars = new List<CarProgress>();
    [Header("Award")]
    // [SerializeField] private List<CarProgress> kartAward;
    [SerializeField] private List<GameObject> characterAwardPodium;
    [SerializeField] private List<GameObject> kartAwardPodium;
    [SerializeField] private int mapID;

    [Header("Audio")]
    [SerializeField] private AudioSource fireworksAudioSource;

    private RaceState previousState;
    private RaceState currState;

    void OnEnable()
    {
        CountDownUI.StartRacing += StartRacing;
        CarProgress.FinisedAllLap+=CheckFinisedRace;
    }

    void OnDisable()
    {
        CountDownUI.StartRacing -= StartRacing;
        CarProgress.FinisedAllLap -= CheckFinisedRace;

    }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        // Tự động tìm tất cả các xe trong Scene (Player và NPC)
        if(raceSetupManager != null) raceSetupManager.Setup(mapID);
        allCars = new List<CarProgress>(FindObjectsByType<CarProgress>(FindObjectsSortMode.None));
        foreach (CarProgress car in allCars)
        {
            car.Setup();
        }
        cameraController.Setup(GetPlayerKart().transform);
        inGameUIManage.Setup(allCars, GetPlayerKart().GetComponent<CarProgress>(), GetPlayerKart().GetComponent<CarMovement>(), GetPlayerKart().GetComponent<PlayerItemController>());
        if(GameManager.Instance.GetCurrState() == GameState.InGame) ChangeState(RaceState.Cinimatic);
        else if(GameManager.Instance.GetCurrState() == GameState.Tutorial) ChangeState(RaceState.Racing);
        UIManager.Instance.HideLoading();
    }

    private void Update()
    {
        CheckRaceFinishers();
        if (currState == RaceState.Finished)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                GameManager.Instance.ChangeGameState(GameState.MainMenu);
            }
        }

        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if(currState != RaceState.Pause)
            {
                ChangeState(RaceState.Pause);
            }
            else
            {
                ChangeState(previousState);
            }
        }

        if(currState == RaceState.Pause)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                GameManager.Instance.ChangeGameState(GameState.MainMenu);
            }
        }
         
    }

    public GameObject GetPlayerKart()
    {
        foreach(CarProgress kart in allCars)
        {
            if(kart.gameObject.CompareTag("Player")) return kart.gameObject;
        }
        return null;
    }

    public RaceState GetCurrState() => currState;

    private void CheckRaceFinishers()
    {
        int i = 1;
        foreach (var car in allCars)
        {
            // Nếu số vòng hiện tại lớn hơn số vòng đua yêu cầu (VD: Đua 3 vòng, khi chạm mốc vòng 4 thì hoàn thành)
            if (!car.HasFinished && car.currentLap > totalLaps)
            {
                finishedCars.Add(car);
                Debug.Log($"<color=yellow>Xe {car.gameObject.name} đã về đích ở vị trí thứ {finishedCars.Count}!</color>");
                car.FinishRace();
                i++;
            }
        }
        // if(i == allCars.Count) 
    }

    /// <summary>
    /// Hàm này dùng để gọi từ UI (hiển thị vị trí Top 1, 2, 3...)
    /// </summary>
    public int GetCarRank(CarProgress car)
    {
        if (car.isEliminated)
        {
            return allCars.Count;
        }

        if (car.HasFinished)
        {
            return finishedCars.Where(c => !c.isEliminated).ToList().IndexOf(car) + 1; // Xe đã về đích giữ nguyên thứ hạng
        }
        
        int finishedCount = finishedCars.Count(c => !c.isEliminated);

        // Xếp hạng các xe chưa về đích dựa trên tổng quãng đường (totalDistance)
        var activeCars = allCars
            .Where(c => !c.HasFinished && !c.isEliminated)
            .OrderByDescending(c => c.totalDistance)
            .ToList();

        return finishedCount + activeCars.IndexOf(car) + 1;
    }

    public void ChangeState(RaceState newState)
    {
        Debug.Log(currState + " -> " + newState);
        previousState = currState;
        currState = newState;
        inGameUIManage.UpdateUI(currState, previousState);
    }

    public void StartRacing()
    {
        ChangeState(RaceState.Racing);
    }

    private void CheckFinisedRace()
    {
        // Lấy rank người chơi và lưu lại tiến trình map
        GameObject playerKart = GetPlayerKart();
        if (playerKart != null)
        {
            CarProgress playerCar = playerKart.GetComponent<CarProgress>();
            
            // Chỉ kết thúc khi player đã về đích hoặc bị loại
            if (!playerCar.HasFinished && !playerCar.isEliminated) return;

            int playerRank = GetCarRank(playerCar);

            var allMaps = DataManager.Instance.GetAllMaps();
            int mapIndex = 0;
            for (int i = 0; i < allMaps.Length; i++)
            {
                if (allMaps[i].mapID == mapID)
                {
                    mapIndex = i;
                    break;
                }
            }
            DataManager.Instance.SaveMapResult(mapID, playerRank, mapIndex);
        }
        else
        {
            // Dự phòng nếu không có player (ví dụ trong lúc test)
            foreach(CarProgress race in allCars)
            {
                if(!race.HasFinished && !race.isEliminated) return;
            }
        }

        TriggerPodium();
    }

    private void TriggerPodium()
    {
        ChangeState(RaceState.Finished);
        List<CarProgress> top3Winners = GetTop3Karts();
        if(GameManager.Instance.GetCurrState() == GameState.Tutorial)
        {
            GameManager.Instance.ChangeGameState(GameState.MainMenu);
            return;
        }

        if (fireworksAudioSource != null)
        {
            fireworksAudioSource.Play();
        }

        if (top3Winners.Count > 0) Debug.Log($"<color=green>TOP 1: {top3Winners[0].gameObject.name}</color>");
        if (top3Winners.Count > 1) Debug.Log($"TOP 2: {top3Winners[1].gameObject.name}");
        if (top3Winners.Count > 2) Debug.Log($"TOP 3: {top3Winners[2].gameObject.name}");
        for(int i = 0; i < top3Winners.Count; i++)
        {
            foreach(Transform child in characterAwardPodium[i].transform)
            {
                Destroy(child.gameObject);
            }

            foreach(Transform child in kartAwardPodium[i].transform)
            {
                Destroy(child.gameObject);
            }

            GameObject charAward = Instantiate(top3Winners[i].GetComponent<CarController>().GetCharacterSO().mainMenuPrefab);
            charAward.transform.SetParent(characterAwardPodium[i].transform);
            charAward.transform.localPosition = Vector3.zero;
            charAward.transform.localScale = new Vector3(0.3f,0.3f,0.3f);
            GameObject kartAward = Instantiate(top3Winners[i].GetComponent<CarController>().GetKartDataSO().mainMenuPrefab);
            kartAward.transform.SetParent(kartAwardPodium[i].transform);
            kartAward.transform.localPosition = Vector3.zero;
            kartAward.transform.localScale = new Vector3(0.4f,0.4f,0.4f);
        }
    }

    public void EliminateCar(CarProgress car)
    {
        car.isEliminated = true;
        if (car.gameObject.CompareTag("Player"))
        {
            CheckFinisedRace();
        }
    }

    public List<CarProgress> GetTop3Karts()
    {
        List<CarProgress> topCars = new List<CarProgress>();

        // 1. Đưa các xe đã cán đích vào trước (chúng đã được add theo đúng thứ tự 1, 2, 3...)
        topCars.AddRange(finishedCars.Where(c => !c.isEliminated));

        // 2. Lấy các xe chưa cán đích, sắp xếp theo quãng đường giảm dần
        var activeCars = allCars
            .Where(c => !c.HasFinished && !c.isEliminated)
            .OrderByDescending(c => c.totalDistance)
            .ToList();

        // 3. Gộp mảng activeCars vào phía sau mảng topCars
        topCars.AddRange(activeCars);

        // 4. Dùng LINQ Take(3) để cắt lấy đúng 3 phần tử đầu tiên trong danh sách tổng
        return topCars.Take(3).ToList();
    }
}
