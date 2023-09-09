#region

using System.Collections;
using UnityEngine;

#endregion

namespace Infrastructure.Services.CoroutineRunner
{
	public interface ICoroutineRunner
	{
		Coroutine StartCoroutine(IEnumerator enumerator);
		void StopCoroutine(Coroutine coroutine);
	}
}