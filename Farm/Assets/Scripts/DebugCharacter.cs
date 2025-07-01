using UnityEngine;

public class DebugCharacter : MonoBehaviour
{
	public MapGrid Grid;

	[Header("Debug Action")]
	public bool DoInteract = false;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
		if (DoInteract) 
		{ 
			Interact();
			DoInteract = false;
		}
    }

	void Interact()
	{ 
	}
}
