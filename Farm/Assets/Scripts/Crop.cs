using System;
using System.Collections.Generic;
using UnityEngine;
using static CropData;

[ExecuteInEditMode]
public class Crop : MonoBehaviour
{
	[Header("Setup")]
	public List<CropData> m_cropTypes;

	[Header("Editor")]
	public CropTypes.Enum m_editorCropTypeChanger = CropTypes.Enum.None;
	public int m_editorCropStepChanger = 0;

	#region private
	private CropRuntime m_runtime;
	private MapTile m_ownerMapTile = null;
	private int m_currentCropStep = 0;
	private CropTypes.Enum m_currentCropType = CropTypes.Enum.None;
	private Dictionary<CropTypes.Enum, List<GameObject>> m_visuals = new Dictionary<CropTypes.Enum, List<GameObject>>();
	#endregion

	[Serializable]
	public class SaveData
	{
		public CropTypes.Enum m_currentCropType;
	}

	void Start()
	{
		
	}

	void Update()
	{
#if UNITY_EDITOR
		EditorCheckValueChanges();
#endif
	}

	public void CreateCropTypes()
	{
		float tileHeight = m_ownerMapTile.GetTileHeight();

		foreach (CropData cropData in m_cropTypes)
		{
			if (cropData.m_type == CropTypes.Enum.None)
				continue;

			List<GameObject> visuals = new List<GameObject>();
			foreach (CropGrowStep step in cropData.m_steps)
			{
				GameObject go = Instantiate(step.m_stepPrefab, transform);
				go.SetActive(false);
				visuals.Add(go);

				MeshRenderer mr = go.GetComponent<MeshRenderer>();
				if (mr != null)
					mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
			}

			if (visuals.Count > 0)
			{
				m_visuals.Add(cropData.m_type, visuals);
			}
			else
			{
				Debug.LogWarning("Crop '"+cropData.m_name+"' has no visuals set. Skipped.");
			}
		}
	}

	public void Init(MapTile owner)
	{
		m_ownerMapTile = owner;
		m_runtime = GetComponent<CropRuntime>();
		CreateCropTypes();
	}

#if UNITY_EDITOR
	private void EditorCheckValueChanges()
	{
		if (m_editorCropTypeChanger != m_currentCropType)
			ChangeCropType(m_editorCropTypeChanger);

		if(m_editorCropStepChanger != m_currentCropStep)
			ChangeCropStep(m_editorCropStepChanger);
	}
#endif

	// [FIXME] this should not be public
	public void ChangeCropType(CropTypes.Enum type)
	{
		DeactivateAllVisualsForCrop(m_currentCropType);

		if (!m_visuals.ContainsKey(type))
		{
			m_currentCropType = CropTypes.Enum.None;
			m_editorCropTypeChanger = m_currentCropType;
			m_currentCropStep = 0;
			m_editorCropStepChanger = m_currentCropStep;
			return;
		}

		m_currentCropType = type;
		m_editorCropTypeChanger = type;

		m_currentCropStep = 0;
		m_editorCropStepChanger = m_currentCropStep;

		m_runtime.Init(GetCropData());
		GetCropData();
		ChangeCropStep(m_currentCropStep);
	}

	// [FIXME] this should not be public
	public void ChangeCropStep(int step)
	{
		if (m_currentCropType == CropTypes.Enum.None)
		{
			m_currentCropStep = 0;
			m_editorCropStepChanger = m_currentCropStep;
			return;
		}

		m_currentCropStep = Mathf.Clamp(step, 0, m_visuals[m_currentCropType].Count - 1);
		m_editorCropStepChanger = m_currentCropStep;

		if (m_currentCropStep >= m_visuals[m_currentCropType].Count)
		{
			return;
		}

		DeactivateAllVisualsForCrop(m_currentCropType);

		m_visuals[m_currentCropType][m_currentCropStep].SetActive(true);	
	}

	private void DeactivateAllVisualsForCrop(CropTypes.Enum type)
	{
		if (m_currentCropType != CropTypes.Enum.None && m_visuals.ContainsKey(m_currentCropType))
		{
			foreach (GameObject go in m_visuals[type])
				go.SetActive(false);
		}
	}

	public CropTypes.Enum GetCropType()
	{
		return m_currentCropType;
	}

	public CropData GetCropData()
	{
		foreach (CropData cropData in m_cropTypes)
		{
			if (cropData.m_type == m_currentCropType)
				return ScriptableObject.Instantiate(cropData) as CropData;
		}

		return null;
	}

	public bool HasAnythingPlanted()
	{ 
		return m_currentCropType != CropTypes.Enum.None;
	}

	public void PlantCrop(CropTypes.Enum type)
	{
		if (HasAnythingPlanted())
			return;

		ChangeCropType(type);
	}

	public void Interact(FarmingTools.Tool tool)
	{ 

	}

	public bool SaveState(Crop.SaveData data)
	{
		return true;
	}

	public bool LoadState(Crop.SaveData data)
	{
		return true;
	}
}