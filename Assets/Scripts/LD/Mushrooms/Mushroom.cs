using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Mushroom : MonoBehaviour
{
    public bool _isEnlightened;
    private bool _isEnlightenedForever;
    public bool _canBeLightenedByPlayer = true;
    [SerializeField] private float _detectionZoneRadius = 50f;
    private List<Collider> _colliders = new List<Collider>();

    [SerializeField] private Material _shaderMat;

    [SerializeField] private float _incrementBefore20;
    [SerializeField] private float _incrementBefore40;
    [SerializeField] private float _incrementBefore60;
    [SerializeField] private float _incrementBefore80;
    [SerializeField] private float _incrementBeforeMax;

    private void Start()
    {
        GetComponent<SphereCollider>().radius = _detectionZoneRadius;
    }

    protected void Update()
    {
        for (int i = _colliders.Count - 1; i >= 0; i--)
        {
            if (_colliders[i].IsDestroyed())
            {
                Remove(_colliders[i]);
            }
        }

        UpdateQuantization();

        if (_isEnlightenedForever) { _isEnlightened = true; }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (_isEnlightenedForever) return;
        if ((IsAmmo(other)  && !_colliders.Contains(other)))
        {
            if(other.GetComponent<ThrowableMushroom>()!= null)
            {
                _isEnlightenedForever = true;
            }
            _colliders.Add(other);
            _isEnlightened = true;
            Debug.Log("Mushroom = lighttrue");
        }

        if( other.gameObject.GetComponent<PlayerStats>() != null && !_colliders.Contains(other) && _canBeLightenedByPlayer)
        {
            _colliders.Add(other);
            _isEnlightened = true;
            Debug.Log("Mushroom = lighttrue");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Remove(other);
    }

    private bool IsAmmo(Collider collider)
    {
        return collider.gameObject.TryGetComponent<Ammo>(out Ammo ammo);
    }

    private void Remove(Collider collider)
    {
        if (_isEnlightenedForever) return;
        if (_colliders.Contains(collider))
        {
            _colliders.Remove(collider);
            if (_colliders.Count == 0)
            {
                _isEnlightened = false;
                Debug.Log("Mushroom = lightfalse");
            }
        }
    }
}