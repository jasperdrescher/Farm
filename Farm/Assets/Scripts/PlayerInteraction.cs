using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerInteraction : MonoBehaviour
{
	public float m_interactionTime = 1.0f; // length of the interaction progressbar.

	private MapGrid m_mapGrid = null;
	private MapTile m_ActiveTile = null;
	private PlayerInventory m_playerInventory = null;
	private Slider m_slider = null;
	private float m_interactionTimer = 0.0f;
	private bool m_interacting = false;
	private bool m_interacted = false;
	private Vector3 m_playerPositionCacheForActiveTile = Vector3.zero;
	private Animator m_animator;
	private InteractionContext m_interactionContext = null;

	void Start()
    {
		m_animator = GetComponent<Animator>();
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

				if (EnsureMapGrid())
				{
					InteractionResult result = m_mapGrid.Interact(m_interactionContext);
					ProcessInteractionResult(result);
					OnActiveTileChanged(m_ActiveTile);
					Reset();
				}
				else
				{
					Debug.LogWarning("Can't find MapGrid for Interaction!");
				}
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
		if (tile != null)
		{
			InventoryPanel panel = FindFirstObjectByType<InventoryPanel>();
			if(panel != null)
				panel.HighlightAvailableTools(tile);
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
        m_slider.gameObject.SetActive(false);
		m_interactionContext = null;
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
			m_interactionContext = CreateInteractionContext();

			FarmingTools.Tool tool = m_playerInventory.GetCurrentTool();
			if (EnsureMapGrid() && m_mapGrid.HasValidInteraction(m_interactionContext))
			{
                foreach (ToolData tooldata in m_playerInventory.m_toolDataObjects)
				{
					if (tooldata.m_tool == tool)
					{
                        m_interactionTime = tooldata.m_interactionTime;
						break;
                    }
				}

				m_slider.gameObject.SetActive(true);

				m_interactionTimer = 0.0f;

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

	private InteractionContext CreateInteractionContext()
	{
		InteractionContext interactionContext = ScriptableObject.CreateInstance<InteractionContext>();
		interactionContext.m_position = transform.position;	
		interactionContext.m_tool = m_playerInventory.GetCurrentTool();

		switch (interactionContext.m_tool)
		{
			case FarmingTools.Tool.Hoe:
			case FarmingTools.Tool.Shovel:
			case FarmingTools.Tool.Sickle:
				break;
			case FarmingTools.Tool.WateringPot:
				{
					/*todo, use real inventory items from player inventory*/
					interactionContext.m_inventoryItem = ScriptableObject.CreateInstance<InventoryItem>();
					interactionContext.m_inventoryItem.m_itemType = InventoryItem.Type.Resource;
					interactionContext.m_inventoryItem.m_resourceType = ResourceTypes.Enum.Water;
					interactionContext.m_inventoryItem.m_amount = 10000; // add how much water we have
				}
				break;
			case FarmingTools.Tool.PlantingTool:
				{
					/*todo, use real inventory items from player inventory*/
					interactionContext.m_inventoryItem = ScriptableObject.CreateInstance<InventoryItem>();
					interactionContext.m_inventoryItem.m_itemType = InventoryItem.Type.Seed;
					interactionContext.m_inventoryItem.m_cropType = CropTypes.Enum.Potato;
					interactionContext.m_inventoryItem.m_amount = 10000; // add how many seeds we have
				}
				break;
		}

		return interactionContext;
	}

	private void ProcessInteractionResult(InteractionResult interactionResult)
	{
		if (interactionResult == null)
		{
			Debug.LogError("Interaction result is null");
			return;
		}

		if (interactionResult.m_result == false)
		{
			Debug.LogWarning("Interaction failed.");
			return;
		}

		if (interactionResult is CropInteractionResult)
		{
			// add rewards to inventory
			CropInteractionResult result = interactionResult as CropInteractionResult;
			// result.m_reward;

			string log = "Gained: \n";
			foreach(InventoryItem item in result.m_reward)
			{
				log += DebugLogItem(item);
			}
			Debug.Log(log);
		}
		else if (interactionResult is TileInteractionResult)
		{
			// remove items from inventory
			TileInteractionResult result = interactionResult as TileInteractionResult;
			// result.m_consumed

			string log = "Consumed: \n";
			foreach (InventoryItem item in result.m_consumed)
			{
				log += DebugLogItem(item);
			}
			Debug.Log(log);
		}
	}

	string DebugLogItem(InventoryItem item)
	{
		switch (item.m_itemType)
		{
			case InventoryItem.Type.Seed:
			case InventoryItem.Type.Crop:
				return new string(item.m_itemType + " (" + item.m_cropType + "): " + item.m_amount + "\n");
			case InventoryItem.Type.Resource:
			case InventoryItem.Type.Currency:
			case InventoryItem.Type.PlayerResource:
				return new string(item.m_itemType + " (" + item.m_resourceType + "): " + item.m_amount + "\n");
		}

		return "";
	}
}
