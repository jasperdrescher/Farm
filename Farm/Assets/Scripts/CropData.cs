using UnityEngine;

[CreateAssetMenu(fileName = "CropData", menuName = "Scriptable Objects/CropData")]
public class CropData : ScriptableObject
{
	public CropTypes.Enum m_type;
	public string m_name;

	[System.Serializable]
	public class CropGrowStep
	{
		public GameObject m_stepPrefab;
		public float m_timeUntilNextStep = 0;
		public FarmingTools.Tool m_requiredTool;
	}

	public CropGrowStep[] m_steps;
	/*
	 * todo
	 * add what item is the "reward" when harvested
	 * etc...
	 */
}
