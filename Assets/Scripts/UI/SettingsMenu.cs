using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{

    [Header("Sensi")]
    public float maxMouseSensi;
    public float maxGamepadSensi;
    [SerializeField] private Slider _mouseSensiXSlider;
    [SerializeField] private Slider _mouseSensiYSlider;
    [SerializeField] private Slider _gamepadSensiXSlider;
    [SerializeField] private Slider _gamepadSensiYSlider;

    [SerializeField] private Slider _masterVolumeSlider;
    public float MaxVolume =2;


    void Start()
    {
        _mouseSensiXSlider.maxValue = maxMouseSensi;
        _mouseSensiYSlider.maxValue = maxMouseSensi;
        _mouseSensiXSlider.value = Camera.main.GetComponent<PlayerCam>().sensiMouseX;
        _mouseSensiYSlider.value = Camera.main.GetComponent<PlayerCam>().sensiMouseY;

        _gamepadSensiXSlider.maxValue = maxGamepadSensi;
        _gamepadSensiYSlider .maxValue = maxGamepadSensi;
        _gamepadSensiXSlider.value = Camera.main.GetComponent<PlayerCam>().sensiGamePadX;
        _gamepadSensiYSlider.value = Camera.main.GetComponent<PlayerCam>().sensiGamePadY;

        _masterVolumeSlider.maxValue = MaxVolume;
        _masterVolumeSlider.value = 1;
        if(SceneManager.GetActiveScene().name == "MainMenu")
        {
            gameObject.SetActive(true);
        }
    }


    public void SendValueToHolder()
    {
        if(SettingsHolder.Instance ==  null) return;
        SettingsHolder.Instance.StoreBeforeLoad(new Vector2(_mouseSensiXSlider.value,_mouseSensiYSlider.value), 
            new Vector2(_gamepadSensiXSlider.value, _gamepadSensiYSlider.value),
            _masterVolumeSlider.value);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
