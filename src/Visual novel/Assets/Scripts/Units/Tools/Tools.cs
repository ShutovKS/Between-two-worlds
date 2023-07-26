using UnityEngine;

namespace Units.Tools
{
    public static class Tools
    {
        public static Sprite ToSprite(this Texture2D texture2D)
        {
            return texture2D == null
                ? null
                : Sprite.Create(texture2D, new Rect(0, 0, texture2D.width, texture2D.height), new Vector2(0.5f, 0.5f));
        }
    }
}