using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
	public float m_interactionTime = 1.0f; // length of the interaction progressbar.

	private MapGrid m_mapGrid = null;
	private PlayerInventory m_playerInventory = null;
	private float m_interactionTimer = 0.0f;
	private bool m_interacting = false;
	private bool m_interacted = false;
	private Animator m_animator;

	void Start()
    {
		m_animator = GetComponent<Animator>();
		m_playerInventory = GetComponent<PlayerInventory>();
    }

    void Update()
    {
		if (m_interacting && !m_interacted)
		{
			m_interactionTimer += Time.deltaTime;

			if (m_interactionTimer >= m_interactionTime)
			{
				m_interacted = true;

				if (EnsureMapGrid())
				{
					FarmingTools.Tool tool = m_playerInventory.GetCurrentTool();
					m_mapGrid.Interact(tool);
				}
				else
				{
					Debug.LogWarning("Can't find MapGrid for Interaction!");
				}
			}
		}
    }

	public bool IsInteracting()
	{ 
		return m_interacting;
	}

	public float GetInteractionProgress()
	{ 
		return m_interacting ? m_interactionTimer / m_interactionTime : 0.0f;
	}

	private void Reset()
	{
		m_interacting = false;
		m_interacted = false;
		m_interactionTimer = 0.0f;
	}

	public void InputInteract(InputAction.CallbackContext callbackContext)
	{
		if (callbackContext.canceled)
		{
			Reset();
			return;
		}

		if (!m_interacted && !m_interacting)
		{
			FarmingTools.Tool tool = m_playerInventory.GetCurrentTool();
			if (EnsureMapGrid() && m_mapGrid.HasValidInteraction(tool))
			{

				m_interactionTimer = 0.0f;

				// todo: get interaction length from tool -> m_interactionTime

				m_interacting = true;
			}
			else
			{ 
				// invalid interaction
				m_interacted = true;
				Debug.LogWarning("Invalid Interaction: Can't do anything with current tool on active tile.");
			}
		}
	}

	private bool EnsureMapGrid()
	{
		if (m_mapGrid == null)
		{
			GameObject go = GameObject.FindGameObjectWithTag("MapGrid");
			if (go != null)
			{
				m_mapGrid = go.GetComponent<MapGrid>();
			}
		}

		return m_mapGrid != null;
	}
}
