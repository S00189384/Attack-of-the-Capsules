using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Didn't have time to rework audio and make a big class to handle all audio.
public class UIAudio : MonoBehaviour
{
    AudioSource audioSource;

    [Header("Audio Clips")]
    public AudioClip buttonPressAudio;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayButtonPressSound()
    {
        audioSource.PlayOneShot(buttonPressAudio);
    }
}
