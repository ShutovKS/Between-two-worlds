#region

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

#endregion

namespace Unit.Tools.Extensions
{
    public static class ToolsExtensions
    {
        public static Sprite ToSprite(this Texture2D texture2D)
        {
            return texture2D == null
                ? null
                : Sprite.Create(texture2D, new Rect(0, 0, texture2D.width, texture2D.height), new Vector2(0.5f, 0.5f));
        }

        public static void RegisterNewCallback(this Button button, UnityAction action)
        {
            button.onClick.RemoveAllListeners();
            if (action != null)
            {
                button.onClick.AddListener(action);
            }
        }
    }
}