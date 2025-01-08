using UnityEngine;
using UnityEngine.EventSystems;

public class OpenLinks : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    public string url;

    private Vector3 originalScale;
    private Vector3 pressedScale = new Vector3(0.9f, 0.9f, 0.9f);

    private bool isPointerInside = true; // Флаг, отслеживающий положение указателя

    private void Start()
    {
        originalScale = transform.localScale;
    }

    public void OpenUrl()
    {
        if (!string.IsNullOrEmpty(url))
        {
            Application.OpenURL(url);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // Уменьшаем объект и отмечаем, что указатель внутри
        transform.localScale = pressedScale;
        isPointerInside = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        // Возвращаем объект в исходное состояние
        transform.localScale = originalScale;

        // Открываем ссылку только если указатель внутри объекта
        if (isPointerInside)
        {
            OpenUrl();
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // Отмечаем, что указатель вышел за пределы объекта
        isPointerInside = false;
        transform.localScale = originalScale;
    }
}