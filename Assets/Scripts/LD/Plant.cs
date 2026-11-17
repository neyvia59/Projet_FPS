using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;

public class Plant : Mushroom
{

    public bool _canKillPlayer;
    [SerializeField]private float _retractedDistance ;
    private Vector3 _basePos;

    public bool _isFullyRetracted;
    private bool _isFullyUnretracted = true;
    public bool _isUnretracting = false;
    public bool _isRetracting;
    public float _currentTime;
    [SerializeField] private float _timeToRetract = 2f;
    [SerializeField] private float _fasterTimeToRetract = 1f;
    [SerializeField] private float _timeToUnretract = 1f;

    [SerializeField] private float _retractedTime = 1f;

    [SerializeField] private float _retractAnimationSpeedDivider = 0.08f;
    [SerializeField] private float _unretractAnimationSpeedDivider = 0.2f;
    public bool _canUnretract = true;
    [SerializeField]private GameObject _retractedRef;
    [SerializeField] private GameObject _plantModel;
    [SerializeField] private GameObject _plantLiane;
    private Vector3 _retractedPos;
    private Vector3 _basePlantPos;
    private Animator _animator;
    private Animator _plantLianeAnimator;

    private void Awake()
    {
        _animator = GetComponentInParent<Animator>();
        _plantLianeAnimator = _plantLiane.GetComponent<Animator>();
        _basePos = transform.localPosition;
        
        
        if (_canKillPlayer)
        {
            _retractedPos = _retractedRef.transform.position;
			_basePlantPos = _plantModel.transform.localPosition;
		}
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //_animator.speed = 0;
    }

    // Update is called once per frame
    void Update()
    {

        base.Update();
        if (_isEnlightened)
        {
            GetComponent<BoxCollider>().enabled = false;
            if (GetComponentInParent<CapsuleCollider>() != null)
            {
                GetComponentInParent<CapsuleCollider>().enabled = false;
            }
            if (_isFullyRetracted == false && _isRetracting == false)
            {
                
                if (_canKillPlayer)
                {
                    SoundManager.PlaySound(SoundManager._plantRetract, SoundManager._plantRetractVolume);

                    transform.DOKill();
                    _plantModel.transform.DOLocalMove(-_plantModel.transform.forward, _timeToRetract);
					transform.DOMove(_retractedPos, _timeToRetract).OnComplete(RetractionEnd);
                    _animator.SetBool("IsRetracting", true);
                    _animator.SetBool("IsUnretracting", false);
                    _plantLianeAnimator.SetBool("IsRetracting", true);
                    _plantLianeAnimator.SetBool("IsUnretracting", false);
                }
                else
                {
                    SoundManager.PlaySound(SoundManager._vineRetract, SoundManager._vineRetractVolume);
                    _animator.SetBool("IsRetracting", false);
                    _animator.SetBool("IsUnretracting", true);
                }

                _isRetracting = true;
                _isFullyRetracted = false;
                _isFullyUnretracted=false;
                _isUnretracting = false;
                _canUnretract = false;
                _animator.speed = _timeToRetract * _retractAnimationSpeedDivider;


            }

        }
        else
        {
			
			if (_isFullyUnretracted == false && _isUnretracting == false && _canUnretract)
            {
                GetComponent<BoxCollider>().enabled = true;
                if (GetComponentInParent<CapsuleCollider>() != null)
                {
                    GetComponentInParent<CapsuleCollider>().enabled = true;
                }


                if (_canKillPlayer)
                {
                    transform.DOKill();
					_plantModel.transform.DOLocalMove(_basePlantPos, _timeToUnretract);
					transform.DOLocalMove(_basePos, _timeToUnretract).OnComplete(UnretractionEnd);
                    SoundManager.PlaySound(SoundManager._plantUnretract, SoundManager._plantUnretractVolume);
                    _animator.SetBool("IsRetracting", false);
                    _animator.SetBool("IsUnretracting", true);
                    _plantLianeAnimator.SetBool("IsRetracting", false);
                    _plantLianeAnimator.SetBool("IsUnretracting", true);
                }
                else
                {
                    SoundManager.PlaySound(SoundManager._vineUnretract, SoundManager._vineUnretractVolume);
                    _animator.SetBool("IsRetracting", true);
                    _animator.SetBool("IsUnretracting", false);
                }
                    _isUnretracting = true;
                _isFullyUnretracted = false;
                _isRetracting = false;
                _isFullyRetracted = false;
                _animator.speed = _timeToUnretract * _unretractAnimationSpeedDivider;
            }
           
        }


        if (_isRetracting)
        {
            if (_currentTime < _timeToRetract && _canKillPlayer == false)
            {
                _currentTime += Time.deltaTime;
            }
            else
            {
                _currentTime = 0;
                RetractionEnd();
            }
        }
    }

    private void RetractionEnd()
    {
        GetComponent<BoxCollider>().enabled = false;
        _isFullyRetracted = true;
        _isRetracting = false;
		_canUnretract = false;
		StartCoroutine(TimeBeforeUnretract());
    }

    private void UnretractionEnd()
    {
        _isFullyUnretracted = true;
        _isUnretracting = false;
    }

    public void CollisionEnter()
    {
        print("collision");


        _canUnretract = false;
        if (_canKillPlayer)
        {
            transform.DOKill();
            _plantModel.transform.DOLocalMove(-_plantModel.transform.forward, _fasterTimeToRetract);
            transform.DOMove(_retractedPos, _fasterTimeToRetract).OnComplete(RetractionEnd);
            SoundManager.PlaySound(SoundManager._plantRetract, SoundManager._plantRetractVolume);
            _animator.SetBool("IsRetracting", true);
            _animator.SetBool("IsUnretracting", false);
            _plantLianeAnimator.SetBool("IsRetracting", true);
            _plantLianeAnimator.SetBool("IsUnretracting", false);
        }
        else
        {
            SoundManager.PlaySound(SoundManager._vineRetract, SoundManager._vineRetractVolume);
            _animator.SetBool("IsRetracting", false);
            _animator.SetBool("IsUnretracting", true);
        }
            _isRetracting = true;
        _isFullyRetracted = false;
        _isFullyUnretracted = false;
        _isUnretracting = false;
        _animator.speed = _fasterTimeToRetract * _retractAnimationSpeedDivider;

    }

    private void FastRetractEnd()
    {
        StartCoroutine(TimeBeforeUnretract());
    }

    private IEnumerator TimeBeforeUnretract()
    {
        yield return new WaitForSeconds(_retractedTime);
        _canUnretract = true;
    }
}
