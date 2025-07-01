using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class CharacterController : MonoBehaviour
{
    public float rotationSpeed = 10.0f;
    public float moveSpeed = 5.0f;

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
            Debug.Log(playerRotation);
            transform.rotation = playerRotation;
            var translation = moveDirection * (moveSpeed * Time.deltaTime);
            transform.Translate(translation);
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
        Debug.Log(input);
        moveDirection = new Vector3(input.x * -1.0f, 0.0f, input.y);
    }
}
