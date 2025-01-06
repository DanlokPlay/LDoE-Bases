using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CameraController : MonoBehaviour
{
    public Vector2 fieldSize = new Vector2(64, 64);
    public float zoomSpeed = 0.01f;
    public float minZoom = 4f;
    public float maxZoom = 15f;
    public float dragSpeed = 0.5f;

    //public GameObject infoPanel;
    public Button changeSceneButton;
    //public Button showInfoButton;
    //public Button closeInfoButton;
    public GameObject infoCanvas;
    public Button openInfo;
    public Button closeInfo;

    public TextMeshProUGUI nameBase;
    public TextMeshProUGUI needBanBase;
    public TextMeshProUGUI minedResources;

    public GameObject FurniturePanel;

    private RectTransform _infoPanelRectTransform;
    private Vector3 _lastTouchPosition;
    private Vector3 _lastMousePosition;
    private bool _isDragging;
    private bool _isDraggingFromUI;

    void Start()
    {

        // Загружаем локализацию
        LocalizationManager.LoadLocalization();

        // Устанавливаем текущий язык
        LocalizationManager.CurrentLanguage = Data.CurrentLanguage;

        // Пример использования
        changeSceneButton.onClick.AddListener(ChangeScene);
        openInfo.onClick.AddListener(OnInfoButtonClicked);
        closeInfo.onClick.AddListener(BackFromInfoButtonClicked);

        infoCanvas.SetActive(false);
        FurniturePanel.SetActive(false);

        string raidName = GameData.RaidName ?? LocalizationManager.GetText("noRaidData");
        bool needBan = GameData.NeedBan;
        string fileContent = GameData.ResourceObjects ?? LocalizationManager.GetText("noResourceData");

        // Устанавливаем тексты на основе локализации
        nameBase.text = $"{LocalizationManager.GetText("baseName")}: {raidName}";
        needBanBase.text = $"{LocalizationManager.GetText("needBan")}: {(needBan ? LocalizationManager.GetText("yesButton") : LocalizationManager.GetText("noButton"))}";
        minedResources.text = $"{LocalizationManager.GetText("minedResources")}:\n{fileContent}";

        GameData.ClearGameData();

        Vector3 startPosition = new Vector3(17f, 7.1f, Camera.main.transform.position.z);
        MoveCamera(startPosition);

        Camera.main.orthographicSize = 10f;
    }

    void Update()
    {
        HandleCameraMovement();
        HandleCameraZoom();
    }

    private void HandleCameraMovement()
    {
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                _isDragging = true;
                _isDraggingFromUI = IsPointerOverUI();
                _lastTouchPosition = touch.position;
            }
            else if (touch.phase == TouchPhase.Moved && _isDragging && !_isDraggingFromUI)
            {
                Vector3 delta = Camera.main.ScreenToWorldPoint(touch.position) - Camera.main.ScreenToWorldPoint(_lastTouchPosition);
                _lastTouchPosition = touch.position;

                Vector3 newPosition = transform.position - delta * dragSpeed;
                MoveCamera(newPosition);
            }
            else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                _isDragging = false;
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            _isDragging = true;
            _isDraggingFromUI = IsPointerOverUI();
            _lastMousePosition = Input.mousePosition;
        }
        if (Input.GetMouseButtonUp(0))
        {
            _isDragging = false;
        }
        if (_isDragging && !_isDraggingFromUI)
        {
            Vector3 delta = Camera.main.ScreenToWorldPoint(Input.mousePosition) - Camera.main.ScreenToWorldPoint(_lastMousePosition);
            _lastMousePosition = Input.mousePosition;

            Vector3 newPosition = transform.position - delta * dragSpeed;
            MoveCamera(newPosition);
        }
    }

    private void MoveCamera(Vector3 newPosition)
    {
        newPosition.x = Mathf.Clamp(newPosition.x, 0, 36);
        newPosition.y = Mathf.Clamp(newPosition.y, 0, 32);
        transform.position = newPosition;
    }

    private void HandleCameraZoom()
    {
        if (IsPointerOverUI())
        {
            return;
        }

        if (Input.touchCount == 2)
        {
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            float prevMagnitude = (touchZero.position - touchZero.deltaPosition - (touchOne.position - touchOne.deltaPosition)).magnitude;
            float currentMagnitude = (touchZero.position - touchOne.position).magnitude;

            float difference = currentMagnitude - prevMagnitude;

            AdjustZoom(difference * zoomSpeed);
        }
        else
        {
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            AdjustZoom(scroll * 10);
        }
    }

    private void AdjustZoom(float increment)
    {
        float newZoom = Camera.main.orthographicSize - increment;
        newZoom = Mathf.Clamp(newZoom, minZoom, maxZoom);
        Camera.main.orthographicSize = newZoom;

        Vector3 newPosition = transform.position;
        newPosition.x = Mathf.Clamp(newPosition.x, 0, 36);
        newPosition.y = Mathf.Clamp(newPosition.y, 0, 32);
        transform.position = newPosition;
    }

    private void ChangeScene()
    {
        SceneManager.LoadScene("Main");
    }

    /*private void ShowInfo()
    {
        infoPanel.SetActive(true);
        FurniturePanel.SetActive(false);
        showInfoButton.gameObject.SetActive(false);
    }

    private void CloseInfo()
    {
        infoPanel.SetActive(false);
        showInfoButton.gameObject.SetActive(true);
    }*/

    // Метод для проверки, находится ли указатель мыши или пальца над UI элементом
    private bool IsPointerOverUI()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return true;
        }

        if (Input.touchCount > 0 && EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
        {
            return true;
        }

        return false;
    }

    void OnInfoButtonClicked()
    {
        infoCanvas.SetActive(true);
    }

    void BackFromInfoButtonClicked()
    {
        infoCanvas.SetActive(false);
    }
}