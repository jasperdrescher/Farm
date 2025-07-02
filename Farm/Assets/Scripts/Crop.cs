using UnityEngine;

[ExecuteInEditMode]
public class Crop : MonoBehaviour
{
	public GameObject[] CropPrefabs;

	public CropType.ECropType CurrentCropType;

	CropType m_CurrentCropType = null;

	void Start()
	{
		ChangeCrop(CropType.ECropType.None);
	}

	void Update()
	{

	}

	public void Init()
	{
		ChangeCrop(CropType.ECropType.None);
	}

	void ChangeCrop(CropType.ECropType Type)
	{
		CurrentCropType = Type;
		m_CurrentCropType = null;

		foreach (GameObject c in CropPrefabs)
		{
			CropType CT = c.GetComponent<CropType>();
			c.SetActive(CT != null && CT.Type == Type);

			if(c.activeSelf)
				m_CurrentCropType = CT;
		}
	}

	public CropType GetCropType()
	{
		return m_CurrentCropType;
	}
}