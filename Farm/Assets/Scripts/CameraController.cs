using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private Vector3 offset = new Vector3(0f, 12f, 10f);

    [SerializeField]
    private float followSpeed = 5f;

    [SerializeField]
    private Vector3 rotation = new Vector3(50f, 180f, 0f);

    private Transform playerTransform;
    private Quaternion fixedRotation;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerTransform = GameObject.FindWithTag("Player").transform;
        if (playerTransform == null)
        {
            Debug.LogError("Failed to find player");
        }

        fixedRotation = Quaternion.Euler(rotation);
    }

    void LateUpdate()
    {
        if (playerTransform == null)
            return;

        Vector3 desiredPosition = playerTransform.position + offset;
        transform.position = Vector3.Lerp(transform.position, desiredPosition, followSpeed * Time.deltaTime);
        transform.rotation = fixedRotation;
    }
}
