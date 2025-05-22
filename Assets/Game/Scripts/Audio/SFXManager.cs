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
    public class UIsounds
    {
        public UISoundsEnum soundType;
        public AudioClip[] clips;
    }
    [SerializeField] private SoundGroup[] soundGroups;
    [SerializeField] private SoundGroup[] soundGroupsOrks;

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private Slider slider;
    [SerializeField] private List<UIsounds> soundGroupUI;

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
    private Dictionary<ActionType, AudioClip[]> soundDictionaryOrks;
    private Dictionary<UISoundsEnum, AudioClip[]> soundDictionarytUI;

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
        soundDictionaryOrks = new Dictionary<ActionType, AudioClip[]>();
        soundDictionarytUI = new Dictionary<UISoundsEnum, AudioClip[]>();
        foreach (var group in soundGroups)
        {
            if (group.clips != null && group.clips.Length > 0)
            {
                soundDictionary[group.actionType] = group.clips;
            }
        }
        foreach (var group in soundGroupsOrks)
        {
            if (group.clips != null && group.clips.Length > 0)
            {
                soundDictionaryOrks[group.actionType] = group.clips;
            }
        }
        foreach(var group in soundGroupUI)
        {
            if (group.clips != null && group.clips.Length > 0)
            {
                soundDictionarytUI[group.soundType] = group.clips;
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
    }

    public void PlaySoundOrk(ActionType actionType)
    {
        if (soundDictionaryOrks.TryGetValue(actionType, out var clips))
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
    }

    public void PlayUISound(UISoundsEnum type)
    {
        if (!soundDictionarytUI.TryGetValue(type,out var clips))
        {
            if (clips.Length > 0)
            {
                AudioClip clip = clips[Random.Range(0, clips.Length)];
                audioSource.PlayOneShot(clip);
            }
        }
    }
}