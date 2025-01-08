using UnityEngine;
using UnityEngine.EventSystems;

public class OpenLinks : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    public string url;

    private Vector3 originalScale;
    private Vector3 pressedScale = new Vector3(0.9f, 0.9f, 0.9f);

    private bool isPointerInside = true; // ����, ������������� ��������� ���������

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
        // ��������� ������ � ��������, ��� ��������� ������
        transform.localScale = pressedScale;
        isPointerInside = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        // ���������� ������ � �������� ���������
        transform.localScale = originalScale;

        // ��������� ������ ������ ���� ��������� ������ �������
        if (isPointerInside)
        {
            OpenUrl();
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // ��������, ��� ��������� ����� �� ������� �������
        isPointerInside = false;
        transform.localScale = originalScale;
    }
}