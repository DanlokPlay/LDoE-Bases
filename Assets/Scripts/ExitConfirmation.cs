using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement; // ��� ���������� �������

public class ExitConfirmation : MonoBehaviour
{
    public GameObject exitConfirmationPanel; // ������ � �������������� ������
    public GameObject additionalPanel; // ������ ������, ������� ����� ����� ��������� ��� ������
    public Text exitQuestion; // ������ � ������ �� ����������
    public Button yesButton; // ������ "��"
    public Button noButton;  // ������ "���"

    public bool isPanelActive = false; // ���� ��� ������������ ��������� �������

    // Singleton pattern
    public static ExitConfirmation Instance { get; private set; }

    private void Awake()
    {
        // ���� ��������� ��� ����������, ���������� �����
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
        // ���������, ��� ������ ��������� ��� ������
        exitConfirmationPanel.SetActive(false);
        additionalPanel.SetActive(false);

        // ���������� ����������� ������� ��� ������
        yesButton.onClick.AddListener(OnYesButtonClicked);
        noButton.onClick.AddListener(OnNoButtonClicked);

        // ������������� ���������� ������ ��� �������������� ������
        AddClickListenerToPanel(additionalPanel, OnAdditionalPanelClicked);

        // �������� ������� ������ Escape
        SceneManager.sceneLoaded += OnSceneLoaded;

        // ���������� ������ ��� �������
        UpdateExitQuestionText();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // �������� ������, ���� ��� ���� ������� � ���������� �����
        exitConfirmationPanel.SetActive(false);
        additionalPanel.SetActive(false);

        // ���������, ����� �� ���������� ������ ��� �������� �����
        if (isPanelActive)
        {
            exitConfirmationPanel.SetActive(true);
            additionalPanel.SetActive(true);
        }

        // ��������� ����� ��� �������� �����
        UpdateExitQuestionText();
    }

    private void Update()
    {
        // �������� ������� ������ Escape
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (exitConfirmationPanel.activeSelf)
            {
                // ���� ������ ������������� ������ �������, ������ ��� ������
                exitConfirmationPanel.SetActive(false);
                additionalPanel.SetActive(false);
            }
            else
            {
                // �������� ������ ������������� ������ � �������������� ������
                exitConfirmationPanel.SetActive(true);
                additionalPanel.SetActive(true);
            }

            // ��������� ����� ��� ��������� ������
            UpdateExitQuestionText();
        }
    }

    private void OnYesButtonClicked()
    {
        // ������� ����������
        Application.Quit();

        // ���� � ��������� Unity, ���������� ���������������
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    private void OnNoButtonClicked()
    {
        // ������ ������ ������ ������������� ������
        exitConfirmationPanel.SetActive(false);
        additionalPanel.SetActive(false);
        isPanelActive = false;
    }

    private void OnAdditionalPanelClicked()
    {
        // ������ ��� ������
        exitConfirmationPanel.SetActive(false);
        additionalPanel.SetActive(false);
        isPanelActive = false;
    }

    private void AddClickListenerToPanel(GameObject panel, System.Action onClickAction)
    {
        // ���������, ���� �� � ������ ��������� EventTrigger, ���� ��� - ��������� ���
        EventTrigger eventTrigger = panel.GetComponent<EventTrigger>();
        if (eventTrigger == null)
        {
            eventTrigger = panel.AddComponent<EventTrigger>();
        }

        // ��������� ���������� �����
        EventTrigger.Entry entry = new EventTrigger.Entry
        {
            eventID = EventTriggerType.PointerClick
        };
        entry.callback.AddListener((data) => { onClickAction?.Invoke(); });
        eventTrigger.triggers.Add(entry);
    }

    public void UpdateExitQuestionText()
    {
        // �������� ������� ���� �� LocalizationManager � ��������� ����� � ������� GetText
        exitQuestion.text = LocalizationManager.GetText("exitQuestion");
        yesButton.GetComponentInChildren<Text>().text = LocalizationManager.GetText("yesButton");
        noButton.GetComponentInChildren<Text>().text = LocalizationManager.GetText("noButton");
    }
}
