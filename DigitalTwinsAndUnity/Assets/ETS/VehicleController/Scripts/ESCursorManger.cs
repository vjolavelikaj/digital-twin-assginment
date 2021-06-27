using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ESCursorManger
{

    public static void Hide_ShowCursor(bool lockstate, bool visibility)
    {
        if (lockstate)
        {
            Cursor.lockState = CursorLockMode.Locked;

        }
        if (!lockstate)
        {
            Cursor.lockState = CursorLockMode.None;

        }
        //
        if (visibility)
        {
            Cursor.visible = true;
        }
        if (!visibility)
        {
            Cursor.visible = false;
        }
    }
}
