using UnityEngine;

[CreateAssetMenu(fileName = "TileData", menuName = "Scriptable Objects/TileData")]
public class TileData : ScriptableObject
{
	public TileTypes.Enum m_tileType = TileTypes.Enum.None;
	public GameObject m_prefab;
	public float m_tileHeight = 2.0f;
}
