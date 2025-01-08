using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using System.Collections.Generic;
using static GameData;
using UnityEngine.U2D;
using TMPro;

public class FurniturePanel : MonoBehaviour
{
    public static FurniturePanel Instance;

    public TextMeshProUGUI rotationText;
    public TextMeshProUGUI colorIdText;
    public TextMeshProUGUI unlockedText;
    public Transform inventoriesContainer;
    public GameObject inventoryCellPrefab;
    public GameObject panel;
    public ScrollRect scrollRect;
    public GameObject descriptionObject;
    public Button closePanel;

    private TextMeshProUGUI descriptionText;
    private Image descriptionImage;

    private Dictionary<string, string> resourceDictionary;
    private Dictionary<string, int> durabilityDictionary;

    public Button showInfoButton;

    public SpriteAtlas buildingSiteAtlas;
    public SpriteAtlas itemsAtlas;
    public SpriteAtlas wallsAtlas;

    void Awake()
    {
        closePanel.onClick.AddListener(HidePanel);
        closePanel.GetComponentInChildren<TextMeshProUGUI>().text = LocalizationManager.GetText("close");


        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        BetterStreamingAssets.Initialize();
        LoadResourceDictionary();
        LoadDurabilityDictionary();

        if (descriptionObject != null)
        {
            descriptionText = descriptionObject.GetComponentInChildren<TextMeshProUGUI>();
            descriptionImage = descriptionObject.GetComponentInChildren<Image>();
        }
    }

    void LoadResourceDictionary()
    {
        string defaultLanguage = "ru";
        string translationsPath;

        try
        {
            // Загружаем данные из файла Data.json
            Data.LoadData();

            // Получаем текущий язык из статического поля Data.CurrentLanguage
            string currentLanguage = !string.IsNullOrEmpty(Data.CurrentLanguage) ? Data.CurrentLanguage : defaultLanguage;

            // Определяем путь к файлу перевода
            translationsPath = $"Translator/Items_{currentLanguage}.json";
        }
        catch
        {
            // Если произошла ошибка, используем язык по умолчанию
            translationsPath = $"Translator/Items_{defaultLanguage}.json";
        }

        try
        {
            // Читаем файл перевода
            string translationsData = BetterStreamingAssets.ReadAllText(translationsPath);
            resourceDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(translationsData);
        }
        catch
        {
            // Если чтение перевода не удалось, создаем пустой словарь
            resourceDictionary = new Dictionary<string, string>();
        }
    }

    void LoadDurabilityDictionary()
    {
        string durabilityPath = "Translator/Durability.json";
        try
        {
            string durabilityData = BetterStreamingAssets.ReadAllText(durabilityPath);
            durabilityDictionary = JsonConvert.DeserializeObject<Dictionary<string, int>>(durabilityData);
        }
        catch
        {
            durabilityDictionary = new Dictionary<string, int>();
        }
    }

    public void ShowInfo(FurnitureData furnitureData)
    {
        ShowPanel();

        // Получаем перевод для описания
        string descriptionIdTranslation = GetTranslation(furnitureData.DescriptionId);
        descriptionText.text = descriptionIdTranslation;

        // Переводим состояние (Открыт/Закрыт)
        string unlockedTranslation = LocalizationManager.GetText(furnitureData.Unlocked ? "unlocked" : "locked");
        unlockedText.text = $"{LocalizationManager.GetText("state")}: {unlockedTranslation}";

        // Если это сундуки, выводим информацию о цвете
        colorIdText.text = null;
        if (furnitureData.DescriptionId == "furniture_chest_8" || furnitureData.DescriptionId == "furniture_chest_16")
        {
            if (resourceDictionary.TryGetValue(furnitureData.ColorID, out string translatedColor))
            {
                colorIdText.text = $"{LocalizationManager.GetText("color")}: {translatedColor}";
            }
            else
            {
                colorIdText.text = $"{LocalizationManager.GetText("color")}: {furnitureData.ColorID}";
            }
        }

        // Перевод для текста поворота
        rotationText.text = $"{LocalizationManager.GetText("rotation")}: {furnitureData.Rotation}";

        // Загрузка спрайта из атласа
        Sprite descriptionSprite = buildingSiteAtlas.GetSprite(furnitureData.DescriptionId);
        if (descriptionSprite != null)
        {
            descriptionImage.sprite = descriptionSprite;
        }
        else
        {
            // Загрузка спрайта по умолчанию из Resources/Sprites
            descriptionImage.sprite = UnityEngine.Resources.Load<Sprite>("Sprites/defaultSprite"); // Путь к файлу спрайта внутри папки Resources
        }
        descriptionImage.gameObject.SetActive(true);

        // Очистка контейнера для инвентаря
        foreach (Transform child in inventoriesContainer)
        {
            Destroy(child.gameObject);
        }

        if (furnitureData.Inventories != null)
        {
            foreach (var inventory in furnitureData.Inventories)
            {
                if (!string.IsNullOrEmpty(inventory.Value.StackId))
                {
                    GameObject cell = Instantiate(inventoryCellPrefab, inventoriesContainer);

                    TextMeshProUGUI stackIdText = cell.transform.Find("StackIdText").GetComponent<TextMeshProUGUI>();
                    TextMeshProUGUI amountDurabilityText = cell.transform.Find("AmountDurabilityText").GetComponent<TextMeshProUGUI>();
                    Image cellImage = cell.transform.Find("ItemImage").GetComponent<Image>(); 
                    Image durabilityBar = cell.transform.Find("DurabilityBar").GetComponent<Image>();

                    string stackIdTranslation = GetTranslation(inventory.Value.StackId);
                    stackIdText.text = stackIdTranslation;

                    if (inventory.Value.Amount != null)
                    {
                        amountDurabilityText.text = $"{inventory.Value.Amount}";
                        durabilityBar.gameObject.SetActive(false);
                    }
                    else if (inventory.Value.Durability != null)
                    {
                        if (durabilityDictionary.TryGetValue(inventory.Value.StackId, out int maxDurability))
                        {
                            if (inventory.Value.Durability > maxDurability)
                            {
                                maxDurability = (int)inventory.Value.Durability;
                            }

                            amountDurabilityText.text = $"{inventory.Value.Durability}/{maxDurability}";

                            float durabilityRatio = (float)inventory.Value.Durability / maxDurability;
                            float maxBarHeight = 100f;
                            float newHeight = maxBarHeight * durabilityRatio;

                            RectTransform durabilityBarRect = durabilityBar.GetComponent<RectTransform>();
                            durabilityBarRect.anchorMin = new Vector2(0.5f, 0);
                            durabilityBarRect.anchorMax = new Vector2(0.5f, 0);
                            durabilityBarRect.pivot = new Vector2(0.5f, 0);

                            durabilityBarRect.sizeDelta = new Vector2(durabilityBarRect.sizeDelta.x, newHeight);
                            durabilityBar.gameObject.SetActive(true);
                        }
                        else
                        {
                            amountDurabilityText.text = $"{inventory.Value.Durability}";
                            RectTransform durabilityBarRect = durabilityBar.GetComponent<RectTransform>();
                            durabilityBarRect.anchorMin = new Vector2(0.5f, 0);
                            durabilityBarRect.anchorMax = new Vector2(0.5f, 0);
                            durabilityBarRect.pivot = new Vector2(0.5f, 0);
                            durabilityBarRect.sizeDelta = new Vector2(durabilityBarRect.sizeDelta.x, 100f);
                            durabilityBar.gameObject.SetActive(true);
                        }
                    }
                    else
                    {
                        amountDurabilityText.text = string.Empty;
                        durabilityBar.gameObject.SetActive(false);
                    }

                    // Загрузка спрайта для ячейки из атласа
                    Sprite cellSprite = itemsAtlas.GetSprite(inventory.Value.StackId);
                    if (cellSprite != null)
                    {
                        cellImage.sprite = cellSprite;
                    }
                    else
                    {
                        // Загрузка спрайта по умолчанию из Resources/Sprites
                        descriptionImage.sprite = UnityEngine.Resources.Load<Sprite>("Sprites/defaultSprite"); // Путь к файлу спрайта внутри папки Resources
                    }
                    cellImage.gameObject.SetActive(true);
                }
            }
        }

        // Обновление высоты контента для прокрутки
        RectTransform contentRectTransform = scrollRect.content;
        LayoutRebuilder.ForceRebuildLayoutImmediate(contentRectTransform);

        float contentHeight = contentRectTransform.rect.height;
        contentRectTransform.sizeDelta = new Vector2(contentRectTransform.sizeDelta.x, contentHeight);

        scrollRect.verticalNormalizedPosition = 1;
    }


    public void ShowInfo(ChestEnemyData chestEnemyData)
    {
        ShowPanel();

        // Переводим состояние (Открыт/Закрыт)
        string unlockedTranslation = LocalizationManager.GetText(chestEnemyData.Unlocked ? "unlocked" : "locked");
        unlockedText.text = $"{LocalizationManager.GetText("state")}: {unlockedTranslation}";

        string descriptionIdTranslation = GetTranslation(chestEnemyData.DescriptionId);
        descriptionText.text = descriptionIdTranslation;

        Sprite descriptionSprite = buildingSiteAtlas.GetSprite(chestEnemyData.DescriptionId);
        if (descriptionSprite != null)
        {
            descriptionImage.sprite = descriptionSprite;
        }
        else
        {
            // Загрузка спрайта по умолчанию из Resources/Sprites
            descriptionImage.sprite = UnityEngine.Resources.Load<Sprite>("Sprites/defaultSprite"); // Путь к файлу спрайта внутри папки Resources
        }
        descriptionImage.gameObject.SetActive(true);

        foreach (Transform child in inventoriesContainer)
        {
            Destroy(child.gameObject);
        }

        if (chestEnemyData.Inventories != null)
        {
            foreach (var inventory in chestEnemyData.Inventories)
            {
                if (!string.IsNullOrEmpty(inventory.Value.StackId))
                {
                    GameObject cell = Instantiate(inventoryCellPrefab, inventoriesContainer);

                    TextMeshProUGUI stackIdText = cell.transform.Find("StackIdText").GetComponent<TextMeshProUGUI>();
                    TextMeshProUGUI amountDurabilityText = cell.transform.Find("AmountDurabilityText").GetComponent<TextMeshProUGUI>();
                    Image cellImage = cell.transform.Find("ItemImage").GetComponent<Image>(); 
                    Image durabilityBar = cell.transform.Find("DurabilityBar").GetComponent<Image>();

                    string stackIdTranslation = GetTranslation(inventory.Value.StackId);
                    stackIdText.text = stackIdTranslation;

                    if (inventory.Value.Amount != null)
                    {
                        amountDurabilityText.text = $"{inventory.Value.Amount}";
                        durabilityBar.gameObject.SetActive(false);
                    }
                    else if (inventory.Value.Durability != null)
                    {
                        if (durabilityDictionary.TryGetValue(inventory.Value.StackId, out int maxDurability))
                        {
                            if (inventory.Value.Durability > maxDurability)
                            {
                                maxDurability = (int)inventory.Value.Durability;
                            }

                            amountDurabilityText.text = $"{inventory.Value.Durability}/{maxDurability}";

                            float durabilityRatio = (float)inventory.Value.Durability / maxDurability;
                            float maxBarHeight = 100f;
                            float newHeight = maxBarHeight * durabilityRatio;

                            RectTransform durabilityBarRect = durabilityBar.GetComponent<RectTransform>();
                            durabilityBarRect.anchorMin = new Vector2(0.5f, 0);
                            durabilityBarRect.anchorMax = new Vector2(0.5f, 0);
                            durabilityBarRect.pivot = new Vector2(0.5f, 0);

                            durabilityBarRect.sizeDelta = new Vector2(durabilityBarRect.sizeDelta.x, newHeight);
                            durabilityBar.gameObject.SetActive(true);
                        }
                        else
                        {
                            amountDurabilityText.text = $"{inventory.Value.Durability}";
                            RectTransform durabilityBarRect = durabilityBar.GetComponent<RectTransform>();
                            durabilityBarRect.anchorMin = new Vector2(0.5f, 0);
                            durabilityBarRect.anchorMax = new Vector2(0.5f, 0);
                            durabilityBarRect.pivot = new Vector2(0.5f, 0);
                            durabilityBarRect.sizeDelta = new Vector2(durabilityBarRect.sizeDelta.x, 100f);
                            durabilityBar.gameObject.SetActive(true);
                        }
                    }
                    else
                    {
                        amountDurabilityText.text = string.Empty;
                        durabilityBar.gameObject.SetActive(false);
                    }

                    Sprite cellSprite = itemsAtlas.GetSprite(inventory.Value.StackId);
                    if (cellSprite != null)
                    {
                        cellImage.sprite = cellSprite;
                    }
                    else
                    {
                        // Загрузка спрайта по умолчанию из Resources/Sprites
                        descriptionImage.sprite = UnityEngine.Resources.Load<Sprite>("Sprites/defaultSprite"); // Путь к файлу спрайта внутри папки Resources
                    }
                    cellImage.gameObject.SetActive(true);
                }
            }
        }

        if (rotationText != null) rotationText.text = string.Empty;
        if (colorIdText != null) colorIdText.text = string.Empty;

        RectTransform contentRectTransform = scrollRect.content;
        LayoutRebuilder.ForceRebuildLayoutImmediate(contentRectTransform);

        float contentHeight = contentRectTransform.rect.height;
        contentRectTransform.sizeDelta = new Vector2(contentRectTransform.sizeDelta.x, contentHeight);

        scrollRect.verticalNormalizedPosition = 1;
    }

    public void ShowInfo(DragBoxObjectData dragBoxObjectData)
    {
        ShowPanel();

        string descriptionIdTranslation = GetTranslation(dragBoxObjectData.DescriptionId);
        descriptionText.text = descriptionIdTranslation;

        Sprite descriptionSprite = buildingSiteAtlas.GetSprite(dragBoxObjectData.DescriptionId);
        if (descriptionSprite != null)
        {
            descriptionImage.sprite = descriptionSprite;
        }
        else
        {
            // Загрузка спрайта по умолчанию из Resources/Sprites
            descriptionImage.sprite = UnityEngine.Resources.Load<Sprite>("Sprites/defaultSprite"); // Путь к файлу спрайта внутри папки Resources
        }

        descriptionImage.gameObject.SetActive(true);

        foreach (Transform child in inventoriesContainer)
        {
            Destroy(child.gameObject);
        }

        if (rotationText != null) rotationText.text = string.Empty;
        if (colorIdText != null) colorIdText.text = string.Empty;
        if (unlockedText != null) unlockedText.text = string.Empty;

        descriptionImage.gameObject.SetActive(true);
    }

    public void ShowInfo(MotorcycleData motorcycleData)
    {
        ShowPanel();

        string descriptionIdTranslation = GetTranslation("chopper");
        descriptionText.text = descriptionIdTranslation;

        Sprite descriptionSprite = buildingSiteAtlas.GetSprite("chopper");
        if (descriptionSprite != null)
        {
            descriptionImage.sprite = descriptionSprite;
        }
        else
        {
            // Загрузка спрайта по умолчанию из Resources/Sprites
            descriptionImage.sprite = UnityEngine.Resources.Load<Sprite>("Sprites/defaultSprite"); // Путь к файлу спрайта внутри папки Resources
        }

        descriptionImage.gameObject.SetActive(true);

        foreach (Transform child in inventoriesContainer)
        {
            Destroy(child.gameObject);
        }

        // Переводим состояние (Открыт/Закрыт)
        string unlockedTranslation = LocalizationManager.GetText(motorcycleData.Unlocked ? "unlocked" : "locked");
        unlockedText.text = $"{LocalizationManager.GetText("state")}: {unlockedTranslation}";

        string paintPatternTranslation = GetTranslation(motorcycleData.PaintPattern);
        string fuelAmountTranslation = GetTranslation(motorcycleData.FuelAmount.ToString());


        rotationText.text = $"{paintPatternTranslation}";
        colorIdText.text = $"{LocalizationManager.GetText("fuel")}: {fuelAmountTranslation}";

        // Отображение инвентарей мотоцикла
        if (motorcycleData.Inventories != null)
        {
            foreach (var inventory in motorcycleData.Inventories)
            {
                if (!string.IsNullOrEmpty(inventory.Value.StackId))
                {
                    GameObject cell = Instantiate(inventoryCellPrefab, inventoriesContainer);

                    TextMeshProUGUI stackIdText = cell.transform.Find("StackIdText").GetComponent<TextMeshProUGUI>();  // Заменено на Text
                    TextMeshProUGUI amountDurabilityText = cell.transform.Find("AmountDurabilityText").GetComponent<TextMeshProUGUI>();  // Заменено на Text
                    Image cellImage = cell.transform.Find("ItemImage").GetComponent<Image>(); 
                    Image durabilityBar = cell.transform.Find("DurabilityBar").GetComponent<Image>();

                    string stackIdTranslation = GetTranslation(inventory.Value.StackId);
                    stackIdText.text = stackIdTranslation;

                    if (inventory.Value.Amount != null)
                    {
                        amountDurabilityText.text = $"{inventory.Value.Amount}";
                        durabilityBar.gameObject.SetActive(false);
                    }
                    else if (inventory.Value.Durability != null)
                    {
                        if (durabilityDictionary.TryGetValue(inventory.Value.StackId, out int maxDurability))
                        {
                            if (inventory.Value.Durability > maxDurability)
                            {
                                maxDurability = (int)inventory.Value.Durability;
                            }

                            amountDurabilityText.text = $"{inventory.Value.Durability}/{maxDurability}";

                            float durabilityRatio = (float)inventory.Value.Durability / maxDurability;
                            float maxBarHeight = 100f;
                            float newHeight = maxBarHeight * durabilityRatio;

                            RectTransform durabilityBarRect = durabilityBar.GetComponent<RectTransform>();
                            durabilityBarRect.anchorMin = new Vector2(0.5f, 0);
                            durabilityBarRect.anchorMax = new Vector2(0.5f, 0);
                            durabilityBarRect.pivot = new Vector2(0.5f, 0);

                            durabilityBarRect.sizeDelta = new Vector2(durabilityBarRect.sizeDelta.x, newHeight);
                            durabilityBar.gameObject.SetActive(true);
                        }
                        else
                        {
                            amountDurabilityText.text = $"{inventory.Value.Durability}";
                            RectTransform durabilityBarRect = durabilityBar.GetComponent<RectTransform>();
                            durabilityBarRect.anchorMin = new Vector2(0.5f, 0);
                            durabilityBarRect.anchorMax = new Vector2(0.5f, 0);
                            durabilityBarRect.pivot = new Vector2(0.5f, 0);
                            durabilityBarRect.sizeDelta = new Vector2(durabilityBarRect.sizeDelta.x, 100f);
                            durabilityBar.gameObject.SetActive(true);
                        }
                    }
                    else
                    {
                        amountDurabilityText.text = string.Empty;
                        durabilityBar.gameObject.SetActive(false);
                    }

                    Sprite cellSprite = itemsAtlas.GetSprite(inventory.Value.StackId);
                    if (cellSprite != null)
                    {
                        cellImage.sprite = cellSprite;
                    }
                    else
                    {
                        // Загрузка спрайта по умолчанию из Resources/Sprites
                        descriptionImage.sprite = UnityEngine.Resources.Load<Sprite>("Sprites/defaultSprite"); // Путь к файлу спрайта внутри папки Resources
                    }
                    cellImage.gameObject.SetActive(true);
                }
            }
        }

        RectTransform contentRectTransform = scrollRect.content;
        LayoutRebuilder.ForceRebuildLayoutImmediate(contentRectTransform);

        float contentHeight = contentRectTransform.rect.height;
        contentRectTransform.sizeDelta = new Vector2(contentRectTransform.sizeDelta.x, contentHeight);

        scrollRect.verticalNormalizedPosition = 1;
    }
    
    public void ShowInfo(WallData wallData)
    {
        ShowPanel();

        string descriptionIdTranslation = GetTranslation($"{wallData.DescriptionId}{wallData.Grade}");
        descriptionText.text = descriptionIdTranslation;

        Sprite descriptionSprite = wallsAtlas.GetSprite(wallData.DescriptionId + wallData.Grade);
        if (descriptionSprite != null)
        {
            descriptionImage.sprite = descriptionSprite;
        }
        else
        {
            // Загрузка спрайта по умолчанию из Resources/Sprites
            descriptionImage.sprite = UnityEngine.Resources.Load<Sprite>("Sprites/defaultSprite"); // Путь к файлу спрайта внутри папки Resources
        }

        descriptionImage.gameObject.SetActive(true);

        foreach (Transform child in inventoriesContainer)
        {
            Destroy(child.gameObject);
        }

        if (rotationText != null) rotationText.text = string.Empty;
        if (colorIdText != null) colorIdText.text = string.Empty;
        if (unlockedText != null) unlockedText.text = string.Empty;

        descriptionImage.gameObject.SetActive(true);
    }
    

    string GetTranslation(string key)
    {
        if (resourceDictionary != null && resourceDictionary.TryGetValue(key, out string translation))
        {
            return translation;
        }
        return key;
    }

    public void ShowPanel()
    {
        if (panel != null)
        {
            panel.SetActive(true);
        }
        showInfoButton.gameObject.SetActive(false);
    }

    public void HidePanel()
    {
        if (panel != null)
        {
            panel.SetActive(false);

            if (ClickableFurniture.currentActiveObject != null)
            {
                ClickableFurniture.currentActiveObject.ResetColor();
                ClickableFurniture.currentActiveObject = null;
            }
        }

        showInfoButton.gameObject.SetActive(true);
    }
}