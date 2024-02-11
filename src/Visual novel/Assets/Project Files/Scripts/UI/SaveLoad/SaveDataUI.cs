#region

using TMPro;
using Unit.Tools.Extensions;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

#endregion

namespace UI.SaveLoad
{
    public class SaveDataUI : MonoBehaviour
    {
        [SerializeField] private Button _button;
        [SerializeField] private TextMeshProUGUI _textTitle;
        [SerializeField] private Image _image;

        public void RegisterButtonCallback(UnityAction callback)
        {
            _button.RegisterNewCallback(callback);
        }

        public void SetTitle(string title)
        {
            _textTitle.text = title;
        }

        public void SetImage(Texture2D texture)
        {
            _image.sprite = texture.ToSprite();
        }
    }
}