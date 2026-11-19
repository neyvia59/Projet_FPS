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
    [SerializeField] private Transform[] _SpawnPosition = new Transform[2];
    [SerializeField] private GameObject _gunGo;
    [SerializeField] private GameObject _shroomGo;
    [SerializeField] private ThrowPreview _throwPreview;
    [SerializeField] private Animator _armsAnimator;
    [SerializeField] private float _reloadAnimSpeed = 1.75f;

    [SerializeField] private float _projectileForce;

    [SerializeField] private List<WeaponType> _weapons = new List<WeaponType>();

    public int _weaponIndex = 0;

    [SerializeField] private WeaponType _currentWeapon;

    [SerializeField] private float _shootingDelay;
    [SerializeField] private float _aimingFOV;
    [SerializeField] private float _aimingSpeed;
    


    [SerializeField] private float _animSpeed;

    [SerializeField] private Vector3 _basePosition;
    [SerializeField] private Vector3 _baseRotation;

    private Vector3 _baseMuzzlePosition = new Vector3(0.17f, 0.387f, 0.864f);
    private Vector3 _baseMuzzleRotation = Vector3.zero;

    [SerializeField] private Vector3 _muzzleAimPosition;
    [SerializeField] private Vector3 _muzzleAimRotation;

    [SerializeField] private Vector3 _aimingPosition;
    [SerializeField] private Vector3 _aimingRotation;

    private string _state = "base";

    public bool _isAiming = false;
    private float _baseFOV;

    private int _currentWeaponIndex = 0;
    public bool _canShoot = true;
    private bool _inShootCD = false;
    private bool _hasSwitched = false;
    private bool _isInAnim = false;


    private int _currentAmmo;
    private int _maxAmmo = 6;
    [SerializeField] private TextMeshProUGUI _ammoText;
    [SerializeField] private float _dropDistance = 2f;

    private bool _isReloading;
    [SerializeField] private float _reloadingDuration = 3f;
    public bool _wasHoldingTrigger = false;


    public float _currentThrowDistance, _maxThrowDistance, _minThrowDistance, _throwDistanceIncrement;

    private void Awake()
    {
        _armsAnimator = _gunGo.GetComponentInParent<Animator>();
        _currentThrowDistance = _weapons[1].GetComponent<ThrowableMushroomGun>()._projectilePrefab.GetComponent<ThrowableMushroom>().CurrentDistance;
        _maxThrowDistance = _weapons[1].GetComponent<ThrowableMushroomGun>()._projectilePrefab.GetComponent<ThrowableMushroom>().MaxDistance;
        _minThrowDistance = _weapons[1].GetComponent<ThrowableMushroomGun>()._projectilePrefab.GetComponent<ThrowableMushroom>().MinDistance;
        _throwDistanceIncrement = _weapons[1].GetComponent<ThrowableMushroomGun>()._projectilePrefab.GetComponent<ThrowableMushroom>().DistanceIncrement;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _currentWeapon = _weapons[_weaponIndex];
        _currentAmmo = _maxAmmo;
        _baseFOV = Camera.main.fieldOfView;
    }

    // Update is called once per frame
    void Update()
    {
        if (PauseMenu.Instance.IsInPause) return;
        CheckInputs();
        UpdateWeaponPos();
    }

    public void ChangeWallrunningState()
    {
        _armsAnimator.SetBool("IsWallrunning" ,GetComponent<PlayerMovement>()._isWallRunning);
    }

    private void CheckInputs()
    {

        if (Gamepad.current != null)
        {
            if (Input.GetMouseButtonDown(0) || Gamepad.current.rightTrigger.isPressed)
            {
                Shoot();
            }


            if (Input.GetMouseButton(1) && _canShoot )
            {
                _isAiming = true;
            }

            if( Gamepad.current.leftTrigger.isPressed && _canShoot)
            {
                _isAiming = true;
                _wasHoldingTrigger = true;
            }


            if (Input.GetMouseButtonUp(1) || Gamepad.current.leftTrigger.isPressed == false && _wasHoldingTrigger)
            {
                _isAiming = false;
                _wasHoldingTrigger = false;
            }


            if (_currentWeapon == _weapons[1])
            {
                if ((Keyboard.current.qKey.wasPressedThisFrame || Gamepad.current.buttonNorth.isPressed == true) && _currentWeapon == _weapons[1])
                {
                    Drop();
                }
                if (Input.mouseScrollDelta.y > 0 || Gamepad.current.dpad.up.isPressed == true)
                {
                    print("distup");
                    if (_currentThrowDistance < _maxThrowDistance)
                    {
                        _currentThrowDistance += _throwDistanceIncrement;
                    }

                }
                if (Input.mouseScrollDelta.y < 0 || Gamepad.current.dpad.down.isPressed == true)
                {
                    print("distup");
                    if (_currentThrowDistance > _minThrowDistance)
                    {
                        _currentThrowDistance -= _throwDistanceIncrement;
                    }

                }

            }


            if (_isAiming)
            {
                Aim();
            }
            else if (_isAiming == false && Camera.main.fieldOfView < _baseFOV) { Unaim(); }
        }
        else
        {
            if (Input.GetMouseButtonDown(0))
            {
                Shoot();
            }



            if (Input.GetMouseButton(1) && _canShoot)
            {
                _isAiming = true;
            }
            if (Input.GetMouseButtonUp(1))
            {
                _isAiming = false;
            }

            if(_currentWeapon == _weapons[1])
            {
                if (Keyboard.current.qKey.wasPressedThisFrame )
                {
                    Drop();
                }

                if (Input.mouseScrollDelta.y > 0)
                {
                    if(_currentThrowDistance< _maxThrowDistance)
                    {
                        _currentThrowDistance += _throwDistanceIncrement;
                    }

                }
                if (Input.mouseScrollDelta.y < 0)
                {
                    if (_currentThrowDistance > _minThrowDistance)
                    {
                        _currentThrowDistance -= _throwDistanceIncrement;
                    }

                }

            }


            if (_isAiming)
            {
                Aim();
            }
            else if (_isAiming == false && Camera.main.fieldOfView < _baseFOV) { Unaim(); }

        }
    }

    private void Drop()
    {
        _currentWeaponIndex = 0;
        _currentWeapon = _weapons[_weaponIndex];
        _throwPreview.Hide();
        HigherGun();
        GameObject go = Instantiate(_shroomGo,transform.position + transform.forward * _dropDistance, Quaternion.identity);
        go.AddComponent<PickableMushroom>();
        go.AddComponent<Rigidbody>();
        go.GetComponent<BoxCollider>().enabled = true;
    }

    private IEnumerator ShootingCooldown()
    {
        yield return new WaitForSeconds(_shootingDelay);
        _inShootCD = false;
    }



    private void Shoot()
    {
        if (_canShoot == false || _inShootCD == true ||_isInAnim || _currentAmmo<=0 || _currentWeapon == _weapons[1] && _isAiming == false || _isReloading) return;
        GameObject go = Instantiate(_currentWeapon._projectilePrefab, _SpawnPosition[_weaponIndex].position,new Quaternion(Camera.main.transform.rotation.x, Camera.main.transform.rotation.y, Camera.main.transform.rotation.z, Camera.main.transform.rotation.w));
        go.GetComponent<Rigidbody>().AddForce(_SpawnPosition[_weaponIndex].forward.normalized * _currentWeapon._projectionForce, ForceMode.Impulse);
        _armsAnimator.SetTrigger("Shoot");
        if(_currentWeapon == _weapons[0])
        {
           
            _currentAmmo--;
            _ammoText.text = $@"{_currentAmmo} / {_maxAmmo}";
            SoundManager.PlaySound(SoundManager._shoot, SoundManager._shootVolume);
        }
        else
        {
            go.GetComponent<ThrowableMushroom>().CurrentDistance = _currentThrowDistance;
            _weaponIndex = 0;
            _currentWeapon = _weapons[_weaponIndex];
            SoundManager.PlaySound(SoundManager._throw, SoundManager._throwVolume);
            _throwPreview.Hide();
            HigherGun();
        }
            _inShootCD = true;
        StartCoroutine(ShootingCooldown());
    }

    private void Aim()
    {
        if (Camera.main.fieldOfView > _aimingFOV)
        {
            Camera.main.fieldOfView -= _aimingSpeed;
            _armsAnimator.SetBool("IsAiming", true);
        }

        if(_weaponIndex == 1)
        {
            UpdatePreview();
        }
    }

    private void UpdatePreview()
    {
        Ray ray = Camera.main.ViewportPointToRay(
            new Vector3(0.5f, 0.5f, 0f)
        );

        Vector3 targetPoint = ray.GetPoint(50f);

        Vector3 direction = (targetPoint - _SpawnPosition[1].position).normalized;

        Vector3 velocity = direction * _currentWeapon._projectionForce;

        _throwPreview.ShowTrajectory(
            _shroomGo.transform.position,
            velocity,
          _currentThrowDistance,
        _currentWeapon._projectilePrefab.GetComponent<ThrowableMushroom>()._zoneRadius

        );
    }


    private void Unaim()
    {
        if (Camera.main.fieldOfView < _baseFOV)
        {
            Camera.main.fieldOfView += _aimingSpeed;
            _armsAnimator.SetBool("IsAiming", false);
        }
        if (_weaponIndex == 1)
        {
            _throwPreview.Hide();
        }
    }



    private void UpdateWeaponPos()
    {
        if (_currentWeapon == _weapons[1]) return;
        if (_canShoot == false && gameObject.GetComponent<PlayerMovement>()._isSprinting && _state !="run" )
        {
            print("toRUn");
            _state = "run";
            _armsAnimator.SetBool("IsRunning", true);
            _armsAnimator.SetBool("IsAiming", false);
            //_isInAnim = true;
            //_armsAnimator.gameObject.transform.DOLocalMove(_basePosition, _animSpeed).SetEase(Ease.InOutSine).OnComplete(ResetAnimBool);
            //_armsAnimator.gameObject.transform.DOLocalRotate(_baseRotation, _animSpeed);
            //_armsAnimator.gameObject.transform.localRotation = Quaternion.Euler(_baseRotation);

            _SpawnPosition[0].transform.DOLocalMove(_baseMuzzlePosition, _animSpeed);
            _SpawnPosition[0].transform.DOLocalRotate(_baseMuzzleRotation, _animSpeed);
            //_SpawnPosition[0].transform.localPosition = _baseMuzzlePosition;

            //_SpawnPosition[0].transform.localRotation = Quaternion.Euler(_baseMuzzleRotation);
        }

        if (_isAiming && _state != "aim")
        {
            print("toAIm");
            _state = "aim";
            _armsAnimator.SetBool("IsRunning", false);
            
            //_isInAnim = true;
            //_armsAnimator.gameObject.transform.DOLocalMove(_aimingPosition, _animSpeed).SetEase(Ease.InOutSine).OnComplete(ResetAnimBool);
            //_armsAnimator.gameObject.transform.DOLocalRotate(_aimingRotation, _animSpeed);
            //_armsAnimator.gameObject.transform.localPosition = _aimingPosition;
            //_armsAnimator.gameObject.transform.localRotation = Quaternion.Euler(_aimingRotation);

            _SpawnPosition[0].transform.DOLocalMove(_muzzleAimPosition, _animSpeed);
            _SpawnPosition[0].transform.DOLocalRotate(_muzzleAimRotation, _animSpeed);
            //_SpawnPosition[0].transform.localPosition = _muzzleAimPosition;
            //_SpawnPosition[0].transform.localRotation = Quaternion.Euler(_muzzleAimRotation);
        }
        else if( _canShoot && _state != "base" && _isAiming == false)
        {
            print("toBase");    
            _state = "base";
            _armsAnimator.SetBool("IsRunning", false);
            
            //_isInAnim = true;
            //_armsAnimator.gameObject.transform.DOLocalMove(_basePosition, _animSpeed).SetEase(Ease.InOutSine).OnComplete(ResetAnimBool);
            //_armsAnimator.gameObject.transform.DOLocalRotate(_baseRotation, _animSpeed);
            //_armsAnimator.gameObject.transform.localPosition = _basePosition;
            //_armsAnimator.gameObject.transform.localRotation = Quaternion.Euler(_baseRotation);

            _SpawnPosition[0].transform.DOLocalMove(_baseMuzzlePosition, _animSpeed);
            _SpawnPosition[0].transform.DOLocalRotate(_baseMuzzleRotation, _animSpeed);
            //_SpawnPosition[0].transform.localPosition = _baseMuzzlePosition;

            //_SpawnPosition[0].transform.localRotation = Quaternion.Euler(_baseMuzzleRotation);
        }
    }

    private void ResetAnimBool()
    {
        _isInAnim = false;
    }

    public void RefillAmmo()
    {
        if (_currentAmmo == _maxAmmo) return;
        _armsAnimator.speed = _reloadAnimSpeed;
        _armsAnimator.SetTrigger("Reload");
        _currentAmmo = _maxAmmo;

        _isReloading = true;
        StartCoroutine(ReloadingDelay());
        
        SoundManager.PlaySound(SoundManager._reload, SoundManager._reloadVolume);

        _ammoText.text = $@"{_currentAmmo} / {_maxAmmo}";
    }

    public void RefillNoAnim()
    {
        _currentAmmo = _maxAmmo;
        _ammoText.text = $@"{_currentAmmo} / {_maxAmmo}";
    }
    public void PickupMushroom()
    {
        _weaponIndex = 1;
        _armsAnimator.SetTrigger("Switch");
        _currentWeapon = _weapons[_weaponIndex];
        SoundManager.PlaySound(SoundManager._pick, SoundManager._pickVolume);
        LowerGun();
    }

    private void LowerGun()
    {
        //_shroomGo.transform.DOLocalMoveY(_gunGo.transform.localPosition.y, _animSpeed);
        //_gunGo.transform.DOLocalMoveY(-2, _animSpeed);
        _shroomGo.SetActive(true);

    }

    private void HigherGun()
    {

        //_gunGo.transform.DOLocalMoveY(_shroomGo.transform.localPosition.y, _animSpeed);
        //_shroomGo.transform.DOLocalMoveY(-2, _animSpeed);
        _shroomGo.SetActive(false);
    }


    private IEnumerator ReloadingDelay()
    {
        yield return new WaitForSeconds(_reloadingDuration);
        _armsAnimator.speed = 1;
        _isReloading = false;
    }

}
