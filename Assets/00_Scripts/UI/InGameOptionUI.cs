using UnityEngine;
using UnityEngine.UI;

public class InGameOptionUI : MonoBehaviour
{
    [SerializeField] private Slider backgroundVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;

    private void Start()
    {
        InitOption();
    }

    private void InitOption()
    {
        backgroundVolumeSlider.onValueChanged.AddListener(ChangeBackgroundVolume);
        sfxVolumeSlider.onValueChanged.AddListener(ChangeSfxVolume);

        backgroundVolumeSlider.value = GameSettingManager.instance.gameSettingData.backgroundSoundVolume;
        sfxVolumeSlider.value = GameSettingManager.instance.gameSettingData.sfxSoundVolume;
    }

    public void ShowOptionUI()
    { 
        gameObject.SetActive(true);
    }

    public void HideOptionUI()
    { 
        gameObject.SetActive(false);
    }

    private void ChangeBackgroundVolume(float value)
    { 
        SoundManager.instance.SetBackgroundVolume(value);
    }

    private void ChangeSfxVolume(float value)
    {
        SoundManager.instance.SetSfxVolume(value);
    }

    private void OnDestroy()
    {
        GameSettingManager.instance.gameSettingData.backgroundSoundVolume = backgroundVolumeSlider.value;
        GameSettingManager.instance.gameSettingData.sfxSoundVolume = sfxVolumeSlider.value;
        GameSettingManager.instance.SaveGameSetting();
        backgroundVolumeSlider.onValueChanged.RemoveListener(ChangeBackgroundVolume);
        sfxVolumeSlider.onValueChanged.RemoveListener(ChangeSfxVolume);
    }
}
