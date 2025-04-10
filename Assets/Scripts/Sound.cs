using UnityEngine;

[System.Serializable]
public class Sound
{
    [field: SerializeField] public string Name { get; private set; }
    [field: SerializeField] public AudioClip Clip { get; private set; }
    
    [field: Range(0f, 1f)]
    [field: SerializeField] public float Volume { get; private set; }
    
    public AudioSource Source { get; set; }
}