using UnityEngine;

[CreateAssetMenu(fileName = "ToolData", menuName = "Scriptable Objects/ToolData")]
public class ToolData : ScriptableObject
{
    public FarmingTools.EFarmingTools tool;
    public GameObject prefab;
}
