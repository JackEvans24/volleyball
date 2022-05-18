using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GroundCheck groundCheck;
    [SerializeField] private GameObject activeIndicator;

    [Header("Active movement")]
    [SerializeField] private float speed;
    [SerializeField] private float smoothing;
    [SerializeField] private float jumpVelocity;

    [Header("Inactive movement")]
    [SerializeField] private Transform inactivePosition;
    [SerializeField] private float inactiveSpeed;

    public bool IsActivePlayer;
    public Vector3? MoveTowards;

    private Rigidbody rb;
    private float horizontal, vertical;
    private bool jump;
    private Vector3 currentVelocity;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (IsActivePlayer)
        {
            horizontal = Input.GetAxis($"Horizontal") * speed;
            vertical = Input.GetAxis($"Vertical") * speed;

            jump |= groundCheck.IsGrounded && Input.GetButtonDown("Jump");
        }
        else
        {
            horizontal = vertical = 0;
        }
    }

    private void FixedUpdate()
    {
        if (IsActivePlayer)
        {
            if (jump)
            {
                rb.AddForce(Vector3.up * jumpVelocity);
                jump = false;
            }

            var targetVelocity = new Vector3(horizontal, rb.velocity.y, vertical);
            rb.velocity = Vector3.SmoothDamp(rb.velocity, targetVelocity, ref currentVelocity, smoothing);
        }
        else if (MoveTowards.HasValue)
        {
            var targetVelocity = (MoveTowards.Value - transform.position).normalized * speed;
            rb.velocity = Vector3.SmoothDamp(rb.velocity, targetVelocity, ref currentVelocity, smoothing);
        }
        else
        {
            var targetVelocity = (inactivePosition.position - transform.position).normalized * speed;
            rb.velocity = Vector3.SmoothDamp(rb.velocity, targetVelocity, ref currentVelocity, smoothing);
        }
    }

    public void SetActivePlayer(bool active)
    {
        this.IsActivePlayer = active;
        this.activeIndicator.SetActive(active);
    }
}
