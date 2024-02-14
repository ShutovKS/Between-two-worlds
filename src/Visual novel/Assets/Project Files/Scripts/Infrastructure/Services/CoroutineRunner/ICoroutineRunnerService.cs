#region

using System.Collections;
using UnityEngine;

#endregion

namespace Infrastructure.Services.CoroutineRunner
{
    public interface ICoroutineRunnerService
    {
        Coroutine StartCoroutine(IEnumerator enumerator);
        void StopCoroutine(Coroutine coroutine);
    }
}