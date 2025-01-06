using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ChestEnemyLoot : MonoBehaviour
{
    public static List<GameData.ChestEnemyData> ChestEnemy { get; set; } = new List<GameData.ChestEnemyData>();

    public void ProcessFileContent()
    {
        // ��������� ������ �� �����
        string fileContent = GameData.FileData;

        // ������������� JSON � ������ RaidContainer
        RaidContainer raidContainer = JsonConvert.DeserializeObject<RaidContainer>(fileContent);

        // �������� ��������������
        if (raidContainer == null)
        {
            Debug.LogError("Failed to deserialize JSON.");
            return;
        }

        // ������� ������ ChestEnemy
        ChestEnemy.Clear();

        // �������� � ��������� ������
        if (raidContainer?.Raid?.Location?.LootObjects?.Items != null)
        {
            var chestHomeEnemyItems = raidContainer.Raid.Location.LootObjects.Items
                .Where(item => item.Value.DescriptionId == "chest_home_enemy")
                .Select(item => item.Value)
                .ToList();

            foreach (var item in chestHomeEnemyItems)
            {
                var chestEnemyData = new GameData.ChestEnemyData
                {
                    DescriptionId = item.DescriptionId,
                    Unlocked = item.Item?.Unlocked ?? false,
                    Inventories = ConvertInventories(item.Item?.Inventories)
                };

                ChestEnemy.Add(chestEnemyData);
            }
        }
        else
        {
            Debug.LogWarning("LootObjects are missing or null.");
        }
    }

    // ����� ��� �������������� Inventories �� Item � ������ GameData.InventoryData
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
}