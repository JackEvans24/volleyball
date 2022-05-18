using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class FireBall : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform court;
    [SerializeField] private PlayerController[] players;
    [SerializeField] private Transform targetIndicator;
    [SerializeField] private Transform setPosition;

    [Header("Ball passing")]
    [SerializeField] private Vector2 fireAngleBounds;
    [SerializeField] private float movementLeading;

    private Rigidbody rb;
    private float gravity;

    private Vector3 minPoint;
    private Vector3 maxPoint;
    private float maxDistance;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        gravity = Physics.gravity.magnitude;

        CalculateMaxDistance();
    }

    private void CalculateMaxDistance()
    {
        minPoint = new Vector3(court.position.x - (court.lossyScale.x / 2), 0, court.position.x - (court.lossyScale.x / 2));
        maxPoint = new Vector3(court.position.x + (court.lossyScale.x / 2), 0, court.position.x + (court.lossyScale.x / 2));

        maxDistance = Vector3.Distance(minPoint, maxPoint);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            ResetPlayerTargets();

            var target = GetTarget(collision.gameObject);

            targetIndicator.position = new Vector3(target.x, targetIndicator.position.y, target.z);

            FireAtTarget(target);
        }
    }

    private void ResetPlayerTargets()
    {
        foreach (var player in players)
        {
            player.MoveTowards = null;
        }
    }

    private Vector3 GetTarget(GameObject currentCollision)
    {
        // Get target player
        var targetPlayer = players
            .Where(t => t.gameObject != currentCollision)
            .OrderBy(t => Random.Range(0, 1))
            .First();

        // Return set position if setting
        var collisionPlayer = currentCollision.GetComponent<PlayerController>();
        if (Input.GetButton("Set"))
        {
            var setTarget = setPosition.position;
            targetPlayer.MoveTowards = setTarget;
            return setTarget;
        }

        var playerRb = targetPlayer.GetComponent<Rigidbody>();

        // Set as target current position + offset for current movement
        var movementOffset = playerRb.velocity * movementLeading;
        var target = playerRb.position + movementOffset;

        // Clamp to boundaries
        target = Vector3.Min(target, maxPoint);
        target = Vector3.Max(target, minPoint);

        targetPlayer.MoveTowards = target;
        return target;
    }

    private void FireAtTarget(Vector3 target)
    {
        // Selected angle in radians
        float angle = GetFireAngle(target);

        // Positions of this object and the target on the same plane
        Vector3 planarTarget = new Vector3(target.x, 0, target.z);
        Vector3 planarPostion = new Vector3(transform.position.x, 0, transform.position.z);

        // Planar distance between objects
        float distance = Vector3.Distance(planarTarget, planarPostion);
        // Distance along the y axis between objects
        float yOffset = transform.position.y - target.y;

        // Get velocity
        float initialVelocity = (1 / Mathf.Cos(angle)) * Mathf.Sqrt((0.5f * gravity * Mathf.Pow(distance, 2)) / (distance * Mathf.Tan(angle) + yOffset));
        Vector3 velocity = new Vector3(0, initialVelocity * Mathf.Sin(angle), initialVelocity * Mathf.Cos(angle));

        // Rotate our velocity to match the direction between the two objects
        float angleBetweenObjects = Vector3.SignedAngle(Vector3.forward, planarTarget - planarPostion, Vector3.up);
        Vector3 finalVelocity = Quaternion.AngleAxis(angleBetweenObjects, Vector3.up) * velocity;

        // Fire!
        rb.velocity = finalVelocity;
    }

    private float GetFireAngle(Vector3 target)
    {
        // Get distance between ball and target
        var targetDistance = Vector3.Distance(
            new Vector3(target.x, 0, target.z),
            new Vector3(transform.position.x, 0, transform.position.z)
        );

        // Lerp between angle boundaries
        var fireAngle = Mathf.Lerp(fireAngleBounds.y, fireAngleBounds.x, targetDistance / maxDistance);
        return fireAngle * Mathf.Deg2Rad;
    }
}
