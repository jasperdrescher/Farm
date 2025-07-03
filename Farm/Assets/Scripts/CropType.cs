using UnityEngine;
using static MapTile;

/*
 * Visuals and Parameters for a Crop
 */
public class CropType : MonoBehaviour
{
	public enum ECropType
	{
		None,
		Potato,
		Garlic,
		Tomato,
		Carrot,
		Leek,
		Wheat,
		Rice,
	}

	public ECropType Type;

	[System.Serializable]
	public class CropGrowStep
	{
		public GameObject StepPrefab;
		public float TimeUntilNextStep = 0;
		public FarmingTools.Tool RequiredTool;
	}

	/*
	 * todo
	 * add what item is the "reward" when harvested
	 */

	public CropGrowStep[] Steps;

	void Start()
    {
		for (int i = 0; i < Steps.Length; i++)
		{
			Steps[i].StepPrefab.transform.position = Vector3.zero;
		}

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
