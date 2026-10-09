using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.UI.Image;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform _cameraTransform;

    [SerializeField] private Transform _playerTransform;

    [Header("Ground Movement")]
    [SerializeField] private float _walkSpeed = 6f;
    [SerializeField] private float _sprintSpeed = 11f;

    [SerializeField] private float _acceleration = 35f;
    [SerializeField] private float _deceleration = 25f;

    [Header("Jump")]
    [SerializeField] private float _jumpForce = 9f;

    [SerializeField] private float _gravity = 28f;
    [SerializeField] private float _maxFallSpeed = 40f;

    [SerializeField] private float _coyotteTimeDuration = 0.5f;
    private bool _canCoyotte;
    private bool _isInCoyotte;
    private float _coyotteTimer;

    [SerializeField] private float _jumpBufferDistance;
    private bool _canJumpBuff;
    [SerializeField] private float _jumpBuffForce = 9f;

    [Header("Wall Ride")]
    [SerializeField] private float _wallCheckDistance = 0.8f;
    [SerializeField] private float _wallCheckHeight = 1.0f;
    [SerializeField] private float _wallMinNormalY = 0.2f;
    [SerializeField] private float _wallRunSpeed = 10f;
    [SerializeField] private float _wallRunGravity = 4f;
    [SerializeField] private float _wallRunDuration = 1.5f;
    [SerializeField] private float _wallJumpForce = 10f;
    [SerializeField] private float _wallJumpAwayForce = 8f;
    [SerializeField] private float _wallJumpForwardForce = 5f;

    private bool _isWallRunning;
    private Vector3 _wallNormal;
    private float _wallRunTimer;


    [Header("Ground Slide")]
    [SerializeField] private float _slideSpeed = 14f;
    [SerializeField] private float _slideSlowSpeed = 1.5f;

    [SerializeField] private float _slideAcceleration = 25f;

    [SerializeField] private float _slideDuration = 2f;
    [SerializeField] private float _slideSlowdownStart = 1.8f;

    [SerializeField] private float _slideStartSpeed = 3f;

    [Header("Ground Slide Steering")]
    [SerializeField] private float _slideTurnSpeed = 8f;

    [Tooltip("Vitesse conservée quand Ctrl est relâché.")]
    [Range(0f, 1f)]
    [SerializeField] private float _slideReleaseVelocityMultiplier = 0f;

    private bool _isSliding;
    private bool _didSlide;
    private float _slideTimer;

    private Vector3 _slideDirection;

    [Header("Slide Visual Feedback")]
    [SerializeField] private float _slideVisualHeight = 0.55f;
    [SerializeField] private float _slideVisualSmooth = 14f;

    private Vector3 _visualNormalScale;

    private Vector3 _horizontalVelocity;
    private float _verticalVelocity;

    private Vector2 _moveInput;

    private bool _sprintHeld;
    private bool _crouchHeld;
    private bool _jumpPressed;


    private CharacterController _controller;

    private void Awake()
    {
        _controller =
            GetComponent<CharacterController>();

        _didSlide =
            false;

        if (_playerTransform != null)
        {
            _visualNormalScale =
                _playerTransform.localScale;
        }
    }

    private void Update()
    {
        ReadInput();

        HandleGroundState();

        HandleSlideInputReset();

        HandleWallRun();

        HandleMovement();

        HandleCoyotte();

        HandleJump();

        HandleJumpBuffer();

        HandleSlide();

        HandleGravity();

        ApplyMovement();

        UpdateVisualFeedback();
    }


    private void ReadInput()
    {
        if (Keyboard.current == null)
            return;

        _moveInput = Vector2.zero;

        if (Keyboard.current.wKey.isPressed)
            _moveInput.y += 1f;

        if (Keyboard.current.sKey.isPressed)
            _moveInput.y -= 1f;

        if (Keyboard.current.aKey.isPressed)
            _moveInput.x -= 1f;

        if (Keyboard.current.dKey.isPressed)
            _moveInput.x += 1f;

        _moveInput =
            Vector2.ClampMagnitude(
                _moveInput,
                1f
            );

        _sprintHeld =
            Keyboard.current.leftShiftKey.isPressed;

        _crouchHeld =
            Keyboard.current.leftCtrlKey.isPressed;

        _jumpPressed =
            Keyboard.current.spaceKey.wasPressedThisFrame;

    }


    private void HandleSlideInputReset()
    {

        if (!_crouchHeld)
        {
            _didSlide = false;
        }
    }

    private void HandleGroundState()
    {
        if (!_controller.isGrounded)
            return;

        if (_verticalVelocity < 0f)
        {
            _verticalVelocity = -2f;
        }
    }
    private void HandleMovement()
    {
        if (_isSliding || _isWallRunning)
            return;

        Vector3 desiredDirection =
            GetMoveDirection();

        if (_controller.isGrounded)
        {
            float targetSpeed =
                _sprintHeld
                    ? _sprintSpeed
                    : _walkSpeed;

            Vector3 targetVelocity =
                desiredDirection *
                targetSpeed;

            _horizontalVelocity =
                Vector3.MoveTowards(
                    _horizontalVelocity,
                    targetVelocity,
                    _acceleration *
                    Time.deltaTime
                );

            if (desiredDirection.sqrMagnitude < 0.01f)
            {
                _horizontalVelocity =
                    Vector3.MoveTowards(
                        _horizontalVelocity,
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

        _horizontalVelocity =
            Vector3.MoveTowards(
                _horizontalVelocity,
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
            _moveInput.y
            +
            right *
            _moveInput.x;

        return Vector3.ClampMagnitude(
            direction,
            1f
        );
    }


    private void HandleCoyotte()
    {
        if(_controller.isGrounded == false && _canCoyotte)
        {
            
            if (_coyotteTimer <= _coyotteTimeDuration)
            {
                _isInCoyotte = true;
                _coyotteTimer += Time.deltaTime;
            }
            else
            {
                _isInCoyotte = false;
                _canCoyotte = false;
                _coyotteTimer = 0f;
            }
        }

        if (_controller.isGrounded)
        {
            _canCoyotte = true;
        }
    }

    private void HandleJumpBuffer()
    {

        if (Physics.Raycast(
            transform.position,
            -transform.up,
            _jumpBufferDistance) && _controller.isGrounded == false)
        {
            _canJumpBuff = true;


        }
        else
        {
            _canJumpBuff = false;
        }
    }

    private void HandleJump()
    {
        if (!_jumpPressed)
            return;

        if (_isWallRunning)
        {
            WallJump();
            return;
        }

        if (_controller.isGrounded || _isInCoyotte || _canJumpBuff)
        {
            if (_canJumpBuff)
            {
                _verticalVelocity =
                    _jumpBuffForce;
            }
            else
            {
                _verticalVelocity =
                    _jumpForce;
            }
                
            _isInCoyotte = false;
            _canCoyotte = false;
            _coyotteTimer = 0f;
            return;
        }



    }

    

    private void HandleSlide()
    {
        if (_isWallRunning)
            return;

        if (_crouchHeld &&
            !_isSliding &&
            !_didSlide &&
            _horizontalVelocity.magnitude >=
            _slideStartSpeed)
        {
            StartSlide();
        }

        if (!_isSliding)
            return;

        if (!_crouchHeld)
        {
            StopSlide(
                reduceVelocity: true
            );

            return;
        }

        _slideTimer +=
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
                    _slideDirection
                );

            Quaternion steeredRotation =
                Quaternion.RotateTowards(
                    slideRotation,
                    targetRotation,
                    _slideTurnSpeed *
                    Time.deltaTime *
                    60f
                );

            _slideDirection =
                steeredRotation *
                Vector3.forward;

            _slideDirection.y = 0f;

            _slideDirection.Normalize();
        }

        float slowFactor =
            GetSlowdownFactor(
                _slideTimer,
                _slideSlowdownStart,
                _slideDuration
            );

        float targetSpeed =
            Mathf.Lerp(
                _slideSpeed,
                _slideSlowSpeed,
                slowFactor
            );

        _horizontalVelocity =
            Vector3.MoveTowards(
                _horizontalVelocity,
                _slideDirection *
                targetSpeed,
                _slideAcceleration *
                Time.deltaTime
            );

        if (_slideTimer >=
            _slideDuration)
        {
            StopSlide(
                reduceVelocity: true
            );
        }
    }

    private void StartSlide()
    {
        _isSliding = true;

        _didSlide = true;

        _slideTimer = 0f;

        _slideDirection =
            _horizontalVelocity.normalized;

        if (_slideDirection.sqrMagnitude <
            0.01f)
        {
            _slideDirection =
                transform.forward;
        }
    }

    private void StopSlide(
        bool reduceVelocity
    )
    {
        _isSliding = false;

        _slideTimer = 0f;

        if (reduceVelocity)
        {
            _horizontalVelocity *=
                _slideReleaseVelocityMultiplier;
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




    private void HandleWallRun()
    {

        if (_controller.isGrounded)
        {
            StopWallRun();
            return;
        }

        Vector3 origin = transform.position + Vector3.up * _wallCheckHeight;

        bool foundWall = false;
        RaycastHit hit;

        if (Physics.Raycast(
            origin,
            -transform.right,
            out hit,
            _wallCheckDistance))
        {
            if (IsValidWall(hit.normal))
            {
                _wallNormal = hit.normal;
                foundWall = true;
            }
        }

        if (!foundWall &&
            Physics.Raycast(
                origin,
                transform.right,
                out hit,
                _wallCheckDistance))
        {
            if (IsValidWall(hit.normal))
            {
                _wallNormal = hit.normal;
                foundWall = true;
            }
        }

        if (!foundWall)
        {
            StopWallRun();
            return;
        }

        if (!_isWallRunning)
        {
            if (Vector3.Dot(_horizontalVelocity, -_wallNormal) < -0.1f)
            {
                StopWallRun();
                return;
            }

            _isWallRunning = true;
            _wallRunTimer = 0f;

            float currentSpeed = _horizontalVelocity.magnitude;
            float targetSpeed = Mathf.Max(currentSpeed, _wallRunSpeed);

            Vector3 wallDirection = GetWallRunDirection(_wallNormal);
            _horizontalVelocity = wallDirection * targetSpeed;
            _verticalVelocity = Mathf.Max(_verticalVelocity, -2f);
        }
        else
        {
            _wallRunTimer += Time.deltaTime;

            if (_wallRunTimer >= _wallRunDuration)
            {
                StopWallRun();
                return;
            }

            Vector3 wallDirection = GetWallRunDirection(_wallNormal);
            float speed = Mathf.Max(_horizontalVelocity.magnitude, _wallRunSpeed);

            _horizontalVelocity =
                Vector3.MoveTowards(
                    _horizontalVelocity,
                    wallDirection * speed,
                    _acceleration * Time.deltaTime
                );
        }
    }

    private bool IsValidWall(Vector3 normal)
    {
        return Mathf.Abs(normal.y) <= _wallMinNormalY;
    }

    private Vector3 GetWallRunDirection(Vector3 normal)
    {
        Vector3 direction =
            Vector3.ProjectOnPlane(transform.forward, normal);

        if (direction.sqrMagnitude < 0.01f)
            direction = Vector3.ProjectOnPlane(_cameraTransform.forward, normal);

        direction.y = 0f;

        if (direction.sqrMagnitude < 0.01f)
            return transform.forward;

        direction.Normalize();

        if (Vector3.Dot(direction, _horizontalVelocity) < 0f)
            direction = -direction;

        return direction;
    }

    private void WallJump()
    {
        _isWallRunning = false;
        _wallRunTimer = 0f;

        Vector3 jumpDirection =
            _wallNormal * _wallJumpAwayForce +
            Vector3.up * _wallJumpForce +
            GetWallRunDirection(_wallNormal) * _wallJumpForwardForce;

        _horizontalVelocity =
            Vector3.ProjectOnPlane(
                jumpDirection,
                Vector3.up
            );

        _verticalVelocity = jumpDirection.y;
    }

    private void StopWallRun()
    {

        _isWallRunning = false;
        _wallRunTimer = 0f;
    }

    private void HandleGravity()
    {
        if (_controller.isGrounded)
            return;

        if (_isWallRunning)
        {
            _verticalVelocity -=
                _wallRunGravity *
                Time.deltaTime;

            _verticalVelocity =
                Mathf.Max(
                    _verticalVelocity,
                    -_wallRunSpeed
                );

            return;
        }

        _verticalVelocity -=
            _gravity *
            Time.deltaTime;

        _verticalVelocity =
            Mathf.Max(
                _verticalVelocity,
                -_maxFallSpeed
            );
    }

    private void ApplyMovement()
    {
        Vector3 velocity =
            _horizontalVelocity +
            Vector3.up *
            _verticalVelocity;

        CollisionFlags flags =
            _controller.Move(
                velocity *
                Time.deltaTime
            );

        if ((flags & CollisionFlags.Above) != 0 &&
            _verticalVelocity > 0f)
        {
            _verticalVelocity =
                0f;
        }
    }

    private void UpdateVisualFeedback()
    {
        if (_playerTransform == null)
            return;


        bool shouldBeLow =
            _crouchHeld &&
            _isSliding;

        float targetHeight =
            shouldBeLow
                ? _slideVisualHeight
                : 1f;

        Vector3 targetScale =
            _visualNormalScale;

        targetScale.y *=
            targetHeight;

        _playerTransform.localScale =
            Vector3.Lerp(
                _playerTransform.localScale,
                targetScale,
                _slideVisualSmooth *
                Time.deltaTime
            );
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawCube(transform.position, new Vector3(_wallCheckDistance, _wallCheckHeight, _wallCheckDistance));
    }
}