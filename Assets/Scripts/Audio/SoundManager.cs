using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;



[RequireComponent(typeof(AudioSource))]
public class SoundManager : MonoBehaviour
{
    public static AudioClip _slide, _shoot, _jump, _land, _reload, _pick, _throw, _jumpPad, _vineRetract, _vineUnretract, _plantRetract, _plantUnretract;
    public  AudioClip Slide, Shoot, Jump, Land, Reload, Pick, Throw, JumpPad, VineRetract, VineUnretract, PlantRetract, PlantUnretract;

    public static List<AudioClip> _walkingSounds = new List<AudioClip>();
    public List<AudioClip> WalkingSounds = new List<AudioClip>();


    public static List<AudioClip> _wallrunSounds = new List<AudioClip>();
    public List<AudioClip> WallrunSounds = new List<AudioClip>();


    private static SoundManager Instance;
    private AudioSource AudioSource;
    private AudioSource _slideAudioSource;
    private void Awake()
    {
        Instance = this;
        _slide = Slide;
        _shoot = Shoot;
        _land = Land;
        _jump = Jump;
        _land= Land;
        _reload = Reload;
        _pick = Pick;
        _throw = Throw;
        _jumpPad = JumpPad;
        _vineRetract = VineRetract;
        _plantRetract = PlantRetract;
        _vineUnretract = VineUnretract;
        _plantUnretract = PlantUnretract;

        GameObject go = new GameObject();

        _slideAudioSource = go.AddComponent<AudioSource>();
        _slideAudioSource.clip = _slide;


        for (int i = 0; i < WallrunSounds.Count; i++)
        {
            _wallrunSounds.Add(WallrunSounds[i]);
        }

        for (int i = 0; i < WalkingSounds.Count; i++)
        {
            _walkingSounds.Add(WalkingSounds[i]);
        }
    }

    private void Start()
    {
        AudioSource = GetComponent<AudioSource>();
    }

    public static void PlaySound(AudioClip sound, float volume = 1)
    {
        if(sound == _slide)
        {
            Instance._slideAudioSource.Play();
        }
        else
        {
            Instance.AudioSource.PlayOneShot(sound, volume);
        }

        
        
    }


    public static void StopSlideSound()
    {
        Instance._slideAudioSource.Stop();
    }
}
