using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public float moveSpeed = 2f; // Скорость движения камеры
    public float zoomSpeed = 2f; // Скорость зумирования
    public float minZoom = 1f;   // Минимальный размер камеры (максимальное приближение)
    public float maxZoom = 4f;   // Максимальный размер камеры (максимальное отдаление)
    Vector2 minBounds;      // Минимальные границы для перемещения камеры
    Vector2 maxBounds;      // Максимальные границы для перемещения камеры
    private Camera cam;
    [SerializeField] private GameObject plane; // Ссылка на компонент Plane
    private bool canMoveAndZoom = true; // Флаг для контроля возможности зумирования и перемещения

    void Start()
    {
        cam = GetComponent<Camera>(); // Получаем компонент камеры
        SetBounds();
        SetStartPosition();
    }

    void SetBounds()
    {
        Vector3 planeSize = plane.GetComponent<RectTransform>().sizeDelta;
        float camHeight = cam.orthographicSize;
        float camWidth = camHeight * cam.aspect;

        // Устанавливаем границы на основе размера Plane и камеры
        minBounds = new Vector2(plane.transform.position.x - planeSize.x / 2 + camWidth,
                                plane.transform.position.y - planeSize.y / 2 + camHeight);
        maxBounds = new Vector2(plane.transform.position.x + planeSize.x / 2 - camWidth,
                                plane.transform.position.y + planeSize.y / 2 - camHeight);
    }

    void Update()
    {
        if (canMoveAndZoom)
        {
            // Движение камеры по горизонтали и вертикали
            float moveX = Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime;
            float moveY = Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime;

            // Новое положение камеры
            Vector3 newPosition = transform.position + new Vector3(moveX, moveY, 0);

            // Ограничиваем положение камеры в пределах карты (minBounds, maxBounds)
            newPosition.x = Mathf.Clamp(newPosition.x, minBounds.x, maxBounds.x);
            newPosition.y = Mathf.Clamp(newPosition.y, minBounds.y, maxBounds.y);

            // Применяем новое ограниченное положение камеры
            transform.position = newPosition;

            // Зумирование с помощью колёсика мыши
            float scrollData = Input.GetAxis("Mouse ScrollWheel");
            if (scrollData != 0)
            {
                cam.orthographicSize -= scrollData * zoomSpeed;
                // Ограничиваем зум минимальными и максимальными значениями
                cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, minZoom, maxZoom);
                // Обновляем границы при изменении масштаба
                SetBounds();
            }
        }
    }

    void SetStartPosition()
    {
        float initialX = plane.transform.position.x; // Центральная позиция по оси X
        float initialY = minBounds.y; 
        cam.transform.position = new Vector3(initialX, initialY, cam.transform.position.z); 
    }

    public void SetCameraMovementEnabled(bool enabled)
    {
        canMoveAndZoom = enabled;
    }
}
