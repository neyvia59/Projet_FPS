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

    private int _currentWeaponIndex = 0;
    
	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
    {
        ChangeWeapon();

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
}
