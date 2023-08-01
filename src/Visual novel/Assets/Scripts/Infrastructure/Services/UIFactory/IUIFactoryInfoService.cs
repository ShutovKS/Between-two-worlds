using UI.Background;
using UI.ChooseLanguage;
using UI.Dialogue;
using UI.MainMenu;
using UI.Settings;

namespace Infrastructure.Services.UIFactory
{
    public interface IUIFactoryInfoService
    {
        DialogueUI DialogueUI { get; }
        MainMenuUI MainMenuUI { get; }
        SettingsUI SettingsUI { get; }
        BackgroundUI BackgroundUI { get; }
        ChooseLanguageUI ChooseLanguageUI { get; }
    }
}