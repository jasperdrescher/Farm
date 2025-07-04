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

    void Start()
    {
        m_playerInventory = transform.gameObject.GetComponent<PlayerInventory>();
    }

    void Update()
    {
		if (m_interacting && !m_interacted)
		{
			m_interactionTimer += Time.deltaTime;

			if (m_interactionTimer >= m_interactionTime)
			{
				m_interacted = true;

				Debug.Log("Interaction Completed!");

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
			Debug.Log("Interaction Ended!");
			return;
		}

		if (!m_interacted && !m_interacting)
		{
			m_interactionTimer = 0.0f;
			
			// todo: get interaction length from tool -> m_interactionTime

			m_interacting = true;

			Debug.Log("Interaction Started!");
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
