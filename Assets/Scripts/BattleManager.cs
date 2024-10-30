using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    private float timer = 5f; 
    private bool timerRunning = false;
    public GameObject battleBack;
    public Camera Camera;
    public Canvas battleCanvas;

    void OnEnable()
    {
        StartTimer();
        CameraAndMapSwitch(true);
        Camera.transform.position = new Vector3 (0, 0, 10);
        Camera.main.orthographicSize = 2f;
    }
    void Update()
    {
        if (timerRunning)
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
        CameraAndMapSwitch(false);
    }
    public void CameraAndMapSwitch(bool switcher)
    {
        CameraMovement cameraMovement = FindObjectOfType<CameraMovement>();
        if (cameraMovement != null)
        {
            cameraMovement.SetCameraMovementEnabled(!switcher);

        }
        battleBack.SetActive(switcher);
        gameObject.SetActive(switcher);
        battleCanvas.gameObject.SetActive(switcher);
    }
}

