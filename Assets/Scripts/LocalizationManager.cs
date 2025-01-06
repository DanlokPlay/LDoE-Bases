using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;

public static class LocalizationManager
{
    private static Dictionary<string, Dictionary<string, string>> localizedTexts;
    private static string currentLanguage = "ru";

    public static string CurrentLanguage
    {
        get => currentLanguage;
        set
        {
            currentLanguage = value;
            Data.CurrentLanguage = value; // ��������� ����� ����� � Data
            Data.SaveData();
        }
    }

    public static void LoadLocalization()
    {
        // �������������� BetterStreamingAssets, ���� ��� ��� �� �������
        BetterStreamingAssets.Initialize();

        // ��������� ���� � ����� �����������
        string filePath = "Localization.json";

        // ���������, ���������� �� ����
        if (BetterStreamingAssets.FileExists(filePath))
        {
            try
            {
                // ������ ���������� �����
                string jsonContent = BetterStreamingAssets.ReadAllText(filePath);

                // ������ JSON � ��������� �����������
                var localizationData = JsonConvert.DeserializeObject<LocalizationData>(jsonContent);
                localizedTexts = localizationData.ToDictionary();
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"������ ��� �������������� ����� �����������: {ex.Message}");
            }
        }
        else
        {
            Debug.LogError($"���� ����������� �� ������: {filePath}");
        }
    }

    public static string GetText(string key)
    {
        if (localizedTexts != null &&
            localizedTexts.ContainsKey(CurrentLanguage) &&
            localizedTexts[CurrentLanguage].ContainsKey(key))
        {
            return localizedTexts[CurrentLanguage][key];
        }
        return $"[{key}]"; // ���������� ����, ���� ����� �� ������
    }

    [System.Serializable]
    public class LocalizationData
    {
        public Dictionary<string, Dictionary<string, string>> languages;

        public Dictionary<string, Dictionary<string, string>> ToDictionary()
        {
            return languages;
        }
    }
}
