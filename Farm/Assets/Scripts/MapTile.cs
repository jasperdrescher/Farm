using UnityEditor.SceneManagement;
using UnityEditor.UI;
using UnityEngine;

[ExecuteInEditMode]
public class MapTile : MonoBehaviour
{
	public float TileHeight;

	#region private 
	GameObject TileAsset = null;
	GameObject TileAssetInstance = null;

	CropRuntime CurrentCrop = null;

	MapGrid OwnerGrid = null;
	#endregion

	void Start()
    {
		
	}

    void Update()
    {
        
    }

	public void Init(float NewTileHeight = 2.0f)
	{
		OwnerGrid = transform.parent.GetComponent<MapGrid>();

		TileHeight = NewTileHeight;

		CreateCollider();

		ChangeTileAsset(OwnerGrid.TileRegistryPrefab.GrassTile);
	}

	void CreateCollider()
	{
		BoxCollider BC = gameObject.AddComponent<BoxCollider>();
		BC.size = new Vector3(OwnerGrid.TileSize.x, 1, OwnerGrid.TileSize.y);
		BC.center = new Vector3(0.0f, TileHeight + 0.5f, 0.0f);
		BC.isTrigger = true;
		BC.transform.parent = gameObject.transform;
	}

	public void ChangeTileAsset(GameObject NewTileAsset, bool Forced = false)
	{
		if (TileAsset == NewTileAsset && !Forced)
			return;

		if (TileAssetInstance)
		{
			GameObject.Destroy(TileAssetInstance);
			TileAssetInstance = null;
		}

		TileAsset = NewTileAsset;

		if (TileAsset != null)
		{
			TileAssetInstance = Instantiate(TileAsset, transform);
			TileAssetInstance.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
		}
	}

	public void PlantCrop(Crop NewCrop)
	{
		if (CurrentCrop)
			return;

		CurrentCrop = transform.gameObject.AddComponent<CropRuntime>();
		CurrentCrop.Init(NewCrop);
	}

	void OnTriggerStay(Collider Other)
	{
		//Debug.Log(Other.transform.name);
	}

	// todo: add an enum value as parameter for the current used tool
	public void Interact()
	{ 

	}
}
