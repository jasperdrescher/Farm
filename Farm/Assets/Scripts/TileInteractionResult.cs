using UnityEngine;
using System.Collections.Generic;

public class TileInteractionResult : InteractionResult
{
	public List<InventoryItem> m_consumed = new List<InventoryItem>();

	public static TileInteractionResult SuccessWithConsume(InventoryItem inventoryItem)
	{ 
		TileInteractionResult result = ScriptableObject.CreateInstance<TileInteractionResult>();
		result.m_result = true;

		if(inventoryItem != null)
			result.m_consumed.Add(inventoryItem);

		return result;
	}
}
