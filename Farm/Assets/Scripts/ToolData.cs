using UnityEngine;

[CreateAssetMenu(fileName = "ToolData", menuName = "Scriptable Objects/ToolData")]
public class ToolData : ScriptableObject
{
    public FarmingTools.Tool m_tool;
    public GameObject m_prefab;
}
