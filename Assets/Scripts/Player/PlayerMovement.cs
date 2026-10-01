using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed;
    public float BaseSpeed;
    public float CrounchSpeed;
    public float SprintSpeed;
    public Transform orientation;
    private float CurrentStam;
    [SerializeField] private float MaxStamina = 5;
    float horizInput;
    float vertInput;

    private bool IsSprinting = false;
    private bool IsSlowing = false;

    public VolumeProfile volumeProfile;
    public float BlurSprint;

    Vector3 moveDirection;

    Rigidbody rb;

    public float PlayerHeight;
    public LayerMask Ground;
    bool IsGrounded;
    public float groundDrag;
    public bool IsCrounched = false;

    public float MaxSlopeAngle;
    private RaycastHit Slopehit;

    void Start()
    {
        CurrentStam = MaxStamina;
        BaseSpeed = moveSpeed;
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    private void Update()
    {
        IsGrounded = Physics.Raycast(transform.position, Vector3.down, PlayerHeight * 0.5f + 0.2f, Ground);
        PlayerInput();
        //DepthOfField dof;

        //if (volumeProfile.TryGet(out dof))
        //{
        //    dof.focusDistance.value = 1f;
        //}
        if (IsGrounded) 
        {
            rb.linearDamping = groundDrag;
        }
        else
        {
            rb.linearDamping = 0;
        }

        if (IsSprinting) 
        {
            if (CurrentStam > 0) //running
            {
                moveSpeed = SprintSpeed;
                CurrentStam -= Time.deltaTime;
                if (CurrentStam < MaxStamina / 3)
                {
                    DepthOfField dof;

                    if (volumeProfile.TryGet(out dof))
                    {
                        dof.focusDistance.value = 1+CurrentStam;
                    }
                }
            }
            else //regen stam
            {
                IsSlowing = true;
                moveSpeed = CrounchSpeed/2;
                IsSprinting = false;
            }

        }
        else
        {
            if (CurrentStam < MaxStamina )
            {
                CurrentStam += Time.deltaTime;
                if (CurrentStam < MaxStamina / 3)
                {
                    DepthOfField dof;

                    if (volumeProfile.TryGet(out dof))
                    {
                        dof.focusDistance.value = 1 + CurrentStam;
                    }
                }
            }
            else
            {
                CurrentStam = MaxStamina;
            }
        }

        if (IsSlowing)
        {
            if (CurrentStam < MaxStamina/2)
            {
                CurrentStam += Time.deltaTime;  
                if (CurrentStam < MaxStamina / 3)
                {
                    DepthOfField dof;

                    if (volumeProfile.TryGet(out dof))
                    {
                        dof.focusDistance.value = 1;
                    }
                }
            }
            else
            {
                IsSlowing = false;
                
            }
        }
        
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void PlayerInput()
    {
        horizInput = Input.GetAxisRaw("Horizontal");
        vertInput = Input.GetAxisRaw("Vertical");

        if (Input.GetKeyDown(KeyCode.C))
        {
            moveSpeed = CrounchSpeed;
            transform.localScale = new Vector3(1, 0.6f, 1);
            rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
        }


        if (Input.GetKeyUp(KeyCode.C))
        {
            {
                moveSpeed = BaseSpeed;
                transform.localScale = new Vector3(1, 1, 1);

            }
        }

        if (Input.GetKeyDown(KeyCode.LeftShift)) 
        { 
            if (!IsSlowing)
            {
                IsSprinting = true;
            }
            
        }

        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            moveSpeed = BaseSpeed;
            IsSprinting = false;
        }
    }

    private void MovePlayer()
    {
        moveDirection = orientation.forward * vertInput + orientation.right * horizInput;
        if (IsSlowing) { moveSpeed = CrounchSpeed / 2; }

 
            rb.AddForce(moveDirection.normalized * moveSpeed, ForceMode.Force);
        
            
    }
}
