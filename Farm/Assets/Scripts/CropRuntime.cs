using System;
using UnityEngine;
using static UnityEngine.Analytics.IAnalytic;

public class CropRuntime : MonoBehaviour
{
	public int m_currentStep = 0;

	[SerializeField] private bool m_timerEnabled = false;
	[SerializeField] private float m_elapsedTime = 0.0f;
	[SerializeField] private float m_targetTime = -1.0f;

	private Crop m_ownerCrop = null;
	public CropData m_data = null;


	[Serializable]
	public class SaveData
	{
		public int m_currentStep = 0;
		public float m_timerProgress = -1.0f;
	}

	[SerializeField] private float m_debug_progress = 0.0f;

	void Start()
    {
		Reset();
		m_ownerCrop = GetComponent<Crop>();
	}

    void Update()
    {
        HandleTimer();

#if UNITY_EDITOR
		m_debug_progress = GetTimerProgress();
#endif
	}

	public void Reset()
	{
		m_currentStep = 0;
		ResetTimer();
	}

	public void Init(CropData cropData)
	{ 
		m_data = cropData;
		ResetTimer();
	}

	public void StartTimerForCurrentCrop()
	{
		float t = m_data.m_growTime;
		m_timerEnabled = t > 0.0f;
		m_elapsedTime = 0.0f;
		m_targetTime = t;
	}

	public void SetTimerPaused(bool v)
	{
		if (m_targetTime < 0.0f)
			return;

		m_timerEnabled = !v;
	}

	void ResetTimer()
	{
		m_timerEnabled = false;
		m_elapsedTime = 0.0f;
		m_targetTime = -1.0f;
	}

	void HandleTimer()
	{
		if (m_timerEnabled)
			m_elapsedTime += Time.deltaTime;
	}

	public bool IsTimerEnabled()
	{
		return m_timerEnabled;
	}

	public float GetTimerProgress()
	{
		return Mathf.Clamp(m_elapsedTime / m_targetTime, 0.0f, 1.0f);
	}

	public void OverrideTimer(float progress)
	{
		float p = Mathf.Clamp(progress, 0.0f, 1.0f);
		m_elapsedTime = m_targetTime * p;
	}

	public bool SaveState(CropRuntime.SaveData data)
	{
		data.m_currentStep = m_currentStep;
		data.m_timerProgress = GetTimerProgress();

		return true;
	}

	public bool LoadState(CropRuntime.SaveData data)
	{
		if (m_data == null)
		{
			Debug.LogError("CropRuntime needs to be initialized, before load. Tile: " + m_ownerCrop.GetOwnerTileIndex());
			return false;
		}

		int step = data.m_currentStep;
		if (step < 0 || step > m_data.GetLastStepIndex())
			return false;

		m_currentStep = step;

		StartTimerForCurrentCrop();
		m_elapsedTime = m_targetTime * data.m_timerProgress;
		
		return true;
	}
}
