using System.Collections.Generic;

namespace Infrastructure.Services.DialogueStories
{
    public class DialogueHistoryService : IDialogueHistoryService
    {
        private readonly List<DialogInHistory> _dialoguesHistory = new();

        public DialogInHistory? GetLatestDialogue()
        {
            return _dialoguesHistory.Count == 0 ? null : _dialoguesHistory[^1];
        }

        public List<DialogInHistory> GetHistory()
        {
            return _dialoguesHistory;
        }

        public void AddDialogue(string id, string name, string text)
        {
            _dialoguesHistory.Add(new DialogInHistory
            {
                id = id,
                name = name,
                text = text,
            });
        }

        public void Clear()
        {
            _dialoguesHistory.Clear();
        }
    }
}