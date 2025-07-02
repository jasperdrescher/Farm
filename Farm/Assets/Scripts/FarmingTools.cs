using UnityEngine;

public class FarmingTools : MonoBehaviour
{
	public enum EFarmingTools
	{ 
		None,
        Hoe,
		Shovel,
		WateringPot,
		Sickle,
		PlantingTool,
	}

	// GrassField --[Shovel]--> Farming Plot --[PlantingTool]-> Crop Step 0 --[Watering]-> ...repeat + time --[Sickle]-> Loot

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
