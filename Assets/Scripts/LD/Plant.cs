using DG.Tweening;
using System;
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
    [SerializeField] private float _timeToUnretract = 1f;
    private Vector3 _retractedPos;

    private void Awake()
    {
        _basePos = transform.localPosition;
        _retractedPos = transform.right * _retractedDistance;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        base.Update();
        if (_isEnlightened)
        {
            if (_isFullyRetracted == false && _isRetracting == false)
            {

                transform.DOKill();
                transform.DOLocalMove(_retractedPos, _timeToRetract).OnComplete(RetractionEnd);
                _isRetracting = true;
                _isFullyRetracted = false;
                _isFullyUnretracted=false;
                _isUnretracting = false;
            }

        }
        else
        {
            if(_isFullyUnretracted == false && _isUnretracting == false)
            {
                transform.DOKill();
                transform.DOLocalMove(_basePos, _timeToUnretract).OnComplete(UnretractionEnd);
                _isUnretracting=true;
                _isFullyUnretracted = false;
                _isRetracting = false;
                _isFullyRetracted = false;
            }
           
        }
    }

    private void RetractionEnd()
    {
        _isFullyRetracted = true;
        _isRetracting = false;
    }

    private void UnretractionEnd()
    {
        _isFullyUnretracted = true;
        _isUnretracting = false;
    }
}
