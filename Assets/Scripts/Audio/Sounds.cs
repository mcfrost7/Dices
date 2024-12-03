using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sounds : MonoBehaviour
{
    private static Sounds instance;
    public static Sounds Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindAnyObjectByType<Sounds>();
            }
            return instance;
        }
    }

    [SerializeField] private AudioClip[] sounds;

    public AudioClip[] Sounds1 { get => sounds; set => sounds = value; }

    private AudioSource audioSource => GetComponent<AudioSource>();

    public void PlaySound(AudioClip clip, float volume = 1f, bool destroyed = false, float p1 = 0.8f, float p2=1.2f)
    {
        audioSource.pitch = Random.Range(p1, p2);

        if (destroyed)
        {
            AudioSource.PlayClipAtPoint(clip,transform.position,volume);
        }
        else
        {
            audioSource.PlayOneShot(clip,volume);
        }
    }
}
 