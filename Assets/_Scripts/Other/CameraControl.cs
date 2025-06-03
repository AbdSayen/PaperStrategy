using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public float scrollZoomSpeed = 0.1f;
    public float pinchZoomSpeed = 0.05f;
    public float minZoom = 5f;
    public float maxZoom = 20f;

    private Camera mainCamera;
    private Vector3 previousMousePosition;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        if (Application.isMobilePlatform)
        {
            HandleTouchInput();
        }
        else
        {
            HandleMouseInput();
        }
    }

    private void HandleMouseInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            previousMousePosition = Input.mousePosition;
        }
        if (Input.GetMouseButton(0))
        {
            Vector3 currentMousePosition = Input.mousePosition;
            Vector3 screenDelta = currentMousePosition - previousMousePosition;

            Vector3 worldDelta = mainCamera.ScreenToWorldPoint(new Vector3(screenDelta.x, screenDelta.y, mainCamera.transform.position.z)) - mainCamera.ScreenToWorldPoint(new Vector3(0, 0, mainCamera.transform.position.z));
            mainCamera.transform.position -= worldDelta; 

            previousMousePosition = currentMousePosition;
        }

        float scrollDelta = Input.mouseScrollDelta.y;
        if (scrollDelta != 0)
        {
            float newZoom = mainCamera.orthographicSize - scrollDelta * scrollZoomSpeed;
            mainCamera.orthographicSize = Mathf.Clamp(newZoom, minZoom, maxZoom);
        }
    }

    private void HandleTouchInput()
    {
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                previousMousePosition = touch.position;
            }
            else if (touch.phase == TouchPhase.Moved)
            {
                Vector2 screenDelta = touch.position - (Vector2)previousMousePosition;
                Vector3 worldDelta = mainCamera.ScreenToWorldPoint(new Vector3(screenDelta.x, screenDelta.y, mainCamera.transform.position.z)) - mainCamera.ScreenToWorldPoint(new Vector3(0, 0, mainCamera.transform.position.z));
                mainCamera.transform.position -= worldDelta;

                previousMousePosition = touch.position;
            }
        }

        if (Input.touchCount == 2)
        {
            Touch touch1 = Input.GetTouch(0);
            Touch touch2 = Input.GetTouch(1);

            if (touch1.phase == TouchPhase.Moved || touch2.phase == TouchPhase.Moved)
            {
                float prevTouchDeltaMag = (touch1.position - touch1.deltaPosition - (touch2.position - touch2.deltaPosition)).magnitude;
                float touchDeltaMag = (touch1.position - touch2.position).magnitude;
                float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

                float newZoom = mainCamera.orthographicSize + deltaMagnitudeDiff * pinchZoomSpeed * Time.deltaTime;
                mainCamera.orthographicSize = Mathf.Clamp(newZoom, minZoom, maxZoom);
            }
        }
    }
}
