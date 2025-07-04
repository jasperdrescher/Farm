using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

public class PlayerCharacterController : MonoBehaviour
{
	public float m_rotationSpeed = 10f;
	public float m_moveSpeed = 4f;
	public float m_runSpeedMultiplier = 1.5f;

	[SerializeField]
	private Vector3 m_moveDirection;

	[SerializeField]
	private float m_runAnimSpeed = 0.8f;

	private Animator m_animator;
	private float m_currentRunSpeed;
	private bool m_canMove = true;

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		m_animator = GetComponent<Animator>();
		m_currentRunSpeed = 1f;
	}

	// Update is called once per frame
	void Update()
	{
		if (m_moveDirection.magnitude != 0.0f)
		{
			var targetRotation = Quaternion.LookRotation(m_moveDirection);
			var playerRotation = Quaternion.Slerp(transform.rotation, targetRotation, m_rotationSpeed * Time.deltaTime);
			transform.rotation = playerRotation;
			transform.position += transform.forward * ((m_moveSpeed * m_currentRunSpeed) * Time.deltaTime);
		}
	}

	public void InputMove(InputAction.CallbackContext callbackContext)
	{
		if (!m_canMove) // see comment in InputInteract
		{
			return;
		}

		if (callbackContext.canceled)
		{
			m_animator.SetBool("IsWalking", false);
			m_moveDirection = Vector3.zero;
			return;
		}

		m_animator.SetBool("IsWalking", true);
		var input = callbackContext.ReadValue<Vector2>();
		m_moveDirection = new Vector3(input.x * -1f, 0f, input.y * -1f);
	}

	public void InputRun(InputAction.CallbackContext callbackContext)
	{
		if (callbackContext.canceled)
		{
			m_animator.SetBool("IsRunning", false);
			m_currentRunSpeed = 1f;
			m_animator.speed = 1f;
			return;
		}

		m_animator.SetBool("IsRunning", true);
		m_currentRunSpeed = m_runSpeedMultiplier;
		m_animator.speed = m_runAnimSpeed;
	}

	public void InputAttack(InputAction.CallbackContext callbackContext)
	{
		if (callbackContext.canceled)
		{
			m_animator.SetBool("IsAttacking", false);
			return;
		}

		m_animator.SetBool("IsAttacking", true);
	}

	public void InputInteract(InputAction.CallbackContext callbackContext)
	{
		if (callbackContext.canceled)
		{
			m_canMove = true;
			return;
		}

		// this is too hacky: does not stops the animations + have to restore movement after stopped interacting
		m_moveDirection = new Vector3(0f, 0f, 0f);
		m_canMove = false;
	}
}