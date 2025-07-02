using UnityEngine;

[ExecuteInEditMode]
public class GameDebug : MonoBehaviour
{
	public int FPSLimiter = 60;

    void Start()
    {
        
    }

    void Update()
    {
		Application.targetFrameRate = FPSLimiter;
    }
}
