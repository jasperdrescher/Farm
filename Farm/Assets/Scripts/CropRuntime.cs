using UnityEngine;

public class CropRuntime : MonoBehaviour
{
	int m_currentStep = 0;
	bool m_timerEnabled = false;
	float m_elapsedTime = 0.0f;

    void Start()
    {
		Reset();
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

	Crop GetCrop()
	{
		return transform.gameObject.GetComponent<Crop>();
	}

	/*CropType GetCropType()
	{
		return GetCrop().GetCropType();
	}*/
}
