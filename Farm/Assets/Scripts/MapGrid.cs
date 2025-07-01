using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

[ExecuteInEditMode]
public class MapGrid : MonoBehaviour
{
	[Header("Setup")]
	public CropRegistry CropRegistryPrefab;
	public TileRegistry TileRegistryPrefab;

	public Vector2 GridSize;
	public Vector2 TileSize;

	[Header("Debug")]
	public bool Enabled = true;
	public bool DebugSpawnCrop = false;
	bool EnabledInternal = false;

    void Start()
    {
		Cleanup();
		GenerateGrid();
	}
	
    void Update()
    {
#if UNITY_EDITOR
		if(Application.isPlaying == false)
			HandleEnabledState();
#endif
	}

#if UNITY_EDITOR
	// debug stuff
	void HandleEnabledState()
	{
		if (Enabled == EnabledInternal)
			return;

		EnabledInternal = Enabled;

		if (Enabled)
		{
			GenerateGrid();
		}
		else
		{
			Cleanup();
		}
	}
#endif

	public void GenerateGrid()
	{
		Vector3 p = transform.position;
		p.x -= (GridSize.x / 2.0f * TileSize.x);
		p.z -= (GridSize.y / 2.0f * TileSize.y);

		for (int i = 0; i < GridSize.x; i++)
		{
			for (int j = 0; j < GridSize.y; j++)
			{
				Vector2 TilePos = new Vector2(p.x + i * TileSize.x, p.z + j * TileSize.y);
				MapTile MT = CreateTileAt(TilePos);

				if(DebugSpawnCrop)
					MT.PlantCrop(CropRegistryPrefab.GetCropByType(Crop.ECropType.Potato));
			}
		}
	}

	private MapTile CreateTileAt(Vector2 Pos)
	{
		GameObject Tile = new GameObject();
		Tile.name = "MapTile";
		Tile.transform.parent = transform;
		Tile.transform.position = new Vector3(Pos.x, 0.0f, Pos.y);

		MapTile MT = Tile.AddComponent<MapTile>();
		MT.Init();
		return MT;
	}

	public void Cleanup()
	{
		for (int i = transform.childCount - 1; i >= 0; i--)
		{
			DestroyImmediate(transform.GetChild(i).gameObject);
		}
	}
}
