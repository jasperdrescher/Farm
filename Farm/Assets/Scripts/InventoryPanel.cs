using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryPanel : MonoBehaviour
{
    [SerializeField]
    private GameObject m_toolImagePrefab;

    [SerializeField]
    private Dictionary<FarmingTools.Tool, GameObject> m_toolImages = new Dictionary<FarmingTools.Tool, GameObject>();

    private FarmingTools.Tool m_currentActiveTool;

    private void Start()
    {
        for (int i = 0; i <= (int)FarmingTools.Tool.PlantingTool; ++i)
        {
            GameObject image = Instantiate(m_toolImagePrefab, gameObject.transform);
            FarmingTools.Tool currentTool = (FarmingTools.Tool) i;
            image.GetComponentInChildren<TextMeshProUGUI>().text = currentTool.ToString();
            image.GetComponent<RectTransform>().position += new Vector3(85f * i, 0f, 0f);
            m_toolImages.Add((FarmingTools.Tool) i, image); 
        }
    }

    public void CycleImage(FarmingTools.Tool tool)
    {
        m_toolImages[m_currentActiveTool].GetComponent<Image>().color = Color.white;
        m_toolImages[tool].GetComponent<Image>().color = Color.yellow;
        m_currentActiveTool = tool;
    }
}
