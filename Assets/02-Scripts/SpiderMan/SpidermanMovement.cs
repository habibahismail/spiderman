using UnityEngine;

public class SpidermanMovement : MonoBehaviour
{
    [SerializeField] private float runSpeed = 5;
    [SerializeField] private float sprintSpeed = 10;
    [SerializeField] private float rotationSpeed = 15;
    
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform groundCheckTransform;
     //[SerializeField] private float fallForce = 125f;

    public AudioSource runningAudio;

    private bool isMoving;
    private bool isJumping;
    
    private bool isGrounded = true;
    private Collider[] groundCollisions;
    
    private float inAirTimer;

    private Rigidbody rb;
    private Vector3 moveDirection;
    private Animator animator;
    private SpidermanController spidermanController;

    private void Start()
    {
        animator = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody>();
        spidermanController = GetComponent<SpidermanController>();
    }

    private void FixedUpdate()
    {
       GroundCheck();
    }

    private void GroundCheck()
    {
        groundCollisions = Physics.OverlapSphere(groundCheckTransform.position, groundCheckRadius, groundLayer);

        if (groundCollisions.Length > 0 ) 
            isGrounded = true;
        else
            isGrounded= false;
    }

    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.red;
    //    Gizmos.DrawWireSphere(groundCheckTransform.position, groundCheckRadius);
    //}

    public void HandleMovement()
    {
        Debug.Log(isGrounded);
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        // No input detected -> Reset movement
        if (horizontal == 0 && vertical == 0)
        {
            isMoving = false;
            spidermanController.CurrentState = SpiderManState.Idle;
            runningAudio.Stop();

            // Explicitly stop movement
            rb.linearVelocity = Vector3.zero;
            animator.SetFloat("MoveSpeed", rb.linearVelocity.magnitude);
            return;
        }

        // Continue movement logic
        isMoving = true;
        moveDirection = (Camera.main.transform.forward * vertical) + (Camera.main.transform.right * horizontal);
        moveDirection.Normalize();
        moveDirection.y = 0;

        float speed = Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : runSpeed;
        moveDirection *= speed;

        spidermanController.CurrentState = SpiderManState.Moving;
      
        if (!runningAudio.isPlaying)
        {
            runningAudio.Play();
        }

        rb.linearVelocity = moveDirection; // Ensure velocity updates with movement
        animator.SetFloat("MoveSpeed", rb.linearVelocity.magnitude);
    }

    public void HandleRotation()
    {
        if (!isMoving) return;

        Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    public void HandleJumping()
    {
        if (isGrounded)
        {
            if (isJumping)
            {
                isJumping = false;
                spidermanController.CurrentState = SpiderManState.Idle;
                return;
            }

            if (isMoving)
                animator.SetTrigger("runJump");
            else
                animator.SetTrigger("jump");

            Debug.Log("JUMP!");
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            
            isGrounded = false;
            isJumping = true;
        }   
    }

    public void JumpInput()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            SpiderManState _currentState = spidermanController.CurrentState;

            if (_currentState == SpiderManState.Moving || _currentState == SpiderManState.Idle)
            {
               spidermanController.CurrentState = SpiderManState.Jumping;
            }
        }
    }

    //public void HandleFalling()
    //{
    //    if (!isGrounded)
    //    {
    //        inAirTimer += Time.deltaTime * 2.5f;
    //        rb.AddForce(fallForce * inAirTimer * Vector3.down);

    //        // Check if Spidey has landed
    //        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 0.05f) && hit.collider.CompareTag("Walkable"))
    //        {
    //            inAirTimer = 0; // Reset fall timer
    //            isGrounded = true;
    //            spidermanController.CurrentState = SpiderManState.Idle;
    //            //animator.SetInteger("State", (int)SpiderManAnimationState.HardLanding);
    //        }
    //    }
    //}

}

