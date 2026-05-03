using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class RaceManager : MonoBehaviour
{
    public static RaceManager Instance { get; private set; }

    [Header("Race Settings")]
    public int totalLaps = 3;

    [Header("Race State")]
    public List<CarProgress> allCars = new List<CarProgress>();
    public List<CarProgress> finishedCars = new List<CarProgress>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        // Tự động tìm tất cả các xe trong Scene (Player và NPC)
        allCars = new List<CarProgress>(Object.FindObjectsByType<CarProgress>(FindObjectsSortMode.None));
    }

    private void Update()
    {
        CheckRaceFinishers();
    }

    private void CheckRaceFinishers()
    {
        foreach (var car in allCars)
        {
            // Nếu số vòng hiện tại lớn hơn số vòng đua yêu cầu (VD: Đua 3 vòng, khi chạm mốc vòng 4 thì hoàn thành)
            if (!car.HasFinished && car.currentLap > totalLaps)
            {
                car.FinishRace();
                finishedCars.Add(car);
                Debug.Log($"<color=yellow>Xe {car.gameObject.name} đã về đích ở vị trí thứ {finishedCars.Count}!</color>");
            }
        }
    }

    /// <summary>
    /// Hàm này dùng để gọi từ UI (hiển thị vị trí Top 1, 2, 3...)
    /// </summary>
    public int GetCarRank(CarProgress car)
    {
        if (car.HasFinished)
        {
            return finishedCars.IndexOf(car) + 1; // Xe đã về đích giữ nguyên thứ hạng
        }
        
        int finishedCount = finishedCars.Count;

        // Xếp hạng các xe chưa về đích dựa trên tổng quãng đường (totalDistance)
        var activeCars = allCars
            .Where(c => !c.HasFinished)
            .OrderByDescending(c => c.totalDistance)
            .ToList();

        return finishedCount + activeCars.IndexOf(car) + 1;
    }
}
