using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInventory : MonoBehaviour
{
    [SerializeField]
    private FarmingTools.Tool m_currentTool;

    public List<ToolData> m_toolDataObjects;

    private Dictionary<FarmingTools.Tool, GameObject> m_spawnedTools = new Dictionary<FarmingTools.Tool, GameObject>();
    private Transform m_toolSocket;
    private InventoryPanel m_inventoryPanel;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        m_inventoryPanel = FindFirstObjectByType<InventoryPanel>();
        if (m_inventoryPanel == null)
        {
            Debug.LogError("Failed to find InventoryPanel");
            return;
        }

        m_toolSocket = GameObject.FindWithTag("ToolSocket").transform;
        if (m_toolSocket == null)
        {
            Debug.LogError("Failed to find ToolSocket");
            return;
        }

        foreach (ToolData toolData in m_toolDataObjects)
        {
            if (toolData.m_tool == FarmingTools.Tool.None)
                continue;

            GameObject spawnedTool = Instantiate(toolData.m_prefab, m_toolSocket);
            spawnedTool.transform.localScale = new Vector3(1f / 100f, 1f / 100f, 1f / 100f);
            spawnedTool.SetActive(false);
            m_spawnedTools.Add(toolData.m_tool, spawnedTool);
        }

        EquipTool(FarmingTools.Tool.None);
    }

	public FarmingTools.Tool GetCurrentTool()
	{
		return m_currentTool;
	}

	public void EquipTool(FarmingTools.Tool tool)
    {
        if (m_currentTool != FarmingTools.Tool.None)
        {
            m_spawnedTools[m_currentTool].SetActive(false);
        }

        if (tool != FarmingTools.Tool.None)
        {
            m_spawnedTools[tool].SetActive(true);
        }

        m_currentTool = tool;

        m_inventoryPanel.CycleImage(tool);
    }

    public void InputNext(InputAction.CallbackContext callbackContext)
    {
        if (!callbackContext.performed)
            return;

        int current = (int)m_currentTool;
        int min = (int)FarmingTools.Tool.None;
        int max = (int)FarmingTools.Tool.PlantingTool;
        FarmingTools.Tool nextTool = FarmingTools.Tool.None;

        if (current == max)
        {
            nextTool = (FarmingTools.Tool)min;
        }
        else
        {
            nextTool = (FarmingTools.Tool)(current + 1);
        }

        EquipTool(nextTool);
    }

    public void InputPrevious(InputAction.CallbackContext callbackContext)
    {
        if (!callbackContext.performed)
            return;

        int current = (int)m_currentTool;
        int min = (int)FarmingTools.Tool.None;
        int max = (int)FarmingTools.Tool.PlantingTool;
        FarmingTools.Tool nextTool = FarmingTools.Tool.None;

        if (current == min)
        {
            nextTool = (FarmingTools.Tool)max;
        }
        else
        {
            nextTool = (FarmingTools.Tool)(current - 1);
        }

        EquipTool(nextTool);
    }
}
