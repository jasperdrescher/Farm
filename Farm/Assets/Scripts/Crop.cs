using UnityEngine;

public class Crop : MonoBehaviour
{
	public enum ECropType
	{
		None,
		Potato,
		Tomato
	}

	[Header("Setup")]
	public string CropName;
	public ECropType CropType;
	public GameObject CropItem;
	public GameObject[] CropGrowSteps;
	public float[] GrowTimes;

	void Start()
	{

	}

	void Update()
	{

	}
}