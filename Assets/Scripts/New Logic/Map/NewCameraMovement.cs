using UnityEngine;

public class NewCameraMovement : MonoBehaviour
{
    public float moveSpeed = 3f;     // Скорость движения камеры
    public float zoomSpeed = 2f;     // Скорость зумирования
    public float minZoom = 1f;       // Минимальный размер камеры (максимальное приближение)
    public float maxZoom = 4f;       // Максимальный размер камеры (максимальное отдаление)

    private Vector2 minBounds;
    private Vector2 maxBounds;

    private Camera cam;
    [SerializeField] private GameObject plane;  // Ссылка на компонент Plane
    private bool canMoveAndZoom = true;         // Флаг для контроля возможности зумирования и перемещения

    private Vector3 lastMouseWorldPosition;     // Последняя позиция мыши в мировых координатах
    private bool isDragging = false;

    void Start()
    {
        cam = GetComponent<Camera>();
        SetBounds();
        SetStartPosition();
    }

    void SetBounds()
    {
        Vector3 planeSize = plane.GetComponent<RectTransform>().sizeDelta;

        float camHeight = cam.orthographicSize;
        float camWidth = camHeight * cam.aspect;

        minBounds = new Vector2(
            plane.transform.position.x - planeSize.x / 2 + camWidth,
            plane.transform.position.y - planeSize.y / 2 + camHeight
        );
        maxBounds = new Vector2(
            plane.transform.position.x + planeSize.x / 2 - camWidth,
            plane.transform.position.y + planeSize.y / 2 - camHeight
        );
    }

    void Update()
    {
        if (canMoveAndZoom)
        {
            HandlePanning();
            HandleZooming();
        }
    }

    void HandlePanning()
    {
        if (Input.GetMouseButtonDown(0))
        {
            isDragging = true;
            lastMouseWorldPosition = cam.ScreenToWorldPoint(Input.mousePosition);
        }

        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }

        if (isDragging)
        {
            Vector3 currentMouseWorldPosition = cam.ScreenToWorldPoint(Input.mousePosition);
            Vector3 delta = lastMouseWorldPosition - currentMouseWorldPosition;

            Vector3 newPosition = transform.position + delta;

            newPosition.x = Mathf.Clamp(newPosition.x, minBounds.x, maxBounds.x);
            newPosition.y = Mathf.Clamp(newPosition.y, minBounds.y, maxBounds.y);

            transform.position = newPosition;
            lastMouseWorldPosition = cam.ScreenToWorldPoint(Input.mousePosition);
        }
    }

    void HandleZooming()
    {
        float scrollData = Input.GetAxis("Mouse ScrollWheel");
        if (scrollData != 0)
        {
            Vector3 mouseWorldPosBefore = cam.ScreenToWorldPoint(Input.mousePosition);

            cam.orthographicSize -= scrollData * zoomSpeed;
            cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, minZoom, maxZoom);

            SetBounds();

            Vector3 mouseWorldPosAfter = cam.ScreenToWorldPoint(Input.mousePosition);
            Vector3 adjustment = mouseWorldPosBefore - mouseWorldPosAfter;

            Vector3 newPosition = transform.position + adjustment;
            newPosition.x = Mathf.Clamp(newPosition.x, minBounds.x, maxBounds.x);
            newPosition.y = Mathf.Clamp(newPosition.y, minBounds.y, maxBounds.y);

            transform.position = newPosition;
        }
    }

    void SetStartPosition()
    {
        float initialX = plane.transform.position.x;
        float initialY = minBounds.y;
        cam.transform.position = new Vector3(initialX, initialY, cam.transform.position.z);
    }

    public void SetCameraMovementEnabled(bool enabled)
    {
        canMoveAndZoom = enabled;
    }
}
