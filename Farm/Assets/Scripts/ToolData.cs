using UnityEngine;

[CreateAssetMenu(fileName = "ToolData", menuName = "Scriptable Objects/ToolData")]
public class ToolData : ScriptableObject
{
    public FarmingTools.Tool m_tool = FarmingTools.Tool.None;
    public GameObject m_prefab;
    public float m_interactionTime = 1f;
}
