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

public class ResultCanvasController : MonoBehaviour
{
    public Text numbersOfBases;
    public Text nameOfBase;
    public Text Base;
    public Button previousButton;
    public Button viewButton;
    public Button nextButton;

    public GameObject mainCanvas;
    public GameObject resultCanvas;
    public GameObject loadingScreen;
    public Slider progressBar;

    private List<string> foundFiles = new();
    private int currentIndex = -1;

    void Start()
    {
        previousButton.onClick.AddListener(OnPreviousClicked);
        nextButton.onClick.AddListener(OnNextClicked);
        viewButton.onClick.AddListener(OnViewClicked);

        previousButton.interactable = false;
        nextButton.interactable = false;
        loadingScreen.SetActive(false);
    }

    public bool PerformSearch(string searchPattern)
    {
        foundFiles.Clear();
        currentIndex = -1;

        if (string.IsNullOrEmpty(searchPattern)) return false;

        string[] specialPatterns = {
        "chinese", "combo", "duplicate", "empty", "english", "number",
        "other", "players", "russian", "special"
    };

        if (specialPatterns.Contains(searchPattern))
            SearchFromFolder(searchPattern);
        else
            SearchWithPattern(searchPattern);

        if (foundFiles.Count > 0)
        {
            ShowResults();
            return true;
        }
        else
        {
            mainCanvas.SetActive(true);
            resultCanvas.SetActive(false);
            return false;
        }
    }


    void SearchFromFolder(string pattern)
    {
        string directory = $"DataOfBases/{pattern}";
        foundFiles.AddRange(BetterStreamingAssets.GetFiles(directory, "*.json.gz.bytes"));
    }

    void SearchWithPattern(string pattern)
    {
        string[] directories = {
            "DataOfBases/chinese", "DataOfBases/combo", "DataOfBases/empty",
            "DataOfBases/english", "DataOfBases/number", "DataOfBases/other",
            "DataOfBases/players", "DataOfBases/russian", "DataOfBases/special"
        };

        string cleanedPattern = pattern.Replace(" ", "").ToLower();

        foreach (var dir in directories)
        {
            foreach (var file in BetterStreamingAssets.GetFiles(dir, "*.json.gz.bytes"))
            {
                string name = Path.GetFileName(file);
                name = Regex.Replace(name, @"^REG\{\d+\}_", "");
                name = Regex.Replace(name, @"\.json\.gz\.bytes$", "", RegexOptions.IgnoreCase);
                int index = name.IndexOf('%');
                if (index >= 0)
                {
                    name = name.Substring(0, index);
                }
                name = name.ToLower();

                if (Regex.IsMatch(name, $@"^{Regex.Escape(cleanedPattern)}(_\d+)?$"))
                {
                    foundFiles.Add(file);
                }
            }
        }
    }

    void ShowResults()
    {
        if (foundFiles.Count > 0)
        {
            mainCanvas.SetActive(false);
            resultCanvas.SetActive(true);
            currentIndex = 0;
            UpdateResultText();
            UpdateNavigationButtons();
        }
        else
        {
            resultCanvas.SetActive(false);
        }
    }

    public void RestoreLastResult()
    {
        foundFiles = new List<string>(GameData.FoundFiles);
        currentIndex = GameData.CurrentIndex;
        UpdateResultText();
        UpdateNavigationButtons();
    }

    void UpdateResultText()
    {
        if (currentIndex < 0 || currentIndex >= foundFiles.Count) return;

        numbersOfBases.text = string.Format(LocalizationManager.GetText("foundBases"), foundFiles.Count);
        string fileName = Path.GetFileName(foundFiles[currentIndex]);
        fileName = Regex.Replace(fileName, @"^REG\{\d+\}_", "");
        fileName = Regex.Replace(fileName, @"\.json\.gz\.bytes$", "", RegexOptions.IgnoreCase);

        nameOfBase.text = LocalizationManager.GetText("exactBaseName");
        Base.text = fileName;
    }

    void UpdateNavigationButtons()
    {
        previousButton.interactable = currentIndex > 0;
        nextButton.interactable = currentIndex < foundFiles.Count - 1;
    }

    void OnPreviousClicked()
    {
        if (currentIndex > 0)
        {
            currentIndex--;
            UpdateResultText();
            UpdateNavigationButtons();
        }
    }

    void OnNextClicked()
    {
        if (currentIndex < foundFiles.Count - 1)
        {
            currentIndex++;
            UpdateResultText();
            UpdateNavigationButtons();
        }
    }

    void OnViewClicked()
    {
        if (currentIndex < 0 || currentIndex >= foundFiles.Count) return;

        GameData.LastActiveCanvas = "InfoCanvas";
        GameData.FoundFiles = new List<string>(foundFiles);
        GameData.CurrentIndex = currentIndex;

        string fullPath = foundFiles[currentIndex];
        string baseName = Path.GetFileName(fullPath);

        Data.LoadData();
        Data.RecentBaseNames.Remove(baseName);
        Data.RecentBaseNames.Insert(0, baseName);
        if (Data.RecentBaseNames.Count > 30)
            Data.RecentBaseNames = Data.RecentBaseNames.Take(30).ToList();
        Data.SaveData();

        StartCoroutine(ShowLoadingAndLoad(fullPath));
    }

    IEnumerator ShowLoadingAndLoad(string path)
    {
        loadingScreen.SetActive(true);
        yield return null;

        string lang = Data.CurrentLanguage;
        GameData.FilePath = path;
        GameData.FileData = ReadCompressedJson(path);

        ProcessContent<ResourceObjectsFind>(lang);
        ProcessContent<ChestEnemyLoot>();
        ProcessContent<FoundamentBuildings>();
        ProcessContent<Furniture>();
        ProcessContent<DragBoxProcessor>();
        ProcessContent<MotorcycleDataProcessor>();

        AsyncOperation op = SceneManager.LoadSceneAsync("Base");

        while (!op.isDone)
        {
            float progress = Mathf.Clamp01(op.progress / 0.9f);
            progressBar.value = progress;
            yield return null;
        }
    }

    string ReadCompressedJson(string path)
    {
        if (!BetterStreamingAssets.FileExists(path))
        {
            Debug.LogError($"Файл не найден: {path}");
            return null;
        }

        try
        {
            using var fileStream = BetterStreamingAssets.OpenRead(path);
            using var memStream = new MemoryStream();
            fileStream.CopyTo(memStream);
            memStream.Seek(0, SeekOrigin.Begin);
            using var gzip = new GZipStream(memStream, CompressionMode.Decompress);
            using var reader = new StreamReader(gzip);
            return reader.ReadToEnd();
        }
        catch (Exception ex)
        {
            Debug.LogError($"Ошибка чтения {path}: {ex}");
            return null;
        }
    }

    void ProcessContent<T>(string lang = null) where T : MonoBehaviour
    {
        var comp = FindFirstObjectByType<T>();
        if (comp == null) return;

        if (comp is ResourceObjectsFind rof && lang != null)
        {
            rof.LoadResourceDictionary(lang);
        }

        var method = typeof(T).GetMethod("ProcessFileContent");
        method?.Invoke(comp, null);
    }

    public void GoBackToMainCanvas()
    {
        resultCanvas.SetActive(false);
        mainCanvas.SetActive(true);
        GameData.LastActiveCanvas = null;
        GameData.FoundFiles.Clear();
        GameData.CurrentIndex = -1;
    }
}
