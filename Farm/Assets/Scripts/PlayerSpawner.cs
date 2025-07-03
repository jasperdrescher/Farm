using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject m_playerPrefab;

    private float m_arrowLength = 2f;
    private Color m_arrowColor = Color.red;
    private float m_capsuleHeight = 2.18f;
    private float m_capsuleRadius = 0.5f;

    void Start()
    {
        Instantiate(m_playerPrefab, transform.position, transform.rotation);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = m_arrowColor;

        // Draw forward arrow
        Vector3 start = transform.position;
        Vector3 direction = transform.forward * m_arrowLength;

        Gizmos.DrawRay(start, direction);

        Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 150f, 0) * Vector3.forward;
        Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(0, -150f, 0) * Vector3.forward;
        Gizmos.DrawRay(start + direction, right * 0.3f);
        Gizmos.DrawRay(start + direction, left * 0.3f);

        // Draw capsule
        Vector3 up = transform.up;
        Vector3 position = transform.position;

        float cylinderHeight = Mathf.Max(0f, m_capsuleHeight - 2f * m_capsuleRadius);

        Vector3 topSphereCenter = position + up * (cylinderHeight / 2f + m_capsuleRadius);
        Vector3 bottomSphereCenter = position - up * (cylinderHeight / 2f + m_capsuleRadius);

        Gizmos.DrawWireSphere(topSphereCenter, m_capsuleRadius);
        Gizmos.DrawWireSphere(bottomSphereCenter, m_capsuleRadius);

        Vector3 capsuleRight = transform.right * m_capsuleRadius;
        Vector3 capsuleForward = transform.forward * m_capsuleRadius;

        Gizmos.DrawLine(topSphereCenter + capsuleRight, bottomSphereCenter + capsuleRight);
        Gizmos.DrawLine(topSphereCenter - capsuleRight, bottomSphereCenter - capsuleRight);
        Gizmos.DrawLine(topSphereCenter + capsuleForward, bottomSphereCenter + capsuleForward);
        Gizmos.DrawLine(topSphereCenter - capsuleForward, bottomSphereCenter - capsuleForward);
    }
}
