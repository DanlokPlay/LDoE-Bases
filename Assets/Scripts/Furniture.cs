using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;

public class Furniture : MonoBehaviour
{
    public static List<GameData.FurnitureData> Furnitures { get; set; } = new List<GameData.FurnitureData>();

    public void ProcessFileContent()
    {
        // Считываем данные из файла
        string fileContent = GameData.FileData;

        // Десериализуем JSON в объект
        RaidContainer raidContainer = JsonConvert.DeserializeObject<RaidContainer>(fileContent);

        // Очищаем список Furnitures
        Furnitures.Clear();

        if (raidContainer?.Raid?.Location?.Builder?.Furnitures?.Items != null)
        {
            var furnitureItems = raidContainer.Raid.Location.Builder.Furnitures.Items;

            // Обработка Furnitures
            foreach (var item in furnitureItems)
            {
                var furniture = item.Value;
                var building = furniture.Building;

                if (building != null)
                {
                    var inventories = new Dictionary<string, GameData.InventoryData>();

                    // Обработка Inventories
                    if (building.Inventories != null)
                    {
                        foreach (var inventory in building.Inventories)
                        {
                            if (inventory.Value.Inventory != null && inventory.Value.Inventory.Cells != null)
                            {
                                foreach (var cell in inventory.Value.Inventory.Cells)
                                {
                                    var stack = cell.Value.Stack;
                                    var inventoryData = new GameData.InventoryData
                                    {
                                        StackId = cell.Value.StackId,
                                        Amount = stack?.Amount
                                    };

                                    // Если Amount равен null, то записываем Durability
                                    if (inventoryData.Amount == null)
                                    {
                                        inventoryData.Durability = stack?.Durability;
                                    }

                                    inventories.Add(cell.Key, inventoryData);
                                }
                            }
                        }
                    }

                    // Обработка InputInventories
                    if (building.Workbench?.InputInventories != null)
                    {
                        foreach (var inputInventory in building.Workbench.InputInventories)
                        {
                            if (inputInventory.Value.Inventory != null && inputInventory.Value.Inventory.Cells != null)
                            {
                                foreach (var cell in inputInventory.Value.Inventory.Cells)
                                {
                                    var stack = cell.Value.Stack;
                                    var inventoryData = new GameData.InventoryData
                                    {
                                        StackId = cell.Value.StackId,
                                        Amount = stack?.Amount
                                    };

                                    // Если Amount равен null, то записываем Durability
                                    if (inventoryData.Amount == null)
                                    {
                                        inventoryData.Durability = stack?.Durability;
                                    }

                                    inventories.Add(cell.Key, inventoryData);
                                }
                            }
                        }
                    }

                    GameData.FurnitureData furnitureData = new GameData.FurnitureData
                    {
                        X = 2 * building.X,
                        Y = 2 * building.Y,
                        Rotation = building.Rotation,
                        DescriptionId = furniture.DescriptionId,
                        Inventories = inventories,
                        ColorID = building.ColorId,
                        Unlocked = building.Unlocked.GetValueOrDefault(false),
                    };

                    Furnitures.Add(furnitureData);
                }
            }
        }
    }
}