using UnityEngine;
using UnityEngine.UI;

public class GlobalSoundManager : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private AudioSource audioSource;

    public AudioSource AudioSource { get => audioSource; set => audioSource = value; }

    private void Start()
    {
        if (slider != null)
        {
            slider.onValueChanged.AddListener(SetVolume);
            SetVolume(slider.value);
        }
    }

    private void SetVolume(float volume)
    {
        audioSource.volume = volume;
    }
}
