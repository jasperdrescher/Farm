using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.Rendering;
using UnityEditor.ShaderGraph;

[ExecuteInEditMode]
public class TileDecor : MonoBehaviour
{
	[Header("Setup")]
	public List<TileDecorData> m_decorDatas;

	[Header("Setup")]
	public TileDecorTypes.Enum m_editorDecorChanger = TileDecorTypes.Enum.None;
	public int m_editorVariationIndexChanger = 0;

	#region private
	private MapTile m_ownerTile = null;
	private TileDecorTypes.Enum m_currentDecor;
	private int m_currentVariationIndex;
	private Dictionary<TileDecorTypes.Enum, GameObject> m_visuals = new Dictionary<TileDecorTypes.Enum, GameObject>();
	private Vector3 m_basePos;
	#endregion

	void Start()
    {
        
    }

    void Update()
    {
#if UNITY_EDITOR
		EditorCheckValueChanges();
#endif
	}

#if UNITY_EDITOR
	void EditorCheckValueChanges()
	{
		if (m_editorDecorChanger != m_currentDecor)
		{
			ChangeDecorType(m_editorDecorChanger);
		}

		if (m_editorVariationIndexChanger != m_currentVariationIndex)
		{
			ChangeVariation(m_editorVariationIndexChanger);
		}
	}
#endif

	public void Init(MapTile OwnerTile)
	{ 
		m_ownerTile = OwnerTile;
		CreateDecorTypes();
	}

	public void SetBasePos(Vector3 pos)
	{ 
		m_basePos = pos;
	}

	void CreateDecorTypes()
	{
		float tileHeight = m_ownerTile.GetTileHeight();

		foreach (TileDecorData decorData in m_decorDatas)
		{
			if (decorData.m_type == TileDecorTypes.Enum.None)
				continue;

			GameObject go = Instantiate(decorData.m_gameObject, transform);
			go.SetActive(false);
			m_visuals.Add(decorData.m_type, go);
		}
	}

	public void ChangeDecorType(TileDecorTypes.Enum type)
	{
		foreach (KeyValuePair<TileDecorTypes.Enum, GameObject> p in m_visuals)
		{
			p.Value.SetActive(p.Key == type);
		}

		m_editorDecorChanger = type;
		m_currentDecor = type;

		ChangeVariation(0);

		if(m_ownerTile)
			m_ownerTile.SetWalkable(GetData().m_tileWalkability);	
	}

	public void ChangeVariation(int index)
	{
		TileDecorData decorData = GetData();
		TileDecorData.TransformVariations variation = decorData.GetVariation(index);

		transform.rotation = Quaternion.Euler(variation.m_rotation);
		transform.position = m_basePos + variation.m_offset;

		m_editorVariationIndexChanger = index;
		m_currentVariationIndex = index;
	}

	public void CycleVariations(bool forward = true)
	{ 
		int dir = forward ? 1 : -1;
		int newIndex = m_currentVariationIndex + dir;

		if (newIndex >= GetData().m_transformVariations.Length)
			newIndex = 0;
		else if(newIndex < 0)
			newIndex = GetData().m_transformVariations.Length - 1;

		ChangeVariation(newIndex);
	}

	public bool HasDecor()
	{ 
		return m_currentDecor != TileDecorTypes.Enum.None;
	}

	private TileDecorData GetData()
	{
		foreach (TileDecorData data in m_decorDatas)
		{
			if (data.m_type == m_currentDecor)
				return data;
		}

		return null;
	}
}
