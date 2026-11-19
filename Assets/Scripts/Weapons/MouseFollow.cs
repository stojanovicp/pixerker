using UnityEngine;

public class MouseFollow : MonoBehaviour
{
    private void Update()
    {
        FaceMouse();
    }
    private void FaceMouse()
    {
        Vector3 mousePostition = Input.mousePosition;
        mousePostition = Camera.main.ScreenToWorldPoint(mousePostition);

        Vector2 direction = transform.position - mousePostition;

        transform.right = -direction;
    }
}
