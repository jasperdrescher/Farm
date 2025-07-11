using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CropPanel : MonoBehaviour
{
    public GameObject m_CropButtonPrefab;

    private Dictionary<CropTypes.Enum, GameObject> m_cropButtons = new Dictionary<CropTypes.Enum, GameObject>();

    void Start()
    {
        CreateInventoryPanels();
    }

    private void CreateInventoryPanels()
    {
        GameObject firstRowPanel = new("FirstRowPanel", typeof(RectTransform));
        firstRowPanel.layer = 5;
        firstRowPanel.transform.SetParent(transform, false);

        RectTransform firstRowRectTransform = firstRowPanel.GetComponent<RectTransform>();
        firstRowRectTransform.pivot = new Vector2(0.5f, 1f);
        firstRowRectTransform.anchorMin = new Vector2(0.5f, 1f);
        firstRowRectTransform.anchorMax = new Vector2(0.5f, 1f);
        firstRowRectTransform.anchoredPosition = new Vector2(0f, 0f);
        firstRowRectTransform.sizeDelta = new Vector2(740f, 110f);
            
        GameObject secondRowPanel = new("SecondRowPanel", typeof(RectTransform));
        secondRowPanel.layer = 5;
        secondRowPanel.transform.SetParent(transform, false);

        RectTransform secondRowRectTransform = secondRowPanel.GetComponent<RectTransform>();
        secondRowRectTransform.pivot = new Vector2(0.5f, 1f);
        secondRowRectTransform.anchorMin = new Vector2(0.5f, 1f);
        secondRowRectTransform.anchorMax = new Vector2(0.5f, 1f);
        secondRowRectTransform.anchoredPosition = new Vector2(0f, -110f);
        secondRowRectTransform.sizeDelta = new Vector2(740f, 110f);

        for (int i = 1; i < (int)CropTypes.Enum.Rice; i++)
        {
            GameObject cropButton = Instantiate(m_CropButtonPrefab, firstRowPanel.transform);

            RectTransform cropButtonRectTransform = cropButton.GetComponent<RectTransform>();
            cropButtonRectTransform.pivot = new Vector2(0f, 1f);
            cropButtonRectTransform.anchorMin = new Vector2(0f, 1f);
            cropButtonRectTransform.anchorMax = new Vector2(0f, 1f);
            cropButtonRectTransform.anchoredPosition = new Vector2(5f + (105f * i), -5f);
            cropButtonRectTransform.sizeDelta = new Vector2(100f, 100f);

            m_cropButtons.Add((CropTypes.Enum)i, cropButton);
        }

        for (int i = 0; i < 7; i++)
        {
            GameObject cropButton = Instantiate(m_CropButtonPrefab, secondRowPanel.transform);

            RectTransform cropButtonRectTransform = cropButton.GetComponent<RectTransform>();
            cropButtonRectTransform.pivot = new Vector2(0f, 1f);
            cropButtonRectTransform.anchorMin = new Vector2(0f, 1f);
            cropButtonRectTransform.anchorMax = new Vector2(0f, 1f);
            cropButtonRectTransform.anchoredPosition = new Vector2(5f + (105f * i), -5f);
            cropButtonRectTransform.sizeDelta = new Vector2(100f, 100f);
        }
    }

    public void ToggleInventory()
    {
        gameObject.SetActive(!gameObject.activeSelf);
    }

    public void RefreshPanels(List<InventoryItem> items)
    {
        m_cropButtons[items[0].m_cropType].GetComponent<Image>().sprite = items[0].m_thumbnail;
    }
}
