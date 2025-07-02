using UnityEngine;
using static MapTile;

public class CropType : MonoBehaviour
{
	public enum ECropType
	{
		None,
		Potato,
	}

	public ECropType Type;

	[System.Serializable]
	public class CropGrowStep
	{
		public GameObject StepPrefab;
		public float TimeUntilNextStep = 0;
		public bool NeedsWaterToChangeNextStep = false;
	}

	public CropGrowStep[] Steps;

	void Start()
    {
		ChangeStep(0);
	}

    void Update()
    {
        
    }

	void ChangeStep(int Step)
	{
		for (int i = 0; i < Steps.Length; i++)
		{
			Steps[i].StepPrefab.SetActive(i == Step);
		}
	}
}
