using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerCam : MonoBehaviour
{
    public float sensiMouseX =5;
    public float sensiMouseY =5;

    public float sensiGamePadX =400;
    public float sensiGamePadY =400;

    public Transform Orientation;

    public string WallSide;
    public float WallrunLeaningDegree;
    public float LeaningSpeed;

    float xrot;
    float yrot;
    float zrot;

    public SettingsHolder settings;

    private void Start()
    {
        if(SceneManager.GetActiveScene().name != "MainMenu")
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            
            UpdateSensiValues();
            Instantiate(settings);
           
        }
        
    }

    public void UpdateSensiValues()
    {
        if (SettingsHolder.Instance == null) return;
        SettingsHolder settings = SettingsHolder.Instance;
        if (settings.CurrentSensiMouse.x == 0) return;
        sensiMouseX = settings.CurrentSensiMouse.x;
        sensiMouseY = settings.CurrentSensiMouse.y;

        sensiGamePadX = settings.CurrentSensiController.x;
        sensiGamePadY = settings.CurrentSensiController.y;
    }

    private void Update()
    {
        if (SceneManager.GetActiveScene().name == "MainMenu") return;
        if (PauseMenu.Instance.IsInPause || Notebook.Instance.IsBookOpen) return;
        var keyboard = Keyboard.current;
        var gamepad = Gamepad.current;

        if (keyboard != null)
        {
            float mouseX = Input.GetAxisRaw("Mouse X") * sensiMouseX;
            float mouseY = Input.GetAxisRaw("Mouse Y") * sensiMouseY;

            yrot += mouseX;
            xrot -= mouseY;
        }

        if (gamepad != null)
        {
            Vector2 stickInput = gamepad.rightStick.ReadValue();

            float stickX = stickInput.x * sensiGamePadX * Time.deltaTime;
            float stickY = stickInput.y * sensiGamePadY * Time.deltaTime;

            yrot += stickX;
            xrot -= stickY;
        }

        xrot = Mathf.Clamp(xrot, -90f, 90f);

        if (Orientation.GetComponentInParent<PlayerMovement>()._isWallRunning)
        {
            print("leaning");
            if(WallSide == "left")
            {
                if (zrot > -WallrunLeaningDegree)
                {
                    zrot -= LeaningSpeed;
                }
                else
                {
                    zrot = -WallrunLeaningDegree;
                }
            }
            else
            {
                if (zrot < WallrunLeaningDegree)
                {
                    zrot += LeaningSpeed;
                }
                else
                {
                    zrot = WallrunLeaningDegree;
                }
            }
        }
        else
        {
            print("Unleaning");
            if (zrot > 0)
            {
                zrot -= LeaningSpeed;
            }
            else if (zrot < 0)
            {
                zrot += LeaningSpeed;

            }
            else
            {
                zrot = 0;
            }
        }

        transform.rotation = Quaternion.Euler(xrot, yrot, zrot);
        Orientation.rotation = Quaternion.Euler(0, yrot, zrot);

    }
}
