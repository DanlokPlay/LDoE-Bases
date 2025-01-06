using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class GameData
{
    public static string FilePath { get; set; }
    public static string FileData { get; set; }
    public static string ResourceObjects { get; set; }
    public static string RaidName { get; set; }
    public static bool NeedBan { get; set; }
    public static List<FoundamentData> Foundaments { get; set; } = new List<FoundamentData>();
    public static List<WallData> Walls { get; set; } = new List<WallData>();
    public static List<FurnitureData> Furnitures { get; set; } = new List<FurnitureData>();
    public static List<ChestEnemyData> ChestEnemy { get; set; } = new List<ChestEnemyData>();
    public static string LastActiveCanvas { get; set; }
    public static List<string> FoundFiles { get; set; } = new List<string>();
    public static int CurrentIndex { get; set; } = -1;
    public static List<DragBoxObjectData> DragBoxObjects { get; set; } = new List<DragBoxObjectData>();
    public static List<MotorcycleData> Motorcycles { get; set; } = new List<MotorcycleData>();

    public static void ClearGameData()
    {
        FilePath = null;
        FileData = null;
        ResourceObjects = null;
        RaidName = null;
        NeedBan = false;
        Foundaments.Clear();
        Walls.Clear();
        Furnitures.Clear();
        ChestEnemy.Clear();
        DragBoxObjects.Clear();
        Motorcycles.Clear();
    }

    public class FoundamentData
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Grade { get; set; }
        public string DescriptionId { get; set; }
    }

    public class WallData
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Grade { get; set; }
        public string DescriptionId { get; set; }
    }

    public class FurnitureData
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Rotation { get; set; }
        public string DescriptionId { get; set; }
        public bool Unlocked { get; set; }
        public string ColorID { get; set; }
        public Dictionary<string, InventoryData> Inventories { get; set; }
    }

    public class InventoryData
    {
        public string StackId { get; set; }
        public int? Amount { get; set; }
        public int? Durability { get; set; }
    }

    public class ChestEnemyData
    {
        public string DescriptionId { get; set; }
        public bool Unlocked { get; set; }
        public Dictionary<string, InventoryData> Inventories { get; set; }
    }

    public class DragBoxObjectData
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
        public string DescriptionId { get; set; }
    }

    public class MotorcycleData
    {
        public bool Unlocked { get; set; }
        public string PaintPattern { get; set; }
        public int FuelAmount { get; set; }
        public Dictionary<string, InventoryData> Inventories { get; set; }
        public Dictionary<string, SlotData> Slots { get; set; }
    }

    public class SlotData
    {
        public string Id { get; set; }
        public InventoryData Inventory { get; set; }
    }
}

public static class Data
{
    public static string CurrentLanguage = "ru"; // Значение по умолчанию - русский

    private static readonly string filePath = Path.Combine(Application.persistentDataPath, "Data.json");

    // Метод для загрузки данных
    public static void LoadData()
    {
        // Проверяем существует ли файл
        if (File.Exists(filePath))
        {
            string jsonContent = File.ReadAllText(filePath);
            if (!string.IsNullOrEmpty(jsonContent))
            {
                // Если файл не пустой, загружаем данные
                var dataSave = JsonUtility.FromJson<DataSave>(jsonContent);
                if (dataSave != null && !string.IsNullOrEmpty(dataSave.CurrentLanguage))
                {
                    CurrentLanguage = dataSave.CurrentLanguage; // Обновляем статическое поле
                }
            }
        }
        else
        {
            // Если файл не существует, сохраняем данные с текущим языком по умолчанию
            SaveData();
        }
    }

    // Метод для сохранения данных
    public static void SaveData()
    {
        // Создаем объект DataSave вручную и присваиваем значения
        DataSave dataSave = new DataSave();
        dataSave.CurrentLanguage = CurrentLanguage;

        // Сохраняем данные в JSON
        string jsonContent = JsonUtility.ToJson(dataSave);
        File.WriteAllText(filePath, jsonContent);
    }

    // Класс для сохранения данных
    [System.Serializable]
    public class DataSave
    {
        public string CurrentLanguage;
    }
}