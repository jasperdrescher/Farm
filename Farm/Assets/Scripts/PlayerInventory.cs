using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInventory : MonoBehaviour
{
    [SerializeField]
    private FarmingTools.Tool m_currentTool;

    [SerializeField]
    private InventoryItem m_TempDebugInventoryItem;

    public List<ToolData> m_toolDataObjects;

    private Dictionary<FarmingTools.Tool, GameObject> m_spawnedTools = new Dictionary<FarmingTools.Tool, GameObject>();
    private List<InventoryItem> m_inventoryItems = new List<InventoryItem>();
    private Transform m_toolSocket;
    private InventoryPanel m_inventoryPanel;
    private CropPanel m_cropPanel;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        m_inventoryPanel = FindFirstObjectByType<InventoryPanel>();
        if (m_inventoryPanel == null)
        {
            Debug.LogError("Failed to find InventoryPanel");
            return;
        }

        m_cropPanel = FindFirstObjectByType<CropPanel>();
        if (m_cropPanel == null)
        {
            Debug.LogError("Failed to find CropPanel");
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

        // TODO: Remove when we can pick up or load inventory items
        AddInventoyItem(m_TempDebugInventoryItem);
        m_cropPanel.gameObject.SetActive(false);
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

    public void InputToggleInventory(InputAction.CallbackContext callbackContext)
    {
        m_cropPanel.ToggleInventory();
    }

    public void AddInventoyItems(List<InventoryItem> items)
    {
        foreach (InventoryItem item in items)
        {
            AddInventoyItem(item);
        }
    }

    public void AddInventoyItem(InventoryItem item)
    {
        m_inventoryItems.Contains(item);
        foreach (InventoryItem inventoryItem in m_inventoryItems)
        {
            if (inventoryItem == item)
            {
                inventoryItem.m_amount++;
                return;
            }
        }

        m_inventoryItems.Add(item);
        m_cropPanel.RefreshPanels(m_inventoryItems);
    }
}
