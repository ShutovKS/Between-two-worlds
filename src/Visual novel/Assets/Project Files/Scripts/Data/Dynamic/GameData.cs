#region

using System;
using JetBrains.Annotations;

#endregion

namespace Data.Dynamic
{
    [Serializable]
    public class GameData
    {
        public GameData()
        {
            dialogues = new DialoguesData[6];
            for (var i = 0; i < dialogues.Length; i++)
            {
                dialogues[i] = new DialoguesData();
            }
        }

        public DialoguesData[] dialogues;
        [CanBeNull] public string currentDialogue;
    }
}