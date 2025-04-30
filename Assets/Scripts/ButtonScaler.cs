using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonScaler : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private Vector3 originalScale;
    public float scaleMultiplier = 0.9f; // ��������� ��������� ������ (90% �� ���������)

    void Start()
    {
        originalScale = transform.localScale;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        transform.localScale = originalScale * scaleMultiplier;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        transform.localScale = originalScale;
    }
}