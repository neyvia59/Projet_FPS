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

    float xrot;
    float yrot;

    private void Start()
    {
        if(SceneManager.GetActiveScene().name != "MainMenu")
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            SettingsHolder settings = SettingsHolder.Instance;

            if (settings.CurrentSensiMouse.x == 0) return;

            sensiMouseX = settings.CurrentSensiMouse.x;
            sensiMouseY = settings.CurrentSensiMouse.y;

            sensiGamePadX = settings.CurrentSensiController.x;
            sensiGamePadY = settings.CurrentSensiController.y;
        }
        
    }

    private void Update()
    {
        if (SceneManager.GetActiveScene().name == "MainMenu") return;
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

        transform.rotation = Quaternion.Euler(xrot, yrot, 0);
        Orientation.rotation = Quaternion.Euler(0, yrot, 0);

    }
}
