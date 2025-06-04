using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Linq;
using System;
using UnityEngine.Networking;

public class FindInfo : MonoBehaviour
{
    public Button updateCheckButton;
    public Button checkVersionButton;
    public GameObject popupCanvas;
    public ScrollRect scrollRect;
    public TextMeshProUGUI popupText;
    public Button backButton;
    public TextMeshProUGUI statusText;
    public Button getCodeButton;
    public TextMeshProUGUI codesText;
    public TextMeshProUGUI basesText;
    public TextMeshProUGUI updatesText;

    private string CONFIG_URL = "https://ldoestatic.cachefly.net/static/ldoez_config.json.gz";
    private static readonly string BasePatchUrl = "https://ldoe-static.ams3.cdn.digitaloceanspaces.com/static/";

    void Start()
    {
        updateCheckButton.onClick.AddListener(OnUpdateButtonClick);
        checkVersionButton.onClick.AddListener(OnCheckVersionClick);
        backButton.onClick.AddListener(OnBackButtonClick);
        getCodeButton.onClick.AddListener(OnGetCodeButtonClick);

        popupCanvas.SetActive(false);
        statusText.gameObject.SetActive(false);
    }

    void OnEnable()
    {
        codesText.text = LocalizationManager.GetText("codes_text");
        basesText.text = LocalizationManager.GetText("bases_text");
        updatesText.text = LocalizationManager.GetText("updates_text");
    }

    private async void OnGetCodeButtonClick()
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            ShowUpdatePopup($"<color=red>{LocalizationManager.GetText("no_internet")}</color>", Data.CurrentLanguage);
            return;
        }

        statusText.gameObject.SetActive(true);
        statusText.text = $"{LocalizationManager.GetText("searching")}...";

        string code = await GetDailyCodeAsync();

        string result = $"{LocalizationManager.GetText("current_code")}:\n{code}";

        statusText.gameObject.SetActive(false);

        ShowUpdatePopup($"<align=center><size=200%>{result}</size></align>", Data.CurrentLanguage);
    }

    private async Task<string> GetDailyCodeAsync()
    {
        string currentYear = DateTime.UtcNow.Year.ToString();
        string currentMonth = DateTime.UtcNow.Month.ToString("D2");
        string currentDay = DateTime.UtcNow.Day.ToString();

        string fileUrl = "http://ldoe.danlokplay.ru:49152/code";
        string code = LocalizationManager.GetText("code_not_found");

        try
        {
            using (UnityWebRequest request = UnityWebRequest.Get(fileUrl))
            {
                await request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
                {
                    return LocalizationManager.GetText("data_fetch_error");
                }

                byte[] jsonBytes = DecompressGZip(request.downloadHandler.data);
                string jsonString = Encoding.UTF8.GetString(jsonBytes);

                var json = JObject.Parse(jsonString);

                if (json[currentYear] != null && json[currentYear][currentMonth] != null)
                {
                    var monthData = json[currentYear][currentMonth];
                    if (monthData[currentDay] != null)
                    {
                        code = monthData[currentDay].ToString();
                    }
                }
            }
        }
        catch
        {
            code = LocalizationManager.GetText("data_fetch_error");
        }

        return code;
    }

    private async void OnUpdateButtonClick()
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            ShowUpdatePopup($"<color=red>{LocalizationManager.GetText("no_internet")}</color>", Data.CurrentLanguage);
            return;
        }

        statusText.gameObject.SetActive(true);
        statusText.text = $"{LocalizationManager.GetText("searching")}...";

        string language = Data.CurrentLanguage;
        string updateMessage = await GetUpdateMessage();

        ShowUpdatePopup(updateMessage, language);

        statusText.gameObject.SetActive(false);
    }

    private async void OnCheckVersionClick()
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            ShowUpdatePopup($"<color=red>{LocalizationManager.GetText("no_internet")}</color>", Data.CurrentLanguage);
            return;
        }

        statusText.gameObject.SetActive(true);
        statusText.text = $"{LocalizationManager.GetText("searching")}...";

        string result = $"<color=red>{LocalizationManager.GetText("version_check_failed")}</color>";

        try
        {
            HttpClient client = new HttpClient();
            var response = await client.GetByteArrayAsync(CONFIG_URL);

            byte[] jsonBytes = DecompressGZip(response);
            string jsonString = Encoding.UTF8.GetString(jsonBytes);

            var json = JObject.Parse(jsonString);
            var versions = json["versions"] as JObject;

            if (versions != null)
            {
                var latestVersion = versions.Properties()
                    .OrderByDescending(p => int.Parse(p.Name))
                    .FirstOrDefault();

                if (latestVersion != null)
                {
                    var raidUrls = latestVersion.Value["raid_locations_url"] as JArray;
                    if (raidUrls != null && raidUrls.Count > 0)
                    {
                        string raidUrl = raidUrls[0]?.ToString();
                        if (!string.IsNullOrEmpty(raidUrl))
                        {
                            raidUrl = raidUrl.TrimEnd('/');
                            string serverVersion = Path.GetFileName(raidUrl);
                            string localVersion = Data.BasesVersion;

                            if (serverVersion == localVersion)
                            {
                                result = $"<color=green>{LocalizationManager.GetText("version_up_to_date")}</color>";
                            }
                            else
                            {
                                result = $"<color=yellow>{LocalizationManager.GetText("new_version_available")}:</color>\n" +
                                         $"{LocalizationManager.GetText("server_version")}: <b>{serverVersion}</b>\n" +
                                         $"{LocalizationManager.GetText("local_version")}: <b>{localVersion}</b>";
                            }
                        }
                    }
                }
            }
        }
        catch
        {
            result = $"<color=red>{LocalizationManager.GetText("version_loading_error")}</color>";
        }

        ShowUpdatePopup($"<align=center><size=200%>{result}</size></align>", Data.CurrentLanguage);

        statusText.gameObject.SetActive(false);
    }

    private async Task<string> GetUpdateMessage()
    {
        string resultMessage = $"<color=red>{LocalizationManager.GetText("update_info_failed")}</color>";

        try
        {
            HttpClient client = new HttpClient();
            var response = await client.GetByteArrayAsync(CONFIG_URL);

            byte[] jsonBytes = DecompressGZip(response);
            string jsonString = Encoding.UTF8.GetString(jsonBytes);

            var json = JObject.Parse(jsonString);

            var versions = json["versions"];
            if (versions != null)
            {
                resultMessage = await ParseVersions(versions);
            }
            else
            {
                resultMessage = $"<color=yellow>{LocalizationManager.GetText("no_version_data")}</color>";
            }
        }
        catch
        {
            resultMessage = $"<color=red>{LocalizationManager.GetText("data_fetch_error")}</color>";
        }

        return resultMessage;
    }

    private async Task<string> ParseVersions(JToken versions)
    {
        StringBuilder messageBuilder = new StringBuilder();

        messageBuilder.AppendLine($"<align=center><size=150%><b><color=#FFD700>{LocalizationManager.GetText("server_updates_title")}:</color></b></size></align>");

        foreach (var version in versions)
        {
            var versionKey = version.Path.Split('.').Last();
            var versionData = version.First();

            var versionCode = versionData["version_code"]?.ToString() ?? LocalizationManager.GetText("unknown");
            var tag = versionData["tag"]?.ToString() ?? LocalizationManager.GetText("unknown");
            var patchUrls = versionData["patch_note_urls"] as JArray;

            if (patchUrls != null && patchUrls.Count > 0)
            {
                var patchFileUrl = patchUrls[0].ToString();
                var patchVersion = patchFileUrl.Split('/').Last().Split('.')[0];
                messageBuilder.AppendLine($"<b><color=#FF5733>{LocalizationManager.GetText("version")}:</color></b> {versionCode}");
                messageBuilder.AppendLine($"<b><color=#4CAF50>{LocalizationManager.GetText("tag")}:</color></b> {tag}");
                messageBuilder.AppendLine($"<b><color=#2196F3>{LocalizationManager.GetText("update")}:</color></b> {versionKey}");
                messageBuilder.AppendLine($"<b><color=#FF9800>{LocalizationManager.GetText("patch")}:</color></b> {patchVersion}");

                var patchData = await GetPatchData(patchVersion);
                string contentLabel = LocalizationManager.GetText("content");
                string patchContent = Data.CurrentLanguage == "ru" ? patchData.ru : patchData.us;
                messageBuilder.AppendLine($"<b><color=#00BCD4>{contentLabel}:</color></b>\n{patchContent}");

                messageBuilder.AppendLine();
            }
        }

        return messageBuilder.ToString();
    }

    private async Task<(string ru, string us)> GetPatchData(string patchVersion)
    {
        string patchUrl = BasePatchUrl + patchVersion + ".json.gz";
        string ruData = LocalizationManager.GetText("no_data_ru");
        string usData = LocalizationManager.GetText("no_data_en");

        try
        {
            HttpClient client = new HttpClient();
            var response = await client.GetByteArrayAsync(patchUrl);

            if (response == null || response.Length == 0)
            {
                return (ruData, usData);
            }

            byte[] jsonBytes = DecompressGZip(response);
            string jsonString = Encoding.UTF8.GetString(jsonBytes);

            var json = JObject.Parse(jsonString);

            ruData = json["Russian"]?.ToString() ?? LocalizationManager.GetText("no_data_ru");
            usData = json["English"]?.ToString() ?? LocalizationManager.GetText("no_data_en");
        }
        catch { }

        return (ruData, usData);
    }

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

    private void ShowUpdatePopup(string message, string language)
    {
        popupCanvas.SetActive(true);
        popupText.text = message;
    }

    private void OnBackButtonClick()
    {
        popupCanvas.SetActive(false);
    }
}
