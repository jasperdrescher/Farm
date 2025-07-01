using UnityEditor.SceneManagement;
using UnityEditor.UI;
using UnityEngine;

[ExecuteInEditMode]
public class MapTile : MonoBehaviour
{
	public float TileHeight = 2.0f;

	#region private 
	CropRuntime CurrentCrop = null;
	MapGrid OwnerGrid = null;
	#endregion

	void Start()
    {
		
	}

    void Update()
    {
        
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
