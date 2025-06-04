using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement; // Для управления сценами

public class ExitConfirmation : MonoBehaviour
{
    public GameObject exitConfirmationPanel; // Панель с подтверждением выхода
    public GameObject additionalPanel; // Вторая панель, которая также будет закрывать обе панели
    public Text exitQuestion; // Вопрос о выходе из приложения
    public Button yesButton; // Кнопка "Да"
    public Button noButton;  // Кнопка "Нет"

    public bool isPanelActive = false; // Флаг для отслеживания состояния панелей

    // Singleton pattern
    public static ExitConfirmation Instance { get; private set; }

    private void Awake()
    {
        // Если экземпляр уже существует, уничтожаем новый
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Application.targetFrameRate = 60;
            QualitySettings.vSyncCount = 0;
        }
    }

    private void Start()
    {
        // Убедитесь, что панели выключены при старте
        exitConfirmationPanel.SetActive(false);
        additionalPanel.SetActive(false);

        // Подключаем обработчики событий для кнопок
        yesButton.onClick.AddListener(OnYesButtonClicked);
        noButton.onClick.AddListener(OnNoButtonClicked);

        // Устанавливаем обработчик кликов для дополнительной панели
        AddClickListenerToPanel(additionalPanel, OnAdditionalPanelClicked);

        // Проверка нажатия кнопки Escape
        SceneManager.sceneLoaded += OnSceneLoaded;

        // Обновление текста при запуске
        UpdateExitQuestionText();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Включаем панели, если они были активны в предыдущей сцене
        exitConfirmationPanel.SetActive(false);
        additionalPanel.SetActive(false);

        // Проверяем, нужно ли показывать панели при загрузке сцены
        if (isPanelActive)
        {
            exitConfirmationPanel.SetActive(true);
            additionalPanel.SetActive(true);
        }

        // Обновляем текст при загрузке сцены
        UpdateExitQuestionText();
    }

    private void Update()
    {
        // Проверка нажатия кнопки Escape
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (exitConfirmationPanel.activeSelf)
            {
                // Если панель подтверждения выхода активна, скрыть обе панели
                exitConfirmationPanel.SetActive(false);
                additionalPanel.SetActive(false);
            }
            else
            {
                // Показать панель подтверждения выхода и дополнительную панель
                exitConfirmationPanel.SetActive(true);
                additionalPanel.SetActive(true);
            }

            // Обновляем текст при активации панели
            UpdateExitQuestionText();
        }
    }

    private void OnYesButtonClicked()
    {
        // Закрыть приложение
        Application.Quit();

        // Если в редакторе Unity, остановить воспроизведение
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    private void OnNoButtonClicked()
    {
        // Скрыть только панель подтверждения выхода
        exitConfirmationPanel.SetActive(false);
        additionalPanel.SetActive(false);
        isPanelActive = false;
    }

    private void OnAdditionalPanelClicked()
    {
        // Скрыть обе панели
        exitConfirmationPanel.SetActive(false);
        additionalPanel.SetActive(false);
        isPanelActive = false;
    }

    private void AddClickListenerToPanel(GameObject panel, System.Action onClickAction)
    {
        // Проверяем, есть ли у панели компонент EventTrigger, если нет - добавляем его
        EventTrigger eventTrigger = panel.GetComponent<EventTrigger>();
        if (eventTrigger == null)
        {
            eventTrigger = panel.AddComponent<EventTrigger>();
        }

        // Добавляем обработчик клика
        EventTrigger.Entry entry = new EventTrigger.Entry
        {
            eventID = EventTriggerType.PointerClick
        };
        entry.callback.AddListener((data) => { onClickAction?.Invoke(); });
        eventTrigger.triggers.Add(entry);
    }

    public void UpdateExitQuestionText()
    {
        // Получаем текущий язык из LocalizationManager и обновляем текст с помощью GetText
        exitQuestion.text = LocalizationManager.GetText("exitQuestion");
        yesButton.GetComponentInChildren<Text>().text = LocalizationManager.GetText("yesButton");
        noButton.GetComponentInChildren<Text>().text = LocalizationManager.GetText("noButton");
    }
}
