using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ButtonSound : MonoBehaviour
{
    private Button button;


    private void Awake()
    {
        button = GetComponent<Button>();

        button.onClick.AddListener(
            PlaySound
        );
    }


    private void PlaySound()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance
                .PlayButtonClick();
        }
    }
}