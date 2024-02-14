#region

using UnityEngine;
using UnityEngine.SceneManagement;

#endregion

namespace Infrastructure.ScenesManagers.Bootstrap
{
    public class Bootstrap : MonoBehaviour
    {
        private void Start()
        {
            GameLoading();
        }

        private void GameLoading()
        {
            SceneManager.LoadScene("1.Loading");
        }
    }
}