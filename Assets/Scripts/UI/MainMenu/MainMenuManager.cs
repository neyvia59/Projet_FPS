using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private Image _fadeImage;
    [SerializeField] private float _fadeDuration = 0.7f;
    [SerializeField] private float _creditsFadeDuration = 0.3f;
    [SerializeField] private GameObject _creditsGo, _settingsGo;


    private bool _isInCredits = false;
    private bool _isInSettings = false;
    private void Awake()
    {

        _creditsGo.SetActive(false);
        _fadeImage.gameObject.SetActive(true);
        _fadeImage.DOFade(1, 0);

    }

    private void Start()
    {

        _fadeImage.DOFade(0, _fadeDuration).OnComplete(DisableFade);

    }


    private void DisableFade()
    {
        _fadeImage.gameObject.SetActive(false);
    }

    public void OnClickPlay()
    {
        _settingsGo.SetActive(true);
        _settingsGo.GetComponentInChildren<Image>().gameObject.SetActive(false);
        _settingsGo.GetComponent<SettingsMenu>().SendValueToHolder();
        _fadeImage.gameObject.SetActive(true);
        _fadeImage.DOFade(1, _fadeDuration).OnComplete(OnCompletePlay);

    }

    private void OnCompletePlay()
    {
        SceneManager.LoadScene("Gameplay");
    }


    public void OnClickCredits()
    {
        _isInCredits = true;
        _creditsGo.gameObject.SetActive(true);
        _creditsGo.GetComponentInChildren<Image>().DOFade(1, _creditsFadeDuration);
    }

    public void OnClickBack()
    {
        if (_isInCredits)
        {
            _creditsGo.GetComponentInChildren<Image>().DOFade(0, _creditsFadeDuration).OnComplete(Hide);
        }
        if (_isInSettings)
        {
            _settingsGo.GetComponentInChildren<Image>().DOFade(0, _creditsFadeDuration).OnComplete(Hide);
        }
        
    }
    private void Hide()
    {
        _creditsGo.gameObject.SetActive(false);
        _settingsGo.gameObject.SetActive(false);
    }

    public void OnClickQuit()
    {
        Application.Quit();
    }

    public void OnClickSettings()
    {
        _isInSettings = true;
        _settingsGo.gameObject.SetActive(true);
        _settingsGo.GetComponentInChildren<Image>().DOFade(1, _creditsFadeDuration);
    }
}

