using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    private float timer = 5f; 
    private bool timerRunning = false;
    public GameObject battleBack;
    public Camera Camera;
    public GameObject mapManager;

    void OnEnable()
    {   
        StartTimer();
        EnableBattleSystem(true);

    }
    void Update()
    {
        if (timerRunning == true)
        {
            timer -= Time.deltaTime; 
            if (timer <= 0)
            {
                timerRunning = false;
                TimerFinished(); 
            }
        }
    }
    public void StartTimer()
    {
        timer = 5f;
        timerRunning = true;
    }
    private void TimerFinished()
    {
        gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        EnableBattleSystem(false);
        GameManager.Instance.EnableManager(mapManager, true);

    }

    void EnableBattleSystem(bool enable)
    {
        CameraMovement cameraMovement = FindObjectOfType<CameraMovement>();
        if (cameraMovement != null)
        {
            cameraMovement.SetCameraMovementEnabled(!enable);

        }
        if (battleBack != null)
            battleBack.SetActive(enable);

    }
}

