using UnityEngine;

public class SpidermanSwing : MonoBehaviour
{
    [SerializeField] private LineRenderer webLine;
    [SerializeField] private Transform handPosition;
    [SerializeField] private float swingForce = 35f;
    [SerializeField] private float damping = 20f;
    [SerializeField] private float maxSwingDistance = 75f;
    
    private SpringJoint _joint;
    private RaycastHit _hit;
    private Rigidbody _rigidbody;
    private bool isSwinging;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    //private void Update()
    //{
    //    HandleWebSwinging();
    //}

    //private void LateUpdate()
    //{
    //    DrawWeb();
    //}

    public void HandleWebSwinging()
    {
        if (Input.GetMouseButtonDown(0) && !isSwinging) // Left-click to swing
        {
            AttemptSwing();
        }
        else if (Input.GetMouseButtonUp(0))
        {
            ReleaseSwing();
        }
    }

    private void AttemptSwing()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out _hit, maxSwingDistance))
        {
            if (_hit.point.y > transform.position.y) // Ensures attachment above the player
            {
                StartSwinging();
            }
        }
    }

    private void StartSwinging()
    {
        isSwinging = true;
        _rigidbody.linearVelocity = Vector3.zero;
        _rigidbody.useGravity = false;

        _joint = gameObject.AddComponent<SpringJoint>();
        _joint.autoConfigureConnectedAnchor = false;
        _joint.connectedAnchor = _hit.point;
        _joint.maxDistance = Vector3.Distance(transform.position, _hit.point) * 0.7f;
        _joint.minDistance = _joint.maxDistance * 0.3f;
        _joint.spring = swingForce;
        _joint.damper = damping;

        Invoke("ReleaseSwing", 2f); // Auto-release after 2 seconds
    }

    private void ReleaseSwing()
    {
        isSwinging = false;
        _rigidbody.useGravity = true;
        Destroy(_joint);
    }

    public void DrawWeb()
    {
        if (isSwinging)
        {
            webLine.positionCount = 2;
            webLine.SetPosition(0, handPosition.position);
            webLine.SetPosition(1, _hit.point);
        }
        else
        {
            webLine.positionCount = 0;
        }
    }
}
