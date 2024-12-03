using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GlobalSoundManager : MonoBehaviour
{

    [SerializeField] Slider slider;
    private AudioSource audioSource;

    public AudioSource AudioSource { get => audioSource; set => audioSource = value; }

    void Start()
    {
        AudioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (slider.value != audioSource.volume)
        {
            audioSource.volume = slider.value;
        }

    }
}
