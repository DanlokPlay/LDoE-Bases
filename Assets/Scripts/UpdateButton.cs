using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Linq;

public class UpdateButton : MonoBehaviour
{
    public Button updateButton; // кнопка для вызова обновлений
    public GameObject popupCanvas; // Canvas для всплывающего окна
    public ScrollRect scrollRect; // ScrollView для прокрутки
    public Text popupText; // Текст внутри ScrollView
    public Button backButton; // Кнопка "Назад"
    private string CONFIG_URL = "https://ldoestatic.cachefly.net/static/ldoez_config.json.gz";
    private static readonly string BasePatchUrl = "https://ldoe-static.ams3.cdn.digitaloceanspaces.com/static/";

    void Start()
    {
        updateButton.onClick.AddListener(OnUpdateButtonClick);
        backButton.onClick.AddListener(OnBackButtonClick);
        popupCanvas.SetActive(false); // изначально скрываем всплывающее окно
    }

    private async void OnUpdateButtonClick()
    {
        string language = Data.CurrentLanguage;
        string updateMessage = await GetUpdateMessage();
        ShowUpdatePopup(updateMessage, language);
    }

    // Метод для запроса данных обновлений
    private async Task<string> GetUpdateMessage()
    {
        string resultMessage = "Не удалось получить информацию об обновлениях.";

        try
        {
            HttpClient client = new HttpClient();
            var response = await client.GetByteArrayAsync(CONFIG_URL); // Получаем байтовый массив

            // Распаковываем GZIP
            byte[] jsonBytes = DecompressGZip(response);

            // Преобразуем байты в строку
            string jsonString = Encoding.UTF8.GetString(jsonBytes);

            // Выводим полученную строку для диагностики
            Debug.Log("Полученные данные JSON:\n" + jsonString);

            // Парсим JSON
            var json = JObject.Parse(jsonString);

            var versions = json["versions"];
            if (versions != null)
            {
                resultMessage = await ParseVersions(versions);
            }
            else
            {
                resultMessage = "Не найдены данные о версиях.";
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Ошибка при запросе: {e.Message}");
        }

        return resultMessage;
    }

    // Метод для распаковки GZIP данных
    private byte[] DecompressGZip(byte[] compressedData)
    {
        using (var compressedStream = new MemoryStream(compressedData))
        using (var gzipStream = new GZipStream(compressedStream, CompressionMode.Decompress))
        using (var memoryStream = new MemoryStream())
        {
            gzipStream.CopyTo(memoryStream);
            return memoryStream.ToArray();
        }
    }

    // Метод для обработки информации о версиях
    private async Task<string> ParseVersions(JToken versions)
    {
        StringBuilder messageBuilder = new StringBuilder();

        foreach (var version in versions)
        {
            var versionKey = version.Path.Split('.').Last();  // Получаем ключ версии
            var versionData = version.First();  // Извлекаем данные для этой версии

            var versionCode = versionData["version_code"]?.ToString() ?? "Unknown";
            var tag = versionData["tag"]?.ToString() ?? "Unknown";
            var patchUrls = versionData["patch_note_urls"] as JArray;

            if (patchUrls != null && patchUrls.Count > 0)
            {
                var patchFileUrl = patchUrls[0].ToString();
                var patchVersion = patchFileUrl.Split('/').Last().Split('.')[0]; // Получаем "pXXX"
                messageBuilder.AppendLine($"<b>Версия:</b> {versionCode}");
                messageBuilder.AppendLine($"<b>Тег:</b> {tag}");
                messageBuilder.AppendLine($"<b>Обновление:</b> {versionKey}");
                messageBuilder.AppendLine($"<b>Патч:</b> {patchVersion}");

                // Заполняем русским и английским текстом патча
                var patchData = await GetPatchData(patchVersion);
                messageBuilder.AppendLine($"🇷🇺 <b>RU:</b> {patchData.ru ?? "Нет данных"}");
                messageBuilder.AppendLine($"🇺🇸 <b>US:</b> {patchData.us ?? "No data"}");
                messageBuilder.AppendLine();
            }
        }

        return messageBuilder.ToString();
    }

    // Метод для получения данных патча
    private async Task<(string ru, string us)> GetPatchData(string patchVersion)
    {
        string patchUrl = BasePatchUrl + patchVersion + ".json.gz";
        string ruData = "Нет данных";
        string usData = "No data";

        try
        {
            // Логируем URL патча для диагностики
            Debug.Log($"Загружаем данные патча с URL: {patchUrl}");

            HttpClient client = new HttpClient();
            var response = await client.GetByteArrayAsync(patchUrl);

            // Проверяем, пришел ли ответ от сервера
            if (response == null || response.Length == 0)
            {
                Debug.LogError("Ответ от сервера патча пустой или поврежден.");
                return (ruData, usData);
            }

            // Распаковываем GZIP
            byte[] jsonBytes = DecompressGZip(response);

            // Преобразуем байты в строку
            string jsonString = Encoding.UTF8.GetString(jsonBytes);

            // Логируем полученные данные
            Debug.Log($"Полученные данные патча: {jsonString}");

            // Парсим JSON
            var json = JObject.Parse(jsonString);

            ruData = json["Russian"]?.ToString() ?? "Нет данных";
            usData = json["English"]?.ToString() ?? "No data";
        }
        catch (System.Exception e)
        {
            // Логируем ошибку
            Debug.LogError($"Ошибка при запросе данных патча: {e.Message}");
        }

        return (ruData, usData);
    }

    // Метод для отображения всплывающего окна
    private void ShowUpdatePopup(string message, string language)
    {
        // Показать всплывающее окно
        popupCanvas.SetActive(true);

        // Вставить текст в ScrollView
        popupText.text = message;
    }

    // Метод для закрытия всплывающего окна
    private void OnBackButtonClick()
    {
        popupCanvas.SetActive(false); // скрыть всплывающее окно
    }
}
