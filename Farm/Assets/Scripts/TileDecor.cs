using UnityEngine;
using System;
using System.Collections.Generic;

public class TileDecor : MonoBehaviour
{
	[Header("Setup")]
	public List<TileDecorData> m_decorDatas;

	#region private
	private TileDecorTypes.Enum m_currentDecor;
	private Dictionary<TileDecorTypes.Enum, GameObject> m_visuals = new Dictionary<TileDecorTypes.Enum, GameObject>();
	#endregion

	void Start()
    {
        
    }

    void Update()
    {
        
    }

	public bool HasDecor()
	{ 
		return m_currentDecor != TileDecorTypes.Enum.None;
	}
}
