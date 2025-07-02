using UnityEngine;

public class CropRuntime : MonoBehaviour
{
	//[Range(0, 1)] public float CurrentStepProgress;

	bool NeedsWater = true;
	bool Growing = false;
	int CurrentStep = 0;
	float GrowingTimer = 0.0f;

	GameObject CurrentVisual = null;
	MapTile OwnerTile = null;

	Crop CropData;

	public void Init(Crop Data)
	{
		CropData = Data;
		OwnerTile = transform.gameObject.GetComponent<MapTile>();
		UpdateVisual();
	}

	void Start()
    {
        
    }

	void Update()
	{
		HandleGrowth();
	}

	void HandleGrowth()
	{
		if (Growing)
		{
			GrowingTimer += Time.deltaTime;

			if (CropData != null && CropData.GrowTimes.Length > CurrentStep && GrowingTimer >= CropData.GrowTimes[CurrentStep])
			{
				NeedsWater = true;
				Growing = false;
				GrowingTimer = 0.0f;
			}
		}
	}

	void UpdateVisual()
	{
		if (CurrentVisual != null)
			GameObject.DestroyImmediate(CurrentVisual);

		CurrentVisual = Instantiate(CropData.CropGrowSteps[CurrentStep], transform);
		//CurrentVisual.transform.position = transform.position + new Vector3(0, OwnerTile.TileHeight, 0);
	}

	public void Interact()
	{
	}
}
