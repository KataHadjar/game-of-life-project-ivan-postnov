using UnityEngine;
using UnityEngine.InputSystem;

public class ClickManager : MonoBehaviour
{
    void Update()
    {
        Vector2 clickPos = Vector2.zero;
        bool clicked = false;

        // Мышь
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            clickPos = Mouse.current.position.ReadValue();
            clicked = true;
        }

        // Тач
        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
        {
            clickPos = Touchscreen.current.primaryTouch.position.ReadValue();
            clicked = true;
        }

        if (clicked)
        {
            Vector2 worldPoint = Camera.main.ScreenToWorldPoint(clickPos);
            RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero);

            if (hit.collider != null)
            {
                Cell cell = hit.collider.GetComponent<Cell>();
                if (cell != null)
                {
                    cell.OnClicked();
                }
            }
        }
    }
}
