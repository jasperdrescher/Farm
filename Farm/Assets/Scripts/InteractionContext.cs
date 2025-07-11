using UnityEngine;

public class InteractionContext : ScriptableObject
{
    public FarmingTools.Tool m_tool = FarmingTools.Tool.None;
	public InventoryItem m_inventoryItem = null;
	public Vector3 m_position = Vector3.zero;
}
