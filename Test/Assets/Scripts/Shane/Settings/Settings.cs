
//Settings has two settings - camera sensitivity and an infinite waves option.
//Settings can be changed in the settings scene loaded from the main menu.
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
