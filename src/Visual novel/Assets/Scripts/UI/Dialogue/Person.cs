using UnityEngine;
using UnityEngine.UI;

namespace UI.Dialogue
{
    public class Person : MonoBehaviour
    {
        [field:SerializeField] public GameObject AvatarGO { get; private set; }
        [field:SerializeField] public Image AvatarImage { get; private set; }
    }
}