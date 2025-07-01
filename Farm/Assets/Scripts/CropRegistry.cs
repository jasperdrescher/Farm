using UnityEngine;

public class CropRegistry : MonoBehaviour
{
	public Crop[] Crops;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

	public Crop GetCropByType(Crop.ECropType Type)
	{
		foreach (Crop c in Crops)
		{
			if(c.CropType == Type)
				return c;
		}

		return null;
	}
}
