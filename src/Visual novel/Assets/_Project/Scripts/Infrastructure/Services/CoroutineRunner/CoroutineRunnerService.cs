﻿#region

using UnityEngine;

#endregion

namespace Infrastructure.Services.CoroutineRunner
{
	public class CoroutineRunnerService : MonoBehaviour, ICoroutineRunner
	{
		private void Awake()
		{
			DontDestroyOnLoad(gameObject);
		}
	}
}