using FischlWorks;
using UnityEngine;

[RequireComponent(typeof(SpidermanMovement))]
[RequireComponent (typeof(SpidermanClimb))]
[RequireComponent(typeof(SpidermanSwing))]
public class SpidermanController : MonoBehaviour
{
    public SpiderManState CurrentState;
    public static bool Grounded;

    [Header("Ground Check Settings")]
    [SerializeField] private float groundCheckDistance = 0.21f;
    [SerializeField] private Transform groundCheckL;
    [SerializeField] private Transform groundCheckR;
    [SerializeField] private LayerMask groundLayer;

    [Space]
    [SerializeField] private float fallForce = 11f;

    private SpidermanMovement movement;
    private SpidermanSwing swing;
    private SpidermanClimb climb;
  
    private Rigidbody rb;

    private csHomebrewIK csHomebrewIK;
    private bool footIkEnabled;

    private void Start()
    {
        movement = GetComponent<SpidermanMovement>();
        swing = GetComponent<SpidermanSwing>();
        climb = GetComponent<SpidermanClimb>();
        
        rb = GetComponent<Rigidbody>();
        csHomebrewIK = GetComponentInChildren<csHomebrewIK>();

        Grounded = true;
        footIkEnabled = true;
    }

    private void Update()
    {
        GroundCheck();

        switch (CurrentState)
        {
            case SpiderManState.Idle: 
            case SpiderManState.Moving:
                movement.JumpInput();
                break;

            case SpiderManState.Jumping:

                if (footIkEnabled)
                {
                    csHomebrewIK.enabled = false;
                    footIkEnabled = false;
                }

                movement.JumpLogic();
                break;

            case SpiderManState.Climbing:

                climb.ClimbInput();

                if (footIkEnabled)
                {
                    csHomebrewIK.enabled = false;
                    footIkEnabled = false;
                }

                break;

            case SpiderManState.Swinging:
                
                break;

            case SpiderManState.Falling:
             
                break;
        }

        if (Grounded)
        {
            if (!footIkEnabled)
            {
                csHomebrewIK.enabled = true;
                footIkEnabled = true;
            }
        }
    }

    private void FixedUpdate()
    {
        switch (CurrentState)
        {
            case SpiderManState.Idle:
            case SpiderManState.Moving:
                if (Grounded)
                {
                    movement.HandleMovement();
                    movement.HandleRotation();
                }
                break;

            case SpiderManState.Jumping:
                movement.HandleJumping();
                //movement.HandleFalling();
                break;

            case SpiderManState.Climbing:
                climb.HandleClimbing();
                break;

            case SpiderManState.Swinging:
                swing.HandleWebSwinging();
                break;

            case SpiderManState.Falling:
                //HandleFalling();
                break;
        }

        if (!Grounded)
        {
            rb.AddForce(1.5f * fallForce * Time.deltaTime * Vector3.down, ForceMode.Acceleration); // Boost downward force
        }
    }

    private void GroundCheck()
    {
        RaycastHit hit;

        if (Physics.Raycast(groundCheckL.position, Vector3.down, out hit, groundCheckDistance, groundLayer)
            || Physics.Raycast(groundCheckR.position, Vector3.down, out hit, groundCheckDistance, groundLayer))
        {
            Grounded = true;
        }
        else
        {
            Grounded = false;
        }

        //Debug.Log("Grounded: " + Grounded);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawRay(groundCheckL.position, Vector3.down);
        Gizmos.DrawRay(groundCheckR.position, Vector3.down);
    }
}

public enum SpiderManState
{
    Idle,
    Moving,
    Jumping,
    Falling,
    Climbing,
    Swinging
}
