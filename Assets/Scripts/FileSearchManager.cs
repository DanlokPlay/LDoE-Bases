using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainCanvasController : MonoBehaviour
{
    public InputField fileNameInputField;
    public Button searchButton;
    public GameObject mainCanvas;
    public GameObject resultCanvas;
    public GameObject aboutCanvas;
    public GameObject faqCanvas;
    public GameObject findIndoCanvas;
    public GameObject UpdateInformationCanvas;
    public GameObject historyCanvas;

    public Button backButton;
    public Button aboutButton;
    public Button faqButton;
    public Button exitButton;
    public Button findInfoButton;
    public Button historyButton;
    public Button closeAboutButton;
    public Button closeFaqButton;
    public Button closeFindInfoButton;
    public Button closeHistoryButton;

    public GameObject loadingScreen;
    public Slider progressBar;

    public TextMeshProUGUI faqContentText;
    public Button changeLanguageButton;
    public Image languageFlag;
    public Sprite flagRu;
    public Sprite flagEn;
    public Text language;
    public Text noResultsText;


    private ExitConfirmation exitConfirmation;
    private ResultCanvasController resultCanvasController;

    void Start()
    {
        BetterStreamingAssets.Initialize();

        searchButton.onClick.AddListener(OnSearchButtonClicked);
        backButton.onClick.AddListener(OnBackButtonClicked);
        aboutButton.onClick.AddListener(OnAboutButtonClicked);
        faqButton.onClick.AddListener(OnFaqButtonClicked);
        exitButton.onClick.AddListener(OnExitButtonClicked);
        findInfoButton.onClick.AddListener(OnFindInfoClicked);
        historyButton.onClick.AddListener(OnHistoryClicked);
        closeAboutButton.onClick.AddListener(() => CloseCanvas(aboutCanvas));
        closeFaqButton.onClick.AddListener(() => CloseCanvas(faqCanvas));
        closeFindInfoButton.onClick.AddListener(() => CloseCanvas(findIndoCanvas));
        closeHistoryButton.onClick.AddListener(() => CloseCanvas(historyCanvas));

        resultCanvas.SetActive(false);
        aboutCanvas.SetActive(false);
        faqCanvas.SetActive(false);
        findIndoCanvas.SetActive(false);
        UpdateInformationCanvas.SetActive(false);
        historyCanvas.SetActive(false);
        noResultsText.gameObject.SetActive(false);
        loadingScreen.SetActive(false);


        exitConfirmation = FindFirstObjectByType<ExitConfirmation>();
        resultCanvasController = resultCanvas.GetComponent<ResultCanvasController>();

        RestoreCanvas();

        Data.LoadData();
        LocalizationManager.CurrentLanguage = Data.CurrentLanguage;
        LocalizationManager.LoadLocalization();
        UpdateUI();

        changeLanguageButton.onClick.AddListener(OnChangeLanguageButtonClicked);
    }

    void UpdateUI()
    {
        var placeholderText = fileNameInputField.placeholder.GetComponent<Text>();
        faqContentText.text =
            LocalizationManager.GetText("aboutFAQ") + "\n\n" +
            LocalizationManager.GetText("question0") + "\n" + LocalizationManager.GetText("answer0") + "\n\n" +
            LocalizationManager.GetText("question1") + "\n" + LocalizationManager.GetText("answer1") + "\n\n" +
            LocalizationManager.GetText("question2") + "\n" + LocalizationManager.GetText("answer2") + "\n\n" +
            LocalizationManager.GetText("question3") + "\n" + LocalizationManager.GetText("answer3") + "\n\n" +
            LocalizationManager.GetText("question4") + "\n" + LocalizationManager.GetText("answer4") + "\n\n" +
            LocalizationManager.GetText("question5") + "\n" + LocalizationManager.GetText("answer5") + "\n\n" +
            LocalizationManager.GetText("question6") + "\n" + LocalizationManager.GetText("answer6") + "\n\n" +
            LocalizationManager.GetText("question7") + "\n" + LocalizationManager.GetText("answer7") + "\n\n" +
            LocalizationManager.GetText("question8") + "\n" + LocalizationManager.GetText("answer8") + "\n\n" +
            LocalizationManager.GetText("question9") + "\n" + LocalizationManager.GetText("answer9") + "\n\n" +
            LocalizationManager.GetText("question10") + "\n" + LocalizationManager.GetText("answer10") + "\n\n" +
            LocalizationManager.GetText("question11") + "\n" + LocalizationManager.GetText("answer11") + "\n\n" +
            LocalizationManager.GetText("question12") + "\n" + LocalizationManager.GetText("answer12") + "\n\n" +
            LocalizationManager.GetText("endFAQ");
        searchButton.GetComponentInChildren<Text>().text = LocalizationManager.GetText("searchButton");
        placeholderText.text = LocalizationManager.GetText("searchPlaceholder");
        language.text = LocalizationManager.GetText("language");
        if (noResultsText.gameObject.activeSelf)
        {
            noResultsText.text = LocalizationManager.GetText("noResults");
        }

        UpdateFlag();
    }

    void UpdateFlag()
    {
        languageFlag.sprite = LocalizationManager.CurrentLanguage == "ru" ? flagRu : flagEn;
    }

    void OnChangeLanguageButtonClicked()
    {
        LocalizationManager.CurrentLanguage = LocalizationManager.CurrentLanguage == "ru" ? "en" : "ru";
        Data.SaveData();
        LocalizationManager.LoadLocalization();
        UpdateUI();
    }

    void OnExitButtonClicked()
    {
        GameObject exitConfirmationPanel = exitConfirmation.exitConfirmationPanel;
        GameObject additionalPanel = exitConfirmation.additionalPanel;
        exitConfirmationPanel.SetActive(true);
        additionalPanel.SetActive(true);
        exitConfirmation.UpdateExitQuestionText();
    }

    void RestoreCanvas()
    {
        if (GameData.LastActiveCanvas == "InfoCanvas")
        {
            mainCanvas.SetActive(false);
            resultCanvas.SetActive(true);

            resultCanvasController.RestoreLastResult();
        }
        else
        {
            resultCanvas.SetActive(false);
            mainCanvas.SetActive(true);
        }

        GameData.LastActiveCanvas = null;
    }

    void OnSearchButtonClicked()
    {
        string searchPattern = fileNameInputField.text.Replace(" ", "").ToLower();
        searchPattern = System.Text.RegularExpressions.Regex.Replace(searchPattern, @"[<>:""/|\?*]", "=");
        bool found = resultCanvasController.PerformSearch(searchPattern);

        if (!found)
        {
            noResultsText.gameObject.SetActive(true);
            noResultsText.text = LocalizationManager.GetText("noResults");
        }
        else
        {
            noResultsText.gameObject.SetActive(false);
        }
    }


    void OnBackButtonClicked() => resultCanvasController.GoBackToMainCanvas();
    void OnAboutButtonClicked() => ShowCanvas(aboutCanvas);
    void OnFaqButtonClicked() => ShowCanvas(faqCanvas);
    void OnFindInfoClicked() => ShowCanvas(findIndoCanvas);
    void OnHistoryClicked() => ShowCanvas(historyCanvas);

    void ShowCanvas(GameObject canvas)
    {
        mainCanvas.SetActive(false);
        canvas.SetActive(true);
    }

    void CloseCanvas(GameObject canvas)
    {
        if (canvas != null) canvas.SetActive(false);
        mainCanvas.SetActive(true);
    }
}
