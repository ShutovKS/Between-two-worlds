#region

using System;
using UnityEngine;

#endregion

namespace Data.Dynamic
{
    [Serializable]
    public class DialoguesData
    {
        public string idLastDialogue;
        public string titleText;
        public bool isDataExist;
        [NonSerialized] public Texture2D Background;
    }
}