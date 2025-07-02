using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public FarmingTools.EFarmingTools currentTool;

    [SerializeField]
    private List<ToolData> ToolDataObjects;

    private Transform toolSocket;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        toolSocket = GameObject.FindWithTag("ToolSocket").transform;
        if (toolSocket == null)
        {
            Debug.LogError("Failed to find ToolSocket");
        }

        EquipTool(FarmingTools.EFarmingTools.Hoe);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void EquipTool(FarmingTools.EFarmingTools tool)
    {
        foreach (ToolData toolData in ToolDataObjects)
        {
            if (toolData.tool == tool)
            {
                GameObject spawnedTool = Instantiate(toolData.prefab, toolSocket);
                spawnedTool.transform.localScale = new Vector3(1f / 100f, 1f / 100f, 1f / 100f);
                currentTool = tool;
                break;
            }
        }
    }
}
