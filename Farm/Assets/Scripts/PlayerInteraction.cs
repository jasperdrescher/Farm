using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerInteraction : MonoBehaviour
{
	public float m_interactionTime = 1.0f; // length of the interaction progressbar.

	[SerializeField]
	private PlayerInteractionType m_interactionType = PlayerInteractionType.None;

	private MapGrid m_mapGrid = null;
	private MapTile m_ActiveTile = null;
	private PlayerInventory m_playerInventory = null;
	private Slider m_slider = null;
	private float m_interactionTimer = 0.0f;
	private bool m_interacting = false;
	private bool m_interacted = false;
	private Vector3 m_playerPositionCacheForActiveTile = Vector3.zero;

	private enum InteractionResult
	{
		None = 0,
		Success = 1,
		Invalid = 2,
		Cancelled = 3,
		Started = 4,
		Interrupted = 5,
    }

    void Start()
    {
		m_playerInventory = GetComponent<PlayerInventory>();
        m_slider = GetComponentInChildren<Slider>();
		m_slider.gameObject.SetActive(false);
    }

    void Update()
    {
		if (m_interacting && !m_interacted)
		{
			m_interactionTimer += Time.deltaTime;

            m_slider.value = GetInteractionProgress();
			m_slider.transform.parent.transform.LookAt(Camera.main.transform.position);
			m_slider.transform.parent.transform.rotation *= Quaternion.Euler(0f, 180f, 0f);

            if (m_interactionTimer >= m_interactionTime)
			{
				m_interacted = true;

				FarmingTools.Tool tool = m_playerInventory.GetCurrentTool();
				m_mapGrid.PlayerInteractionFinished(tool, transform.position);

				InputInteractionSucceeded();
			}
		}

		UpdateActiveTile();
	}

	void UpdateActiveTile()
	{ 
		Vector3 pos = transform.position;

		if (m_playerPositionCacheForActiveTile == pos)
			return;

		if (Vector3.Distance(pos, m_playerPositionCacheForActiveTile) < 0.5f)
			return;

		m_playerPositionCacheForActiveTile = pos;

		MapTile prevTile = m_ActiveTile;

		if (EnsureMapGrid())
			m_ActiveTile = m_mapGrid.GetTileAtPos(pos);

		if (prevTile != m_ActiveTile)
			OnActiveTileChanged(m_ActiveTile);
	}

	private void OnActiveTileChanged(MapTile tile)
	{
		FindFirstObjectByType<InventoryPanel>().HighlightAvailableTools(tile);
	}

	public bool IsInteracting()
	{ 
		return m_interacting;
	}

	public float GetInteractionProgress()
	{ 
		return m_interacting ? m_interactionTimer / m_interactionTime : 0.0f;
	}

	private void ResetInteraction()
	{
		m_interacting = false;
		m_interacted = false;
		m_interactionTimer = 0.0f;
        m_slider.gameObject.SetActive(false);
		m_interactionType = PlayerInteractionType.None;
    }

    private void InputInteractionStarted()
    {
        m_interacting = true;
        m_slider.gameObject.SetActive(true);
        InteractionResult interactionResult = InteractionResult.Started;
        Debug.Log("Input Interaction of type " + m_interactionType + " has result " + interactionResult.ToString());
    }

    private void InputInteractionCancelled()
    {
        InteractionResult interactionResult = InteractionResult.Cancelled;
        Debug.Log("Input Interaction of type " + m_interactionType + " had result " + interactionResult.ToString());
        ResetInteraction();
    }

    private void InputInteractionInterrupted()
    {
        InteractionResult interactionResult = InteractionResult.Interrupted;
        Debug.Log("Input Interaction of type " + m_interactionType + " had result " + interactionResult.ToString());
        ResetInteraction();
    }

    private void InputInteractionFailed()
	{
        m_interacted = true;
        InteractionResult interactionResult = InteractionResult.Invalid;
        Debug.LogWarning("Input Interaction of type " + m_interactionType + " had result " + interactionResult.ToString());
        ResetInteraction();
    }

    private void InputInteractionSucceeded()
    {
        InteractionResult interactionResult = InteractionResult.Success;
        Debug.Log("Input Interaction of type " + m_interactionType + " had result " + interactionResult.ToString());
        ResetInteraction();
    }

    public void InputInteract(InputAction.CallbackContext callbackContext)
	{
		if (callbackContext.canceled)
		{
			if (m_interacting)
			{
				InputInteractionCancelled();
			}

			return;
		}

		if (!m_interacted && !m_interacting)
		{
			if (EnsureMapGrid())
			{
				MapTile tile = m_mapGrid.GetTileAtPos(transform.position);
                if (tile == null)
				{
					InputInteractionFailed();

					return;
                }

				FarmingTools.Tool currentTool = m_playerInventory.GetCurrentTool();
				m_interactionType = tile.GetPlayerInteractionType(currentTool);
                switch (m_interactionType)
                {
					case PlayerInteractionType.None:
						InputInteractionFailed();
						break;
                    case PlayerInteractionType.Tile:
                        foreach (ToolData tooldata in m_playerInventory.m_toolDataObjects)
                        {
                            if (tooldata.m_tool == currentTool)
                            {
                                m_interactionTime = tooldata.m_interactionTime;
                                break;
                            }
                        }

                        break;
					case PlayerInteractionType.Crop:
                        m_interactionTime = tile.GetTimeForCropStep();
                        break;
					case PlayerInteractionType.PlantCrop:
                        m_interactionTime = 0f;
						break;
                }

				InputInteractionStarted();
            }
            else
			{
                InputInteractionFailed();
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

	public void PlayerEquippedTool()
	{
		if (!m_interacting)
			return;

		InputInteractionInterrupted();
    }
}
