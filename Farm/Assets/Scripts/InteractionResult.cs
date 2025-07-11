using UnityEngine;

public class InteractionResult : ScriptableObject
{
	public bool m_result = false;

	public static InteractionResult Success()
	{
		InteractionResult result = ScriptableObject.CreateInstance<InteractionResult>();
		result.m_result = true;
		return result;
	}

	public static InteractionResult Failure()
	{
		return ScriptableObject.CreateInstance<InteractionResult>();
	}
}
