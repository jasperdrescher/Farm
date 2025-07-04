using UnityEngine;

public class CropRuntime : MonoBehaviour
{
	public int m_currentStep = 0;

	private bool m_timerEnabled = false;
	private float m_elapsedTime = 0.0f;

	private Crop m_ownerCrop = null;
	private CropData m_data = null;

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

	void StartTimer()
	{
		m_timerEnabled = true;
		m_elapsedTime = 0.0f;
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
		if(m_timerEnabled)
			m_elapsedTime += Time.deltaTime;
	}
}
