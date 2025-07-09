using System;
using UnityEngine;

public class CropRuntime : MonoBehaviour
{
	public int m_currentStep = 0;

	private bool m_timerEnabled = false;
	private float m_elapsedTime = 0.0f;
	private float m_targetTime = 0.0f;

	private Crop m_ownerCrop = null;
	public CropData m_data = null;


	[Serializable]
	public class SaveData
	{
		public int m_currentStep = 0;
		public float m_timerProgress = -1.0f;
	}

	void Start()
    {
		Reset();
		m_ownerCrop = GetComponent<Crop>();
	}

    void Update()
    {
        HandleTimer();
    }

	void Reset()
	{
		m_currentStep = 0;
		ResetTimer();
	}

	public void Init(CropData cropData)
	{ 
		m_data = cropData;
	}

	public void StartTimerForCurrentStep()
	{
		float t = m_data.m_steps[m_currentStep].m_timeUntilNextStep;
		StartTimer(t);
	}

	void StartTimer(float time)
	{
		m_timerEnabled = time > 0.0f;
		m_elapsedTime = 0.0f;
		m_targetTime = time;
	}

	void StopTimer()
	{
		m_timerEnabled = false;
	}

	void ResetTimer()
	{
		StopTimer();
		m_elapsedTime = 0.0f;
	}

	void HandleTimer()
	{
		if (m_timerEnabled)
		{
			m_elapsedTime += Time.deltaTime;

			if (m_elapsedTime >= m_targetTime)
			{
				ResetTimer();
				OnTimerEnd();
			}
		}
	}

	public bool IsTimerEnabled()
	{
		return m_timerEnabled;
	}

	public float GetTimerProgress()
	{
		return m_timerEnabled ? m_elapsedTime / m_targetTime : 0.0f;
	}

	void OnTimerEnd()
	{ 
	}

	public bool SaveState(CropRuntime.SaveData data)
	{
		data.m_currentStep = m_currentStep;
		data.m_timerProgress = m_timerEnabled ? GetTimerProgress() : -1.0f;

		return true;
	}

	public bool LoadState(CropRuntime.SaveData data)
	{
		int step = data.m_currentStep;
		if (step < 0 || step > m_data.GetLastStepIndex())
			return false;

		m_currentStep = step;

		if (data.m_timerProgress >= 0.0f)
		{
			StartTimerForCurrentStep();
			m_elapsedTime = m_targetTime * data.m_timerProgress;
		}

		return true;
	}
}
