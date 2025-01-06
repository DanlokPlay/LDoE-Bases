using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static GameData;

public class MotorcycleDataProcessor : MonoBehaviour
{
    public static List<GameData.MotorcycleData> Motorcycles { get; set; } = new List<GameData.MotorcycleData>();
    public static bool DataLoadedSuccessfully { get; private set; } = false;

    public void ProcessFileContent()
    {
        // Считываем данные из файла
        string fileContent = GameData.FileData;

        // Десериализуем JSON в объект RaidContainer
        RaidContainer raidContainer = JsonConvert.DeserializeObject<RaidContainer>(fileContent);

        // Проверка десериализации
        if (raidContainer == null)
        {
            return;
        }

        // Очищаем список Motorcycles
        Motorcycles.Clear();

        // Проверка и обработка данных
        if (raidContainer?.Raid?.Location?.Transports?.Items != null)
        {
            var motorcycleItems = raidContainer.Raid.Location.Transports.Items
                .Where(item => item.Value.DescriptionId == "motorcycle")
                .Select(item => item.Value)
                .ToList();

            foreach (var item in motorcycleItems)
            {
                var motorcycleData = new MotorcycleData
                {
                    Unlocked = item.Item?.Unlocked ?? false,
                    PaintPattern = item.Item?.PaintPattern,
                    // Обновленный код для извлечения количества топлива
                    FuelAmount = item.Item?.Resources != null
                        && item.Item.Resources.ContainsKey("motorcycle_fuel")
                        && item.Item.Resources["motorcycle_fuel"] != null
                        ? (int)item.Item.Resources["motorcycle_fuel"].Amount
                        : 0,
                    Inventories = ConvertInventories(item.Item?.Inventories),
                    Slots = ConvertSlots(item.Item?.Slots)
                };
                Motorcycles.Add(motorcycleData);
            }
            DataLoadedSuccessfully = true;
        }
        else
        {
            DataLoadedSuccessfully = false;
        }
    }

    // Метод для преобразования Inventories из Item в формат GameData.InventoryData
    private Dictionary<string, GameData.InventoryData> ConvertInventories(Dictionary<string, Inventories> inventories)
    {
        var result = new Dictionary<string, GameData.InventoryData>();

        if (inventories != null)
        {
            foreach (var inventory in inventories)
            {
                var inv = inventory.Value.Inventory;
                if (inv != null)
                {
                    foreach (var cell in inv.Cells)
                    {
                        result[cell.Key] = new GameData.InventoryData
                        {
                            StackId = cell.Value.StackId,
                            Amount = cell.Value.Stack?.Amount,
                            Durability = cell.Value.Stack?.Durability
                        };
                    }
                }
            }
        }

        return result;
    }

    // Метод для преобразования Slots из Item в формат GameData.SlotData
    private Dictionary<string, GameData.SlotData> ConvertSlots(Dictionary<string, Slots> slots)
    {
        var result = new Dictionary<string, GameData.SlotData>();

        if (slots != null)
        {
            foreach (var slot in slots)
            {
                var slotData = new GameData.SlotData
                {
                    Id = slot.Value.Id,
                    Inventory = ConvertSlotInventory(slot.Value.Inventory)
                };
                result[slot.Key] = slotData;
            }
        }

        return result;
    }

    // Метод для преобразования инвентаря в слоте
    private GameData.InventoryData ConvertSlotInventory(Inventory inventory)
    {
        if (inventory?.Cells != null && inventory.Cells.Count > 0)
        {
            var firstCell = inventory.Cells.First().Value;
            return new GameData.InventoryData
            {
                StackId = firstCell.StackId,
                Amount = firstCell.Stack?.Amount,
                Durability = firstCell.Stack?.Durability
            };
        }

        return null;
    }
}