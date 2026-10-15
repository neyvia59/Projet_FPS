using DG.Tweening;
using UnityEngine;

public class SpeedVFX : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [Header("SpeedWindVFX")]

    [SerializeField] private Material _speedVFXContrast;
    [SerializeField] private Material _speedVFXFadeContrast;
    [Range(0,1f)]
    [SerializeField] private float _vfxUpTransitionSpeed;
    [Range(0, 1f)]
    [SerializeField] private float _vfxDownTransitionSpeed;

    [SerializeField] private float _vfxIntensityDivider;
    [SerializeField] private float _vfxFadeContrastMult;
    [SerializeField] private float _speedVFXFadeContrastDefault;

    [Header("FOV Slide")]

    [SerializeField] private float _slidingFOV;
    [SerializeField] private float _FOVDuration;
    private float _baseFOV;


    void Start()
    {
        _baseFOV = Camera.main.fieldOfView;
    }

    // Update is called once per frame
    void Update()
    {
      
    }

    public void MoveToSlidingFOV()
    {
        Camera.main.DOFieldOfView(_slidingFOV, _FOVDuration);
    }

    public void ResetFOV()
    {
        Camera.main.DOFieldOfView(_baseFOV, _FOVDuration);
    }


    public void UpdateWindVFX(float _currentSpeed)
    {
        if(_currentSpeed<=0) return;
        //_speedVFXContrast.SetFloat("_TextureContrast", _currentSpeed / _vfxIntensityDivider);
        _speedVFXContrast.SetFloat("_TextureContrast", Mathf.Lerp(_speedVFXContrast.GetFloat("_TextureContrast"), _currentSpeed / _vfxIntensityDivider, _vfxUpTransitionSpeed));
       
        //_speedVFXFadeContrast.SetFloat("_FadeConstrast", _vfxFadeContrastMult / _currentSpeed);
        _speedVFXFadeContrast.SetFloat("_FadeConstrast", Mathf.Lerp(_speedVFXContrast.GetFloat("_FadeConstrast"), _vfxFadeContrastMult / _currentSpeed, _vfxUpTransitionSpeed));


    }

    public void ResetWindVFX()
    {

        _speedVFXFadeContrast.SetFloat("_FadeConstrast", Mathf.Lerp(_speedVFXContrast.GetFloat("_FadeConstrast"), _speedVFXFadeContrastDefault, _vfxDownTransitionSpeed));
    }
}
