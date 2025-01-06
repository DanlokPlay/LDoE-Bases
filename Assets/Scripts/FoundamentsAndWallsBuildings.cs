using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;

public class FoundamentBuildings : MonoBehaviour
{
    public static List<GameData.FoundamentData> Foundaments { get; set; } = new List<GameData.FoundamentData>();
    public static List<GameData.WallData> Walls { get; set; } = new List<GameData.WallData>();

    public void ProcessFileContent()
    {
        // Считываем данные из файла
        string fileContent = GameData.FileData;

        // Десериализуем JSON в объект
        RaidContainer raidContainer = JsonConvert.DeserializeObject<RaidContainer>(fileContent);

        // Очищаем списки Foundaments и Walls
        Foundaments.Clear();
        Walls.Clear();

        if (raidContainer?.Raid?.Location?.Builder != null)
        {
            var builder = raidContainer.Raid.Location.Builder;

            // Обработка Foundaments
            if (builder.Foundaments?.Items != null)
            {
                foreach (var item in builder.Foundaments.Items)
                {
                    var foundament = item.Value;

                    GameData.FoundamentData foundamentData = new GameData.FoundamentData
                    {
                        X = 2 * foundament.Building?.X ?? 0,
                        Y = 2 * foundament.Building?.Y ?? 0,
                        Grade = foundament.Building?.Grade ?? 0,
                        DescriptionId = foundament.DescriptionId
                    };

                    Foundaments.Add(foundamentData);
                }
            }

            // Обработка Walls
            if (builder.Walls?.Items != null)
            {
                foreach (var item in builder.Walls.Items)
                {
                    var wall = item.Value;

                    GameData.WallData wallData = new GameData.WallData
                    {
                        X = wall.Building?.X ?? 0,
                        Y = wall.Building?.Y ?? 0,
                        Grade = wall.Building?.Grade ?? 0,
                        DescriptionId = wall.DescriptionId
                    };

                    Walls.Add(wallData);
                }
            }
        }
    }
}