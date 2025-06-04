using System;
using System.Collections;
using System.IO;
using System.IO.Compression;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Reflection;

public class HistoryCanvasController : MonoBehaviour
{
    public GameObject buttonPrefab;
    public Transform contentParent;
    public Slider progressBar;
    public GameObject loadingScreen;
    public TextMeshProUGUI searchHistory;

    private readonly string[] subfolders = new string[]
    {
        "chinese", "combo", "empty", "english", "number",
        "other", "players", "russian", "special"
    };

    void Start()
    {
        Data.LoadData();
        ShowHistoryList();
    }

    void OnEnable()
    {
        searchHistory.text = LocalizationManager.GetText("search_text") + ":";
    }

    void ShowHistoryList()
    {
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < Data.RecentBaseNames.Count; i++)
        {
            string originalFileName = Data.RecentBaseNames[i];
            string displayName = ProcessBaseName(originalFileName);

            var button = Instantiate(buttonPrefab, contentParent);
            button.SetActive(true);

            var texts = button.GetComponentsInChildren<TextMeshProUGUI>();
            foreach (var text in texts)
            {
                if (text.gameObject.name.ToLower().Contains("index"))
                {
                    text.text = $"{i + 1}.";
                }
                else if (text.gameObject.name.ToLower().Contains("name"))
                {
                    text.text = displayName;
                }
            }

            button.GetComponent<Button>().onClick.AddListener(() => OnHistoryButtonClicked(originalFileName));
        }
    }

    string ProcessBaseName(string baseName)
    {
        baseName = baseName.Replace(".json.gz.bytes", "");
        baseName = System.Text.RegularExpressions.Regex.Replace(baseName, @"^REG\{\d+\}_", "");
        return baseName;
    }

    void OnHistoryButtonClicked(string baseName)
    {
        StartCoroutine(LoadBaseScene(baseName));
    }

    IEnumerator LoadBaseScene(string baseName)
    {
        loadingScreen.SetActive(true);

        string fullPath = FindBasePath(baseName);
        if (fullPath == null)
        {
            loadingScreen.SetActive(false);
            yield break;
        }

        GameData.FilePath = fullPath;
        GameData.FileData = ReadCompressedJson(fullPath);

        if (GameData.FileData == null)
        {
            loadingScreen.SetActive(false);
            yield break;
        }

        string currentLanguage = Data.CurrentLanguage;
        ProcessContent<ResourceObjectsFind>(currentLanguage);
        ProcessContent<ChestEnemyLoot>();
        ProcessContent<FoundamentBuildings>();
        ProcessContent<Furniture>();

        yield return StartCoroutine(LoadSceneAsync("Base"));
    }

    string FindBasePath(string baseFileName)
    {
        foreach (string folder in subfolders)
        {
            string relativePath = Path.Combine("DataOfBases", folder, baseFileName).Replace("\\", "/");
            if (BetterStreamingAssets.FileExists(relativePath))
            {
                return relativePath;
            }
        }
        return null;
    }

    IEnumerator LoadSceneAsync(string sceneName)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        while (!operation.isDone)
        {
            progressBar.value = Mathf.Clamp01(operation.progress / 0.9f);
            yield return null;
        }
    }

    string ReadCompressedJson(string filePath)
    {
        if (!BetterStreamingAssets.FileExists(filePath))
        {
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
        catch (Exception)
        {
            return null;
        }
    }

    void ProcessContent<T>(string language = null) where T : MonoBehaviour
    {
        var component = FindFirstObjectByType<T>();
        if (component != null)
        {
            if (component is ResourceObjectsFind resourceObjectsFind && language != null)
            {
                resourceObjectsFind.LoadResourceDictionary(language);
            }

            MethodInfo method = typeof(T).GetMethod("ProcessFileContent", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            method?.Invoke(component, null);
        }
    }
}
