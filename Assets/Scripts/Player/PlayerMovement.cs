using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private Transform _cameraTransform;
    [SerializeField] private Vector3 _crouchSize;
    private Vector3 _baseSize;

    [Header("Ground Movement")]
    [SerializeField] private float _walkSpeed = 6f;
    [SerializeField] private float _sprintSpeed = 11f;
    [SerializeField] private float _crouchSpeed = 4f;

    [SerializeField] private float _acceleration = 35f;
    [SerializeField] private float _deceleration = 25f;

    [Header("Jump")]
    [SerializeField] private float _jumpForce = 9f;

    [SerializeField] private float _gravity = 28f;
    [SerializeField] private float _maxFallSpeed = 40f;

    private Vector3 horizontalVelocity;
    private float verticalVelocity;

    private Vector2 moveInput;

    private bool sprintHeld;
    private bool crouchHeld;
    private bool jumpPressed;

    private CharacterController controller;

    private void Awake()
    {
        controller =
            GetComponent<CharacterController>();

        _baseSize = gameObject.transform.localScale;

     

    }

    private void Update()
    {
        ReadInput();

        HandleGroundState();

        HandleMovement();

        HandleJump();

        HandleGravity();

        ApplyMovement();

    }


    private void ReadInput()
    {
        if (Keyboard.current == null)
            return;

        moveInput = Vector2.zero;

        if (Keyboard.current.wKey.isPressed)
            moveInput.y += 1f;

        if (Keyboard.current.sKey.isPressed)
            moveInput.y -= 1f;

        if (Keyboard.current.aKey.isPressed)
            moveInput.x -= 1f;

        if (Keyboard.current.dKey.isPressed)
            moveInput.x += 1f;

        moveInput =
            Vector2.ClampMagnitude(
                moveInput,
                1f
            );

        sprintHeld =
            Keyboard.current.leftShiftKey.isPressed;

        crouchHeld =
            Keyboard.current.leftCtrlKey.isPressed;
        if(crouchHeld) { HandleCrouch(); }

        if (Input.GetKeyUp(KeyCode.LeftControl)) { HandleStand(); }

        jumpPressed =
            Keyboard.current.spaceKey.wasPressedThisFrame;
    }


    private void HandleGroundState()
    {
        if (!controller.isGrounded)
            return;

        if (verticalVelocity < 0f)
        {
            verticalVelocity = -2f;
        }
    }
    private void HandleMovement()
    {

        Vector3 desiredDirection =
            GetMoveDirection();

        if (controller.isGrounded)
        {
            float targetSpeed =sprintHeld? _sprintSpeed: _walkSpeed;

            targetSpeed = crouchHeld ? _crouchSpeed : _walkSpeed;

            Vector3 targetVelocity =
                desiredDirection *
                targetSpeed;

            horizontalVelocity =
                Vector3.MoveTowards(
                    horizontalVelocity,
                    targetVelocity,
                    _acceleration *
                    Time.deltaTime
                );

            if (desiredDirection.sqrMagnitude < 0.01f)
            {
                horizontalVelocity =
                    Vector3.MoveTowards(
                        horizontalVelocity,
                        Vector3.zero,
                        _deceleration *
                        Time.deltaTime
                    );
            }
        }

        else
        {
            HandleAirMovement(
                desiredDirection
            );
        }

        if (desiredDirection.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation =
                Quaternion.LookRotation(
                    desiredDirection
                );

            transform.rotation =
                Quaternion.Slerp(
                    transform.rotation,
                    targetRotation,
                    15f *
                    Time.deltaTime
                );
        }
    }
    private void HandleAirMovement(
        Vector3 desiredDirection
    )
    {
        Vector3 targetVelocity =
            desiredDirection *
            14f;

        horizontalVelocity =
            Vector3.MoveTowards(
                horizontalVelocity,
                targetVelocity,
                12f *
                Time.deltaTime
            );
    }
    private Vector3 GetMoveDirection()
    {
        if (_cameraTransform == null)
            return Vector3.zero;

        Vector3 forward =
            _cameraTransform.forward;

        Vector3 right =
            _cameraTransform.right;

        forward.y = 0f;
        right.y = 0f;

        forward.Normalize();
        right.Normalize();

        Vector3 direction =
            forward *
            moveInput.y
            +
            right *
            moveInput.x;

        return Vector3.ClampMagnitude(
            direction,
            1f
        );
    }
    private void HandleJump()
    {
        if (!jumpPressed)
            return;

        if (controller.isGrounded)
        {
            verticalVelocity =
                _jumpForce;

            return;
        }

    }

    private void HandleGravity()
    {
        if (controller.isGrounded)
            return;

        verticalVelocity -=
            _gravity *
            Time.deltaTime;

        verticalVelocity =
            Mathf.Max(
                verticalVelocity,
                -_maxFallSpeed
            );
    }

    private void ApplyMovement()
    {
        Vector3 velocity =
            horizontalVelocity +
            Vector3.up *
            verticalVelocity;

        CollisionFlags flags =
            controller.Move(
                velocity *
                Time.deltaTime
            );

        if ((flags & CollisionFlags.Above) != 0 &&
            verticalVelocity > 0f)
        {
            verticalVelocity =
                0f;
        }
    }

    private void HandleCrouch()
    {
        gameObject.transform.localScale = _crouchSize;
    }

    private void HandleStand()
    {
        gameObject.transform.localScale =_baseSize;
    }
}