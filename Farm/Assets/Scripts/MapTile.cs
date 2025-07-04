using UnityEngine;
using System.Collections.Generic;

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
	#endregion

	void Start()
	{
	}

	public void Init(MapGrid owner, TileTypes.Enum type)
	{
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

		Vector2 tileSize = m_ownerGrid.m_tileSize;
		Vector3 p1 = transform.position + new Vector3(-tileSize.x, 0.1f, -tileSize.y);
		Vector3 p2 = transform.position + new Vector3(-tileSize.x, 0.1f, tileSize.y);
		Vector3 p3 = transform.position + new Vector3(tileSize.x, 0.1f, tileSize.y);
		Vector3 p4 = transform.position + new Vector3(tileSize.x, 0.1f, -tileSize.y);
		Debug.DrawLine(p1, p2, Color.magenta, Time.deltaTime, false);
		Debug.DrawLine(p2, p3, Color.magenta, Time.deltaTime, false);
		Debug.DrawLine(p3, p4, Color.magenta, Time.deltaTime, false); 
		Debug.DrawLine(p4, p1, Color.magenta, Time.deltaTime, false);
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

		if (m_currentTileType != TileTypes.Enum.None && m_spawnedTiles.ContainsKey(m_currentTileType))
		{
			m_spawnedTiles[m_currentTileType].SetActive(true);

			if (m_cropGameObject)
				m_cropGameObject.transform.position = transform.position + new Vector3(0.0f, GetTileHeight(), 0.0f);
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.CompareTag("Player"))
		{
			if (m_ownerGrid)
				m_ownerGrid.SetActiveTile(this);

			Debug.Log("Active Tile: " + transform.position.x + ";" + transform.position.z);
		}
	}

	// todo: add an enum value as parameter for the current used tool
	public void Interact(FarmingTools.Tool tool)
	{
		// just a testing

		if (m_currentTileType == TileTypes.Enum.Grass)
		{
			ChangeTileType(TileTypes.Enum.FarmField);
		}
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

	// todo: remove this, just using for the quick presentation
	public void OverrideCrop(CropTypes.Enum type, int step)
	{
		if (!m_crop)
			return;

		m_crop.ChangeCropType(type);
		m_crop.ChangeCropStep(step);
	}
}
