using UnityEngine;

public class Crop : MonoBehaviour
{
	public GameObject[] CropPrefabs;

	public CropType.ECropType CurrentCropType;

	void Start()
	{
		ChangeCrop(CropType.ECropType.None);
	}

	void Update()
	{

	}

	public void Init()
	{
		ChangeCrop(CropType.ECropType.None);
	}

	void ChangeCrop(CropType.ECropType Type)
	{
		CurrentCropType = Type;

		foreach (GameObject c in CropPrefabs)
		{
			CropType CT = c.GetComponent<CropType>();
			c.SetActive(CT != null && CT.Type == Type);
		}
	}
}