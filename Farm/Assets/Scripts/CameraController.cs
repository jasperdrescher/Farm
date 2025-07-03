using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private Vector3 m_offset = new Vector3(0f, 12f, 10f);

    [SerializeField]
    private float m_followSpeed = 5f;

    [SerializeField]
    private Vector3 m_rotation = new Vector3(50f, 180f, 0f);

    private Transform m_playerTransform;
    private Quaternion m_fixedRotation;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        m_playerTransform = GameObject.FindWithTag("Player").transform;
        if (m_playerTransform == null)
        {
            Debug.LogError("Failed to find player");
        }

        m_fixedRotation = Quaternion.Euler(m_rotation);
    }

    void LateUpdate()
    {
        if (m_playerTransform == null)
            return;

        Vector3 desiredPosition = m_playerTransform.position + m_offset;
        transform.position = Vector3.Lerp(transform.position, desiredPosition, m_followSpeed * Time.deltaTime);
        transform.rotation = m_fixedRotation;
    }
}
