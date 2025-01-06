using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;

public class Furniture : MonoBehaviour
{
    public static List<GameData.FurnitureData> Furnitures { get; set; } = new List<GameData.FurnitureData>();

    public void ProcessFileContent()
    {
        // ��������� ������ �� �����
        string fileContent = GameData.FileData;

        // ������������� JSON � ������
        RaidContainer raidContainer = JsonConvert.DeserializeObject<RaidContainer>(fileContent);

        // ������� ������ Furnitures
        Furnitures.Clear();

        if (raidContainer?.Raid?.Location?.Builder?.Furnitures?.Items != null)
        {
            var furnitureItems = raidContainer.Raid.Location.Builder.Furnitures.Items;

            // ��������� Furnitures
            foreach (var item in furnitureItems)
            {
                var furniture = item.Value;
                var building = furniture.Building;

                if (building != null)
                {
                    var inventories = new Dictionary<string, GameData.InventoryData>();

                    // ��������� Inventories
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

                                    // ���� Amount ����� null, �� ���������� Durability
                                    if (inventoryData.Amount == null)
                                    {
                                        inventoryData.Durability = stack?.Durability;
                                    }

                                    inventories.Add(cell.Key, inventoryData);
                                }
                            }
                        }
                    }

                    // ��������� InputInventories
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

                                    // ���� Amount ����� null, �� ���������� Durability
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