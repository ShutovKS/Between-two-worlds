using Units.Tools;
using UnityEngine;
using UnityEngine.UI;

namespace UI.MainMenu
{
    public class BackgroundUI : MonoBehaviour
    {
        [SerializeField] private Image _backgroundImage;

        public void SetBackground(Texture2D texture2D) => _backgroundImage.sprite = texture2D.ToSprite();
    }
}