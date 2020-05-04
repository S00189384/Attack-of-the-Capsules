using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Settings
{
    public static int playerCameraSensitivity = 100;

    public static void SetPlayerCameraSensitivity(int valueToSet)
    {
        playerCameraSensitivity = valueToSet;
    }
}
