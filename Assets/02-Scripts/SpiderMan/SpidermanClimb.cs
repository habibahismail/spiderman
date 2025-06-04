using UnityEngine;

public class SpidermanClimb : MonoBehaviour
{
    public float climbSpeed = 3f;
    public float maxClimbAngle = 140f;
    private bool isClimbing;
    private Rigidbody _rigidbody;
    private RaycastHit _hit;
    private GameObject climbingSurface;

    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    //private void Update()
    //{
    //    HandleClimbing();
    //}

    public void HandleClimbing()
    {
        if (Input.GetKey(KeyCode.W) && isClimbing)
        {
            _rigidbody.linearVelocity = transform.up * climbSpeed;
        }

        if (Input.GetKeyDown(KeyCode.Space) && isClimbing)
        {
            JumpOffWall();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Climbable"))
        {
            climbingSurface = collision.gameObject;
            isClimbing = true;
            _rigidbody.useGravity = false;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject == climbingSurface)
        {
            StopClimbing();
        }
    }

    private void JumpOffWall()
    {
        _rigidbody.AddForce(-transform.forward * 500 + Vector3.up * 300);
        StopClimbing();
    }

    private void StopClimbing()
    {
        isClimbing = false;
        _rigidbody.useGravity = true;
        climbingSurface = null;
    }
}

