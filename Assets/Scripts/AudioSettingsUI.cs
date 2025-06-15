using UnityEngine;
using UnityEngine.UI;

public class AudioSettingsUI : MonoBehaviour
{
    public Slider bgmSlider;
    public Slider gameMusicSlider;
    public GameObject soundSet;

    void Start()
    {
        if (bgmSlider != null)
        {
            bgmSlider.value = PlayerPrefs.GetFloat("BGMVolume", 1f);
            bgmSlider.onValueChanged.AddListener(AudioManager.instance.SetBGMVolume);
        }

        if (gameMusicSlider != null)
        {
            gameMusicSlider.value = PlayerPrefs.GetFloat("GameMusicVolume", 1f);
            gameMusicSlider.onValueChanged.AddListener(AudioManager.instance.SetGameMusicVolume);
        }
    }

    public void BackBtn()
    {
        AudioManager.instance.ClickSound();
        soundSet.SetActive(false);
    }
    public void SettingsBtn()
    {
        AudioManager.instance.ClickSound();
        soundSet.SetActive(true);
    }
}
