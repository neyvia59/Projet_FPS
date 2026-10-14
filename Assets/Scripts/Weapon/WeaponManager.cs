using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    [SerializeField] private GameObject _prefabBasicProjectile;
    [SerializeField] private Transform _SpawnPosition;
    [SerializeField] private GameObject _gunGo;

    [SerializeField] private float _projectileForce;

    [SerializeField] private List<WeaponType> _weapons = new List<WeaponType>();

    [SerializeField] private WeaponType _currentWeapon;

    [SerializeField] private float _aimingFOV;
    [SerializeField] private float _aimingSpeed;




    [SerializeField] private Vector3 _basePosition;
    [SerializeField] private Vector3 _baseRotation;

    [SerializeField] private Vector3 _runningPosition;
    [SerializeField] private Vector3 _runningRotation;

    [SerializeField] private Vector3 _aimingPosition;
    [SerializeField] private Vector3 _aimingRotation;

    private bool _isAiming = false;
    private float _baseFOV;

    private int _currentWeaponIndex = 0;
    public bool _canShoot = true;

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

        UpdateWeaponPos();
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
            if (_currentWeaponIndex < 0) { _currentWeaponIndex = _weapons.Count - 1; }
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
        _currentWeapon = _weapons[_currentWeaponIndex];
    }



    private void Shoot()
    {
        if (_canShoot == false) return;

        GameObject go = Instantiate(_currentWeapon._projectilePrefab, _SpawnPosition.position, Quaternion.identity);
        go.GetComponent<Rigidbody>().AddForce(_SpawnPosition.forward.normalized * _currentWeapon._projectionForce, ForceMode.Impulse);
    }

    private void Aim()
    {
        if (Camera.main.fieldOfView > _aimingFOV)
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


    // Faut noter les décalage à faire, on fait position += offset

    private void UpdateWeaponPos()
    {
        if (_canShoot == false)
        {
            _gunGo.transform.localPosition = _runningPosition;
            _gunGo.transform.rotation = Quaternion.Euler(_runningRotation);

            return;
        }

        if (_isAiming)
        {
            _gunGo.transform.localPosition = _aimingPosition;
            _gunGo.transform.rotation = Quaternion.Euler(_aimingRotation);
        }
        else
        {
            _gunGo.transform.localPosition = _basePosition;
            _gunGo.transform.rotation = Quaternion.Euler(_baseRotation);
        }
    }
}
