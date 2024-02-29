#region

using UI.Dialogue;

#endregion

namespace Infrastructure.ScenesManagers.Core
{
    public class HistoryManager
    {
        public HistoryManager(HistoryUI historyUI)
        {
            _historyUI = historyUI;
            ClearHistory();
        }

        private readonly HistoryUI _historyUI;

        public void OpenDialogHistory()
        {
            _historyUI.SetActivePanel(true);
            _historyUI.OnBackButtonClicked = () => _historyUI.SetActivePanel(false);
        }

        public void AddedDialogInHistory(string id, string name, string text)
        {
            _historyUI.CreateHistoryPhrase(id, name, text);
        }

        public void ClearHistory()
        {
            _historyUI.ClearHistory();
        }
    }
}