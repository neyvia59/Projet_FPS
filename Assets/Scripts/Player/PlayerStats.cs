using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PlayerStats : MonoBehaviour
{
    static public PlayerStats Instance;

    [SerializeField] Gauge SporeBar;
    public float _sporeContamination = 0f;
    [SerializeField] private float _careSporeContamination = 0.001f;
    [SerializeField] private float _timeBetweenCoughs = 4f;

    public bool IsInSpore;
    [Header("Visual Spore effect")]

    [SerializeField] private VolumeProfile _profile;
    [SerializeField] private float _incrementBefore20;
    [SerializeField] private float _incrementBefore40;
    [SerializeField] private float _incrementBefore60;
    [SerializeField] private float _incrementBefore80;
    [SerializeField] private float _incrementBeforeMax;

    private float _currentTimer;

   

    [SerializeField] private List<Material> _quantizationMats = new List<Material>();
    [SerializeField] private float _quantiIncrementBefore20;
    [SerializeField] private float _quantiIncrementBefore40;
    [SerializeField] private float _quantiIncrementBefore60;
    [SerializeField] private float _quantiIncrementBefore80;
    [SerializeField] private float _quantiIncrementBeforeMax;




    private void Awake()
    {

        Instance = this;
    }

    private void Start()
    {
        ChromaticAberration chro;

        if (_profile.TryGet(out chro))
        {
            chro.intensity.value = 0;
        }
        foreach (Material mat in _quantizationMats)
        {
            mat.SetFloat("_QuantizationStrength", 1);
        }
    }

    private void Update()
    {
        if (IsInSpore == false)
        {
            _currentTimer = 0;
            return;
        }
        if (IsInSpore)
        {
            if (_currentTimer < _timeBetweenCoughs)
            {
                _currentTimer += Time.deltaTime;
            }
            else
            {
                _currentTimer = 0;
                print("cough");
                CoughSFX();
            }
        }

    }


    private void UpdateChromatic()
    {
        float newQuantization = 1 ;
        ChromaticAberration chro;

        if (_profile.TryGet(out chro))
        {
            if (_sporeContamination < 0.2f)
            {
                chro.intensity.value = _sporeContamination * _incrementBefore20;
                newQuantization = _sporeContamination * _quantiIncrementBefore20;
            }
            else if (_sporeContamination < 0.4f)
            {
                newQuantization = _sporeContamination * _quantiIncrementBefore40;
                chro.intensity.value = _sporeContamination * _incrementBefore40;
            }
            else if (_sporeContamination < 0.6f)
            {
                newQuantization = _sporeContamination * _quantiIncrementBefore60;
                chro.intensity.value = _sporeContamination * _incrementBefore60;

            }
            else if (_sporeContamination < 0.8f)
            {
                newQuantization = _sporeContamination * _quantiIncrementBefore80;
                chro.intensity.value = _sporeContamination * _incrementBefore80;
            }
            else
            {
                newQuantization = _sporeContamination * _quantiIncrementBeforeMax;
                chro.intensity.value = _sporeContamination * _incrementBeforeMax;
            }
        }

        foreach(Material mat in _quantizationMats)
        {
            mat.SetFloat("_QuantizationStrength", newQuantization);
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
        {
            UpdateChromatic();
            SporeBar.UpdateGauge(_sporeContamination);
        }


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
