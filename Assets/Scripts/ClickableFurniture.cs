using UnityEngine;

public class ClickableFurniture : MonoBehaviour
{
    // Определение данных для каждого типа объекта
    public GameData.FurnitureData FurnitureData { get; set; }
    public GameData.ChestEnemyData ChestEnemyData { get; set; }
    public GameData.DragBoxObjectData DragBoxObjectData { get; set; }
    public GameData.MotorcycleData MotorcycleData { get; set; }
    public GameData.WallData WallData { get; set; }

    public Vector2 InitialTouchPosition { get; private set; }
    public float SwipeThreshold => 25f;

    private Color originalColor;
    private Renderer objectRenderer;
    public Color highlightColorAfterClick = Color.red;

    // Статическое поле для текущего активного объекта
    public static ClickableFurniture currentActiveObject;

    void Start()
    {
        objectRenderer = GetComponent<Renderer>();
        if (objectRenderer != null)
        {
            originalColor = objectRenderer.material.color;
        }
    }

    public void OnTouchBegan()
    {
        InitialTouchPosition = Input.mousePosition;  // Сохраняем начальную позицию касания
    }

    public void OnTouchEnded()
    {
        ShowInfo();
        HighlightObject();
    }

    private void ShowInfo()
    {
        if (ChestEnemyData != null)
        {
            FurniturePanel.Instance.ShowInfo(ChestEnemyData);
        }
        else if (FurnitureData != null)
        {
            FurniturePanel.Instance.ShowInfo(FurnitureData);
        }
        else if (DragBoxObjectData != null)
        {
            FurniturePanel.Instance.ShowInfo(DragBoxObjectData);
        }
        else if (MotorcycleData != null)
        {
            FurniturePanel.Instance.ShowInfo(MotorcycleData);
        }
        else if (WallData != null)
        {
            FurniturePanel.Instance.ShowInfo(WallData);
        }
    }

    private void HighlightObject()
    {
        if (currentActiveObject != null && currentActiveObject != this)
        {
            currentActiveObject.ResetColor();
        }

        currentActiveObject = this;  // Устанавливаем текущий активный объект
        objectRenderer.material.color = highlightColorAfterClick;
    }

    public void ResetColor()
    {
        objectRenderer.material.color = originalColor;
    }
}