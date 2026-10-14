using UnityEngine;

public class Headbob : MonoBehaviour
{

    [Header("Parametres")]
    [SerializeField] private float berjalanFrequency = 14.0f;
    [SerializeField] private float berjalanAmplitude = 0.08f;
    [SerializeField] private float idleFrequency = 1.0f;
    [SerializeField] private float idleAmplitude = 0.02f;

    [Header("References")]
    [SerializeField] private CharacterController controller;
    [SerializeField] private Transform cameraHolder;

    private float timer = 0.0f;
    private Vector3 startPos;

    void Start()
    {
        if (cameraHolder != null)
        {
            startPos = cameraHolder.localPosition;
        }
    }

    void Update()
    {

        CheckForHeadBobTrigger();
        StopHeadBob();
    }

    private void CheckForHeadBobTrigger()
    {
        // Vérifie si le joueur est au sol et en train de se déplacer
        float inputMagnitude = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")).magnitude;

        if (inputMagnitude > 0 && controller.isGrounded)
        {
            // Mouvement de marche
            timer += Time.deltaTime * berjalanFrequency;
            cameraHolder.localPosition = new Vector3(
                startPos.x + Mathf.Cos(timer / 2) * berjalanAmplitude,
                startPos.y + Mathf.Sin(timer) * berjalanAmplitude,
                startPos.z
            );
        }
        else
        {
            // Mouvement subtil au repos (Idle)
            timer += Time.deltaTime * idleFrequency;
            cameraHolder.localPosition = new Vector3(
                startPos.x,
                startPos.y + Mathf.Sin(timer) * idleAmplitude,
                startPos.z
            );
        }
    }

    private void StopHeadBob()
    {
        // Réinitialise progressivement la position si le joueur s'arrête brusquement
        if (Mathf.Abs(Input.GetAxis("Horizontal")) == 0 && Mathf.Abs(Input.GetAxis("Vertical")) == 0)
        {
            timer = 0;
            cameraHolder.localPosition = Vector3.Lerp(cameraHolder.localPosition, startPos, Time.deltaTime * 6f);
        }
    }
}
