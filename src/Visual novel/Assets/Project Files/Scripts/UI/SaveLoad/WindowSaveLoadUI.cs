#region

using System;
using TMPro;
using Tools.Extensions;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

#endregion

namespace UI.SaveLoad
{
    public class WindowSaveLoadUI : MonoBehaviour
    {
        public Action OnButtonClicked;

        [SerializeField] private Button _button;
        [SerializeField] private TextMeshProUGUI _textTitle;
        [SerializeField] private Image _image;
        
        private void Awake()
        {
            _button.RegisterNewCallback(() => OnButtonClicked?.Invoke());
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