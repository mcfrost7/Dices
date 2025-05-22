using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SFXManager : MonoBehaviour
{
    public static SFXManager Instance { get; private set; }
    public AudioSource AudioSource { get => audioSource; set => audioSource = value; }

    public float CurrentVolume {  get; set; }

    [System.Serializable]
    public class SoundGroup
    {
        public ActionType actionType;
        public AudioClip[] clips;
    }
    [System.Serializable]
    public class UIsounds
    {
        public UISoundsEnum soundType;
        public AudioClip[] clips;
    }
    [System.Serializable]
    public class StartBattleSound
    {
        public EnemyType enemyType;
        public AudioClip[] clips;
    }

    [SerializeField] private SoundGroup[] soundGroups;
    [SerializeField] private SoundGroup[] soundGroupsOrks;
    [SerializeField] private UIsounds[] soundGroupUI;
    [SerializeField] private StartBattleSound[] startBattleSounds;
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
            AudioSource.volume = lastVolume;
            CurrentVolume = lastVolume;
            slider.onValueChanged.AddListener(HandleVolumeChanged);
            Invoke(nameof(EnableInitialization), 0.1f);
        }
    }

    private void EnableInitialization()
    {
        initialized = true;
    }


    private void HandleVolumeChanged(float newVolume)
    {
        AudioSource.volume = newVolume;
        lastVolume = newVolume;
        CurrentVolume = lastVolume;
        if (!initialized) return;

        volumeChangedPending = true;
        CancelInvoke(nameof(PlayDebouncedSound));
        Invoke(nameof(PlayDebouncedSound), debounceDelay);
    }

    private void PlayDebouncedSound()
    {
        if (volumeChangedPending)
        {
            PlayUISound(UISoundsEnum.Click);
            volumeChangedPending = false;
        }
    }

    private Dictionary<ActionType, AudioClip[]> soundDictionary;
    private Dictionary<ActionType, AudioClip[]> soundDictionaryOrks;
    private Dictionary<UISoundsEnum, AudioClip[]> soundDictionarytUI;
    private Dictionary<EnemyType, AudioClip[]> soundDictionarytStartBattleSound;

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
        soundDictionarytStartBattleSound = new Dictionary<EnemyType, AudioClip[]>();
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
        foreach (var group in startBattleSounds)
        {
            if (group.clips != null && group.clips.Length > 0)
            {
                soundDictionarytStartBattleSound[group.enemyType] = group.clips;
            }
        }
    }

    public void SetCurrentVolume()
    {
        AudioSource.volume = CurrentVolume;
    }

    public void PlaySoundSM(ActionType actionType) => PlaySound(actionType);
    public void PlaySoundOrk(ActionType actionType) => PlaySound(actionType, true);

    public void PlaySound(ActionType actionType, bool isOrk = false)
    {
        var dictionary = isOrk ? soundDictionaryOrks : soundDictionary;
        if (dictionary.TryGetValue(actionType, out var clips))
            PlayRandomClipWithPitch(clips);
    }

    public void PlayUISound(UISoundsEnum type)
    {
        if (soundDictionarytUI.TryGetValue(type, out var clips))
        {
            PlayRandomClip(clips);
        }
    }

    public void PlayStartBattleSound(EnemyType type)
    {
        if (soundDictionarytStartBattleSound.TryGetValue(type, out var clips))
        {
            PlayRandomClip(clips);
        }
    }

    private void PlayRandomClip(AudioClip[] clips)
    {
        if (clips.Length == 0) return;
        AudioClip clip = clips[UnityEngine.Random.Range(0, clips.Length)];
        AudioSource.PlayOneShot(clip);
    }

    private void PlayRandomClipWithPitch(AudioClip[] clips)
    {
        if (clips.Length == 0) return;

        float originalPitch = AudioSource.pitch;
        AudioSource.pitch = UnityEngine.Random.Range(0.8f, 1.2f);
        PlayRandomClip(clips);
        AudioSource.pitch = originalPitch;
    }
}