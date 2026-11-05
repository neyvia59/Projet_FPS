using Unity.VisualScripting;
using UnityEngine;

public class Headbob : MonoBehaviour
{

    [Header("Parametres")]
    [SerializeField] private float _sprintFrequency = 18.0f;
    [SerializeField] private float _sprintAmplitude = 0.12f;
    [SerializeField] private float _movingFrequency = 14.0f;
    [SerializeField] private float _movingAmplitude = 0.08f;
    [SerializeField] private float idleFrequency = 1.0f;
    [SerializeField] private float idleAmplitude = 0.02f;

    //[Header("References")]
    private CharacterController _controller;
    private Transform _camTransform;

    private float timer = 0.0f;
    private Vector3 startPos;

    void Start()
    {
        _camTransform = GetComponent<PlayerMovement>()._cameraTransform;
        _controller = GetComponent<CharacterController>();
        if (_camTransform != null)
        {
            startPos = _camTransform.localPosition;
        }
    }

    void Update()
    {

        CheckForHeadBobTrigger();
        StopHeadBob();
    }

    private void CheckForHeadBobTrigger()
    {

        float inputMagnitude = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")).magnitude;

        if (inputMagnitude > 0 && _controller.isGrounded && _controller.gameObject.GetComponent<PlayerMovement>()._isSliding == false)
        {
            if(_controller.gameObject.GetComponent<PlayerMovement>()._isSprinting == false)
            {
                timer += Time.deltaTime * _movingFrequency;
                _camTransform.localPosition = new Vector3(
                    startPos.x + Mathf.Cos(timer / 2) * _movingAmplitude,
                    startPos.y + Mathf.Sin(timer) * _movingAmplitude,
                    startPos.z
                );
            }
            else
            {
                timer += Time.deltaTime * _sprintFrequency;
                _camTransform.localPosition = new Vector3(
                    startPos.x + Mathf.Cos(timer / 2) * _sprintAmplitude,
                    startPos.y + Mathf.Sin(timer) * _sprintAmplitude,
                    startPos.z
                );
            }
        }
        else if(_controller.gameObject.GetComponent<PlayerMovement>()._isSliding == false)
        {
            timer += Time.deltaTime * idleFrequency;
            _camTransform.localPosition = new Vector3(
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
            _camTransform.localPosition = Vector3.Lerp(_camTransform.localPosition, startPos, Time.deltaTime * 6f);
        }
    }
}
