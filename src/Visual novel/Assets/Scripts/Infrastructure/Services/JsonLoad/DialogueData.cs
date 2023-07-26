using JetBrains.Annotations;

namespace Infrastructure.Services.JsonLoad
{
    [System.Serializable]
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