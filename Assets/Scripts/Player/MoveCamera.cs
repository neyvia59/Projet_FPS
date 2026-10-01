using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    public Transform CamTransform;

    void Update()
    {
        transform.position = CamTransform.position;
    }
}
