using UnityEngine;
using UnityEngine.InputSystem;

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
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {

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
