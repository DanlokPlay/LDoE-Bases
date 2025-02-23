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
    public float dragSpeed = 1f;

    public Button changeSceneButton;
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

    private Image infoImage;

    void Start()
    {
        // Загружаем локализацию
        LocalizationManager.LoadLocalization();

        // Устанавливаем текущий язык
        LocalizationManager.CurrentLanguage = Data.CurrentLanguage;

        infoImage = infoCanvas.transform.Find("InfoGWDW").GetComponent<Image>();
        if (infoImage != null)
        {
            string imageName = Data.CurrentLanguage == "ru" ? "InfoGWDW_ru" : "InfoGWDW_en";
            Sprite infoSprite = UnityEngine.Resources.Load<Sprite>(imageName);
            if (infoSprite != null)
            {
                infoImage.sprite = infoSprite;
            }
        }

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
#if UNITY_ANDROID || UNITY_IOS
        if (Input.touchCount == 1)
        {
            HandleCameraMovement();
        }
        else if (Input.touchCount == 2)
        {
            HandleCameraZoom();
        }
#endif

#if UNITY_EDITOR || UNITY_STANDALONE
        HandleMouseMovement();
        HandleMouseZoom();
#endif
    }

    private void HandleMouseMovement()
    {
        if (Input.GetMouseButtonDown(0)) // Левая кнопка мыши нажата
        {
            _isDragging = true;
            _isDraggingFromUI = IsPointerOverUI();
            _lastMousePosition = Input.mousePosition;
        }
        else if (Input.GetMouseButton(0) && _isDragging && !_isDraggingFromUI) // Двигаем, если удерживаем кнопку
        {
            Vector3 delta = (Vector3)((Vector2)(Input.mousePosition - _lastMousePosition) *
                            (Camera.main.orthographicSize * 2f / Screen.height));
            _lastMousePosition = Input.mousePosition;

            Vector3 newPosition = transform.position - delta;
            MoveCamera(newPosition);
        }
        else if (Input.GetMouseButtonUp(0)) // Отпустили кнопку — сбрасываем флаг
        {
            _isDragging = false;
        }
    }

    private void HandleMouseZoom()
    {
        if (IsPointerOverUI()) return;

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) > 0.01f) // Проверяем, есть ли прокрутка
        {
            AdjustZoom(scroll * 5f); // Убрали минус, теперь зум корректный
        }
    }


    private void HandleCameraMovement()
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
            // Конвертируем пиксельное смещение в мировые координаты с учетом масштаба камеры
            Vector3 delta = (Vector3)(touch.deltaPosition * (Camera.main.orthographicSize * 2f / Screen.height));
            Vector3 newPosition = transform.position - delta;

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

        Touch touchZero = Input.GetTouch(0);
        Touch touchOne = Input.GetTouch(1);

        float prevMagnitude = (touchZero.position - touchZero.deltaPosition - (touchOne.position - touchOne.deltaPosition)).magnitude;
        float currentMagnitude = (touchZero.position - touchOne.position).magnitude;

        float difference = currentMagnitude - prevMagnitude;

        AdjustZoom(difference * zoomSpeed);
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


    void OnInfoButtonClicked()
    {
        infoCanvas.SetActive(true);
    }

    void BackFromInfoButtonClicked()
    {
        infoCanvas.SetActive(false);
    }

    // Метод для проверки, находится ли указатель мыши или пальца над UI элементом
    private bool IsPointerOverUI()
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return true;
        }
#endif

#if UNITY_ANDROID || UNITY_IOS
            if (Input.touchCount > 0 && EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
        {
            return true;
        }
#endif
        return false;
    }
}