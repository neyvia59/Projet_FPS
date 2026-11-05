using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;

public class Plant : Mushroom
{

    public bool _canKillPlayer;
    [SerializeField]private float _retractedDistance ;
    private Vector3 _basePos;

    private bool _isFullyRetracted;
    private bool _isFullyUnretracted = true;
    private bool _isUnretracting = false;
    private bool _isRetracting;
    [SerializeField] private float _timeToRetract = 2f;
    [SerializeField] private float _fasterTimeToRetract = 1f;
    [SerializeField] private float _timeToUnretract = 1f;

    [SerializeField] private float _retractedTime = 1f;

    [SerializeField] private float _retractAnimationSpeedDivider = 0.08f;
    [SerializeField] private float _unretractAnimationSpeedDivider = 0.2f;
    private bool _canUnretract = true;
    [SerializeField]private GameObject _retractedRef;
    [SerializeField] private GameObject _plantModel;
    private Vector3 _retractedPos;
    private Vector3 _basePlantPos;
    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponentInParent<Animator>();
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
            if (_isFullyRetracted == false && _isRetracting == false)
            {
                _animator.SetBool("IsRetracting", true);
                _animator.SetBool("IsUnretracting", false);
                if (_canKillPlayer)
                {
					transform.DOKill();
                    _plantModel.transform.DOLocalMove(-_plantModel.transform.forward, _timeToRetract);
					transform.DOMove(_retractedPos, _timeToRetract).OnComplete(RetractionEnd);
				}

				_isRetracting = true;
                _isFullyRetracted = false;
                _isFullyUnretracted=false;
                _isUnretracting = false;
                _animator.speed = _timeToRetract * _retractAnimationSpeedDivider;
            }

        }
        else
        {
			GetComponent<BoxCollider>().enabled = true;
			if (_isFullyUnretracted == false && _isUnretracting == false && _canUnretract)
            {

                _animator.SetBool("IsRetracting", false);
                _animator.SetBool("IsUnretracting", true);
                if (_canKillPlayer)
                {
                    transform.DOKill();
					_plantModel.transform.DOLocalMove(_basePlantPos, _timeToUnretract);
					transform.DOLocalMove(_basePos, _timeToUnretract).OnComplete(UnretractionEnd);
                }
					_isUnretracting =true;
                _isFullyUnretracted = false;
                _isRetracting = false;
                _isFullyRetracted = false;
                _animator.speed = _timeToUnretract * _unretractAnimationSpeedDivider;
            }
           
        }
    }

    private void RetractionEnd()
    {
        
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
        _animator.SetBool("IsRetracting", true);
        _animator.SetBool("IsUnretracting", false);
        _canUnretract = false;
        if (_canKillPlayer)
        {
            transform.DOKill();
            _plantModel.transform.DOLocalMove(-_plantModel.transform.forward, _fasterTimeToRetract);
            transform.DOMove(_retractedPos, _fasterTimeToRetract).OnComplete(RetractionEnd);
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
