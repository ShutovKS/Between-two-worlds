#region

using UnityEngine;

#endregion

namespace Infrastructure.Services.CoroutineRunner
{
	public class CoroutineRunnerServiceService : MonoBehaviour, ICoroutineRunnerService
	{
		private void Awake()
		{
			DontDestroyOnLoad(gameObject);
		}
	}
}