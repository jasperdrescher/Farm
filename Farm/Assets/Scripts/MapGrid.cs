using System.Collections.Generic;
using UnityEngine;
using static MapTile;
using static UnityEditor.PlayerSettings;

[ExecuteInEditMode]
public class MapGrid : MonoBehaviour
{
	[Header("Setup")]
	public GameObject m_tilePrefab;
	public Vector2 m_gridSize;
	public Vector2 m_tileSize;

	[Header("Generate From Image")]
	public Texture2D m_mapAsset;

	[System.Serializable]
	public class MapAssetColorTilePairs // todo: rename, it is not just a pair anymore
	{
		public Color m_pixelColor = Color.white;
		public TileTypes.Enum m_tileType = TileTypes.Enum.None;
		public CropTypes.Enum m_cropType = CropTypes.Enum.None;
		public int m_cropStep = 0;
	}

	public TileTypes.Enum m_defaultTileType;
	public MapAssetColorTilePairs[] m_mapAssetColorTileAssignment;

	[Header("Editor")]
	public bool m_editorGenerateGrid = false;

	#region private
	private MapTile m_currentActiveTile = null;
	#endregion

	///////////////////////////////////

	void Start()
    {
		// if the map generation was not enabled in the editor, we spawn tiles on startup
		if (transform.childCount > 0)
			return;

		GenerateGrid();
	}

	private void OnDisable()
	{
		// if the tiles were spawned runtime not from editor, despawn
		if (transform.childCount > 0 && m_editorGenerateGrid)
			Cleanup();
	}

	void Update()
    {
#if UNITY_EDITOR 
		// check if we want to spawn from editor
		if(Application.isPlaying == false)
			HandleEditorGridGeneration();
#endif
	}

#if UNITY_EDITOR
	// editor stuff
	void HandleEditorGridGeneration()
	{
		if (transform.childCount > 0 && m_editorGenerateGrid)
			return;

		if (m_editorGenerateGrid)
		{
			GenerateGrid();
		}
		else
		{
			Cleanup();
		}
	}
#endif

	private Color GetPixel(Color[] Pixels, int x, int y, int h, int w) 
	{ 
		if(x >= w || y >= h)
			return Color.white;

		int idx = y * w + x;
		
		return idx < Pixels.Length ? Pixels[idx] : Color.white;
	}

	private TileTypes.Enum GetTileTypeForColor(Color c)
	{
		foreach (MapAssetColorTilePairs cttp in m_mapAssetColorTileAssignment)
		{
			if (cttp.m_pixelColor == c)
				return cttp.m_tileType;
		}

		return m_defaultTileType;
	}

	private MapAssetColorTilePairs GetColorEncodedData(Color c)
	{
		foreach (MapAssetColorTilePairs cttp in m_mapAssetColorTileAssignment)
		{
			if (cttp.m_pixelColor == c)
				return cttp;
		}

		return new MapAssetColorTilePairs();
	}

	public void GenerateGrid()
	{
		bool useMapAsset = m_mapAsset != null;
		if(useMapAsset && !m_mapAsset.isReadable)
		{
			Debug.LogError("MapAsset must be Readable! Enable Advanced/Read-Write option on texture asset)");
			useMapAsset = false;
		}

		Color[] pixels = useMapAsset ? m_mapAsset.GetPixels() : null;

		Vector3 p = transform.position;
		p.x -= (m_gridSize.x / 2.0f * m_tileSize.x);
		p.z -= (m_gridSize.y / 2.0f * m_tileSize.y);

		for (int i = 0; i < m_gridSize.x; i++)
		{
			for (int j = 0; j < m_gridSize.y; j++)
			{
				GameObject tile = Instantiate(m_tilePrefab, transform);
				tile.transform.position = new Vector3(p.x + i * m_tileSize.x, 0.0f, p.z + j * m_tileSize.y);

				Color c = useMapAsset ? GetPixel(pixels, i, j, m_mapAsset.width, m_mapAsset.height) : Color.white;

				MapTile mapTile = tile.GetComponent<MapTile>();
				mapTile.Init(this, GetTileTypeForColor(c));

				MapAssetColorTilePairs cced = GetColorEncodedData(c);
				if (cced.m_tileType != TileTypes.Enum.None)
				{
					if (cced.m_cropType != CropTypes.Enum.None)
					{
						mapTile.OverrideCrop(cced.m_cropType, cced.m_cropStep);
					}
				}
			}
		}
	}

	public void Cleanup()
	{
		for (int i = transform.childCount - 1; i >= 0; i--)
		{
			DestroyImmediate(transform.GetChild(i).gameObject);
		}
	}

	public bool HasValidInteraction(FarmingTools.Tool tool)
	{
		if (m_currentActiveTile)
			return m_currentActiveTile.HasValidInteraction(tool);

		return false;
	}

	public void Interact(FarmingTools.Tool tool)
	{
		if (m_currentActiveTile)
			m_currentActiveTile.Interact(tool);
	}

	public void SetActiveTile(MapTile tile)
	{ 
		m_currentActiveTile = tile;
	}

	void OnDrawGizmos()
	{
		Gizmos.color = Color.magenta;

		float halfW = m_gridSize.x / 2.0f * m_tileSize.x;
		float halfH = m_gridSize.y / 2.0f * m_tileSize.y;

		Vector3 p1 = transform.position + new Vector3(-halfW, 0.0f, -halfH);
		Vector3 p2 = transform.position + new Vector3(-halfW, 0.0f, halfH);
		Vector3 p3 = transform.position + new Vector3(halfW, 0.0f, halfH);
		Vector3 p4 = transform.position + new Vector3(halfW, 0.0f, -halfH);

		Gizmos.DrawLine(p1, p2);
		Gizmos.DrawLine(p2, p3);
		Gizmos.DrawLine(p3, p4);
		Gizmos.DrawLine(p4, p1);

		Gizmos.DrawLine(p1, p3);
		Gizmos.DrawLine(p2, p4);
	}
}
