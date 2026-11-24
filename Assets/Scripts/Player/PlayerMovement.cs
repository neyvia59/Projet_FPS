using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;
using UnityEngineInternal;
using static UnityEngine.UI.Image;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{

    [Header("References")]
    public Transform _cameraTransform;

    [SerializeField] private Transform _playerTransform;

    [SerializeField] private SpeedVFX _speedVFXManager;

    [Header("Ground Movement")]
    [SerializeField] private float _walkSpeed = 6f;
    [SerializeField] private float _sprintSpeed = 11f;

    [SerializeField] private float _acceleration = 35f;

    [SerializeField] private float _landingSpeedMult = 0.9f;
    private float _velocityMult = 1;
    [SerializeField] private float _landingSlowDuration = 0.7f;

    [SerializeField] private float _directionChangeSpeed = 200f;

    [Range(0, 1f)]
    [SerializeField] private float _inputsInfluenceDuringLaunch;

    private bool _isBeingLaunch;
    private float _currentAcceleration;
    [SerializeField] private float _timeBetweenStepsWalk = 1.2f;
    [SerializeField] private float _timeBetweenStepsRun = 0.4f;
    private float _currentTimeBetweenSteps;
    private Coroutine _stepsCoroutine;


    [Header("Air Movement")]
    [SerializeField] private float _airAcceleration = 2.5f;
    [SerializeField] private float _airControlSpeed = 10f;
    [SerializeField] private float _maxAirStrafeSpeed = 10;
    [Range(0f, 1f)]
    [SerializeField] private float _dirChangeSpeedAirMult = 0.9f;

    [Header("Jump")]
    [SerializeField] private float _jumpForce = 9f;

    [SerializeField] private float _gravity = 28f;
    [SerializeField] private float _maxFallSpeed = 40f;

    [SerializeField] private float _coyotteTimeDuration = 0.5f;
    private bool _canCoyotte;
    private bool _isInCoyotte;
    private float _coyotteTimer;
    private bool _isInJump;
    private bool _isSprintingInJump;

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
    [SerializeField] private float _wallRunTimeBetweenSteps = 0.6f;

    [SerializeField] private float _maxCamRotationOffset = 5f;

    private GameObject _unallowedWallToRide;
    private bool _wallJumped;
    private string _side;

    public bool _isWallRunning;

    public bool _canWallRun;
    private Vector3 _wallNormal;
    public float _wallRunTimer;


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

    public bool _isSliding;
    private bool _didSlide;
    private float _slideTimer;

    private Vector3 _slideDirection;

    [Header("Slide Visual Feedback")]
    [SerializeField] private float _slideVisualHeight = 0.55f;
    [SerializeField] private float _slideVisualSmooth = 14f;
    public bool CanStandUp = true;



    private Vector3 _visualNormalScale;



    private Vector3 _horizontalVelocity;
    private float _verticalVelocity;

    private Vector2 _moveInput;

    private bool _sprintHeld;
    private bool _crouchHeld;
    private bool _jumpPressed;
    public bool _isSprinting;



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

        HandleWallRun();

        HandleMovement();

        HandleCoyotte();

        HandleJump();

        HandleJumpBuffer();

        HandleSlide();

        HandleGravity();

        ApplyMovement();

        UpdateVisualFeedback();

        UpdateRotation();

        UpdateSpeedVFX();

        UpdateShootBool();

    }



    private void ReadInput()
    {

        var keyboard = Keyboard.current;
        var gamepad = Gamepad.current;

        _moveInput = Vector2.zero;
        _sprintHeld = false;
        _crouchHeld = false;
        _jumpPressed = false;


        if (keyboard != null)
        {
            if (keyboard.wKey.isPressed)
                _moveInput.y += 1f;

            if (keyboard.sKey.isPressed)
                _moveInput.y -= 1f;

            if (keyboard.aKey.isPressed)
                _moveInput.x -= 1f;

            if (keyboard.dKey.isPressed)
                _moveInput.x += 1f;

            _sprintHeld |= keyboard.leftShiftKey.isPressed;
            _crouchHeld |= keyboard.cKey.isPressed;
            _jumpPressed |= keyboard.spaceKey.wasPressedThisFrame;
        }

        if (gamepad != null)
        {

            Vector2 stickInput = gamepad.leftStick.ReadValue();

            _moveInput += stickInput;

            _sprintHeld |= gamepad.leftStickButton.isPressed;

            _crouchHeld |= gamepad.buttonEast.isPressed;


            _jumpPressed |= gamepad.buttonSouth.wasPressedThisFrame;
        }



        _moveInput = Vector2.ClampMagnitude(_moveInput, 1f);

        if (_isBeingLaunch)
        {
            _moveInput *= _inputsInfluenceDuringLaunch;
        }

        if (!_sprintHeld)
            _isSprintingInJump = false;

        if (!_crouchHeld)
            _didSlide = false;
    }



    private void HandleGroundState()
    {
        if (!_controller.isGrounded)
            return;

        _canWallRun = true;
        _isSprintingInJump = false;

        if (_verticalVelocity < 0f)
        {
            _verticalVelocity = -2f;
        }

        if (_isInJump)
        {
            StartCoroutine(LandingSlowDown());
            _velocityMult = _landingSpeedMult;
            SoundManager.PlaySound(SoundManager._land, SoundManager._landVolume);
            _isInJump = false;
        }

    }
    private void HandleMovement()
    {
        if (_isSliding || _isWallRunning)
            return;

        Vector3 desiredDirection = GetMoveDirection();

        if (_controller.isGrounded)
        {
            _isBeingLaunch = false;
            _wallJumped = false;
            float targetSpeed = _sprintHeld
                ? _sprintSpeed
                : _walkSpeed;

            if (_sprintHeld) _isSprinting = true;
            else _isSprinting = false;


            Vector3 targetVelocity = desiredDirection * targetSpeed;

            float acceleration = _acceleration;

            if (desiredDirection.sqrMagnitude > 0.01f &&
                _horizontalVelocity.sqrMagnitude > 0.01f)
            {
                float directionDot = Vector3.Dot(
                    _horizontalVelocity.normalized,
                    desiredDirection.normalized
                );

                float directionChange = 1f - directionDot;


                acceleration = Mathf.Lerp(
                    _acceleration,
                    _directionChangeSpeed,
                    directionChange
                );

                if (_isSprinting)
                {
                    if (_currentTimeBetweenSteps != _timeBetweenStepsRun)
                    {
                        if (_stepsCoroutine != null)
                        {
                            StopCoroutine(_stepsCoroutine);
                            _stepsCoroutine = null;
                        }
                    }
                }
                else if (_currentTimeBetweenSteps != _timeBetweenStepsWalk)
                {
                    if (_stepsCoroutine != null)
                    {
                        StopCoroutine(_stepsCoroutine);
                        _stepsCoroutine = null;
                    }

                }


                if (_isSprinting && _stepsCoroutine == null)
                {
                    _currentTimeBetweenSteps = _timeBetweenStepsRun;
                    System.Random rand = new System.Random();
                    int index = rand.Next(0, SoundManager._walkingSounds.Count);
                    SoundManager.PlaySound(SoundManager._walkingSounds[index], SoundManager._walkingSoundsVolume);
                    _stepsCoroutine = StartCoroutine(StepsSFXDelay(_timeBetweenStepsRun));
                }
                else if (_stepsCoroutine == null)
                {

                    _currentTimeBetweenSteps = _timeBetweenStepsWalk;
                    System.Random rand = new System.Random();
                    int index = rand.Next(0, SoundManager._walkingSounds.Count);
                    SoundManager.PlaySound(SoundManager._walkingSounds[index], SoundManager._walkingSoundsVolume);
                    _stepsCoroutine = StartCoroutine(StepsSFXDelay(_timeBetweenStepsWalk));
                }
            }

            _horizontalVelocity = Vector3.MoveTowards(
                _horizontalVelocity,
                targetVelocity,
                acceleration * Time.deltaTime
            );


        }
        else
        {
            HandleAirMovement(desiredDirection);
        }

        if (desiredDirection.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation =
                Quaternion.LookRotation(desiredDirection);

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                15f * Time.deltaTime
            );
        }
    }

    private IEnumerator StepsSFXDelay(float length)
    {
        yield return new WaitForSeconds(length);
        _stepsCoroutine = null;
    }

    private void HandleAirMovement(Vector3 desiredDirection)
    {
        desiredDirection.y = 0f;

        if (desiredDirection.sqrMagnitude < 0.01f)
            return;

        desiredDirection.Normalize();

        float directionChangeSpeed =
            _directionChangeSpeed * _dirChangeSpeedAirMult;

        float directionDot = 0f;

        if (_horizontalVelocity.sqrMagnitude > 0.01f)
        {
            directionDot = Vector3.Dot(
                _horizontalVelocity.normalized,
                desiredDirection
            );
        }

        float directionChange = 1f - directionDot;

        float acceleration = Mathf.Lerp(
            _airAcceleration,
            directionChangeSpeed,
            directionChange
        );

        float speedBeforeAcceleration = _horizontalVelocity.magnitude;

        float accelerationSpeed =
            acceleration * _airControlSpeed * Time.deltaTime;

        _horizontalVelocity +=
            desiredDirection * accelerationSpeed;

        float allowedSpeed = Mathf.Max(
            _maxAirStrafeSpeed,
            speedBeforeAcceleration
        );

        if (_horizontalVelocity.sqrMagnitude >
            allowedSpeed * allowedSpeed)
        {
            _horizontalVelocity =
                _horizontalVelocity.normalized * allowedSpeed;
        }
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
        if (_controller.isGrounded == false && _canCoyotte)
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
        RaycastHit hit;
        if (Physics.Raycast(
            transform.position,
            -transform.up,
            out hit,
            _jumpBufferDistance) && _controller.isGrounded == false && _isSliding == false)
        {
            if (hit.transform.GetComponent<Floor>() != null)
            {
                _canJumpBuff = true;
            }
            else
            {
                _canJumpBuff = false;
            }


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
            if (_sprintHeld) _isSprintingInJump = true;
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

            SoundManager.PlaySound(SoundManager._jump, SoundManager._jumpVolume);
            Vector3 desiredDirection = GetMoveDirection();
            HandleAirMovement(desiredDirection);
            _isInJump = true;
            _isInCoyotte = false;
            _canCoyotte = false;
            _coyotteTimer = 0f;
            return;
        }



    }

    public void Launch(float Force, Vector3 target)
    {
        _isBeingLaunch = true;
        Vector3 launchVelocity = target.normalized * Force;

        _horizontalVelocity = new Vector3(
            launchVelocity.x,
            0f,
            launchVelocity.z
        );

        _verticalVelocity = launchVelocity.y;


        SoundManager.PlaySound(SoundManager._jumpPad, SoundManager._jumpPadVolume);
        _isWallRunning = false;
        _isSliding = false;
    }





    private void HandleSlide()
    {
        if (_isWallRunning)
            return;

        if (_crouchHeld &&
            !_isSliding &&
            !_didSlide &&
            _horizontalVelocity.magnitude >=
            _slideStartSpeed && _controller.isGrounded && _sprintHeld)
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
        _speedVFXManager.MoveToSlidingFOV();

        _isSliding = true;

        _didSlide = true;

        _slideTimer = 0f;

        _slideDirection =
            _horizontalVelocity.normalized;


        SoundManager.PlaySound(SoundManager._slide, SoundManager._slideVolume);

        if (_slideDirection.sqrMagnitude <
            0.01f)
        {
            _slideDirection =
                transform.forward;
        }
    }

    private void StopSlide(bool reduceVelocity)
    {
        _isSliding = false;
        _speedVFXManager.ResetFOV();
        _slideTimer = 0f;

        SoundManager.StopSlideSound();

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

        if (_controller.isGrounded || _verticalVelocity > 0 || _verticalVelocity < 0 && _controller.isGrounded)
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
                if (_unallowedWallToRide == hit.transform.gameObject && _wallJumped)
                {
                    StopWallRun();
                    return;
                }

                _wallNormal = hit.normal;
                if (hit.transform.gameObject.GetComponent<RunnableWall>() != null)
                {
                    _side = "left";
                    Camera.main.GetComponent<PlayerCam>().WallSide = "left";
                    _wallJumped = false;
                    _unallowedWallToRide = hit.transform.gameObject;
                    foundWall = true;
                }

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
                if (_unallowedWallToRide == hit.transform.gameObject && _wallJumped)
                {
                    StopWallRun();
                    return;
                }
                _wallNormal = hit.normal;
                if (hit.transform.gameObject.GetComponent<RunnableWall>() != null)
                {
                    _side = "right";
                    Camera.main.GetComponent<PlayerCam>().WallSide = "right";
                    _wallJumped = false;
                    _unallowedWallToRide = hit.transform.gameObject;
                    foundWall = true;
                }
            }
        }

        if (!foundWall)
        {
            StopWallRun();
            return;
        }


        if (!_isWallRunning && _canWallRun == true)
        {
            if (Vector3.Dot(_horizontalVelocity, -_wallNormal) < -0.1f)
            {
                StopWallRun();
                return;
            }

            _canWallRun = false;
            _isWallRunning = true;
            _wallRunTimer = 0f;

            float currentSpeed = _horizontalVelocity.magnitude;
            float targetSpeed = Mathf.Max(currentSpeed, _wallRunSpeed);
            _verticalVelocity = 0;

            //Vector3 wallDirection = GetWallRunDirection(_wallNormal);
            //_horizontalVelocity = wallDirection * targetSpeed;

        }
        else
        {

            _wallRunTimer += Time.deltaTime;

            if (_wallRunTimer >= _wallRunDuration ||
                Mathf.Abs(Vector3.SignedAngle(hit.transform.right, Camera.main.transform.forward, Vector3.up)) < 90 + _maxCamRotationOffset && _side == "left" ||
                Mathf.Abs(Vector3.SignedAngle(hit.transform.right, Camera.main.transform.forward, Vector3.up)) > 90 - _maxCamRotationOffset && _side == "right")
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

            if (_stepsCoroutine != null && _currentTimeBetweenSteps != _wallRunTimeBetweenSteps)
            {
                StopCoroutine(_stepsCoroutine);
                _stepsCoroutine = null;
            }

            if (_stepsCoroutine == null)
            {
                _currentTimeBetweenSteps = _wallRunTimeBetweenSteps;
                System.Random rand = new System.Random();
                int index = rand.Next(0, SoundManager._wallrunSounds.Count);
                SoundManager.PlaySound(SoundManager._wallrunSounds[index], SoundManager._wallrunVolume);
                _stepsCoroutine = StartCoroutine(StepsSFXDelay(_wallRunTimeBetweenSteps));
            }

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
        _canWallRun = true;
        _wallJumped = true;
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

        SoundManager.PlaySound(SoundManager._jump, SoundManager._jumpVolume);

        _verticalVelocity = jumpDirection.y;
    }

    public void StopWallRun()
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
        if (GetComponent<PlayerCollisions>().IsInDeathAnim) _horizontalVelocity = Vector2.zero;
        Vector3 velocity =
            _horizontalVelocity +
            Vector3.up *
            _verticalVelocity;



        CollisionFlags flags =
            _controller.Move(
                velocity *
                Time.deltaTime * _velocityMult
            );
        if ((flags & CollisionFlags.Above) != 0 &&
        _verticalVelocity > 0f)
        {
            _verticalVelocity =
                0f;
        }

    }

    private IEnumerator LandingSlowDown()
    {
        yield return new WaitForSeconds(_landingSlowDuration);
        _velocityMult = 1f;
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

        if (targetHeight == 1)
        {
            targetHeight = CanStandUp ? 1f : _slideVisualHeight;
        }


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

    private void UpdateRotation()
    {
        transform.rotation = Quaternion.Euler(0, _cameraTransform.rotation.eulerAngles.y, 0);
    }

    private void UpdateSpeedVFX()
    {
        if (_isSliding || _sprintHeld && _controller.isGrounded || _sprintHeld && _isSprintingInJump)
        {
            _speedVFXManager.UpdateWindVFX(Mathf.Abs(_controller.velocity.x) + Mathf.Abs(_controller.velocity.y) + Mathf.Abs(_controller.velocity.z));

        }
        else
        {
            _speedVFXManager.ResetWindVFX();
        }
    }

    private void UpdateShootBool()
    {
        if (_sprintHeld && _isSliding == false && _isWallRunning == false || _isSprintingInJump)
        {
            gameObject.GetComponent<WeaponManager>()._canShoot = false;
        }
        else
        {
            gameObject.GetComponent<WeaponManager>()._canShoot = true;
        }

        if (_isWallRunning)
        {
            gameObject.GetComponent<WeaponManager>()._canShoot = true;
        }
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawCube(transform.position, new Vector3(_wallCheckDistance, _wallCheckHeight, _wallCheckDistance));
    }
}