using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GlobalSoundManager : MonoBehaviour
{
    public static GlobalSoundManager Instance { get; private set; }

    [System.Serializable]
    public class MusicForTheme
    {
        public MusicTheme MusicTheme;
        public AudioClip[] clips;

    }
    [SerializeField] private Slider slider;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private List<MusicForTheme> themeMusic;

    private MusicTheme currentTheme;

    public AudioSource AudioSource { get => audioSource; set => audioSource = value; }
    public MusicTheme CurentTheme { get => currentTheme; set => currentTheme = value; }

    private void Start()
    {
        if (slider != null)
        {
            slider.onValueChanged.AddListener(SetVolume);
            SetVolume(slider.value);
        }
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        RegisterMusic();
    }

    private void SetVolume(float volume)
    {
        audioSource.volume = volume;
    }

    private Dictionary<MusicTheme, AudioClip[]> musicDictionary;


    private void RegisterMusic()
    {
        musicDictionary = new Dictionary<MusicTheme, AudioClip[]>();
        foreach (var group in themeMusic)
        {
            if (group.clips != null && group.clips.Length > 0)
            {
                musicDictionary[group.MusicTheme] = group.clips;
            }
        }
    }

    public void PlayMusic(MusicTheme theme)
    {
        if (currentTheme == theme && audioSource.isPlaying)
            return; // уже играет нужная тема
        musicDictionary.TryGetValue(theme, out var clips);
        AudioClip clipToPlay = clips[Random.Range(0, clips.Length)];
        audioSource.clip = clipToPlay;
        audioSource.loop = true; 
        audioSource.Play();
        currentTheme = theme;
    }


}


