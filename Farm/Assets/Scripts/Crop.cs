using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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
	private CropRuntime m_runtime = null;
	private Slider m_slider = null;
	private MapTile m_ownerMapTile = null;
	private CropTypes.Enum m_currentCropType = CropTypes.Enum.None;
	private Dictionary<CropTypes.Enum, List<GameObject>> m_visuals = new Dictionary<CropTypes.Enum, List<GameObject>>();
	#endregion

	[Serializable]
	public class SaveData
	{
		public CropTypes.Enum m_currentCropType;
		public CropRuntime.SaveData m_runtime;
	}

	void Start()
	{
		
	}

	void Update()
	{
#if UNITY_EDITOR
		EditorCheckValueChanges();
#endif

		if (m_slider && m_runtime)
		{
			m_slider.value = m_runtime.GetTimerProgress();	
			m_slider.gameObject.SetActive(m_runtime.IsTimerEnabled());
			m_slider.transform.parent.transform.LookAt(Camera.main.transform.position);
			m_slider.transform.parent.transform.rotation *= Quaternion.Euler(0f, 180f, 0f);
		}
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

		m_slider = GetComponentInChildren<Slider>();
		m_slider.gameObject.SetActive(false);

		CreateCropTypes();
	}

#if UNITY_EDITOR
	private void EditorCheckValueChanges()
	{
		if (m_editorCropTypeChanger != m_currentCropType)
			ChangeCropType(m_editorCropTypeChanger);

		if(m_editorCropStepChanger != m_runtime.m_currentStep)
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
			m_runtime.m_currentStep = 0;
			m_editorCropStepChanger = m_runtime.m_currentStep;
			return;
		}

		m_currentCropType = type;
		m_editorCropTypeChanger = type;

		m_runtime.m_currentStep = 0;
		m_editorCropStepChanger = m_runtime.m_currentStep;

		m_runtime.Init(GetCropData());
		GetCropData();
		ChangeCropStep(m_runtime.m_currentStep);
	}

	// [FIXME] this should not be public
	public void ChangeCropStep(int step)
	{
		if (m_currentCropType == CropTypes.Enum.None)
		{
			m_runtime.m_currentStep = 0;
			m_editorCropStepChanger = m_runtime.m_currentStep;
			return;
		}

		m_runtime.m_currentStep = Mathf.Clamp(step, 0, m_visuals[m_currentCropType].Count - 1);
		m_editorCropStepChanger = m_runtime.m_currentStep;

		if (m_runtime.m_currentStep >= m_visuals[m_currentCropType].Count)
		{
			return;
		}

		DeactivateAllVisualsForCrop(m_currentCropType);

		m_visuals[m_currentCropType][m_runtime.m_currentStep].SetActive(true);	
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

		m_runtime.Init(GetCropData());
		ChangeCropType(type);
	}

	public void Interact(FarmingTools.Tool tool)
	{
		if (!HasAnythingPlanted())
			return;

		if (m_runtime.IsTimerEnabled())
			return;

		CropData cropData = GetCropData();
		if (m_runtime.m_currentStep < cropData.GetLastStepIndex())
		{
			FarmingTools.Tool currentRequiredTool = cropData.m_steps[m_runtime.m_currentStep].m_requiredTool;
			if (tool == currentRequiredTool)
			{
				ChangeCropStep(m_runtime.m_currentStep + 1);

				m_runtime.StartTimerForCurrentStep();	
			}
		}
		else if (m_runtime.m_currentStep == cropData.GetLastStepIndex())
		{
			// todo harvest
			ChangeCropType(CropTypes.Enum.None);
		}
	}

	public bool HasValidInteraction(FarmingTools.Tool tool)
	{
		if (!HasAnythingPlanted())
			return false;

		if (m_runtime.IsTimerEnabled())
			return false;

		CropData cropData = GetCropData();
		if (m_runtime.m_currentStep < cropData.GetLastStepIndex())
		{
			FarmingTools.Tool currentRequiredTool = cropData.m_steps[m_runtime.m_currentStep].m_requiredTool;
			return tool == currentRequiredTool;
		}

		return false;
	}

	public bool SaveState(Crop.SaveData data)
	{
		data.m_currentCropType = m_currentCropType;
		data.m_runtime = new CropRuntime.SaveData();
		return m_runtime.SaveState(data.m_runtime);
	}

	public bool LoadState(Crop.SaveData data)
	{
		m_currentCropType = data.m_currentCropType;
		if (m_currentCropType != CropTypes.Enum.None)
		{
			ChangeCropType(m_currentCropType);

			bool result = m_runtime.LoadState(data.m_runtime);
			if (!result)
				return false;

			ChangeCropStep(m_runtime.m_currentStep);
		}

		return true;
	}
}