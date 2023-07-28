using JetBrains.Annotations;

namespace Data.Dialogue
{
    [System.Serializable]
    public class DialogueOption
    {
        public string text;
        [CanBeNull] public string next_dialogue_id;
    }
}