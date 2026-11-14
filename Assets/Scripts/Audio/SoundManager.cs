using System.Runtime.CompilerServices;
using UnityEngine;



[RequireComponent(typeof(AudioSource))]
public class SoundManager : MonoBehaviour
{
    public static AudioClip _flareShot, _reload, _walk, _run;
    public AudioClip FlareShot, Reload, Walk, Run;
    private static SoundManager Instance;
    private AudioSource AudioSource;
    private void Awake()
    {
        Instance = this;
        _flareShot = FlareShot;
        _reload = Reload;
        _run = Run;
        _walk = Walk;
    }

    private void Start()
    {
        AudioSource = GetComponent<AudioSource>();
    }

    public static void PlaySound(AudioClip sound, float volume = 1)
    {
        Instance.AudioSource.PlayOneShot(sound, volume);
    }

}
