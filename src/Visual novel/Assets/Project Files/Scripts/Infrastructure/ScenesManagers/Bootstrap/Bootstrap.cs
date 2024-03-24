#region

using UnityEngine;
using UnityEngine.SceneManagement;

#endregion

namespace Infrastructure.ScenesManagers.Bootstrap
{
    public class Bootstrap : MonoBehaviour
    {
        public void Start()
        {
            SceneManager.LoadScene("1.Loading");
        }
    }
}