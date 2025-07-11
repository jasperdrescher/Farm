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
	public int m_maxStackSize = 0; // <=0 unlimited, 1=non stackable, other: stack size

	public Sprite m_thumbnail;

	public bool IsVisibleInInventory()
	{
		return !m_hideInInventory && m_itemType != Type.Currency && m_itemType != Type.PlayerResource;
	}

	public bool IsStackable()
	{ 
		return m_maxStackSize != 1;
	}

	public bool IsSameItem(InventoryItem otherItem)
	{ 
		return m_itemType == otherItem.m_itemType && m_cropType == otherItem.m_cropType && m_resourceType == otherItem.m_resourceType;
	}

	public bool CanStackWith(InventoryItem otherItem)
	{ 
		return IsStackable() && IsSameItem(otherItem);
	}

	// return value: false, if item is fully stacked and no need to keep it.
	// false: you have to keep the input item, because it was not stacked or partially (amount modified)
	public bool StackItem(ref InventoryItem otherItem)
	{
		if (!CanStackWith(otherItem))
			return true;

		if (m_maxStackSize < 1)
		{
			m_amount += otherItem.m_amount;
			return false;
		}
		else
		{
			// assuming that m_amount is not already greater then the max stack size...
			if (m_amount > m_maxStackSize)
			{
				Debug.LogWarning("Inventory item ("+m_itemType+") exceeds the max stack size... you will loose some items...");
			}

			int sum = m_amount + otherItem.m_amount;
			int remainder = sum - Mathf.Min(sum, m_maxStackSize);
			otherItem.m_amount = Mathf.Min(remainder, m_maxStackSize);

			return true;
		}
	}

	// return value will be null if item count <= 1
	// otherwise you have to store the new item
	public InventoryItem SplitStack()
	{
		if (m_amount <= 1)
			return null;

		int amount = Mathf.FloorToInt(m_amount / 2.0f);
		m_amount -= amount;

		InventoryItem item = ScriptableObject.Instantiate(this) as InventoryItem;
		item.m_amount = amount;
		return item;
	}
}
