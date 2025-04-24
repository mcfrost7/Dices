using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SFXManager : MonoBehaviour
{
    public static SFXManager Instance { get; private set; }

    [System.Serializable]
    public class SoundGroup
    {
        public ActionType actionType;
        public AudioClip[] clips;
    }

    [SerializeField] private SoundGroup[] soundGroups;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private Slider slider;

    private float lastVolume;
    private float debounceDelay = 0.2f;
    private bool volumeChangedPending;
    private bool initialized;

    private void Start()
    {
        if (slider != null)
        {
            lastVolume = slider.value;
            audioSource.volume = lastVolume;
            slider.onValueChanged.AddListener(HandleVolumeChanged);

            // Откладываем установку initialized, чтобы игнорировать первое событие
            Invoke(nameof(EnableInitialization), 0.1f);
        }
    }

    private void EnableInitialization()
    {
        initialized = true;
    }


    private void HandleVolumeChanged(float newVolume)
    {
        audioSource.volume = newVolume;
        lastVolume = newVolume;

        if (!initialized) return;

        volumeChangedPending = true;
        CancelInvoke(nameof(PlayDebouncedSound));
        Invoke(nameof(PlayDebouncedSound), debounceDelay);
    }

    private void PlayDebouncedSound()
    {
        if (volumeChangedPending)
        {
            PlaySound(ActionType.Heal);
            volumeChangedPending = false;
        }
    }





    private Dictionary<ActionType, AudioClip[]> soundDictionary;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        InitializeSoundDictionary();
    }

    private void InitializeSoundDictionary()
    {
        soundDictionary = new Dictionary<ActionType, AudioClip[]>();

        foreach (var group in soundGroups)
        {
            if (group.clips != null && group.clips.Length > 0)
            {
                soundDictionary[group.actionType] = group.clips;
            }
        }
    }

    public void PlaySound(ActionType actionType)
    {
        if (soundDictionary.TryGetValue(actionType, out var clips))
        {
            if (clips.Length > 0)
            {
                float originalPitch = audioSource.pitch;
                audioSource.pitch = Random.Range(0.8f, 1.2f);
                AudioClip clip = clips[Random.Range(0, clips.Length)];
                audioSource.PlayOneShot(clip);
                audioSource.pitch = originalPitch;
            }
        }
        else
        {
            Debug.LogWarning($"No sounds found for action type: {actionType}");
        }
    }
}