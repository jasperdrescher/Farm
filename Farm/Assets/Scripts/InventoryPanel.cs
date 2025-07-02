using UnityEngine;
using UnityEngine.UI;

public class InventoryPanel : MonoBehaviour
{
    [SerializeField]
    private GameObject HoeImageObject;

    [SerializeField]
    private GameObject ShovelImageObject;

    [SerializeField]
    private GameObject WateringPotImageObject;

    [SerializeField]
    private GameObject SickleImageObject;

    [SerializeField]
    private GameObject PlantingToolImageObject;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void CycleImage()
    {
        HoeImageObject.GetComponent<Image>().color = Color.yellow;
    }
}
