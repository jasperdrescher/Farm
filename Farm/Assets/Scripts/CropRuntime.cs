using UnityEngine;

public class CropRuntime : MonoBehaviour
{
	int CurrentStep = 0;
	bool TimerEnabled = false;
	float ElapsedTime = 0.0f;

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
		CurrentStep = 0;
		ResetTimer();
	}

	void StartTimer()
	{
		TimerEnabled = true;
		ElapsedTime = 0.0f;
	}

	void StopTimer()
	{ 
		TimerEnabled = false;
	}

	void ResetTimer()
	{
		StopTimer();
		ElapsedTime = 0.0f;
	}

	void HandleTimer()
	{
		if(TimerEnabled)
			ElapsedTime += Time.deltaTime;
	}

	Crop GetCrop()
	{
		return transform.gameObject.GetComponent<Crop>();
	}

	CropType GetCropType()
	{
		return GetCrop().GetCropType();
	}
}
