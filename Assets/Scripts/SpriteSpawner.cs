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
        // Создаем список для хранения вершин, треугольников и UV
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Vector2> uv = new List<Vector2>();

        // Получаем спрайт травы из атласа
        Sprite grassSprite = groundsAtlas.GetSprite("grass");
        if (grassSprite == null)
        {
            Debug.LogWarning("Sprite 'grass' not found in the atlas.");
            return; // Выходим, если спрайт не найден
        }

        for (int x = -2; x < 38; x += 2)
        {
            for (int y = -2; y < 38; y += 2)
            {
                // Получаем данные спрайта
                Vector2[] spriteVertices = grassSprite.vertices;
                ushort[] spriteTriangles = grassSprite.triangles;
                Vector2[] spriteUV = grassSprite.uv;

                // Смещаем координаты вершин в зависимости от позиции
                Vector3 positionOffset = new Vector3(x, y, 9); // Смещение для текущей плитки

                // Добавляем вершины
                for (int i = 0; i < spriteVertices.Length; i++)
                {
                    // Увеличиваем размеры в 2 раза и преобразуем в Vector3
                    Vector3 vertex = new Vector3(spriteVertices[i].x, spriteVertices[i].y, 0) * 2 + positionOffset; // Увеличиваем размер
                    vertices.Add(vertex);
                }

                // Добавляем треугольники
                for (int i = 0; i < spriteTriangles.Length; i++)
                {
                    triangles.Add(vertices.Count - spriteVertices.Length + spriteTriangles[i]);
                }

                // Добавляем UV координаты
                for (int i = 0; i < spriteUV.Length; i++)
                {
                    uv.Add(spriteUV[i]);
                }
            }
        }

        // Создаем Mesh
        Mesh mesh = new Mesh
        {
            vertices = vertices.ToArray(),
            triangles = triangles.ToArray(),
            uv = uv.ToArray()
        };

        // Создаем объект с MeshFilter и MeshRenderer
        GameObject groundObject = new GameObject("Ground");
        groundObject.isStatic = true; // Помечаем объект как статический
        MeshFilter meshFilter = groundObject.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = groundObject.AddComponent<MeshRenderer>();

        // Устанавливаем материал
        Material material = new Material(Shader.Find("Sprites/Default"));
        material.mainTexture = grassSprite.texture; // Устанавливаем текстуру спрайта в материал
        meshRenderer.material = material;

        // Присваиваем mesh к MeshFilter
        meshFilter.mesh = mesh;

        // Теперь все спрайты травы будут представлены как один объект
    }




    void PlaceFoundamentGrounds()
    {
        // Создаем списки для хранения вершин, треугольников и UV
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Vector2> uv = new List<Vector2>();

        // Переменная для хранения текстуры
        Texture2D lastTexture = null;

        foreach (var foundament in FoundamentBuildings.Foundaments)
        {
            // Формируем имя спрайта на основе Grade
            string spriteName = $"default{foundament.Grade}";

            // Получаем спрайт из атласа
            Sprite groundSprite = groundsAtlas.GetSprite(spriteName);
            if (groundSprite == null)
            {
                Debug.LogWarning($"Sprite '{spriteName}' not found in the atlas.");
                continue; // Пропускаем, если спрайт не найден
            }

            // Сохраняем текстуру последнего найденного спрайта
            lastTexture = groundSprite.texture;

            // Получаем данные спрайта
            Vector2[] spriteVertices = groundSprite.vertices;
            ushort[] spriteTriangles = groundSprite.triangles;
            Vector2[] spriteUV = groundSprite.uv;

            // Смещаем координаты вершин в зависимости от позиции
            Vector3 positionOffset = new Vector3(foundament.X, foundament.Y, 8); // Смещение для текущего фундамента

            // Добавляем вершины
            for (int i = 0; i < spriteVertices.Length; i++)
            {
                // Увеличиваем размеры в 2 раза и преобразуем в Vector3
                Vector3 vertex = new Vector3(spriteVertices[i].x, spriteVertices[i].y, 0) * 2 + positionOffset; // Увеличиваем размер
                vertices.Add(vertex);
            }

            // Добавляем треугольники
            for (int i = 0; i < spriteTriangles.Length; i++)
            {
                triangles.Add(vertices.Count - spriteVertices.Length + spriteTriangles[i]);
            }

            // Добавляем UV координаты
            for (int i = 0; i < spriteUV.Length; i++)
            {
                uv.Add(spriteUV[i]);
            }
        }

        // Создаем Mesh
        Mesh mesh = new Mesh
        {
            vertices = vertices.ToArray(),
            triangles = triangles.ToArray(),
            uv = uv.ToArray()
        };

        // Создаем объект с MeshFilter и MeshRenderer
        GameObject foundamentObject = new GameObject("FoundamentGround");
        foundamentObject.isStatic = true; // Помечаем объект как статический
        MeshFilter meshFilter = foundamentObject.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = foundamentObject.AddComponent<MeshRenderer>();

        // Проверяем, есть ли текстура для установки
        if (lastTexture != null)
        {
            // Устанавливаем материал
            Material material = new Material(Shader.Find("Sprites/Default")); // Убедитесь, что используется шейдер для 2D спрайтов
            material.mainTexture = lastTexture; // Устанавливаем текстуру последнего найденного спрайта в материал
            meshRenderer.material = material;
        }

        // Присваиваем mesh к MeshFilter
        meshFilter.mesh = mesh;

        // Теперь все спрайты фундамента будут представлены как один объект
    }



    void PlaceWalls()
    {
        // Создаем контейнер для стен
        GameObject wallContainer = new GameObject("WallContainer");
        wallContainer.isStatic = true;

        foreach (var wall in FoundamentBuildings.Walls)
        {
            // Формируем имя спрайта на основе DescriptionId и Grade
            string spriteName = $"{wall.DescriptionId}{wall.Grade}"; // Например: simple0, door1, window2 и т.д.

            // Получаем спрайт из атласа
            Sprite wallSprite = groundsAtlas.GetSprite(spriteName);

            if (wallSprite != null)
            {
                Vector3 position;
                Quaternion rotation;

                // Определяем позицию и вращение в зависимости от координат
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

                // Создаем временный объект для стены
                GameObject wallObject = new GameObject(spriteName); // Имя объекта спрайта
                SpriteRenderer spriteRenderer = wallObject.AddComponent<SpriteRenderer>();
                spriteRenderer.sprite = wallSprite; // Устанавливаем спрайт из атласа

                // Устанавливаем позицию и вращение объекта
                wallObject.transform.position = position;
                wallObject.transform.rotation = rotation;
                wallObject.transform.localScale = new Vector3(2, 2, 1); // Увеличиваем размер

                // Добавляем BoxCollider
                BoxCollider boxCollider = wallObject.AddComponent<BoxCollider>();
                boxCollider.size = new Vector3(0.15f, 1f, 0f); // Устанавливаем размер коллайдера
                boxCollider.center = new Vector3(-0.425f, 0f, 0f); // Устанавливаем смещение коллайдера

                // Создаем ClickableFurniture и устанавливаем данные
                var clickable = wallObject.AddComponent<ClickableFurniture>();
                clickable.WallData = wall; // Устанавливаем данные о стене

                // Устанавливаем контейнер как родителя
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
        // Убедитесь, что атлас и спрайт загружены
        if (itemsAtlas == null) return;

        // Извлекаем спрайт из атласа
        Sprite chestHomeEnemySprite = itemsAtlas.GetSprite("chest_home_enemy");
        if (chestHomeEnemySprite == null) return;

        // Создаем новый GameObject
        var chestEnemy = new GameObject("ChestHomeEnemy");
        chestEnemy.isStatic = true;

        // Добавляем компонент SpriteRenderer и присваиваем ему спрайт
        var spriteRenderer = chestEnemy.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = chestHomeEnemySprite;

        // Добавляем BoxCollider с стандартным размером
        var boxCollider = chestEnemy.AddComponent<BoxCollider>(); // Стандартный размер (1, 1, 1)

        // Устанавливаем позицию объекта
        chestEnemy.transform.position = new Vector3(x, y, z);

        // Добавляем ClickableFurniture и присваиваем данные
        var clickable = chestEnemy.AddComponent<ClickableFurniture>();
        clickable.ChestEnemyData = ChestEnemyLoot.ChestEnemy.FirstOrDefault();
    }

    public void SpawnDragBoxObjects()
    {
        // Убедитесь, что атлас и спрайт загружены
        if (itemsAtlas == null) return;

        foreach (var dragBoxObjectData in GameData.DragBoxObjects)
        {
            var position = new Vector3(
                dragBoxObjectData.X + 17,
                dragBoxObjectData.Z + 17,
                5
            );

            // Извлекаем спрайт из атласа
            Sprite watchTowerGeneratorSprite = itemsAtlas.GetSprite("watch_tower_generator");
            if (watchTowerGeneratorSprite == null) continue; // Пропустите итерацию, если спрайт не найден

            // Создаем новый GameObject
            var dragBoxObject = new GameObject("DragBoxObject");
            dragBoxObject.isStatic = true;

            // Добавляем компонент SpriteRenderer и присваиваем ему спрайт
            var spriteRenderer = dragBoxObject.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = watchTowerGeneratorSprite;

            // Устанавливаем масштаб для увеличения спрайта
            dragBoxObject.transform.localScale = new Vector3(1f, 1f, 1f); // Увеличиваем размер спрайта

            // Добавляем BoxCollider с стандартным размером
            var boxCollider = dragBoxObject.AddComponent<BoxCollider>(); // Стандартный размер (1, 1, 1)

            // Устанавливаем позицию объекта
            dragBoxObject.transform.position = position;

            // Добавляем ClickableFurniture и присваиваем данные
            var clickable = dragBoxObject.AddComponent<ClickableFurniture>();
            clickable.DragBoxObjectData = dragBoxObjectData;
        }
    }

    public void SpawnMotorcycle(Vector3 position)
    {
        // Убедитесь, что атлас и спрайт загружены
        if (itemsAtlas == null) return;

        // Извлекаем спрайт из атласа
        Sprite buildingSiteChopperSprite = itemsAtlas.GetSprite("motorcycle");
        if (buildingSiteChopperSprite == null) return; // Пропускаем, если спрайт не найден

        // Создаем новый GameObject
        var motorcycle = new GameObject("Motorcycle");
        motorcycle.isStatic = true;

        // Добавляем компонент SpriteRenderer и присваиваем ему спрайт
        var spriteRenderer = motorcycle.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = buildingSiteChopperSprite;

        // Устанавливаем масштаб для увеличения спрайта
        motorcycle.transform.localScale = new Vector3(2f, 2f, 1f); // Увеличиваем размер спрайта

        // Добавляем BoxCollider с стандартным размером
        var boxCollider = motorcycle.AddComponent<BoxCollider>(); // Стандартный размер (1, 1, 1)

        // Устанавливаем позицию объекта
        motorcycle.transform.position = position;

        // Добавляем ClickableFurniture и присваиваем данные
        var clickable = motorcycle.AddComponent<ClickableFurniture>();
        clickable.MotorcycleData = MotorcycleDataProcessor.Motorcycles.FirstOrDefault();
    }

    void PlaceFurniture()
    {
        // Создаем контейнер для всех объектов мебели
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

        // Получаем данные для мебели и фона
        var positionFurniture = positionData.positionFurniture;
        var scaleFurniture = positionData.scaleFurniture;

        // Создаём объект для мебели
        var furnitureObject = new GameObject(descriptionId);
        furnitureObject.isStatic = true;
        var spriteRenderer = furnitureObject.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = sprite;
        furnitureObject.transform.position = positionFurniture; // Позиция мебели
        furnitureObject.transform.rotation = rotationQuat;
        furnitureObject.transform.localScale = scaleFurniture; // Масштаб мебели

        // Добавляем BoxCollider для обработки нажатий
        var boxCollider = furnitureObject.AddComponent<BoxCollider>();
        boxCollider.isTrigger = true; // Устанавливаем это значение, если хотите, чтобы коллайдер работал как триггер

        // Помещаем объект мебели в контейнер
        furnitureObject.transform.parent = container.transform;

        // Добавляем компоненты для взаимодействия, если есть данные
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
        Vector3 scaleFurniture = new Vector3(1.25f, 1.25f, 1f); // Общий размер для всех спрайтов

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
                scaleFurniture = new Vector3(4.5f, 4.5f, 1f); // Специальный размер для фона
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
        // Получаем спрайт из атласа
        return itemsAtlas.GetSprite(descriptionId);
    }

    Quaternion GetFurnitureRotation(string descriptionId, int rotation)
    {
        // Логика для поворота (осталась без изменений)
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
        // Логика для получения данных о мебели (осталась без изменений)
        return Furniture.Furnitures.FirstOrDefault(f => f.DescriptionId == descriptionId && f.X == x && f.Y == y && f.Rotation == rotation);
    }
}