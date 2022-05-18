using System;
using UnityEngine;

public class GroundCheck : MonoBehaviour
{
    [SerializeField] private float checkDistance;
    [SerializeField] private LayerMask groundLayers;

    [NonSerialized] public bool IsGrounded;

    private void Update()
    {
        IsGrounded = Physics.Raycast(transform.position, Vector3.down, checkDistance, groundLayers);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, transform.position + (Vector3.down * checkDistance));
    }
}
