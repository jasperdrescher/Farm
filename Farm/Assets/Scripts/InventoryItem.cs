using UnityEngine;

[CreateAssetMenu(fileName = "InventoryItem", menuName = "Scriptable Objects/InventoryItem")]
public class InventoryItem : ScriptableObject
{
	public enum Type
	{ 
		None,
		Seed, // seed to plant with planting tool
		Crop, // harvested crop items
		Resource, // eg.: water
		Currency, // tbd (money)
		PlayerResource, //tbd (stamina?)
	}

	public Type m_itemType = Type.None;
	public int m_amount = 1;

	public CropTypes.Enum m_cropType; // can be used for both seed and harvested crop
	public ResourceTypes.Enum m_resourceType; // can be used for Resource, Currency, PlayerResource

	public GameObject m_gameObjectPrefab;

	public bool m_hideInInventory = false;

	public bool IsVisibleInInventory()
	{
		return !m_hideInInventory && m_itemType != Type.Currency && m_itemType != Type.PlayerResource;
	}
}
