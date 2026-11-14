using UnityEngine;
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

    [Header("VFX")]
    
    [SerializeField] private Slider _vfxOpacitySlider;


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

        _vfxOpacitySlider.maxValue = 255;
        _vfxOpacitySlider.value = 122;
    }


    public void SendValueToHolder()
    {
        SettingsHolder.Instance.StoreBeforeLoad(new Vector2(_mouseSensiXSlider.value,_mouseSensiYSlider.value), 
            new Vector2(_gamepadSensiXSlider.value, _gamepadSensiYSlider.value),
            _vfxOpacitySlider.value);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
