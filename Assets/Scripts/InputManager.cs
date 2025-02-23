using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    private ClickableFurniture activeObject;
    public float swipeThreshold = 25f;

    void Update()
    {
        HandleInput();
    }

    private void HandleInput()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            ProcessTouch(touch);
        }
        else if (Input.GetMouseButtonDown(0))
        {
            ProcessMouseInput(Input.mousePosition);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            ProcessMouseInput(Input.mousePosition, true);
        }
    }

    private void ProcessTouch(Touch touch)
    {
        if (touch.phase == TouchPhase.Began || touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
        {
            if (!IsPointerOverUI(touch.position))
            {
                if (touch.phase == TouchPhase.Began)
                {
                    HandleTouchBegan(touch.position);
                }
                else
                {
                    HandleTouchEnded(touch.position);
                }
            }
        }
    }

    private void ProcessMouseInput(Vector2 mousePosition, bool isEnd = false)
    {
        if (!IsPointerOverUI(mousePosition))
        {
            if (!isEnd)
            {
                HandleTouchBegan(mousePosition);
            }
            else
            {
                HandleTouchEnded(mousePosition);
            }
        }
    }

    private void HandleTouchBegan(Vector2 position)
    {
        if (TryGetHitObject(position, out RaycastHit hit))
        {
            ClickableFurniture clickedFurniture = hit.collider.GetComponent<ClickableFurniture>();

            if (clickedFurniture != null)
            {
                activeObject = clickedFurniture;  // Устанавливаем текущий объект
                activeObject.OnTouchBegan();  // Вызываем метод обработки касания
            }
        }
    }

    private void HandleTouchEnded(Vector2 position)
    {
        if (activeObject != null)
        {
            float distance = Vector2.Distance(activeObject.InitialTouchPosition, position);
            if (distance < swipeThreshold)  // Используем порог из InputManager
            {
                activeObject.OnTouchEnded();  // Вызываем метод окончания касания
            }

            activeObject = null;  // Сбрасываем текущий объект
        }
    }

    private bool IsPointerOverUI(Vector2 position)
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current) { position = position };
        var results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);
        return results.Count > 0;
    }

    private bool TryGetHitObject(Vector2 position, out RaycastHit hit)
    {
        Ray ray = Camera.main.ScreenPointToRay(position);
        return Physics.Raycast(ray, out hit);
    }
}
