using System;
using UnityEngine;

public class LightingBaitAmmo : MonoBehaviour
{
    [SerializeField] private Light _light;
    [SerializeField] private Material _material;
    private BaitAmmo _baitAmmo;

    private void Awake()
    {
        _baitAmmo = GetComponent<BaitAmmo>();
    }

    private void Update()
    {
        if (_baitAmmo._isPlanted)
        {
            _baitAmmo._isPlanted = false;
            // _light.bounceIntensity = 50;
            // _light.transform.localScale *= 10;
        }
    }
}
