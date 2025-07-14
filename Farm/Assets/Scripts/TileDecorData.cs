using UnityEngine;

[CreateAssetMenu(fileName = "TileDecorData", menuName = "Scriptable Objects/TileDecorData")]
public class TileDecorData : ScriptableObject
{
	public TileDecorTypes.Enum m_type;
	public string m_name;

	public GameObject m_gameObject;
	public Vector3 m_offset;
	public Vector3 m_rotation;
}
