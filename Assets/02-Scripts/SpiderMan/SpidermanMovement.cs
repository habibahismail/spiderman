using UnityEngine;

public class SpidermanMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float runSpeed = 5;
    [SerializeField] private float sprintSpeed = 10;
    [SerializeField] private float rotationSpeed = 15;

    [Header("Jump Settings")]
    [SerializeField] private float jumpForce = 10f;

    private bool isMoving;
    private bool isJumping;

    //private float inAirTimer;

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

    private float GetPlayerHeight()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 100f))
        {
            return transform.position.y - hit.point.y; // Distance from ground
        }
        return 0f; // Default fallback
    }

    public void HandleMovement()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        // No input detected -> Reset movement
        if (horizontal == 0 && vertical == 0)
        {
            isMoving = false;
            spidermanController.CurrentState = SpiderManState.Idle;

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
        if (SpidermanController.Grounded && !isJumping)
        {
            rb.AddForce(Vector3.up * jumpForce *Time.deltaTime, ForceMode.Impulse);
            animator.SetBool("Grounded", false);
            
            SpidermanController.Grounded = false;
            isJumping = true;
        }

        if (isJumping)
        {
            float smoothedHeight = GetPlayerHeight();
            smoothedHeight = Mathf.Lerp(smoothedHeight, GetPlayerHeight(), Time.deltaTime * 5);
            animator.SetFloat("Height", smoothedHeight);
        }
    }

    public void JumpInput()
    {
        if (Input.GetKeyDown(KeyCode.Space) && SpidermanController.Grounded)
        {
            spidermanController.CurrentState = SpiderManState.Jumping;
        }
    }    

    public void JumpLogic()
    {
        if (isJumping && SpidermanController.Grounded && rb.linearVelocity.y <= 0)
        {
            isJumping = false;
            animator.SetBool("Grounded", true);
            spidermanController.CurrentState = SpiderManState.Idle;
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

