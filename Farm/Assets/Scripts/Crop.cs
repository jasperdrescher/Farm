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
	[Range(0f, 1f)] public float m_editorCropProgressChanger = 1.0f;

	#region private
	private CropRuntime m_runtime = null;
	private MapTile m_ownerMapTile = null;
	private CropTypes.Enum m_currentCropType = CropTypes.Enum.None;
	private Dictionary<CropTypes.Enum, List<GameObject>> m_visuals = new Dictionary<CropTypes.Enum, List<GameObject>>();

	private float m_editorCropProgressChangerBuffer = 1.0f;
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

		HandleCropGrowthTimer();
		HandleCropGrowVisual();
	}

	public void CreateCropTypes()
	{
		float tileHeight = m_ownerMapTile.GetTileHeight();

		foreach (CropData cropData in m_cropTypes)
		{
			if (cropData.m_type == CropTypes.Enum.None)
				continue;

			List<GameObject> visuals = new List<GameObject>();
			foreach (GameObject step in cropData.m_steps)
			{
				GameObject go = Instantiate(step, transform);
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

		if (m_runtime && m_editorCropProgressChanger != m_editorCropProgressChangerBuffer)
		{
			m_editorCropProgressChangerBuffer = m_editorCropProgressChanger;
			ChangeCropProgress(m_editorCropProgressChanger);
		}
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
			m_editorCropProgressChanger = 0.0f;
			return;
		}

		m_currentCropType = type;
		m_editorCropTypeChanger = type;

		m_runtime.m_currentStep = 0;
		m_editorCropProgressChanger = 0.0f;

		m_runtime.Init(GetCropData());
	}

	public void ChangeCropProgress(float progress)
	{
		float p = Mathf.Clamp01(progress);

#if UNITY_EDITOR
		m_editorCropProgressChanger = p;
		m_editorCropProgressChangerBuffer = p;
#endif

		if(m_runtime != null)
			m_runtime.OverrideTimer(p);
	}

	private bool IsStepActive(int step)
	{
		if (m_currentCropType == CropTypes.Enum.None || m_visuals.ContainsKey(m_currentCropType) == false)
			return false;

		if (step < 0 || step >= m_visuals[m_currentCropType].Count)
			return false;

		return m_visuals[m_currentCropType][step].activeSelf;
	}

	private void ChangeCropStep(int step)
	{
		if (m_currentCropType == CropTypes.Enum.None)
		{
			m_runtime.m_currentStep = 0;
			return;
		}

		m_runtime.m_currentStep = Mathf.Clamp(step, 0, m_visuals[m_currentCropType].Count - 1);

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

	public InteractionResult PlantCrop(InventoryItem inventoryItem)
	{
		if (HasAnythingPlanted())
			return TileInteractionResult.Failure();

		if (inventoryItem.m_itemType != InventoryItem.Type.Seed)
			return TileInteractionResult.Failure();

		ChangeCropType(inventoryItem.m_cropType);

		if (m_runtime == null || m_runtime.m_data == null)
		{
			Debug.Log("Something off with planted crop...");
			return TileInteractionResult.Success();
		}

		return TileInteractionResult.SuccessWithConsume(m_runtime.m_data.m_seedItem);
	}

	public InteractionResult Interact(InteractionContext context)
	{
		if (!HasAnythingPlanted())
			return InteractionResult.Failure();

		CropData cropData = GetCropData();
		if (m_runtime != null && m_runtime.GetTimerProgress() == 1.0f)
		{
			CropInteractionResult result = CropInteractionResult.SuccessfullHarvest(m_runtime.m_data);

			ChangeCropType(CropTypes.Enum.None);

			return result;
		}

		return InteractionResult.Failure();
	}

	public bool CanUseTool(FarmingTools.Tool tool)
	{
		if (!HasAnythingPlanted() && tool == FarmingTools.Tool.PlantingTool)
			return true;

		if (m_runtime != null && m_runtime.GetTimerProgress() == 1.0f)
		{
			FarmingTools.Tool currentRequiredTool = m_runtime.m_data.m_harvestTool;
			return tool == currentRequiredTool;
		}

		return false;
	}

	public bool HasValidInteraction(InteractionContext context)
	{
		if (HasAnythingPlanted())
		{
			if (m_runtime != null && m_runtime.GetTimerProgress() == 1.0f)
			{
				FarmingTools.Tool currentRequiredTool = m_runtime.m_data.m_harvestTool;
				return context.m_tool == currentRequiredTool;
			}
		}
		else
		{
			if (context.m_tool == FarmingTools.Tool.PlantingTool)
			{
				return context.m_inventoryItem.m_itemType == InventoryItem.Type.Seed;
			}
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
			{
				Debug.LogError("Faield to load Crop runtime for Tile:" + GetOwnerTileIndex());
				return false;
			}
		}

		return true;
	}

	void HandleCropGrowthTimer()
	{
		if (m_runtime == null || m_ownerMapTile == null)
			return;

		if (!HasAnythingPlanted()) 
			return;

		m_runtime.SetTimerPaused(!m_ownerMapTile.IsGroundWet());		
	}

	void HandleCropGrowVisual()
	{
		if (m_runtime == null || m_ownerMapTile == null)
			return;

		if (!HasAnythingPlanted())
			return;

		CropData data = m_runtime.m_data;
		if (data.m_steps.Length == 0)
			return;

		int targetStep = 0;
		if (data.m_steps.Length > 2) 
		{
			//we need at least 3... 0 only show for 0%, last only for 100%. distribute others between

			float p = m_runtime.GetTimerProgress();
			if (p > 0.0f && p < 1.0f)
			{
				int s = data.m_steps.Length - 2;
				float sections = 1.0f / s;
				for (int i = 0; i < s; i++)
				{
					if (p > i * sections)
						targetStep = i + 1;
					else 
						break;
				}
			}
			else if(p == 1.0f)
			{
				targetStep = data.m_steps.Length - 1;
			}
		}

		if (!IsStepActive(targetStep))
		{
			ChangeCropStep(targetStep);
		}
	}

	public int GetOwnerTileIndex()
	{ 
		return m_ownerMapTile != null ? m_ownerMapTile.GetIndex() : -1;
	}
}