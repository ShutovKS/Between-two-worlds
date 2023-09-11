using UI.Dialogue;

namespace Infrastructure.ScenesManagers.Core
{
	public class HistoryManager
	{
		public HistoryManager(HistoryUI historyUI)
		{
			_historyUI = historyUI;
		}

		private readonly HistoryUI _historyUI;

		public void OpenDialogHistory()
		{
			_historyUI.SetActivePanel(true);
			_historyUI.RegisterBackButtonCallback(() => _historyUI.SetActivePanel(false));
		}

		public void AddedDialogInHistory(string id,string name, string text)
		{
			_historyUI.CreateHistoryPhrase(id, name, text);
		}
	}
}