using System.Collections.Generic;
using UnityEngine;

public class KartVFXManager : MonoBehaviour
{
    [Header("Hiệu ứng hạt")]
    [SerializeField] ParticleSystem[] sandDustParticles;
    [SerializeField] private CarMovement carMovement;
    [Header("Hiệu ứng đuôi")]
    [SerializeField] private TrailRenderer[] skidMarks;
    [SerializeField] private CarDrift playerDrift;
    private float maxEmissionRate = 100f;
    void OnEnable()
    {
        if(playerDrift != null) playerDrift.OnDriftStateChange += HandleDrift;
    }
    void OnDisable()
    {
        if(playerDrift != null) playerDrift.OnDriftStateChange -= HandleDrift;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        UpdateSandDust(carMovement.currSpeed, carMovement.GetDefaultMoveSpeed() * carMovement.amountAccelerate);
    }

    private void UpdateSandDust(float currSpeed, float maxSpeed)
    {
        float speedRatio = Mathf.Clamp01(currSpeed/maxSpeed);
        float targetEmissionRate = speedRatio * maxEmissionRate;
        // Debug.Log(targetEmissionRate);
        foreach(ParticleSystem ps in sandDustParticles)
        {
            var emission = ps.emission;
            if(currSpeed >= 1f)
            {
                emission.enabled = true;
                emission.rateOverTime = targetEmissionRate;
            }
            else
            {
                emission.enabled = false;
            }
        }
    }
    private void HandleDrift(bool isDrifting)
    {
        // Debug.Log("Is Drift");
        foreach (TrailRenderer trail in skidMarks)
        {
            trail.emitting = isDrifting;
        }
    }
}
