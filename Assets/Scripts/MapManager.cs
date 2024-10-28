using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    public GameObject map;
    public Camera mapCamera;
    bool switcher = false;

    public void CameraAndMapSwitch()
    {
        CameraMovement cameraMovement = FindObjectOfType<CameraMovement>();
        if (cameraMovement != null)
        {
            cameraMovement.SetCameraMovementEnabled(switcher);
            
        }
        if (map != null)
        {
            map.SetActive(switcher); // Включение/выключение карты
        }
        switcher = !switcher; // Инвертируем значение
    }

}
