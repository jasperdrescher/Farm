using UnityEngine;
using System.Collections.Generic;
using System;

[ExecuteInEditMode]
public class MapTile : MonoBehaviour
{
	[Header("Setup")]
	public List<TileData> m_tileTypes;
	public GameObject m_cropPrefab;

	[Header("Editor")]
	public TileTypes.Enum m_editorTileTypeChanger = TileTypes.Enum.None;

	#region private
	private MapGrid m_ownerGrid = null;
	private Crop m_crop = null;
	private GameObject m_cropGameObject = null;
	private TileTypes.Enum m_currentTileType = TileTypes.Enum.None;
	private Dictionary<TileTypes.Enum, GameObject> m_spawnedTiles = new Dictionary<TileTypes.Enum, GameObject>();
	private int m_index = 0;
	#endregion

	[Serializable]
	public class SaveData
	{
		public int m_index;
		public TileTypes.Enum m_tileType;
		public Crop.SaveData m_crop;
	};

	void Start()
	{
	}

	public void Init(int index, MapGrid owner, TileTypes.Enum type)
	{
		m_index = index;
		m_ownerGrid = owner;
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
#endif
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

	public PlayerInteractionType GetPlayerInteractionType(FarmingTools.Tool tool)
	{
        if (!HasValidInteraction(tool))
            return PlayerInteractionType.None;

        switch (tool)
        {
            case FarmingTools.Tool.Shovel:
				return PlayerInteractionType.Tile;
            case FarmingTools.Tool.Hoe:
            case FarmingTools.Tool.WateringPot:
            case FarmingTools.Tool.Sickle:
				return PlayerInteractionType.Crop;
            case FarmingTools.Tool.PlantingTool:
				return PlayerInteractionType.PlantCrop;
        }

        return PlayerInteractionType.None;
	}

	public void PlayerInteractionFinished(FarmingTools.Tool tool)
	{
		switch (tool)
		{
			case FarmingTools.Tool.None:
				break;
            case FarmingTools.Tool.Shovel:
				ChangeTileType(TileTypes.Enum.FarmField);
				break;
			case FarmingTools.Tool.Hoe:
			case FarmingTools.Tool.WateringPot:
			case FarmingTools.Tool.Sickle:
				m_crop.ChangeCrop(tool);
				break;
			case FarmingTools.Tool.PlantingTool:
				m_crop.PlantCrop(CropTypes.Enum.Potato); // [FIXME] selectable crop type
				break;
		}
	}

	public float GetTimeForCropStep()
	{
		return m_crop.GetTimeForCropStep();
	}
	public bool HasValidInteraction(FarmingTools.Tool tool)
	{
		switch (tool)
		{
			case FarmingTools.Tool.Shovel:
				return m_currentTileType == TileTypes.Enum.Grass;
			case FarmingTools.Tool.Hoe:
			case FarmingTools.Tool.WateringPot:
			case FarmingTools.Tool.Sickle:
				return m_currentTileType == TileTypes.Enum.FarmField && m_crop.HasValidInteraction(tool);
			case FarmingTools.Tool.PlantingTool:
				return m_currentTileType == TileTypes.Enum.FarmField && !m_crop.HasAnythingPlanted();
			default:
				break;
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
	public void OverrideCrop(CropTypes.Enum type, int step)
	{
		if (!m_crop)
			return;

		m_crop.ChangeCropType(type);
		m_crop.ChangeCropStep(step);
	}

	public bool SaveState(MapTile.SaveData data)
	{
		data.m_index = m_index;
		data.m_tileType = m_currentTileType;

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
		ChangeTileType(data.m_tileType);

		bool result = m_crop.LoadState(data.m_crop);
		if (!result)
		{
			Debug.LogError("Failed to Load Crop for Tile #" + m_index);
		}

		return true;
	}
}
