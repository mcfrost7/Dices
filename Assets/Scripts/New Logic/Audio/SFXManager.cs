using System.Collections.Generic;
using UnityEngine;
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