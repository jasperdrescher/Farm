using UnityEngine;
using System.Collections.Generic;
using System;

[ExecuteInEditMode]
public class MapTile : MonoBehaviour
{
	[Header("Global Setup")]
	public List<TileData> m_tileTypes;
	public GameObject m_cropPrefab;

	[Header("Tile Config")]
	[Min(0f)] public float m_groundDryTime = 120.0f;
	public TileTypes.Enum[] m_DryStages;
	[Min(0f)] public float m_minimumWateringTreshold = 0.8f;
	public InventoryItem m_WaterInventoryItem = null;
	public int m_waterUnitConsumptionForFullWatering = 5;

	[Header("Editor")]
	public TileTypes.Enum m_editorTileTypeChanger = TileTypes.Enum.None;
	public float m_debug_ground_wetness = 0.0f;

	#region private
	private MapGrid m_ownerGrid = null;
	private Crop m_crop = null;
	private GameObject m_cropGameObject = null;
	private TileTypes.Enum m_currentTileType = TileTypes.Enum.None;
	private Dictionary<TileTypes.Enum, GameObject> m_spawnedTiles = new Dictionary<TileTypes.Enum, GameObject>();
	private int m_index = 0;
	private float m_timeSinceLastWatering = 0;
	#endregion

	[Serializable]
	public class SaveData
	{
		public int m_index;
		public TileTypes.Enum m_tileType;
		public Crop.SaveData m_crop;
		public float m_timeSinceLastWatering;
	};

	// I wanted to make the grid+tile system work from editor, and Start is not executed there...
	// That is why I use Init function that is called from the MapGrid
	void Start()
	{
	}

	public void Init(int index, MapGrid owner, TileTypes.Enum type)
	{
		m_index = index;
		m_ownerGrid = owner;
		m_timeSinceLastWatering = m_groundDryTime;
		CreateTileTypes();
		CreateCrop();

		ChangeTileType(type);
		m_editorTileTypeChanger = type;
	}

	private void CreateTileTypes()
	{
		foreach (TileData tileData in m_tileTypes)
		{
			if (tileData.m_tileType == TileTypes.Enum.None)
				continue;

			GameObject go = Instantiate(tileData.m_prefab, transform);
			go.SetActive(false);

			go.name = tileData.m_tileType.ToString();

			MeshRenderer meshRenderer = go.GetComponent<MeshRenderer>();
			if (meshRenderer != null)
				meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

			// make sure mesh renderer's shadow casting is off
			// we also have a box collider on the main prefab, so no need for them on the sub objects
			BoxCollider boxCollider = go.GetComponent<BoxCollider>();
			if (boxCollider != null)
				boxCollider.enabled = false;

			m_spawnedTiles.Add(tileData.m_tileType, go);	
		}
	}

	private void CreateCrop()
	{
		m_cropGameObject = Instantiate(m_cropPrefab, transform);
		m_crop = m_cropGameObject.GetComponent<Crop>();
		m_crop.Init(this);
	}

    void Update()
    {
#if UNITY_EDITOR
		EditorCheckValueChanges();
		m_debug_ground_wetness = GetGroundWaterLevel();
#endif

		HandleGroundDryness();
	}

#if UNITY_EDITOR
	void EditorCheckValueChanges()
	{
		if (m_editorTileTypeChanger != m_currentTileType)
		{
			ChangeTileType(m_editorTileTypeChanger);
		}
	}
#endif

	void ChangeTileType(TileTypes.Enum NewTileType)
	{
		if(m_currentTileType != TileTypes.Enum.None)
			m_spawnedTiles[m_currentTileType].SetActive(false);

		m_currentTileType = NewTileType;
		m_editorTileTypeChanger = m_currentTileType;

		if (m_currentTileType != TileTypes.Enum.None && m_spawnedTiles.ContainsKey(m_currentTileType))
		{
			m_spawnedTiles[m_currentTileType].SetActive(true);

			if (m_cropGameObject)
				m_cropGameObject.transform.position = transform.position + new Vector3(0.0f, GetTileHeight(), 0.0f);
		}
	}

	// todo: add an enum value as parameter for the current used tool
	public InteractionResult Interact(InteractionContext context)
	{
		if (!HasValidInteraction(context))
			return InteractionResult.Failure();

		switch (context.m_tool)
		{
			case FarmingTools.Tool.Shovel:
				{
					ChangeTileType(TileTypes.Enum.FarmField);
					return InteractionResult.Success();
				}
			case FarmingTools.Tool.Hoe:
			case FarmingTools.Tool.Sickle:
					return m_crop.Interact(context);
			case FarmingTools.Tool.WateringPot:
				{
					return WaterGround();
				}
			case FarmingTools.Tool.PlantingTool:
				{
					DryGound();
					return m_crop.PlantCrop(context.m_inventoryItem);
				}
		}

		return InteractionResult.Failure();
	}

	public bool CanUseTool(FarmingTools.Tool tool)
	{
		switch (tool)
		{
			case FarmingTools.Tool.Shovel:
				return m_currentTileType == TileTypes.Enum.Grass;
			case FarmingTools.Tool.Hoe:
			case FarmingTools.Tool.Sickle:
				return m_crop != null && m_crop.CanUseTool(tool);
			case FarmingTools.Tool.WateringPot:
				return m_crop != null && m_crop.HasAnythingPlanted() && CanWaterGround();
			case FarmingTools.Tool.PlantingTool:
				return m_currentTileType == TileTypes.Enum.FarmField && m_crop != null && !m_crop.HasAnythingPlanted();
			default:
				break;
		}

		return false;
	}

	public bool HasValidInteraction(InteractionContext context)
	{
		if (context != null)
		{
			switch (context.m_tool)
			{
				case FarmingTools.Tool.Shovel:
					return m_currentTileType == TileTypes.Enum.Grass;
				case FarmingTools.Tool.Hoe:
				case FarmingTools.Tool.Sickle:
					return m_crop != null && m_crop.HasValidInteraction(context);
				case FarmingTools.Tool.WateringPot:
					return m_crop != null && m_crop.HasAnythingPlanted() && CanWaterGround();
				case FarmingTools.Tool.PlantingTool:
					return m_currentTileType == TileTypes.Enum.FarmField && m_crop && !m_crop.HasAnythingPlanted();
				default:
					break;
			}
		}

		return false;
	}

	public int GetIndex()
	{
		return m_index;
	}

	public float GetTileHeight()
	{
		/*
		foreach (TileData tileData in m_tileTypes)
		{
			if (tileData.m_tileType == m_currentTileType)
				return tileData.m_tileHeight;
		}
		*/

		return 2.0f;
	}

	// [FIXME] this was made for map generation
	public void OverrideCrop(CropTypes.Enum type, float progress)
	{
		if (m_crop == null)
			return;

		m_crop.ChangeCropType(type);
		m_crop.ChangeCropProgress(progress);
	}

	public bool SaveState(MapTile.SaveData data)
	{
		data.m_index = m_index;
		data.m_tileType = m_currentTileType;

		data.m_timeSinceLastWatering = m_timeSinceLastWatering < m_groundDryTime ? m_timeSinceLastWatering : -1.0f;

		data.m_crop = new Crop.SaveData();

		bool result = m_crop.SaveState(data.m_crop);
		if (!result)
		{
			Debug.LogError("Failed to Save Crop for Tile #" + m_index);
		}
	
		return true;
	}

	public bool LoadState(MapTile.SaveData data)
	{
		m_timeSinceLastWatering = data.m_timeSinceLastWatering < 0 ? m_groundDryTime : data.m_timeSinceLastWatering;

		ChangeTileType(data.m_tileType);

		bool result = m_crop.LoadState(data.m_crop);
		if (!result)
		{
			Debug.LogError("Failed to Load Crop for Tile #" + m_index);
		}

		return true;
	}

	public bool IsGroundWet()
	{ 
		return GetGroundWaterLevel() > 0.0f;	
	}

	private float GetGroundWaterLevel()
	{
		float p = 1.0f - Mathf.Clamp(m_timeSinceLastWatering / m_groundDryTime, 0.0f, 1.0f);
		// 1 = wet, 0 dry
		return p;
	}

	private int GetRequiredWater()
	{ 
		float p = 1.0f - GetGroundWaterLevel();
		return Mathf.CeilToInt(p * m_waterUnitConsumptionForFullWatering);
	}

	private void HandleGroundDryness()
	{
		if (m_crop == null || !m_crop.HasAnythingPlanted())
			return;

		if(m_timeSinceLastWatering <= m_groundDryTime)
			m_timeSinceLastWatering += Time.deltaTime;

		float p = GetGroundWaterLevel();

		int stages = m_DryStages.Length;
		if (stages == 0)
			return; // nothing to do here...

		TileTypes.Enum targetTileType = m_DryStages[0];
		if (stages > 1)
		{
			float sections = 1.0f / (stages - 1);
			for (int i = 0; i < (stages - 1); i++)
			{
				if (p > (i * sections))
					targetTileType = m_DryStages[i + 1];
				else
					break;
			}
		}

		if (m_currentTileType == targetTileType)
			return;

		ChangeTileType(targetTileType);
	}

	private void DryGound()
	{
		m_timeSinceLastWatering = m_groundDryTime + 1.0f;
	}

	private TileInteractionResult WaterGround()
	{
		InventoryItem waterConsumption = ScriptableObject.Instantiate(m_WaterInventoryItem);
		waterConsumption.m_amount = GetRequiredWater();
		TileInteractionResult result = TileInteractionResult.SuccessWithConsume(waterConsumption);

		m_timeSinceLastWatering = 0.0f;

		int stages = m_DryStages.Length;
		if (stages > 0)
		{
			TileTypes.Enum wetGround = m_DryStages[stages - 1];
			if (m_currentTileType != wetGround)
				ChangeTileType(wetGround);
		}

		return result;
	}

	private bool CanWaterGround()
	{ 
		return GetGroundWaterLevel() < m_minimumWateringTreshold;
	}
}
