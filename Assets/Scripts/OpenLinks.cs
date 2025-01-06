using UnityEngine;
using UnityEngine.EventSystems;

public class OpenLinks : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    [SerializeField] private string url; // ����, ��������� � ����������

    // ������� �������
    private Vector3 originalScale;
    public Vector3 pressedScale = new Vector3(0.65f, 0.65f, 0.65f); // ����������� ������

    private void Start()
    {
        originalScale = transform.localScale; // ��������� ������������ ������
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
        // ��������� ������ �������
        transform.localScale = pressedScale;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        // ���������� ������ ������� �������
        transform.localScale = originalScale;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // ���� ��������� ������� �� ������� �������, ���������� ������
        transform.localScale = originalScale;
    }
}
