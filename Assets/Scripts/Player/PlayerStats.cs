using System;
using System.Collections;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    static public PlayerStats Instance;

    [SerializeField] Gauge SporeBar;
    public float _sporeContamination = 0f;
    [SerializeField] private float _careSporeContamination = 0.001f;
    [SerializeField] private float _timeBetweenCoughs = 4f;
    public bool IsInSpore;
    private float _currentTimer;

    private void Awake()
    {

        Instance = this;
    }

    private void Update()
    {
        if(IsInSpore == false)
        {
            _currentTimer = 0;
            return;
        }
        if (IsInSpore )
        {
            if (_currentTimer < _timeBetweenCoughs)
            {
                _currentTimer+= Time.deltaTime;
            }
            else
            {
                _currentTimer = 0;
                print("cough");
                CoughSFX();
            }
        }
    }

    public void ResetSporeTimer()
    {
        
    }

    public void AddSporeContamination(float addSpore)
    {

        _sporeContamination += addSpore;
        if (_sporeContamination > 1)
        {
            PlayerStats.Instance.gameObject.GetComponent<PlayerCollisions>().KillPlayer();
        }
        else if (_sporeContamination >= 0)
            SporeBar.UpdateGauge(_sporeContamination);

    }

    public void CareSporeContamination()
    {
        if (_sporeContamination <= 0) return;
        AddSporeContamination(-_careSporeContamination);
    }

    public void CoughSFX()
    {

        System.Random rand = new System.Random();
        int index = rand.Next(0, SoundManager._coughingSounds.Count);
        SoundManager.PlaySound(SoundManager._coughingSounds[index], SoundManager._coughVolume);
    }
}
