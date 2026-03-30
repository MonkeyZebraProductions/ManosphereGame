using UnityEngine;

[System.Serializable]
public class Sound
{
    public string clipName;

    public AudioClip clip;

    public bool isMusic;
    public bool threeD;
    public bool mute;
    public bool loop;
    public bool ignorePause;

    [HideInInspector]
    public AudioSource source;
}
