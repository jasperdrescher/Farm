using UnityEngine;
using UnityEngine.UI;

public class InventoryPanel : MonoBehaviour
{
    [SerializeField]
    private GameObject m_hoeImageObject;

    [SerializeField]
    private GameObject m_shovelImageObject;

    [SerializeField]
    private GameObject m_wateringPotImageObject;

    [SerializeField]
    private GameObject m_sickleImageObject;

    [SerializeField]
    private GameObject m_plantingToolImageObject;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void CycleImage()
    {
        m_hoeImageObject.GetComponent<Image>().color = Color.yellow;
    }
}
