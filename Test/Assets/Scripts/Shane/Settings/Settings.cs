using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Settings
{
    public static int playerCameraSensitivity = 65;
    public static bool InfiniteWaves = false;

    public static void SetPlayerCameraSensitivity(int valueToSet)
    {
        playerCameraSensitivity = valueToSet;
    }

    public static void SetInfiniteWavesStatus(bool value)
    {
        InfiniteWaves = value;
    }
}
