using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponManager : MonoBehaviour
{
    [SerializeField] private GameObject _prefabBasicProjectile;
    [SerializeField] private Transform _SpawnPosition;
    [SerializeField] private GameObject _gunGo;

    [SerializeField] private float _projectileForce;

    [SerializeField] private List<WeaponType> _weapons = new List<WeaponType>();

    [SerializeField] private WeaponType _currentWeapon;

    [SerializeField] private float _shootingDelay;
    [SerializeField] private float _aimingFOV;
    [SerializeField] private float _aimingSpeed;


    [SerializeField] private float _animSpeed;

    [SerializeField] private Vector3 _basePosition;
    [SerializeField] private Vector3 _baseRotation;

    [SerializeField] private Vector3 _runningPosition;
    [SerializeField] private Vector3 _runningRotation;

    [SerializeField] private Vector3 _aimingPosition;
    [SerializeField] private Vector3 _aimingRotation;

    private string _state = "base";

    private bool _isAiming = false;
    private float _baseFOV;

    private int _currentWeaponIndex = 0;
    public bool _canShoot = true;
    private bool _inShootCD = false;
    private bool _hasSwitched = false;
    private bool _isInAnim = false;


    private int _currentAmmo;
    private int _maxAmmo = 6;
    [SerializeField] private TextMeshProUGUI _ammoText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _currentAmmo = _maxAmmo;
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

        if (Gamepad.current != null)
        {
            if (Input.GetMouseButtonDown(0) || Gamepad.current.rightTrigger.isPressed)
            {
                Shoot();
            }
            if (Input.mouseScrollDelta.y > 0 || Gamepad.current.rightShoulder.isPressed && _hasSwitched == false)
            {
                _currentWeaponIndex++;
                _hasSwitched = true;
                if (_currentWeaponIndex >= _weapons.Count) { _currentWeaponIndex = 0; }
                ChangeWeapon();

            }
            if (Input.mouseScrollDelta.y < 0 || Gamepad.current.leftShoulder.isPressed && _hasSwitched == false)
            {
                _currentWeaponIndex--;
                _hasSwitched = true;
                if (_currentWeaponIndex < 0) { _currentWeaponIndex = _weapons.Count - 1; }
                ChangeWeapon();
            }

            if (Gamepad.current.rightShoulder.isPressed == false && Gamepad.current.leftShoulder.isPressed == false && _hasSwitched == true)
            {
                _hasSwitched = false;
            }


            if (Input.GetMouseButton(1) && _canShoot || Gamepad.current.leftTrigger.isPressed && _canShoot)
            {
                _isAiming = true;
            }
            if (Input.GetMouseButtonUp(1) || Gamepad.current.leftTrigger.isPressed == false)
            {
                _isAiming = false;
            }

        }
        else
        {
            if (Input.GetMouseButtonDown(0))
            {
                Shoot();
            }
            if (Input.mouseScrollDelta.y > 0)
            {
                _currentWeaponIndex++;
                _hasSwitched = true;
                if (_currentWeaponIndex >= _weapons.Count) { _currentWeaponIndex = 0; }
                ChangeWeapon();

            }
            if (Input.mouseScrollDelta.y < 0)
            {
                _currentWeaponIndex--;
                _hasSwitched = true;
                if (_currentWeaponIndex < 0) { _currentWeaponIndex = _weapons.Count - 1; }
                ChangeWeapon();
            }


            if (Input.GetMouseButton(1) && _canShoot)
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
    }

    private void ChangeWeapon()
    {
        _currentWeapon = _weapons[_currentWeaponIndex];
    }

    private IEnumerator ShootingCooldown()
    {
        yield return new WaitForSeconds(_shootingDelay);
        _inShootCD = false;
    }



    private void Shoot()
    {
        if (_canShoot == false || _inShootCD == true ||_isInAnim || _currentAmmo<=0) return;

        GameObject go = Instantiate(_currentWeapon._projectilePrefab, _SpawnPosition.position, Quaternion.identity);
        go.GetComponent<Rigidbody>().AddForce(_SpawnPosition.forward.normalized * _currentWeapon._projectionForce, ForceMode.Impulse);

        _inShootCD = true;
        _currentAmmo--;
        _ammoText.text = $@"{_currentAmmo} / {_maxAmmo}";
        StartCoroutine(ShootingCooldown());
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



    private void UpdateWeaponPos()
    {
        if (_canShoot == false && gameObject.GetComponent<PlayerMovement>()._isSprinting && _state != "run")
        {
            _state = "run";
            _isInAnim = true;
            _gunGo.transform.DOLocalMove(_runningPosition, _animSpeed).OnComplete(ResetAnimBool);
            _gunGo.transform.DOLocalRotate(_runningRotation, _animSpeed);
            //_gunGo.transform.localRotation = Quaternion.Euler(_runningRotation);

        }

        if (_isAiming && _state != "aim")
        {
            _state = "aim";
            _isInAnim = true;
            _gunGo.transform.DOLocalMove(_aimingPosition, _animSpeed).OnComplete(ResetAnimBool);
            _gunGo.transform.DOLocalRotate(_aimingRotation, _animSpeed);
            //_gunGo.transform.localPosition = _aimingPosition;
            //_gunGo.transform.localRotation = Quaternion.Euler(_aimingRotation);
        }
        else if(_state != "base" && _canShoot)
        {
            _state = "base";
            _isInAnim = true;
            _gunGo.transform.DOLocalMove(_basePosition, _animSpeed).OnComplete(ResetAnimBool);
            _gunGo.transform.DOLocalRotate(_baseRotation, _animSpeed);
            //_gunGo.transform.localPosition = _basePosition;
            //_gunGo.transform.localRotation = Quaternion.Euler(_baseRotation);
        }
    }

    private void ResetAnimBool()
    {
        _isInAnim = false;
    }

    public void RefillAmmo()
    {
        _currentAmmo = _maxAmmo;
        _ammoText.text = $@"{_currentAmmo} / {_maxAmmo}";
    }
}
