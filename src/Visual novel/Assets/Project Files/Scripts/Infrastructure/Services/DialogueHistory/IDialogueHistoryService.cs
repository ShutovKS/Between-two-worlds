using System.Collections.Generic;

namespace Infrastructure.Services.DialogueStories
{
    public interface IDialogueHistoryService
    {
        DialogInHistory? GetLatestDialogue();
        List<DialogInHistory> GetHistory();
        void AddDialogue(string id, string name, string text);
        void Clear();
    }

    public struct DialogInHistory
    {
        public string id;
        public string name;
        public string text;
    }
}