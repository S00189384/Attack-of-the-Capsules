using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsScene : MonoBehaviour
{
    public TextMeshProUGUI TMProSensitivityValue;
    public Slider sensitivitySlider;

    private void Start()
    {
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
        print(Settings.playerCameraSensitivity);
    }
}
