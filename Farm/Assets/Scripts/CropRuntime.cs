using System;
using UnityEngine;

public class CropRuntime : MonoBehaviour
{
	public int m_currentStep = 0;
	public CropData m_data = null;

	[Serializable]
	public class SaveData
	{
		public int m_currentStep = 0;
	}

	void Start()
    {
		Reset();
	}

    void Update()
    {

    }

	void Reset()
	{
		m_currentStep = 0;
	}

	public void Init(CropData cropData)
	{ 
		m_data = cropData;
	}

	public float GetTimeForCurrentStep()
	{
		return m_data.m_steps[m_currentStep].m_timeUntilNextStep;
	}
	public bool SaveState(CropRuntime.SaveData data)
	{
		data.m_currentStep = m_currentStep;

		return true;
	}

	public bool LoadState(CropRuntime.SaveData data)
	{
		int step = data.m_currentStep;
		if (step < 0 || step > m_data.GetLastStepIndex())
			return false;

		m_currentStep = step;

		return true;
	}
}
