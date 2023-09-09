#region

using UI.Background;
using UI.ChooseLanguage;
using UI.Confirmation;
using UI.Dialogue;
using UI.MainMenu;
using UI.SaveLoad;
using UI.Settings;

#endregion

namespace Infrastructure.Services.UIFactory
{
	public interface IUIFactoryInfoService
	{
		DialogueUI DialogueUI { get; }
		MainMenuUI MainMenuUI { get; }
		SettingsUI SettingsUI { get; }
		BackgroundUI BackgroundUI { get; }
		ChooseLanguageUI ChooseLanguageUI { get; }
		ConfirmationUI ConfirmationUI { get; }
		SaveLoadUI SaveLoadUI { get; }
	}
}