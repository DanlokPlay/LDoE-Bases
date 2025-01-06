using UnityEngine;
using Newtonsoft.Json;
using System.Collections.Generic;

public class DragBoxProcessor : MonoBehaviour
{
    public void ProcessFileContent()
    {
        // Получаем данные из GameData
        string fileData = GameData.FileData;

        // Десериализуем JSON в объект RaidContainer
        RaidContainer raidContainer = JsonConvert.DeserializeObject<RaidContainer>(fileData);

        // Обрабатываем данные drag_box_objects
        ProcessDragBoxObjects(raidContainer);
    }

    private void ProcessDragBoxObjects(RaidContainer raidContainer)
    {
        // Очищаем список DragBoxObjects
        GameData.DragBoxObjects.Clear();

        // Проверяем наличие секции drag_box_objects и элементов в ней
        if (raidContainer?.Raid?.Location?.DragBoxObjects?.Items != null)
        {
            foreach (var item in raidContainer.Raid.Location.DragBoxObjects.Items)
            {
                // Проверяем наличие позиции (position) у объекта
                var position = item.Value.Item?.Position;
                if (position != null && position.Count > 0)  // Убедимся, что позиция содержит три элемента
                {
                    var dragBoxObjectData = new GameData.DragBoxObjectData
                    {
                        X = position[0],
                        Y = position[1],
                        Z = position[2],
                        DescriptionId = item.Value.DescriptionId
                    };

                    // Добавляем объект в список
                    GameData.DragBoxObjects.Add(dragBoxObjectData);
                }
            }
        }
    }
}