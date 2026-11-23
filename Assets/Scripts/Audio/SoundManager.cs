using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;



[RequireComponent(typeof(AudioSource))]
public class SoundManager : MonoBehaviour
{
    public static AudioClip _slide, _shoot, _jump, _land, _reload, _pick, _throw, _jumpPad, _vineRetract, _vineUnretract, _plantRetract, _plantUnretract ,_cough
        ,_emptyMagazine, _pageSound;
    public  AudioClip Slide, Shoot, Jump, Land, Reload, Pick, Throw, JumpPad, VineRetract, VineUnretract, PlantRetract, PlantUnretract, EmptyMagazine, PageSound;

    public static float _walkingSoundsVolume, _slideVolume, _shootVolume, _jumpVolume, _landVolume, _reloadVolume, _pickVolume,
        _throwVolume, _jumpPadVolume, _vineRetractVolume, _vineUnretractVolume, _plantRetractVolume, _plantUnretractVolume, 
        _wallrunVolume, _coughVolume, _emptyVolume, _pageVolume;

    public float WalkingSoundsVolume = 1, SlideVolume = 1, ShootVolume = 1, JumpVolume = 1, LandVolume = 1, ReloadVolume = 1, PickVolume = 1,
        ThrowVolume = 1, JumpPadVolume = 1, VineRetractVolume = 1, VineUnretractVolume = 1, PlantRetractVolume = 1, PlantUnretractVolume = 1, WallRunVolume = 1, CoughVolume = 1, EmptyVolume = 1, PageVolume = 1;

    public static List<AudioClip> _walkingSounds = new List<AudioClip>();
    public List<AudioClip> WalkingSounds = new List<AudioClip>();

    public static List<AudioClip> _wallrunSounds = new List<AudioClip>();
    public List<AudioClip> WallrunSounds = new List<AudioClip>();

    public static List<AudioClip> _coughingSounds = new List<AudioClip>();
    public List<AudioClip> CoughingSounds = new List<AudioClip>();


    private static SoundManager Instance;
    private AudioSource AudioSource;
    private AudioSource _slideAudioSource;
    private AudioSource _wallrunAudioSource;

    public float MasterVolume;
    public static float _masterVolume;
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
        _emptyMagazine = EmptyMagazine;
        _pageSound = PageSound;


        _walkingSoundsVolume = WalkingSoundsVolume;
        _wallrunVolume = WallRunVolume;
        _slideVolume = SlideVolume;
        _jumpVolume = JumpVolume;
        _throwVolume = ThrowVolume;
        _landVolume = LandVolume;
        _shootVolume = ShootVolume;
        _reloadVolume = ReloadVolume;
        _pickVolume = PickVolume;
        _jumpPadVolume = JumpPadVolume;
        _vineRetractVolume = VineRetractVolume;
        _vineUnretractVolume = VineUnretractVolume;
        _plantRetractVolume = PlantRetractVolume;
        _plantUnretractVolume = PlantUnretractVolume;
        _masterVolume = MasterVolume;
        _coughVolume = CoughVolume;
        _emptyVolume = EmptyVolume;
        _pageVolume = PageVolume;

        GameObject go = new GameObject();

        _slideAudioSource = go.AddComponent<AudioSource>();
        _slideAudioSource.clip = _slide;

        //GameObject go1 = new GameObject();

        //_wallrunAudioSource = go1.AddComponent<AudioSource>();


        for (int i = 0; i < WallrunSounds.Count; i++)
        {
            _wallrunSounds.Add(WallrunSounds[i]);
        }

        for (int i = 0; i < WalkingSounds.Count; i++)
        {
            _walkingSounds.Add(WalkingSounds[i]);
        }

        for (int i = 0; i < CoughingSounds.Count; i++)
        {
            _coughingSounds.Add(CoughingSounds[i]);
        }

    }

    private void Start()
    {
        AudioSource = GetComponent<AudioSource>();
        if (SettingsHolder.Instance == null)
        {
            ChangeMasterVolume(1);
        }
        else
        {
            ChangeMasterVolume(SettingsHolder.Instance.CurrentMasterVolume);
        }
           
    }

    public static void ChangeMasterVolume(float newVolume)
    {

        _masterVolume = newVolume;
    }

    public static void PlaySound(AudioClip sound, float volume = 1)
    {
        ChangeMasterVolume(SettingsHolder.Instance.CurrentMasterVolume);
        if(sound == _slide)
        {
            Instance._slideAudioSource.volume = volume * _masterVolume;
            Instance._slideAudioSource.Play();
        }
        //else if (_wallrunSounds.Contains(sound))
        //{
        //    Instance._wallrunAudioSource.Play();
        //}
        else
        {
            
            Instance.AudioSource.PlayOneShot(sound, volume*_masterVolume);

        }
        
    }


    public static void StopSlideSound()
    {
        Instance._slideAudioSource.Stop();
    }


}
