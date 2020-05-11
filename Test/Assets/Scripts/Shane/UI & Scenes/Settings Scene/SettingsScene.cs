using TMPro;
using UnityEngine;
using UnityEngine.UI;

//Scene where player can change settings - settings static class is updated when player presses save button.
public class SettingsScene : MonoBehaviour
{
    [Header("Sensitivity")]
    public TextMeshProUGUI TMProSensitivityValue;
    public Slider sensitivitySlider;

    [Header("Infinite Waves")]
    public Toggle infiniteWavesToggle;
    public bool InfiniteWavesStatus = false;
    
    private void Start()
    {
        if (Settings.InfiniteWaves)
            infiniteWavesToggle.isOn = true;
        else
            infiniteWavesToggle.isOn = false;

        sensitivitySlider.value = Settings.playerCameraSensitivity;
        UpdateSensitivityValue();
    }

    public void UpdateSensitivityValue()
    {
        TMProSensitivityValue.text = sensitivitySlider.value.ToString();
    }
    public void SaveSettings()
    {
        Settings.SetPlayerCameraSensitivity((int)sensitivitySlider.value);

        if (infiniteWavesToggle.isOn)
            Settings.SetInfiniteWavesStatus(true);
        else
            Settings.SetInfiniteWavesStatus(false);
    }
}
