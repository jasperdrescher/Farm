using UnityEditor.Experimental.GraphView;
using UnityEditor.SceneManagement;
using UnityEditor.UI;
using UnityEngine;

[ExecuteInEditMode]
public class MapTile : MonoBehaviour
{
	public enum ETileType
	{ 
		Grass,
		FarmField,
	}

	[System.Serializable]
	public class TileTypeEntry
	{
		public ETileType TileType;
		public GameObject Prefab;
		public float TileHeight = 2.0f;
	}

	public TileTypeEntry[] TileTypes;

	public ETileType TileType = ETileType.Grass;

#region private
	// do not change directly... use TileType
	private ETileType m_TileType = ETileType.Grass;
#endregion

	void Start()
	{
		// make sure mesh renderer's shadow casting is off
		// we also have a box collider on the main prefab, so no need for them on the sub objects
		foreach (TileTypeEntry entry in TileTypes)
		{
			MeshRenderer meshRenderer =	entry.Prefab.GetComponent<MeshRenderer>();
			if(meshRenderer != null)
				meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

			BoxCollider boxCollider = entry.Prefab.GetComponent<BoxCollider>();
			if(boxCollider != null)
				boxCollider.enabled = false;
		}

		// init visual
		OnTileTypeChange(m_TileType);
	}

    void Update()
    {
		CheckValueChanges();
	}

	// not the nicest solution, but I need it to work from editor too...
	void CheckValueChanges()
	{
		if (TileType != m_TileType)
		{ 
			m_TileType = TileType;
			OnTileTypeChange(m_TileType);
		}
	}

	void OnTileTypeChange(ETileType NewTileType)
	{
		foreach (TileTypeEntry entry in TileTypes)
		{
			entry.Prefab.SetActive(entry.TileType == NewTileType);
		}
	}

	/*
	public void PlantCrop(Crop NewCrop)
	{
		if (CurrentCrop)
			return;

		CurrentCrop = transform.gameObject.AddComponent<CropRuntime>();
		CurrentCrop.Init(NewCrop);
	}
	*/

	void OnTriggerStay(Collider Other)
	{
		//Debug.Log(Other.transform.name);
	}

	// todo: add an enum value as parameter for the current used tool
	public void Interact()
	{ 

	}
}
