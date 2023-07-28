using System;
using JetBrains.Annotations;

namespace Data.Dialogue
{
    [Serializable]
    public class DialogueData
    {
        public string id;
        public string text;
        public string character;
        public string image;
        [CanBeNull] public string next_dialogue_id;
        [CanBeNull] public DialogueOption[] options;
    }
}