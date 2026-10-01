using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    [SerializeField] private GameObject _prefabBasicProjectile;
    [SerializeField] private Transform _SpawnPosition;

    [SerializeField] private float _projectileForce;

    [SerializeField]private List<WeaponType> _weapons = new List<WeaponType> ();

    [SerializeField] private WeaponType _currentWeapon;

    [SerializeField] private float _aimingFOV;
    [SerializeField] private float _aimingSpeed;
    private bool _isAiming = false;
    private float _baseFOV;

    private int _currentWeaponIndex = 0;
    
	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
    {
        ChangeWeapon();
        _baseFOV = Camera.main.fieldOfView;
	}

    // Update is called once per frame
    void Update()
    {
        CheckInputs();
    }

    private void CheckInputs() 
    {
        if (Input.GetMouseButtonDown(0)) 
        {
            Shoot();
        }
        if (Input.mouseScrollDelta.y > 0) 
        {
            _currentWeaponIndex++;
            if (_currentWeaponIndex >= _weapons.Count) { _currentWeaponIndex = 0; }
            ChangeWeapon();

		}
		if (Input.mouseScrollDelta.y < 0)
		{
			_currentWeaponIndex--;
			if (_currentWeaponIndex < 0) { _currentWeaponIndex = _weapons.Count-1; }
            ChangeWeapon();
		}


        if (Input.GetMouseButton(1))
        {
            _isAiming = true;
        }
        if (Input.GetMouseButtonUp(1))
        {
            _isAiming = false;
        }

        if (_isAiming)
        {
            Aim();
        }
        else if (_isAiming == false && Camera.main.fieldOfView < _baseFOV) { Unaim(); }
    }

    private void ChangeWeapon() 
    { 
        _currentWeapon = _weapons[ _currentWeaponIndex ];
    }



    private void Shoot() 
    {
        GameObject go = Instantiate(_currentWeapon._projectilePrefab, _SpawnPosition.position, Quaternion.identity);
        go.GetComponent<Rigidbody>().AddForce(_SpawnPosition.forward.normalized * _currentWeapon._projectionForce, ForceMode.Impulse);
    }

    private void Aim()
    {
        if(Camera.main.fieldOfView > _aimingFOV)
        {
            Camera.main.fieldOfView -= _aimingSpeed;
        }
    }

    private void Unaim()
    {
        if (Camera.main.fieldOfView < _baseFOV)
        {
            Camera.main.fieldOfView += _aimingSpeed;
        }

    }
}
