using UnityEngine;
using System.Collections.Generic;
using System;

public class CropInteractionResult : InteractionResult
{
	public List<InventoryItem> m_reward = new List<InventoryItem>();

	public static CropInteractionResult SuccessfullHarvest(CropData data)
	{
		CropInteractionResult result = ScriptableObject.CreateInstance<CropInteractionResult>();
		result.m_result = true;

		if (data != null)
		{
			foreach (CropData.HarvestRewards r in data.m_harvestRewards)
			{
				float chanceRoll = UnityEngine.Random.Range(0.0f, 1.0f);
				if (chanceRoll <= r.m_chance)
				{
					int amount = (int)((r.m_amountMinMax.x == r.m_amountMinMax.y) ? r.m_amountMinMax.x : UnityEngine.Random.Range(r.m_amountMinMax.x, r.m_amountMinMax.y));
					if (amount > 0)
					{
						InventoryItem item = ScriptableObject.Instantiate(r.m_inventoryItem);
						item.m_amount = amount;
						result.m_reward.Add(item);
					}
				}
			}
		}

		return result;
	}
}
