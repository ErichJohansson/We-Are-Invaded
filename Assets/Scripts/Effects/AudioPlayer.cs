﻿using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class AudioPlayer : MonoBehaviour
{
    public bool useOnUI;
    public AudioClip[] variedSounds;

    private AudioSource source;

    public AudioClip Clip { get { return source?.clip; } }

    private void Awake()
    {
        source = GetComponent<AudioSource>();
        if (useOnUI)
        {
            Button b = GetComponent<Button>();
            b?.onClick.AddListener(Play);
        }
    }

    public void Play()
    {
        if (variedSounds.Length > 0) source.clip = variedSounds[Random.Range(0, variedSounds.Length)];
        source?.Play();
    }

    public void Play(float delay)
    {
        source?.PlayDelayed(delay);
    }

    public void Stop()
    {
        source?.Stop();
    }
}
