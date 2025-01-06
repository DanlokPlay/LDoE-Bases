using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;
using Better.StreamingAssets;

public class ResourceObjectsFind : MonoBehaviour
{
    private Dictionary<string, string> resourceDictionary;
    private string raidName;
    private bool needBan;

    public void LoadResourceDictionary(string language)
    {
        string defaultLanguage = "ru";
        string translationsPath;

        // Используем язык, переданный в метод
        string currentLanguage = !string.IsNullOrEmpty(language) ? language : defaultLanguage;

        // Определяем путь к файлу перевода
        translationsPath = $"Translator/ObjectsOnBase_{currentLanguage}.json";

        try
        {
            // Читаем файл перевода
            string translationsData = BetterStreamingAssets.ReadAllText(translationsPath);
            resourceDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(translationsData);
        }
        catch (System.Exception)
        {
            // В случае ошибки создаем пустой словарь
            resourceDictionary = new Dictionary<string, string>();
        }
    }

    public void ProcessFileContent()
    {
        try
        {
            string jsonData = GameData.FileData;
            string resultString = ProcessJsonData(jsonData);
            GameData.RaidName = raidName;
            GameData.NeedBan = needBan;
            GameData.ResourceObjects = resultString;
        }
        catch (System.Exception) { }
    }

    private string ProcessJsonData(string jsonData)
    {
        Dictionary<string, int> resourceCounts = new Dictionary<string, int>();

        foreach (var id in resourceDictionary.Keys)
        {
            resourceCounts[id] = 0;
        }

        try
        {
            var raidContainer = JsonConvert.DeserializeObject<RaidContainer>(jsonData);
            if (raidContainer != null && raidContainer.Raid != null)
            {
                raidName = raidContainer.Raid.Name;
                needBan = raidContainer.Raid.NeedBan;
            }
            else
            {
                return "Нет данных для отображения.";
            }

            if (raidContainer.Raid.Location?.ResourceObjects?.Items != null)
            {
                foreach (var itemEntry in raidContainer.Raid.Location.ResourceObjects.Items)
                {
                    Items item = itemEntry.Value;

                    if (resourceDictionary.ContainsKey(item.DescriptionId) && item.Item.Amount > 0)
                    {
                        resourceCounts[item.DescriptionId]++;
                    }
                }

                var result = new List<string>();
                foreach (var entry in resourceCounts)
                {
                    string description = resourceDictionary.ContainsKey(entry.Key) ? resourceDictionary[entry.Key] : entry.Key;
                    result.Add($"{description}: {entry.Value}");
                }

                return string.Join("\n", result);
            }
            else
            {
                return "Нет данных для отображения.";
            }
        }
        catch (System.Exception)
        {
            return "Ошибка обработки данных.";
        }
    }
}