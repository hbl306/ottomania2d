using UnityEngine;
using UnityEngine.UI;

public class AudioSettingsUI : MonoBehaviour
{
    [Header("Sliders")]
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider soundSlider;


    private void OnEnable()
    {
        RefreshUI();
    }


    private void Start()
    {
        RefreshUI();
    }


    private void RefreshUI()
    {
        if (AudioManager.Instance == null)
            return;

        musicSlider.SetValueWithoutNotify(
            AudioManager.Instance.GetMusicVolume()
        );

        soundSlider.SetValueWithoutNotify(
            AudioManager.Instance.GetSoundVolume()
        );
    }


    public void ChangeMusicVolume(float value)
    {
        if (AudioManager.Instance == null)
            return;

        AudioManager.Instance.SetMusicVolume(value);
    }


    public void ChangeSoundVolume(float value)
    {
        if (AudioManager.Instance == null)
            return;

        AudioManager.Instance.SetSoundVolume(value);
    }
}