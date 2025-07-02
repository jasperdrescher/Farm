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
	}

	public CropGrowStep[] Steps;
	public int CurrentStep = 0;

	void Start()
    {
		ChangeStep(0);
	}

    void Update()
    {
        
    }

	void ChangeStep(int Step)
	{
		CurrentStep = Mathf.Clamp(Step, 0, Steps.Length - 1);
		for (int i = 0; i < Steps.Length; i++)
		{
			Steps[i].StepPrefab.SetActive(i == Step);
		}
	}
}
