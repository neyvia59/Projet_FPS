using UnityEngine;

public class AmbiantAudio : MonoBehaviour
{
    public static AmbiantAudio Instance;
    private float _baseVolume;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        _baseVolume = GetComponent<AudioSource>().volume;
        ChangeVolume();
    }
    public void ChangeVolume()
    {
        if (SettingsHolder.Instance == null) return;
        GetComponent<AudioSource>().volume = _baseVolume * SettingsHolder.Instance.CurrentMasterVolume;
    }
}
