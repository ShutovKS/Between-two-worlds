#region

using UI.Background;
using UI.ChooseLanguage;
using UI.Confirmation;
using UI.Dialogue;
using UI.LastWords;
using UI.MainMenu;
using UI.SaveLoad;

#endregion

namespace Infrastructure.Services.UIFactory
{
    public interface IUIFactoryInfoService
    {
        DialogueUI DialogueUI { get; }
        MainMenuUI MainMenuUI { get; }
        BackgroundUI BackgroundUI { get; }
        ChooseLanguageUI ChooseLanguageUI { get; }
        ConfirmationUI ConfirmationUI { get; }
        SaveLoadUI SaveLoadUI { get; }
        LastWordsUI LastWordsUI { get; }
    }
}