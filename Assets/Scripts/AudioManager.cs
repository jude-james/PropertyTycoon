using System;
using UnityEngine;

public class AudioManager : Singleton<AudioManager>
{
    [field: SerializeField] public Sound[] Sounds { get; set; }

    private void Awake()
    {
        foreach (var sound in Sounds)
        {
            sound.Source = gameObject.AddComponent<AudioSource>();
            sound.Source.clip = sound.Clip;
            sound.Source.volume = sound.Volume;
        }
    }

    public void Play(string name)
    {
        var sound = Array.Find(Sounds, sound => sound.Name == name);
        sound.Source.Play();
    }
}