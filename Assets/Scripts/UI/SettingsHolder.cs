using UnityEngine;

public class SettingsHolder : MonoBehaviour
{
    public static SettingsHolder Instance;

    public Vector2 CurrentSensiMouse;
    public Vector2 CurrentSensiController;
    public float CurrentMasterVolume =1;
    private void Awake()
    {

        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); 
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

    }

    private void Start()
    {
        if (CurrentSensiMouse == Vector2.zero) return;

        PlayerCam cam = Camera.main.GetComponent<PlayerCam>();

        cam.sensiMouseX = CurrentSensiMouse.x;
        cam.sensiMouseY = CurrentSensiMouse.y;

        cam.sensiGamePadX = CurrentSensiController.x;
        cam.sensiGamePadY = CurrentSensiController.y;

        CurrentMasterVolume = 1;
        
    }

    public void StoreBeforeLoad(Vector2 newMouseSensi, Vector2 newGamepadSensi, float newMaxVolume)
    {
        CurrentSensiMouse = newMouseSensi;
        CurrentSensiController = newGamepadSensi;
        CurrentMasterVolume = newMaxVolume;
    }

    
}