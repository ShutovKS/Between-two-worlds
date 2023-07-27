using Units.Tools;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Background
{
    public class BackgroundUI : MonoBehaviour
    {
        [SerializeField] private Image _backgroundImage;

        public void SetBackgroundImage(Texture2D texture2D)
        {
            _backgroundImage.color = Color.white;
            _backgroundImage.sprite = texture2D.ToSprite();
        }

        public void SetBackgroundColor(Color color)
        {
            _backgroundImage.color = color;
            _backgroundImage.sprite = null;
        }
    }
}