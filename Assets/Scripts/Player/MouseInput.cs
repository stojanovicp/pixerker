using UnityEngine;
using UnityEngine.InputSystem;

public static class MouseInput
{
    public static Vector2 ScreenPosition => Mouse.current.position.ReadValue();

    public static Vector2 WorldPosition => Camera.main.ScreenToWorldPoint(ScreenPosition);
}
