using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform cameraTransform;

    [SerializeField] private Transform visualRoot;

    [Header("Ground Movement")]
    [SerializeField] private float walkSpeed = 6f;
    [SerializeField] private float sprintSpeed = 11f;

    [SerializeField] private float acceleration = 35f;
    [SerializeField] private float deceleration = 25f;

    [Header("Jump")]
    [SerializeField] private float jumpForce = 9f;

    [SerializeField] private float gravity = 28f;
    [SerializeField] private float maxFallSpeed = 40f;


    [Header("Ground Slide")]
    [SerializeField] private float slideSpeed = 14f;
    [SerializeField] private float slideSlowSpeed = 1.5f;

    [SerializeField] private float slideAcceleration = 25f;

    [SerializeField] private float slideDuration = 2f;
    [SerializeField] private float slideSlowdownStart = 1.8f;

    [SerializeField] private float slideStartSpeed = 3f;

    [Header("Ground Slide Steering")]
    [SerializeField] private float slideTurnSpeed = 8f;

    [Tooltip("Vitesse conservée quand Ctrl est relâché.")]
    [Range(0f, 1f)]
    [SerializeField] private float slideReleaseVelocityMultiplier = 0f;

    private bool isSliding;
    private bool didSlide;
    private float slideTimer;

    private Vector3 slideDirection;

    [Header("Slide Visual Feedback")]
    [SerializeField] private float slideVisualHeight = 0.55f;
    [SerializeField] private float slideVisualSmooth = 14f;

    private Vector3 visualNormalScale;

    private Vector3 horizontalVelocity;
    private float verticalVelocity;

    private Vector2 moveInput;

    private bool sprintHeld;
    private bool crouchHeld;
    private bool jumpPressed;
    private bool aimHeld;

    private CharacterController controller;

    private void Awake()
    {
        controller =
            GetComponent<CharacterController>();

        didSlide =
            false;

        if (visualRoot != null)
        {
            visualNormalScale =
                visualRoot.localScale;
        }
    }

    private void Update()
    {
        ReadInput();

        HandleGroundState();

        HandleSlideInputReset();

        HandleMovement();

        HandleJump();

        HandleSlide();

        HandleGravity();

        ApplyMovement();

        UpdateVisualFeedback();
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

        jumpPressed =
            Keyboard.current.spaceKey.wasPressedThisFrame;

        aimHeld =
            Mouse.current != null &&
            Mouse.current.rightButton.isPressed;
    }


    private void HandleSlideInputReset()
    {

        if (!crouchHeld)
        {
            didSlide = false;
        }
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
        if (isSliding)
            return;

        Vector3 desiredDirection =
            GetMoveDirection();

        if (controller.isGrounded)
        {
            float targetSpeed =
                sprintHeld
                    ? sprintSpeed
                    : walkSpeed;

            Vector3 targetVelocity =
                desiredDirection *
                targetSpeed;

            horizontalVelocity =
                Vector3.MoveTowards(
                    horizontalVelocity,
                    targetVelocity,
                    acceleration *
                    Time.deltaTime
                );

            if (desiredDirection.sqrMagnitude < 0.01f)
            {
                horizontalVelocity =
                    Vector3.MoveTowards(
                        horizontalVelocity,
                        Vector3.zero,
                        deceleration *
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
        if (cameraTransform == null)
            return Vector3.zero;

        Vector3 forward =
            cameraTransform.forward;

        Vector3 right =
            cameraTransform.right;

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
                jumpForce;

            return;
        }

    }

    private void HandleSlide()
    {

      

        if (crouchHeld &&
            !isSliding &&
            !didSlide &&
            horizontalVelocity.magnitude >=
            slideStartSpeed)
        {
            StartSlide();
        }

        if (!isSliding)
            return;

        if (!crouchHeld)
        {
            StopSlide(
                reduceVelocity: true
            );

            return;
        }

        slideTimer +=
            Time.deltaTime;

        Vector3 desiredDirection =
            GetMoveDirection();

        if (desiredDirection.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation =
                Quaternion.LookRotation(
                    desiredDirection
                );

            Quaternion slideRotation =
                Quaternion.LookRotation(
                    slideDirection
                );

            Quaternion steeredRotation =
                Quaternion.RotateTowards(
                    slideRotation,
                    targetRotation,
                    slideTurnSpeed *
                    Time.deltaTime *
                    60f
                );

            slideDirection =
                steeredRotation *
                Vector3.forward;

            slideDirection.y = 0f;

            slideDirection.Normalize();
        }

        float slowFactor =
            GetSlowdownFactor(
                slideTimer,
                slideSlowdownStart,
                slideDuration
            );

        float targetSpeed =
            Mathf.Lerp(
                slideSpeed,
                slideSlowSpeed,
                slowFactor
            );

        horizontalVelocity =
            Vector3.MoveTowards(
                horizontalVelocity,
                slideDirection *
                targetSpeed,
                slideAcceleration *
                Time.deltaTime
            );

        if (slideTimer >=
            slideDuration)
        {
            StopSlide(
                reduceVelocity: true
            );
        }
    }

    private void StartSlide()
    {
        isSliding = true;

        // IMPORTANT :
        // Dès qu'on entre dans le slide,
        // cette tentative est consommée.
        didSlide = true;

        slideTimer = 0f;

        slideDirection =
            horizontalVelocity.normalized;

        if (slideDirection.sqrMagnitude <
            0.01f)
        {
            slideDirection =
                transform.forward;
        }
    }

    private void StopSlide(
        bool reduceVelocity
    )
    {
        isSliding = false;

        slideTimer = 0f;

        if (reduceVelocity)
        {
            horizontalVelocity *=
                slideReleaseVelocityMultiplier;
        }
    }

    

    

    private float GetSlowdownFactor(
        float timer,
        float slowdownStart,
        float duration
    )
    {
        if (timer <= slowdownStart)
            return 0f;

        float remaining =
            duration -
            slowdownStart;

        if (remaining <= 0f)
            return 1f;

        return Mathf.Clamp01(
            (timer - slowdownStart) /
            remaining
        );
    }

   


    private void HandleGravity()
    {
        if (controller.isGrounded)
            return;


        verticalVelocity -=
            gravity *
            Time.deltaTime;

        verticalVelocity =
            Mathf.Max(
                verticalVelocity,
                -maxFallSpeed
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

    private void UpdateVisualFeedback()
    {
        if (visualRoot == null)
            return;


        bool shouldBeLow =
            crouchHeld &&
            isSliding ;

        float targetHeight =
            shouldBeLow
                ? slideVisualHeight
                : 1f;

        Vector3 targetScale =
            visualNormalScale;

        targetScale.y *=
            targetHeight;

        visualRoot.localScale =
            Vector3.Lerp(
                visualRoot.localScale,
                targetScale,
                slideVisualSmooth *
                Time.deltaTime
            );
    }
}