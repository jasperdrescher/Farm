using UnityEngine;

[CreateAssetMenu(fileName = "CropData", menuName = "Scriptable Objects/CropData")]
public class CropData : ScriptableObject
{
	public CropTypes.Enum m_type;
	public string m_name;

	public GameObject[] m_steps;
	[Min(0f)] public float m_growTime;
	public FarmingTools.Tool m_harvestTool;
	public InventoryItem m_seedItem = null;

	[System.Serializable]
	public class HarvestRewards
	{
		public InventoryItem m_inventoryItem = null;
		public Vector2 m_amountMinMax;
		[Range(0f, 1f)] public float m_chance = 1.0f;
	}

	public HarvestRewards[] m_harvestRewards;

	

	public int GetLastStepIndex()
	{
		return m_steps.Length - 1;
	}
}
