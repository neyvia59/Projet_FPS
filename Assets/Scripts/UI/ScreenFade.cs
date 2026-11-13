using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class ScreenFade : MonoBehaviour
{

    [SerializeField] private Image _fadeImage;
    [SerializeField] private float _fadeDuration = 0.7f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _fadeImage.gameObject.SetActive(true);
        _fadeImage.DOFade(0, _fadeDuration).OnComplete(HideFadeImage);
    }

    private void HideFadeImage()
    {
        _fadeImage.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
