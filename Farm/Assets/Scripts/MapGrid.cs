using UnityEngine;
using UnityEngine.InputSystem.LowLevel;
using static UnityEditor.PlayerSettings;

[ExecuteInEditMode]
public class MapGrid : MonoBehaviour
{
	[Header("Setup")]
	public GameObject TilePrefab;

	public Vector2 GridSize;
	public Vector2 TileSize;

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
				GameObject Tile = Instantiate(TilePrefab, transform);
				Tile.transform.position = new Vector3(p.x + i * TileSize.x, 0.0f, p.z + j * TileSize.y);
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
}
