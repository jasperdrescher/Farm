using UnityEngine;

[CreateAssetMenu(fileName = "InventoryItem", menuName = "Scriptable Objects/InventoryItem")]
public class InventoryItem : ScriptableObject
{
	public enum Type
	{ 
		None,
		Seed, // seed to plant with planting tool
		Crop, // harvested crop items
		Currency, // tbd
	}

	public Type m_itemType = Type.None;
	public int m_amount = 1;

	public CropTypes.Enum m_cropType; // can be used for both seed and harvested crop

	public GameObject m_gameObjectPrefab;
}
