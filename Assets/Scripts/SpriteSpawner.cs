using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UIElements;
using static GameData;


public class SpriteSpawner : MonoBehaviour
{
    public SpriteAtlas groundsAtlas;
    public SpriteAtlas itemsAtlas;

    void Start()
    {
        PlaceGround();

        PlaceFoundamentGrounds();

        PlaceWalls();


        PlaceFurniture();

        SpawnChestHomeEnemy(4, 7, 8);

        SpawnDragBoxObjects();

        if (MotorcycleDataProcessor.DataLoadedSuccessfully)
        {
            SpawnMotorcycle(new Vector3(4, -2, 8));
        }
    }

    void PlaceGround()
    {
        // ������� ������ ��� �������� ������, ������������� � UV
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Vector2> uv = new List<Vector2>();

        // �������� ������ ����� �� ������
        Sprite grassSprite = groundsAtlas.GetSprite("grass");
        if (grassSprite == null)
        {
            Debug.LogWarning("Sprite 'grass' not found in the atlas.");
            return; // �������, ���� ������ �� ������
        }

        for (int x = -2; x < 38; x += 2)
        {
            for (int y = -2; y < 38; y += 2)
            {
                // �������� ������ �������
                Vector2[] spriteVertices = grassSprite.vertices;
                ushort[] spriteTriangles = grassSprite.triangles;
                Vector2[] spriteUV = grassSprite.uv;

                // ������� ���������� ������ � ����������� �� �������
                Vector3 positionOffset = new Vector3(x, y, 9); // �������� ��� ������� ������

                // ��������� �������
                for (int i = 0; i < spriteVertices.Length; i++)
                {
                    // ����������� ������� � 2 ���� � ����������� � Vector3
                    Vector3 vertex = new Vector3(spriteVertices[i].x, spriteVertices[i].y, 0) * 2 + positionOffset; // ����������� ������
                    vertices.Add(vertex);
                }

                // ��������� ������������
                for (int i = 0; i < spriteTriangles.Length; i++)
                {
                    triangles.Add(vertices.Count - spriteVertices.Length + spriteTriangles[i]);
                }

                // ��������� UV ����������
                for (int i = 0; i < spriteUV.Length; i++)
                {
                    uv.Add(spriteUV[i]);
                }
            }
        }

        // ������� Mesh
        Mesh mesh = new Mesh
        {
            vertices = vertices.ToArray(),
            triangles = triangles.ToArray(),
            uv = uv.ToArray()
        };

        // ������� ������ � MeshFilter � MeshRenderer
        GameObject groundObject = new GameObject("Ground");
        groundObject.isStatic = true; // �������� ������ ��� �����������
        MeshFilter meshFilter = groundObject.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = groundObject.AddComponent<MeshRenderer>();

        // ������������� ��������
        Material material = new Material(Shader.Find("Sprites/Default"));
        material.mainTexture = grassSprite.texture; // ������������� �������� ������� � ��������
        meshRenderer.material = material;

        // ����������� mesh � MeshFilter
        meshFilter.mesh = mesh;

        // ������ ��� ������� ����� ����� ������������ ��� ���� ������
    }




    void PlaceFoundamentGrounds()
    {
        // ������� ������ ��� �������� ������, ������������� � UV
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Vector2> uv = new List<Vector2>();

        // ���������� ��� �������� ��������
        Texture2D lastTexture = null;

        foreach (var foundament in FoundamentBuildings.Foundaments)
        {
            // ��������� ��� ������� �� ������ Grade
            string spriteName = $"default{foundament.Grade}";

            // �������� ������ �� ������
            Sprite groundSprite = groundsAtlas.GetSprite(spriteName);
            if (groundSprite == null)
            {
                Debug.LogWarning($"Sprite '{spriteName}' not found in the atlas.");
                continue; // ����������, ���� ������ �� ������
            }

            // ��������� �������� ���������� ���������� �������
            lastTexture = groundSprite.texture;

            // �������� ������ �������
            Vector2[] spriteVertices = groundSprite.vertices;
            ushort[] spriteTriangles = groundSprite.triangles;
            Vector2[] spriteUV = groundSprite.uv;

            // ������� ���������� ������ � ����������� �� �������
            Vector3 positionOffset = new Vector3(foundament.X, foundament.Y, 8); // �������� ��� �������� ����������

            // ��������� �������
            for (int i = 0; i < spriteVertices.Length; i++)
            {
                // ����������� ������� � 2 ���� � ����������� � Vector3
                Vector3 vertex = new Vector3(spriteVertices[i].x, spriteVertices[i].y, 0) * 2 + positionOffset; // ����������� ������
                vertices.Add(vertex);
            }

            // ��������� ������������
            for (int i = 0; i < spriteTriangles.Length; i++)
            {
                triangles.Add(vertices.Count - spriteVertices.Length + spriteTriangles[i]);
            }

            // ��������� UV ����������
            for (int i = 0; i < spriteUV.Length; i++)
            {
                uv.Add(spriteUV[i]);
            }
        }

        // ������� Mesh
        Mesh mesh = new Mesh
        {
            vertices = vertices.ToArray(),
            triangles = triangles.ToArray(),
            uv = uv.ToArray()
        };

        // ������� ������ � MeshFilter � MeshRenderer
        GameObject foundamentObject = new GameObject("FoundamentGround");
        foundamentObject.isStatic = true; // �������� ������ ��� �����������
        MeshFilter meshFilter = foundamentObject.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = foundamentObject.AddComponent<MeshRenderer>();

        // ���������, ���� �� �������� ��� ���������
        if (lastTexture != null)
        {
            // ������������� ��������
            Material material = new Material(Shader.Find("Sprites/Default")); // ���������, ��� ������������ ������ ��� 2D ��������
            material.mainTexture = lastTexture; // ������������� �������� ���������� ���������� ������� � ��������
            meshRenderer.material = material;
        }

        // ����������� mesh � MeshFilter
        meshFilter.mesh = mesh;

        // ������ ��� ������� ���������� ����� ������������ ��� ���� ������
    }



    void PlaceWalls()
    {
        // ������� ��������� ��� ����
        GameObject wallContainer = new GameObject("WallContainer");
        wallContainer.isStatic = true;

        foreach (var wall in FoundamentBuildings.Walls)
        {
            // ��������� ��� ������� �� ������ DescriptionId � Grade
            string spriteName = $"{wall.DescriptionId}{wall.Grade}"; // ��������: simple0, door1, window2 � �.�.

            // �������� ������ �� ������
            Sprite wallSprite = groundsAtlas.GetSprite(spriteName);

            if (wallSprite != null)
            {
                Vector3 position;
                Quaternion rotation;

                // ���������� ������� � �������� � ����������� �� ���������
                if (wall.X % 2 == 0)
                {
                    position = new Vector3(wall.X, wall.Y - 1, 7);
                    rotation = Quaternion.identity;
                }
                else if (wall.Y % 2 == 0)
                {
                    position = new Vector3(wall.X - 1, wall.Y, 7);
                    rotation = Quaternion.Euler(0, 0, 90);
                }
                else
                {
                    position = new Vector3(wall.X, wall.Y, 7);
                    rotation = Quaternion.identity;
                }

                // ������� ��������� ������ ��� �����
                GameObject wallObject = new GameObject(spriteName); // ��� ������� �������
                SpriteRenderer spriteRenderer = wallObject.AddComponent<SpriteRenderer>();
                spriteRenderer.sprite = wallSprite; // ������������� ������ �� ������

                // ������������� ������� � �������� �������
                wallObject.transform.position = position;
                wallObject.transform.rotation = rotation;
                wallObject.transform.localScale = new Vector3(2, 2, 1); // ����������� ������

                // ��������� BoxCollider
                BoxCollider boxCollider = wallObject.AddComponent<BoxCollider>();
                boxCollider.size = new Vector3(0.15f, 1f, 0f); // ������������� ������ ����������
                boxCollider.center = new Vector3(-0.425f, 0f, 0f); // ������������� �������� ����������

                // ������� ClickableFurniture � ������������� ������
                var clickable = wallObject.AddComponent<ClickableFurniture>();
                clickable.WallData = wall; // ������������� ������ � �����

                // ������������� ��������� ��� ��������
                wallObject.transform.SetParent(wallContainer.transform);
            }
            else
            {
                Debug.LogWarning($"Sprite not found for wall: {spriteName}");
            }
        }
    }

    public void SpawnChestHomeEnemy(int x, int y, int z)
    {
        // ���������, ��� ����� � ������ ���������
        if (itemsAtlas == null) return;

        // ��������� ������ �� ������
        Sprite chestHomeEnemySprite = itemsAtlas.GetSprite("chest_home_enemy");
        if (chestHomeEnemySprite == null) return;

        // ������� ����� GameObject
        var chestEnemy = new GameObject("ChestHomeEnemy");
        chestEnemy.isStatic = true;

        // ��������� ��������� SpriteRenderer � ����������� ��� ������
        var spriteRenderer = chestEnemy.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = chestHomeEnemySprite;

        // ��������� BoxCollider � ����������� ��������
        var boxCollider = chestEnemy.AddComponent<BoxCollider>(); // ����������� ������ (1, 1, 1)

        // ������������� ������� �������
        chestEnemy.transform.position = new Vector3(x, y, z);

        // ��������� ClickableFurniture � ����������� ������
        var clickable = chestEnemy.AddComponent<ClickableFurniture>();
        clickable.ChestEnemyData = ChestEnemyLoot.ChestEnemy.FirstOrDefault();
    }

    public void SpawnDragBoxObjects()
    {
        // ���������, ��� ����� � ������ ���������
        if (itemsAtlas == null) return;

        foreach (var dragBoxObjectData in GameData.DragBoxObjects)
        {
            var position = new Vector3(
                dragBoxObjectData.X + 17,
                dragBoxObjectData.Z + 17,
                5
            );

            // ��������� ������ �� ������
            Sprite watchTowerGeneratorSprite = itemsAtlas.GetSprite("watch_tower_generator");
            if (watchTowerGeneratorSprite == null) continue; // ���������� ��������, ���� ������ �� ������

            // ������� ����� GameObject
            var dragBoxObject = new GameObject("DragBoxObject");
            dragBoxObject.isStatic = true;

            // ��������� ��������� SpriteRenderer � ����������� ��� ������
            var spriteRenderer = dragBoxObject.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = watchTowerGeneratorSprite;

            // ������������� ������� ��� ���������� �������
            dragBoxObject.transform.localScale = new Vector3(1f, 1f, 1f); // ����������� ������ �������

            // ��������� BoxCollider � ����������� ��������
            var boxCollider = dragBoxObject.AddComponent<BoxCollider>(); // ����������� ������ (1, 1, 1)

            // ������������� ������� �������
            dragBoxObject.transform.position = position;

            // ��������� ClickableFurniture � ����������� ������
            var clickable = dragBoxObject.AddComponent<ClickableFurniture>();
            clickable.DragBoxObjectData = dragBoxObjectData;
        }
    }

    public void SpawnMotorcycle(Vector3 position)
    {
        // ���������, ��� ����� � ������ ���������
        if (itemsAtlas == null) return;

        // ��������� ������ �� ������
        Sprite buildingSiteChopperSprite = itemsAtlas.GetSprite("motorcycle");
        if (buildingSiteChopperSprite == null) return; // ����������, ���� ������ �� ������

        // ������� ����� GameObject
        var motorcycle = new GameObject("Motorcycle");
        motorcycle.isStatic = true;

        // ��������� ��������� SpriteRenderer � ����������� ��� ������
        var spriteRenderer = motorcycle.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = buildingSiteChopperSprite;

        // ������������� ������� ��� ���������� �������
        motorcycle.transform.localScale = new Vector3(2f, 2f, 1f); // ����������� ������ �������

        // ��������� BoxCollider � ����������� ��������
        var boxCollider = motorcycle.AddComponent<BoxCollider>(); // ����������� ������ (1, 1, 1)

        // ������������� ������� �������
        motorcycle.transform.position = position;

        // ��������� ClickableFurniture � ����������� ������
        var clickable = motorcycle.AddComponent<ClickableFurniture>();
        clickable.MotorcycleData = MotorcycleDataProcessor.Motorcycles.FirstOrDefault();
    }

    void PlaceFurniture()
    {
        // ������� ��������� ��� ���� �������� ������
        GameObject furnituresContainer = new GameObject("FurnituresContainer");

        foreach (var furniture in Furniture.Furnitures)
        {
            SpawnFurniture(furniture.DescriptionId, furniture.X, furniture.Y, furniture.Rotation, furnituresContainer);
        }
    }

    void SpawnFurniture(string descriptionId, int x, int y, int rotation, GameObject container)
    {
        var sprite = GetFurnitureSprite(descriptionId);
        if (sprite == null) return;

        var rotationQuat = GetFurnitureRotation(descriptionId, rotation);
        var positionData = GetFurniturePosition(descriptionId, x, y, rotation);

        // �������� ������ ��� ������ � ����
        var positionFurniture = positionData.positionFurniture;
        var scaleFurniture = positionData.scaleFurniture;

        // ������ ������ ��� ������
        var furnitureObject = new GameObject(descriptionId);
        furnitureObject.isStatic = true;
        var spriteRenderer = furnitureObject.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = sprite;
        furnitureObject.transform.position = positionFurniture; // ������� ������
        furnitureObject.transform.rotation = rotationQuat;
        furnitureObject.transform.localScale = scaleFurniture; // ������� ������

        // ��������� BoxCollider ��� ��������� �������
        var boxCollider = furnitureObject.AddComponent<BoxCollider>();
        boxCollider.isTrigger = true; // ������������� ��� ��������, ���� ������, ����� ��������� ������� ��� �������

        // �������� ������ ������ � ���������
        furnitureObject.transform.parent = container.transform;

        // ��������� ���������� ��� ��������������, ���� ���� ������
        var furnitureData = GetFurnitureData(descriptionId, x, y, rotation);
        if (furnitureData != null)
        {
            var clickable = furnitureObject.AddComponent<ClickableFurniture>();
            clickable.FurnitureData = furnitureData;
        }
    }

    (Vector3 positionFurniture, Vector3 scaleFurniture) GetFurniturePosition(string descriptionId, int x, int y, int rotation)
    {
        Vector3 positionFurniture = new Vector3(x, y, 6);
        Vector3 scaleFurniture = new Vector3(1.25f, 1.25f, 1f); // ����� ������ ��� ���� ��������

        switch (descriptionId)
        {
            /*case var id when new[] { "turret_grade_1", "turret_grade_2", "turret_grade_3", "furniture_horsefeeder", "furniture_wiretrap", "buildingsite_radiotable", "furniture_shower", "workbench_smeltery_enemy", "workbench_alloy_smeltery_enemy" }.Contains(id):
                scaleFurniture = new Vector3(1.5f, 1.5f, 1f);
                break;
            case var id when new[] { "workbench_medictable_enemy", "furniture_weapon_stand", "workbench_bench_enemy", "workbench_smeltery_enemy", "workbench_alloy_smeltery_enemy", "workbench_bonfire_enemy", "furniture_lamp", "workbench_meatdryer_enemy" }.Contains(id):
                scaleFurniture = new Vector3(1.4f, 1.4f, 1f);
                break;*/
            case var id when new[] { "furniture_chest_4", "furniture_chest_8", "furniture_chest_16", "furniture_chest_24" }.Contains(id):
                scaleFurniture = new Vector3(1.15f, 1.15f, 1f);
                break;
            case "buildingsite_atv":
                positionFurniture = new Vector3(x + 2f, y + 2f, 6);
                scaleFurniture = new Vector3(4.5f, 4.5f, 1f); // ����������� ������ ��� ����
                break;
            case "workbench_dog_enclosure_enemy":
                positionFurniture = new Vector3(x + 1f, y + 3f, 6);
                scaleFurniture = new Vector3(2.5f, 6.5f, 1f);
                break;
            case var id when new[] { "workbench_acid_bath_enemy", "workbench_gardenbed_enemy", "buildingsite_chopper", "buildingsite_acid_bath" }.Contains(id):
                positionFurniture = new Vector3(x + 1f, y + 1f, 6);
                scaleFurniture = new Vector3(2.5f, 2.5f, 1f);
                break;
            case var id when new[] { "workbench_gunsmith_enemy", "buildingsite_gunsmith" }.Contains(id):
                switch (rotation)
                {
                    case 0:
                        positionFurniture = new Vector3(x + 1f, y + 2f, 6);
                        scaleFurniture = new Vector3(2.5f, 1.5f, 1f);
                        break;
                    case 1:
                        positionFurniture = new Vector3(x + 2f, y + 1f, 6);
                        scaleFurniture = new Vector3(2.5f, 1.5f, 1f);
                        break;
                    case 2:
                        positionFurniture = new Vector3(x + 1f, y + 0f, 6);
                        scaleFurniture = new Vector3(2.5f, 1.5f, 1f);
                        break;
                    case 3:
                        positionFurniture = new Vector3(x + 0f, y + 1f, 6);
                        scaleFurniture = new Vector3(2.5f, 1.5f, 1f);
                        break;
                    default:
                        positionFurniture = new Vector3(x + 1f, y + 1f, 6);
                        break;
                }
                break;
        }
        return (positionFurniture, scaleFurniture);
    }

    Sprite GetFurnitureSprite(string descriptionId)
    {
        // �������� ������ �� ������
        return itemsAtlas.GetSprite(descriptionId);
    }

    Quaternion GetFurnitureRotation(string descriptionId, int rotation)
    {
        // ������ ��� �������� (�������� ��� ���������)
        if (descriptionId == "workbench_dog_enclosure_enemy")
        {
            return Quaternion.Euler(0, 0, 0);
        }
        else if (descriptionId.StartsWith("turret_grade_"))
        {
            return Quaternion.Euler(0, 0, rotation * 90);
        }

        return Quaternion.Euler(0, 0, 180 - (rotation * 90));
    }

    FurnitureData GetFurnitureData(string descriptionId, int x, int y, int rotation)
    {
        // ������ ��� ��������� ������ � ������ (�������� ��� ���������)
        return Furniture.Furnitures.FirstOrDefault(f => f.DescriptionId == descriptionId && f.X == x && f.Y == y && f.Rotation == rotation);
    }
}