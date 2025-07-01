using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject playerPrefab;

    private float arrowLength = 2f;
    private Color arrowColor = Color.red;
    private float capsuleHeight = 2.18f;
    private float capsuleRadius = 0.5f;

    void Start()
    {
        Instantiate(playerPrefab, transform.position, transform.rotation);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = arrowColor;

        // Draw forward arrow
        Vector3 start = transform.position;
        Vector3 direction = transform.forward * arrowLength;

        Gizmos.DrawRay(start, direction);

        Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 150f, 0) * Vector3.forward;
        Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(0, -150f, 0) * Vector3.forward;
        Gizmos.DrawRay(start + direction, right * 0.3f);
        Gizmos.DrawRay(start + direction, left * 0.3f);

        // Draw capsule
        Vector3 up = transform.up;
        Vector3 position = transform.position;

        float cylinderHeight = Mathf.Max(0f, capsuleHeight - 2f * capsuleRadius);

        Vector3 topSphereCenter = position + up * (cylinderHeight / 2f + capsuleRadius);
        Vector3 bottomSphereCenter = position - up * (cylinderHeight / 2f + capsuleRadius);

        Gizmos.DrawWireSphere(topSphereCenter, capsuleRadius);
        Gizmos.DrawWireSphere(bottomSphereCenter, capsuleRadius);

        Vector3 capsuleRight = transform.right * capsuleRadius;
        Vector3 capsuleForward = transform.forward * capsuleRadius;

        Gizmos.DrawLine(topSphereCenter + capsuleRight, bottomSphereCenter + capsuleRight);
        Gizmos.DrawLine(topSphereCenter - capsuleRight, bottomSphereCenter - capsuleRight);
        Gizmos.DrawLine(topSphereCenter + capsuleForward, bottomSphereCenter + capsuleForward);
        Gizmos.DrawLine(topSphereCenter - capsuleForward, bottomSphereCenter - capsuleForward);
    }
}
