using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInventory : MonoBehaviour
{
    public FarmingTools.EFarmingTools currentTool;

    [SerializeField]
    private List<ToolData> ToolDataObjects;

    private Dictionary<FarmingTools.EFarmingTools, GameObject> spawnedTools = new Dictionary<FarmingTools.EFarmingTools, GameObject>();
    private Transform toolSocket;
    private InventoryPanel inventoryPanel;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        inventoryPanel = FindFirstObjectByType<InventoryPanel>();
        if (inventoryPanel == null)
        {
            Debug.LogError("Failed to find InventoryPanel");
            return;
        }

        toolSocket = GameObject.FindWithTag("ToolSocket").transform;
        if (toolSocket == null)
        {
            Debug.LogError("Failed to find ToolSocket");
            return;
        }

        foreach (ToolData toolData in ToolDataObjects)
        {
            if (toolData.tool == FarmingTools.EFarmingTools.None)
                continue;

            GameObject spawnedTool = Instantiate(toolData.prefab, toolSocket);
            spawnedTool.transform.localScale = new Vector3(1f / 100f, 1f / 100f, 1f / 100f);
            spawnedTool.SetActive(false);
            spawnedTools.Add(toolData.tool, spawnedTool);
        }
    }

    public void EquipTool(FarmingTools.EFarmingTools tool)
    {
        if (currentTool != FarmingTools.EFarmingTools.None)
            spawnedTools[currentTool].SetActive(false);
        
        spawnedTools[tool].SetActive(true);
        currentTool = tool;

        inventoryPanel.CycleImage();
    }

    public void InputNext(InputAction.CallbackContext callbackContext)
    {
        if (callbackContext.canceled)
            return;

        int current = (int)currentTool;
        int min = (int)FarmingTools.EFarmingTools.None + 1;
        int max = (int)FarmingTools.EFarmingTools.PlantingTool;
        FarmingTools.EFarmingTools nextTool = FarmingTools.EFarmingTools.None;

        if (current >= max)
        {
            nextTool = (FarmingTools.EFarmingTools)min;
        }
        else
        {
            nextTool = (FarmingTools.EFarmingTools)(current + 1);
        }

        EquipTool(nextTool);
    }

    public void InputPrevious(InputAction.CallbackContext callbackContext)
    {
        if (callbackContext.canceled)
            return;

        int current = (int)currentTool;
        int min = (int)FarmingTools.EFarmingTools.None + 1;
        int max = (int)FarmingTools.EFarmingTools.PlantingTool;
        FarmingTools.EFarmingTools nextTool = FarmingTools.EFarmingTools.None;

        if (current <= min)
        {
            nextTool = (FarmingTools.EFarmingTools)max;
        }
        else if (current == 0)
        {
            nextTool = (FarmingTools.EFarmingTools)max;
        }
        else
        {
            nextTool = (FarmingTools.EFarmingTools)(current - 1);
        }

        EquipTool(nextTool);
    }
}
