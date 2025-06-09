using UnityEngine;

public class SpidermanClimb : MonoBehaviour
{
    [SerializeField] private float climbSpeed = 3f;
    [SerializeField] private float turnSpeed = 2f;

    [Header("Boxcast Properties")]
    [SerializeField] private float yOffset = 0.5f;
    [SerializeField] private float zOffset = 0f;
    [SerializeField] private float maxDistance = 0.5f;
    [SerializeField] private Vector3 boxSize = Vector3.one;
    [SerializeField] private LayerMask climbLayer;

    private Vector3 characterNormal;
    private Vector3 wallNormal;
    private bool isClimbing;
    
    private Animator animator;
    private SpidermanController spidermanController;
   
    private Rigidbody rb;
    private Vector3 moveDirection;
    
    private RaycastHit hit;
    private bool hitDetect;
    private Vector3 castOrigin;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();
        spidermanController = GetComponent<SpidermanController>();

        characterNormal = transform.up;
   }

    public void HandleClimbing()
    {
        if (!isClimbing)
        {
            animator.SetTrigger("Climbing");
            isClimbing = true;
        }

        float vertical = Input.GetAxisRaw("Vertical");
        float horizontal = Input.GetAxisRaw("Horizontal");

        moveDirection = transform.forward * vertical;
        moveDirection += transform.right * horizontal;
        moveDirection.Normalize();
        moveDirection *= climbSpeed;

        rb.AddForce(-wallNormal * 10f, ForceMode.Force); //Make sure spidey stick nicely to the wall

        if (moveDirection.magnitude > 0)
        {
            characterNormal = Vector3.Lerp(characterNormal, wallNormal, turnSpeed * Time.deltaTime);

            // find forward direction with new myNormal:
            //var characterForward = Vector3.Cross(transform.right, characterNormal);

            // align character to the new characterNormal while keeping the forward direction:
            var targetRot = Quaternion.LookRotation(moveDirection, characterNormal);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, turnSpeed * Time.deltaTime);
        }

        rb.linearVelocity = moveDirection;
        animator.SetFloat("ClimbSpeed", rb.linearVelocity.magnitude);

    }

    public void ClimbInput()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isClimbing)
        {
            animator.SetTrigger("JumpOffWall");
            JumpOffWall();
        }
    }

    private void FixedUpdate()
    {
        WallCheck();
    }

    private void WallCheck()
    {
        castOrigin = new Vector3(transform.position.x, transform.position.y + yOffset, transform.position.z + zOffset);
        hitDetect = Physics.BoxCast(castOrigin, boxSize * 0.5f, transform.forward, out hit, transform.rotation, maxDistance, climbLayer);

        if (hitDetect)
        {
            Debug.Log("Hit : " + hit.collider.name);

            if (!isClimbing)
            {
                wallNormal = hit.normal;
                rb.useGravity = false; // Disable gravity while climbing
                rb.AddForce(transform.up * 50f + (-wallNormal * 15f), ForceMode.Force); //Move up a bit and stick to the wall

                spidermanController.CurrentState = SpiderManState.Climbing;

            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        //Check if there has been a hit yet
        if (hitDetect)
        {
            //Draw a Ray forward from GameObject toward the hit
            Gizmos.DrawRay(castOrigin, transform.forward * hit.distance);
            //Draw a cube that extends to where the hit exists
            Gizmos.DrawWireCube(castOrigin + transform.forward * hit.distance, boxSize);
        }
        //If there hasn't been a hit yet, draw the ray at the maximum distance
        else
        {
            //Draw a Ray forward from GameObject toward the maximum distance
            Gizmos.DrawRay(castOrigin, transform.forward * maxDistance);
            //Draw a cube at the maximum distance
            Gizmos.DrawWireCube(castOrigin + transform.forward * maxDistance , boxSize);
        }
    }


    private void JumpOffWall()
    {
        characterNormal = transform.up;

        var targetRot = Quaternion.LookRotation(moveDirection, characterNormal);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, turnSpeed * Time.deltaTime);

        isClimbing = false;
        rb.AddForce(-wallNormal * 500f + Vector3.up * 300f);
        StopClimbing();
        
        spidermanController.CurrentState = SpiderManState.Jumping;
    }

    private void StopClimbing()
    {
        rb.useGravity = true;
        
    }
}

