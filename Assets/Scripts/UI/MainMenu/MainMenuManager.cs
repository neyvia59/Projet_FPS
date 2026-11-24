using DG.Tweening;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private Image _fadeImage;
    [SerializeField] private float _fadeDuration = 0.7f;
    [SerializeField] private float _creditsFadeDuration = 0.3f;
    [SerializeField] private GameObject _creditsGo, _settingsGo, _bindingsGo;

    public float PlayLength = 8;
    private bool _isInCredits = false;
    private bool _isInSettings = false;
    private bool _isInBindings = false;

    private void Awake()
    {
        _creditsGo.SetActive(false);
        _settingsGo.SetActive(false);
        _bindingsGo.SetActive(false);
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
        Camera.main.GetComponent<Animator>().SetTrigger("Play");
        StartCoroutine(PlayDelay());
        _settingsGo.SetActive(true);
        _settingsGo.GetComponentInChildren<Image>().gameObject.SetActive(false);
        _settingsGo.GetComponent<SettingsMenu>().SendValueToHolder();

        _settingsGo.SetActive(false);
        _creditsGo.SetActive(false);
        _bindingsGo.SetActive(false);
    }

    private IEnumerator PlayDelay()
    {
        yield return new WaitForSeconds(PlayLength);
        SceneManager.LoadScene("Gameplay");
    }

    public void OnClickCredits()
    {
        _isInCredits = true;
        Camera.main.GetComponent<Animator>().SetBool("IsInCredits", true);
        _creditsGo.gameObject.SetActive(true);
    }

    public void OnClickBack()
    {
        if (_isInCredits)
        {
            Camera.main.GetComponent<Animator>().SetBool("IsInCredits", false);

        }

        if (_isInSettings)
        {
            Camera.main.GetComponent<Animator>().SetBool("IsInSettings", false);

        }

        if (_isInBindings)
        {
            Camera.main.GetComponent<Animator>().SetBool("IsInBindings", false);
            Image[] _bindingsImage = _bindingsGo.GetComponentsInChildren<Image>();

        }
    }

    private void Hide()
    {
        _creditsGo.gameObject.SetActive(false);
        _settingsGo.gameObject.SetActive(false);
        _bindingsGo.gameObject.SetActive(false);
    }

    public void OnClickQuit()
    {
        Application.Quit();
    }

    public void OnClickBindings()
    {
        _isInBindings = true;
        Camera.main.GetComponent<Animator>().SetBool("IsInBindings", true);
        _bindingsGo.gameObject.SetActive(true);

    }

    public void OnClickSettings()
    {
        _isInSettings = true;
        Camera.main.GetComponent<Animator>().SetBool("IsInSettings", true);
        _settingsGo.gameObject.SetActive(true);

       
    }
}