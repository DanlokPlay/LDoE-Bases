using UnityEngine;
using Newtonsoft.Json;
using System.Collections.Generic;

public class DragBoxProcessor : MonoBehaviour
{
    public void ProcessFileContent()
    {
        // �������� ������ �� GameData
        string fileData = GameData.FileData;

        // ������������� JSON � ������ RaidContainer
        RaidContainer raidContainer = JsonConvert.DeserializeObject<RaidContainer>(fileData);

        // ������������ ������ drag_box_objects
        ProcessDragBoxObjects(raidContainer);
    }

    private void ProcessDragBoxObjects(RaidContainer raidContainer)
    {
        // ������� ������ DragBoxObjects
        GameData.DragBoxObjects.Clear();

        // ��������� ������� ������ drag_box_objects � ��������� � ���
        if (raidContainer?.Raid?.Location?.DragBoxObjects?.Items != null)
        {
            foreach (var item in raidContainer.Raid.Location.DragBoxObjects.Items)
            {
                // ��������� ������� ������� (position) � �������
                var position = item.Value.Item?.Position;
                if (position != null && position.Count > 0)  // ��������, ��� ������� �������� ��� ��������
                {
                    var dragBoxObjectData = new GameData.DragBoxObjectData
                    {
                        X = position[0],
                        Y = position[1],
                        Z = position[2],
                        DescriptionId = item.Value.DescriptionId
                    };

                    // ��������� ������ � ������
                    GameData.DragBoxObjects.Add(dragBoxObjectData);
                }
            }
        }
    }
}