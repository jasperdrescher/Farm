using UnityEngine;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.Tilemaps;
using static MapTile;
using static UnityEditor.PlayerSettings;

[ExecuteInEditMode]
public class MapGrid : MonoBehaviour
{
	[Header("Setup")]
	public GameObject TilePrefab;
	public Vector2 GridSize;
	public Vector2 TileSize;

	[Header("Generate From Image")]
	public Texture2D MapAsset;
	[System.Serializable]
	public class MapAssetColorTilePairs
	{
		public Color PixelColor;
		public MapTile.ETileType TileType;
	}

	public MapTile.ETileType DefaultTileType;
	public MapAssetColorTilePairs[] MapAssetColorTileAssignment;

	[Header("Editor")]
	public bool EditorGenerateGrid = false;

	void Start()
    {
	}

	void Update()
    {
#if UNITY_EDITOR
		if(Application.isPlaying == false)
			HandleEditorGridGeneration();
#endif
	}

#if UNITY_EDITOR
	// editor stuff
	void HandleEditorGridGeneration()
	{
		if (transform.childCount > 0 && EditorGenerateGrid)
			return;

		if (EditorGenerateGrid)
		{
			GenerateGrid();
		}
		else
		{
			Cleanup();
		}
	}

	private Color GetPixel(Color[] Pixels, int X, int Y, int H, int W) 
	{ 
		if(X >= W || Y >= H)
			return Color.white;

		int idx = Y * W + X;
		
		return idx < Pixels.Length ? Pixels[idx] : Color.white;
	}

	private MapTile.ETileType GetTileTypeForColor(Color c)
	{
		foreach (MapAssetColorTilePairs cttp in MapAssetColorTileAssignment)
		{
			if (cttp.PixelColor == c)
				return cttp.TileType;
		}

		return DefaultTileType;
	}

	public void GenerateGrid()
	{
		bool useMapAsset = MapAsset != null;
		Color[] pixels = useMapAsset ? MapAsset.GetPixels() : null;

		Vector3 p = transform.position;
		p.x -= (GridSize.x / 2.0f * TileSize.x);
		p.z -= (GridSize.y / 2.0f * TileSize.y);

		for (int i = 0; i < GridSize.x; i++)
		{
			for (int j = 0; j < GridSize.y; j++)
			{
				GameObject Tile = Instantiate(TilePrefab, transform);
				Tile.transform.position = new Vector3(p.x + i * TileSize.x, 0.0f, p.z + j * TileSize.y);

				if (useMapAsset)
				{
					Color c = GetPixel(pixels, i, j, MapAsset.width, MapAsset.height);
					MapTile.ETileType tt = GetTileTypeForColor(c);

					MapTile mt = Tile.GetComponent<MapTile>();
					if(mt != null)
						mt.TileType = tt;
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
#endif

	//todo: have the current equipped tool passed as param both here and on the tile. Or Get it from player?
	public void Interact()
	{
		foreach (Transform t in transform) 
		{
			MapTile tile = t.gameObject.GetComponent<MapTile>();
			if (tile != null && tile.IsActive())
			{ 
				tile.Interact();
			}
		}
	}
}
