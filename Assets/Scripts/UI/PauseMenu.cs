using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public static PauseMenu Instance;
    [SerializeField] private SpeedVFX _speedvfx;

    public Image FadeImage;

    public float PauseFadeDuration = 0.4f;

    public Image PauseMenuImage;
    public TextMeshProUGUI PauseText;
    public KeyCode PauseKey = KeyCode.Tab;

    public bool IsInPause = false;

    [SerializeField] private PlayerCollisions _stats;

    private void Awake()
    {
        Instance = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        PauseMenuImage.gameObject.SetActive(false);
        _stats.VictoryImage = FadeImage;
    }

    // Update is called once per frame
    void Update()
    {
        
        if (Input.GetKeyDown(PauseKey))
        {
            IsInPause = !IsInPause;

            if (IsInPause)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                PauseMenuImage.gameObject.SetActive(true);
                PauseText.text = $@"The game is pause.
Press {PauseKey} again to resume.";
                PauseMenuImage.DOFade(1,PauseFadeDuration);
                Time.timeScale = 0;

            }
            else
            {
                Time.timeScale = 1;
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                GetComponentInChildren<SettingsMenu>().SendValueToHolder();
                SoundManager.ChangeMasterVolume(SettingsHolder.Instance.CurrentMasterVolume);
                AmbiantAudio.Instance.ChangeVolume();
                Camera.main.gameObject.GetComponent<PlayerCam>().UpdateSensiValues();
                PauseMenuImage.DOFade(0, 0).OnComplete(Hide);
            }
        }

        if (Gamepad.current == null) return;

        if (Gamepad.current.startButton.wasPressedThisFrame)
        {
            IsInPause = !IsInPause;

            if (IsInPause)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                PauseMenuImage.gameObject.SetActive(true);
                PauseText.text = $@"The game is pause.
Press {PauseKey} again to resume.";
                PauseMenuImage.DOFade(1, PauseFadeDuration);
                Time.timeScale = 0;

            }
            else
            {
                Time.timeScale = 1;
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                GetComponentInChildren<SettingsMenu>().SendValueToHolder();
                AmbiantAudio.Instance.ChangeVolume();
                Camera.main.gameObject.GetComponent<PlayerCam>().UpdateSensiValues();
                SoundManager.ChangeMasterVolume(SettingsHolder.Instance.CurrentMasterVolume);
                PauseMenuImage.DOFade(0, 0).OnComplete(Hide);
            }
        }


    }

    public void OnClickRestart()
    {
        _stats.KillPlayer();
    }

    public void OnClickBack()
    {
        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        GetComponentInChildren<SettingsMenu>().SendValueToHolder();
        AmbiantAudio.Instance.ChangeVolume();
        Camera.main.gameObject.GetComponent<PlayerCam>().UpdateSensiValues();
        SoundManager.ChangeMasterVolume(SettingsHolder.Instance.CurrentMasterVolume);
        PauseMenuImage.DOFade(0, 0).OnComplete(Hide);
    }

    public void OnClickMenu()
    {
        GetComponentInChildren<SettingsMenu>().SendValueToHolder();
        FadeImage.gameObject.SetActive(true);
        FadeImage.DOFade(1, PauseFadeDuration).OnComplete(OnCompleteMenu);
    }

    private void OnCompleteMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    private void Hide()
    {
        
        PauseMenuImage.gameObject.SetActive(false);
    }
}
