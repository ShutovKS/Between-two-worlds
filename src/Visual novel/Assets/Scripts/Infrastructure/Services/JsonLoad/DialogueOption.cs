using JetBrains.Annotations;

namespace Infrastructure.Services.JsonLoad
{
    [System.Serializable]
    public class DialogueOption
    {
        public string text;
        [CanBeNull] public string next_dialogue_id;
    }
}