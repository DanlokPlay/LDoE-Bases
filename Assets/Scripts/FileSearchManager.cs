using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FileSearchManager : MonoBehaviour
{
    public InputField fileNameInputField;
    public Button searchButton;
    public GameObject mainCanvas;
    public GameObject infoCanvas;
    public GameObject aboutCanvas;
    public Text numbersOfBases;
    public Text nameOfBase;
    public Text Base;
    public Text noResultsText;
    public Button previousButton;
    public Button viewButton;
    public Button nextButton;
    public Button backButton;
    public Button aboutButton;
    public Button closeAboutButton;
    public Button exitButton;

    public GameObject loadingScreen;
    public Slider progressBar;

    public GameObject faqCanvas;
    public Button faqButton;
    public Button closeFaqButton;


    public Text faqContentText;



    public Button changeLanguageButton;

    public Image languageFlag;
    public Sprite flagRu;
    public Sprite flagEn;

    public Text botInfo;
    public Text chat;
    public Text checkUpdates;
    public Text donationText;
    public Text language;



    private List<string> foundFiles = new List<string>();
    private int currentIndex = -1;
    private ExitConfirmation exitConfirmation;

    void Start()
    {
        BetterStreamingAssets.Initialize();

        searchButton.onClick.AddListener(OnSearchButtonClicked);
        previousButton.onClick.AddListener(OnPreviousButtonClicked);
        viewButton.onClick.AddListener(OnShowContentButtonClicked);
        nextButton.onClick.AddListener(OnNextButtonClicked);
        backButton.onClick.AddListener(OnBackButtonClicked);
        aboutButton.onClick.AddListener(OnAboutButtonClicked);
        closeAboutButton.onClick.AddListener(OnCloseAboutButtonClicked);
        faqButton.onClick.AddListener(OnFaqButtonClicked);
        closeFaqButton.onClick.AddListener(OnCloseFaqButtonClicked);
        exitButton.onClick.AddListener(OnExitButtonClicked);

        infoCanvas.SetActive(false);
        aboutCanvas.SetActive(false);
        faqCanvas.SetActive(false);
        noResultsText.gameObject.SetActive(false);

        exitConfirmation = FindFirstObjectByType<ExitConfirmation>();

        previousButton.interactable = false;
        nextButton.interactable = false;
        loadingScreen.SetActive(false);

        RestoreCanvas();

        // ��������� ������
        Data.LoadData(); // ��������� ������� ����

        // ������������� ������� ���� � LocalizationManager
        LocalizationManager.CurrentLanguage = Data.CurrentLanguage; // ������������� ����

        // ��������� �����������
        LocalizationManager.LoadLocalization();

        // ��������� UI ��� ������
        UpdateUI();

        // �������� �� ������� ������ ����� �����
        changeLanguageButton.onClick.AddListener(OnChangeLanguageButtonClicked);
    }


    void UpdateUI()
    {
        var placeholderText = fileNameInputField.placeholder.GetComponent<Text>();

        // ���������� ������ UI
        faqContentText.text = LocalizationManager.GetText("faqText");
        searchButton.GetComponentInChildren<Text>().text = LocalizationManager.GetText("searchButton");
        placeholderText.text = LocalizationManager.GetText("searchPlaceholder");

        botInfo.text = LocalizationManager.GetText("botInfo");
        chat.text = LocalizationManager.GetText("chat");
        checkUpdates.text = LocalizationManager.GetText("checkUpdates");

        previousButton.GetComponentInChildren<Text>().text = LocalizationManager.GetText("previous");
        viewButton.GetComponentInChildren<Text>().text = LocalizationManager.GetText("view");
        nextButton.GetComponentInChildren<Text>().text = LocalizationManager.GetText("next");

        donationText.text = LocalizationManager.GetText("donationText");
        language.text = LocalizationManager.GetText("language");

        // ��������� ����� ������ ���� �� �������
        if (noResultsText.gameObject.activeSelf)
        {
            noResultsText.text = LocalizationManager.GetText("noResults");
        }

        // ���������� �����
        UpdateFlag();
    }

    void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            FixInputField();
        }
    }

    private void FixInputField()
    {
        fileNameInputField.DeactivateInputField();
    }

    void UpdateFlag()
    {
        // ��������� ����� � ����������� �� �������� �����
        if (LocalizationManager.CurrentLanguage == "ru")
        {
            languageFlag.sprite = flagRu;
        }
        else if (LocalizationManager.CurrentLanguage == "en")
        {
            languageFlag.sprite = flagEn;
        }
    }

    // ����� ��� ����� �����
    void OnChangeLanguageButtonClicked()
    {
        // ����������� ����
        if (LocalizationManager.CurrentLanguage == "ru")
        {
            LocalizationManager.CurrentLanguage = "en";
        }
        else
        {
            LocalizationManager.CurrentLanguage = "ru";
        }

        // ��������� ����� ����� � ����
        Data.SaveData();

        // ��������� ����� ����� ��� ���������� �����
        LocalizationManager.LoadLocalization();

        // ��������� ���������
        UpdateUI();
    }


    public void OnExitButtonClicked()
    {
        // �������� ������ ������������� ������ � �������������� ������ �� ���������� ExitConfirmation
        GameObject exitConfirmationPanel = exitConfirmation.exitConfirmationPanel;
        GameObject additionalPanel = exitConfirmation.additionalPanel;

        // ���������� ������
        exitConfirmationPanel.SetActive(true);
        additionalPanel.SetActive(true);

        // ��������� ����� �� ������� � ����������� �� �������� �����
        exitConfirmation.UpdateExitQuestionText();
    }


    void RestoreCanvas()
    {

        if (GameData.LastActiveCanvas == "InfoCanvas")
        {
            mainCanvas.SetActive(false);
            infoCanvas.SetActive(true);


            foundFiles = new List<string>(GameData.FoundFiles);
            currentIndex = GameData.CurrentIndex;

            UpdateResultText();
            UpdateNavigationButtons();
        }
        else
        {
            infoCanvas.SetActive(false);
            mainCanvas.SetActive(true);
        }


        GameData.LastActiveCanvas = null;
    }

    void OnSearchButtonClicked()
    {
        string searchPattern = fileNameInputField.text.Replace(" ", "").ToLower();

        // �������� ��� ������� [<>:"/\|?*] �� ���� ���������
        searchPattern = Regex.Replace(searchPattern, @"[<>:""/|\?*]", "=");

        foundFiles.Clear();
        currentIndex = -1;

        if (!string.IsNullOrEmpty(searchPattern))
        {
            // ����������� �������� �����
            string[] specialPatterns = new string[] {
            "chinese", "combo", "duplicate", "empty", "english", "number",
            "other", "players", "russian", "special"
        };

            if (specialPatterns.Contains(searchPattern))
            {
                // ���� ������� ���� �� �������� ����, ��������� ��� ����� �� ��������������� �����
                SearchFilesFromSpecificFolder(searchPattern);
            }
            else
            {
                // ����� ��������� ����� �� ��������
                SearchFilesWithPattern(searchPattern);
            }
        }

        UpdateResultCanvas();
    }

    

    void SearchFilesWithPattern(string pattern)
    {
        string[] directories = new string[]
        {
        "DataOfBases/chinese",
        "DataOfBases/combo",
        //"DataOfBases/duplicate",  -- ������ �������� �������
        "DataOfBases/empty",
        "DataOfBases/english",
        "DataOfBases/number",
        "DataOfBases/other",
        "DataOfBases/players",
        "DataOfBases/russian",
        "DataOfBases/special"
        };

        string lowerPattern = pattern.ToLower();

        foreach (string directory in directories)
        {
            string[] compressedFiles = BetterStreamingAssets.GetFiles(directory, "*.json.gz.bytes");
            foreach (string filePath in compressedFiles)
            {
                string fileName = Path.GetFileName(filePath).Replace(" ", "").ToLower();

                fileName = Regex.Replace(fileName, @"^REG\{\d+\}_", "", RegexOptions.IgnoreCase);

                if (fileName.StartsWith(lowerPattern) && Regex.IsMatch(fileName, $@"^{Regex.Escape(lowerPattern)}(_\d+)?\.json\.gz\.bytes$", RegexOptions.IgnoreCase))
                {
                    foundFiles.Add(filePath);
                }
            }
        }
    }

    void SearchFilesFromSpecificFolder(string pattern)
    {
        string directory = $"DataOfBases/{pattern}";
        string[] jsonFiles = BetterStreamingAssets.GetFiles(directory, "*.json.gz.bytes");

        foreach (string filePath in jsonFiles)
        {
            foundFiles.Add(filePath);
        }
    }

    void UpdateResultCanvas()
    {
        if (foundFiles.Count > 0)
        {
            mainCanvas.SetActive(false);
            infoCanvas.SetActive(true);
            currentIndex = 0;
            UpdateResultText();
            UpdateNavigationButtons();
            noResultsText.gameObject.SetActive(false);
        }
        else
        {
            noResultsText.gameObject.SetActive(true);
            noResultsText.text = LocalizationManager.GetText("noResults");
            infoCanvas.SetActive(false);
        }
    }

    void UpdateResultText()
    {
        if (currentIndex >= 0 && currentIndex < foundFiles.Count)
        {
            numbersOfBases.text = string.Format(LocalizationManager.GetText("foundBases"), foundFiles.Count);

            // �������� ��� �����
            string fileName = Path.GetFileName(foundFiles[currentIndex]);

            // ������� ������ REG{�����}_ (���� �� ����)
            fileName = Regex.Replace(fileName, @"^REG\{\d+\}_", "");

            // ������� ���������� ".json.gz.bytes"
            fileName = Regex.Replace(fileName, @"\.json\.gz\.bytes$", "", RegexOptions.IgnoreCase);

            // ���������� ��� ����� ��� ������ ����������
            nameOfBase.text = LocalizationManager.GetText("exactBaseName");
            Base.text = fileName;
        }
    }

    void UpdateNavigationButtons()
    {
        previousButton.interactable = currentIndex > 0;
        nextButton.interactable = currentIndex < foundFiles.Count - 1;
    }

    void OnPreviousButtonClicked()
    {
        if (foundFiles.Count > 0 && currentIndex > 0)
        {
            currentIndex--;
            UpdateResultText();
            UpdateNavigationButtons();
        }
    }

    void OnNextButtonClicked()
    {
        if (foundFiles.Count > 0 && currentIndex < foundFiles.Count - 1)
        {
            currentIndex++;
            UpdateResultText();
            UpdateNavigationButtons();
        }
    }

    void OnShowContentButtonClicked()
    {
        if (foundFiles.Count > 0 && currentIndex >= 0 && currentIndex < foundFiles.Count)
        {
            // ������ ��������� ��������� INFOCanvas
            GameData.LastActiveCanvas = "InfoCanvas";
            GameData.FoundFiles = new List<string>(foundFiles); // ��������� ��������� �����
            GameData.CurrentIndex = currentIndex; // ��������� ������� ������

            StartCoroutine(ShowLoadingScreenAndProcessFiles());
        }
    }

    IEnumerator ShowLoadingScreenAndProcessFiles()
    {
        loadingScreen.SetActive(true);
        yield return null;

        // ��������� ������� ����
        Data.LoadData();
        string currentLanguage = Data.CurrentLanguage;

        GameData.FilePath = foundFiles[currentIndex];

        // ������ ������ JSON
        GameData.FileData = ReadCompressedJson(GameData.FilePath);

        ProcessContent<ResourceObjectsFind>(currentLanguage);
        ProcessContent<ChestEnemyLoot>();
        ProcessContent<FoundamentBuildings>();
        ProcessContent<Furniture>();
        ProcessContent<DragBoxProcessor>();
        ProcessContent<MotorcycleDataProcessor>();

        StartCoroutine(LoadSceneAsync("Base"));
    }

    void ProcessContent<T>(string language = null) where T : MonoBehaviour
    {
        var component = FindFirstObjectByType<T>(); // ������� ��������� � �����
        if (component != null)
        {
            // ���� ��������� � ��� ResourceObjectsFind, �������� ����
            if (component is ResourceObjectsFind resourceObjectsFind && language != null)
            {
                resourceObjectsFind.LoadResourceDictionary(language); // ��������� ������� � ����������
            }

            // ���������� ��������� ��� ������ ������ ProcessFileContent
            var method = typeof(T).GetMethod("ProcessFileContent");
            method?.Invoke(component, null); // �������� ����� ��� ����������
        }
    }


    IEnumerator LoadSceneAsync(string sceneName)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            progressBar.value = progress;
            yield return null;
        }
    }

    string ReadCompressedJson(string filePath)
    {
        if (!BetterStreamingAssets.FileExists(filePath))
        {
            Debug.LogError($"���� {filePath} �� ������ � StreamingAssets!");
            return null;
        }

        try
        {
            using (Stream fileStream = BetterStreamingAssets.OpenRead(filePath))
            using (MemoryStream memoryStream = new MemoryStream())
            {
                fileStream.CopyTo(memoryStream);
                memoryStream.Seek(0, SeekOrigin.Begin);

                using (GZipStream gzipStream = new GZipStream(memoryStream, CompressionMode.Decompress))
                using (StreamReader reader = new StreamReader(gzipStream))
                {
                    return reader.ReadToEnd();
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"������ ��� ������ {filePath}: {e}");
            return null;
        }
    }


    void OnBackButtonClicked()
    {
        infoCanvas.SetActive(false);
        mainCanvas.SetActive(true);

        GameData.LastActiveCanvas = null;
        GameData.FoundFiles.Clear();
        GameData.CurrentIndex = -1;
    }

    void OnAboutButtonClicked()
    {
        aboutCanvas.SetActive(true);
        mainCanvas.SetActive(false);
    }

    void OnCloseAboutButtonClicked()
    {
        aboutCanvas.SetActive(false);
        mainCanvas.SetActive(true);
    }

    void OnFaqButtonClicked()
    {
        mainCanvas.SetActive(false);
        faqCanvas.SetActive(true);
    }

    void OnCloseFaqButtonClicked()
    {
        faqCanvas.SetActive(false);
        mainCanvas.SetActive(true);
    }
}