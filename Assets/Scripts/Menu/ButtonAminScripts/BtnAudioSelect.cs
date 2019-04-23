using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BtnAudioSelect : MonoBehaviour
{
    public new AudioClip audio;
    public AudioSource audioSource;

    void Start()
    {
    }

    void Update()
    {
    }

    void AudioSound()
    {
        audioSource.Stop();
        audioSource.PlayOneShot(audio);

        if (Input.GetAxis("Vertical") == 0)
        {
            audioSource.Stop();
        }
    }
}