using System;
using UnityEngine;

[CreateAssetMenu(fileName = "TileDecorData", menuName = "Scriptable Objects/TileDecorData")]
public class TileDecorData : ScriptableObject
{
	public TileDecorTypes.Enum m_type;
	public string m_name;

	public GameObject m_gameObject;

	[Serializable]
	public class TransformVariations
	{
		public Vector3 m_offset = Vector3.zero;
		public Vector3 m_rotation = Vector3.zero;
	}

	public TransformVariations[] m_transformVariations;

	public bool m_tileWalkability = true;
	public bool m_canCoexistWithCrop = false;

	public TransformVariations GetVariation(int index)
	{
		if (index >= 0 && index < m_transformVariations.Length)
		{ 
			return m_transformVariations[index];
		}

		return new TransformVariations();
	}
}
