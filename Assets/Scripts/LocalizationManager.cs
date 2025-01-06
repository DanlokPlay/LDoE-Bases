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
            Data.CurrentLanguage = value; // Сохраняем выбор языка в Data
            Data.SaveData();
        }
    }

    public static void LoadLocalization()
    {
        // Инициализируем BetterStreamingAssets, если это ещё не сделано
        BetterStreamingAssets.Initialize();

        // Указываем путь к файлу локализации
        string filePath = "Localization.json";

        // Проверяем, существует ли файл
        if (BetterStreamingAssets.FileExists(filePath))
        {
            try
            {
                // Читаем содержимое файла
                string jsonContent = BetterStreamingAssets.ReadAllText(filePath);

                // Парсим JSON и загружаем локализацию
                var localizationData = JsonConvert.DeserializeObject<LocalizationData>(jsonContent);
                localizedTexts = localizationData.ToDictionary();
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Ошибка при десериализации файла локализации: {ex.Message}");
            }
        }
        else
        {
            Debug.LogError($"Файл локализации не найден: {filePath}");
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
        return $"[{key}]"; // Возвращаем ключ, если текст не найден
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
