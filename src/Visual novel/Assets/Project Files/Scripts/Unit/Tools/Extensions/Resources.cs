using UnityEngine;

namespace Tools.Extensions
{
    public static class Resources
    {
        public static Texture2D GetTexture2D(string path)
        {
            var texture2D = UnityEngine.Resources.Load<Texture2D>("Data/" + path);

            return texture2D;
        }
    }
}