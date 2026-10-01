using UnityEngine;

public class PlayerCam : MonoBehaviour
{
    public float sensiX;
    public float sensiY;

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
        float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * sensiX;
        float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * sensiY;

        yrot += mouseX;

        xrot -= mouseY;
        xrot = Mathf.Clamp(xrot, -90f, 90f);
        
        transform.rotation = Quaternion.Euler(xrot, yrot, 0);
        Orientation.rotation = Quaternion.Euler(0, yrot, 0);
    }
}
