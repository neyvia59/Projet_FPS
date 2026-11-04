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

    [SerializeField] private float _animationSpeedDivider = 0.9f;
    private bool _canUnretract = true;
    private Vector3 _retractedPos;
    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponentInParent<Animator>();
        _basePos = transform.localPosition;
        _retractedPos = transform.right * _retractedDistance;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _animator.speed = 0;
    }

    // Update is called once per frame
    void Update()
    {
        base.Update();
        if (_isEnlightened)
        {
            if (_isFullyRetracted == false && _isRetracting == false)
            {
                _animator.SetBool("IsRetracting", true);
                _animator.SetBool("IsUnretracting", false);
                transform.DOKill();
                transform.DOLocalMove(_retractedPos, _timeToRetract).OnComplete(RetractionEnd);
                _isRetracting = true;
                _isFullyRetracted = false;
                _isFullyUnretracted=false;
                _isUnretracting = false;
                _animator.speed = _timeToRetract * _animationSpeedDivider;
                
            }

        }
        else
        {
            if(_isFullyUnretracted == false && _isUnretracting == false && _canUnretract)
            {
                _animator.SetBool("IsRetracting", false);
                _animator.SetBool("IsUnretracting", true);
                transform.DOKill();
                transform.DOLocalMove(_basePos, _timeToUnretract).OnComplete(UnretractionEnd);
                _isUnretracting=true;
                _isFullyUnretracted = false;
                _isRetracting = false;
                _isFullyRetracted = false;
                _animator.speed = _timeToUnretract * _animationSpeedDivider;
            }
           
        }
    }

    private void RetractionEnd()
    {
        _isFullyRetracted = true;
        _isRetracting = false;
        StartCoroutine(TimeBeforeUnretract());
    }

    private void UnretractionEnd()
    {
        _isFullyUnretracted = true;
        _isUnretracting = false;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.GetComponent<Ammo>() != null)
        {
            _animator.SetBool("IsRetracting", true);
            _animator.SetBool("IsUnretracting", false);
            _canUnretract = false;
            transform.DOKill();
            transform.DOLocalMove(_retractedPos, _timeToRetract).OnComplete(FastRetractEnd);
            _isRetracting = true;
            _isFullyRetracted = false;
            _isFullyUnretracted = false;
            _isUnretracting = false;
            _animator.speed = _fasterTimeToRetract * _animationSpeedDivider;

        }
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
