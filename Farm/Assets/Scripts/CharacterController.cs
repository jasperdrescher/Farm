using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterController : MonoBehaviour
{
    public float rotationSpeed = 10f;
    public float moveSpeed = 5f;

    [SerializeField]
    private Vector3 moveDirection;

    private Animator animator;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (moveDirection.magnitude != 0.0f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            Quaternion playerRotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            transform.rotation = playerRotation;
            transform.position += transform.forward * moveSpeed * Time.deltaTime;
        }
    }

    public void InputMove(InputAction.CallbackContext callbackContext)
    {
        if (callbackContext.canceled)
        {
            animator.SetBool("IsWalking", false);
            moveDirection = Vector3.zero;
            return;
        }

        animator.SetBool("IsWalking", true);
        Vector2 input = callbackContext.ReadValue<Vector2>();
        moveDirection = new Vector3(input.x * -1f, 0f, input.y * -1f);
    }
}
