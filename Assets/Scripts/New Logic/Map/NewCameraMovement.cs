using System.Collections;
using UnityEngine;
public class NewCameraMovement : MonoBehaviour
{
    public float moveSpeed = 3f;
    public float zoomSpeed = 2f;
    public float minZoom = 1f;
    public float maxZoom = 4f;

    private Vector2 minBounds;
    private Vector2 maxBounds;

    [SerializeField] private Camera cam;
    [SerializeField] private GameObject plane;

    private bool canMoveAndZoom = true;
    private Vector3 lastMouseWorldPosition;
    private bool isDragging = false;

    void Start()
    {
        if (cam == null)
            cam = Camera.main;

        SetBounds();
        SetStartPosition();

        GlobalWindowController.Instance.OnCanvasSwitched += HandleCanvasSwitched;
        var currentCanvas = GlobalWindowController.Instance.CurrentActiveCanvas;
        canMoveAndZoom = currentCanvas == GlobalWindowController.Instance.GlobalCanvas;
        isDragging = false;
    }


    private void HandleCanvasSwitched(GameObject newCanvas)
    {
        if (newCanvas == GlobalWindowController.Instance.GlobalCanvas)
        {
            Input.ResetInputAxes();
            canMoveAndZoom = true;
            isDragging = false;
        }
        else
        {
            canMoveAndZoom = false;
            isDragging = false;
        }
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

    void SetStartPosition()
    {
        float initialX = plane.transform.position.x;
        float initialY = minBounds.y;
        cam.transform.position = new Vector3(initialX, initialY, cam.transform.position.z);
    }
}
