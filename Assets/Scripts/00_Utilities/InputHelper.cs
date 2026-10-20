using UnityEngine;
using UnityEngine.InputSystem;

public static class InputHelper
{
    #region PRESSED
    public static bool WasPressed(this Key key)
        => Keyboard.current[key].wasPressedThisFrame;

    public static bool WasReleased(this Key key)
        => Keyboard.current[key].wasReleasedThisFrame;

    public static bool IsPressed(this Key key)
        => Keyboard.current[key].isPressed;

    public static bool WasPressedAnyKey()
        => Keyboard.current.anyKey.wasPressedThisFrame;
    #endregion

    #region AXIS RAW
    public static float GetHorizontalAxisRaw()
    {
        float axis = 0f;

        bool isRightKeyPressed = Key.RightArrow.IsPressed();
        bool isLeftKeyPressed = Key.LeftArrow.IsPressed();

        bool isAnyKeyPressed = isRightKeyPressed || isLeftKeyPressed;

        if (isAnyKeyPressed == false) return 0f;
        if (isRightKeyPressed) axis += 1f;
        if (isLeftKeyPressed) axis -= 1f;

        return axis;
    }

    public static float GetVerticalAxisRaw()
    {
        float axis = 0f;

        bool isUpKeyPressed = Key.UpArrow.IsPressed();
        bool isDownKeyPressed = Key.DownArrow.IsPressed();

        bool isAnyKeyPressed = isUpKeyPressed || isDownKeyPressed;

        if (isAnyKeyPressed == false) return 0f;
        if (isUpKeyPressed) axis += 1f;
        if (isDownKeyPressed) axis -= 1f;

        return axis;
    }

    public static Vector2 GetMoveAxisRaw()
    {
        Vector2 axis = Vector2.zero;
        axis.x = GetHorizontalAxisRaw();
        axis.y = GetVerticalAxisRaw();

        return axis;
    }

    public static Vector2 GetMoveAxisNormalized()
    {
        Vector2 axis = GetMoveAxisRaw();

        if (axis.sqrMagnitude > 1f)
            axis = axis.normalized;

        return axis;
    }
    #endregion
}
