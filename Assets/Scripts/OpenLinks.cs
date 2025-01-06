using UnityEngine;
using UnityEngine.EventSystems;

public class OpenLinks : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    [SerializeField] private string url; // Поле, доступное в инспекторе

    // Размеры объекта
    private Vector3 originalScale;
    public Vector3 pressedScale = new Vector3(0.65f, 0.65f, 0.65f); // Уменьшенный размер

    private void Start()
    {
        originalScale = transform.localScale; // Сохраняем оригинальный размер
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!string.IsNullOrEmpty(url))
        {
            Application.OpenURL(url);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // Уменьшаем размер объекта
        transform.localScale = pressedScale;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        // Возвращаем размер объекта обратно
        transform.localScale = originalScale;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // Если указатель выходит за пределы объекта, возвращаем размер
        transform.localScale = originalScale;
    }
}
