using Infrastructure.PSM.Core;
using Infrastructure.Services.LocalisationDataLoad;
using Infrastructure.Services.WindowsService;
using UI.ChooseLanguage;

namespace Infrastructure.PSM.States
{
    public class LanguageSelectionState : IState<Bootstrap>, IEnterable, IExitable
    {
        public LanguageSelectionState(Bootstrap initializer, IWindowService windowService,
            ILocalisationDataLoadService localisationService)
        {
            _windowService = windowService;
            _localisationService = localisationService;
            Initializer = initializer;
        }

        public Bootstrap Initializer { get; }
        private readonly IWindowService _windowService;
        private readonly ILocalisationDataLoadService _localisationService;
        private ChooseLanguageUI _chooseLanguageUI;

        public void OnEnter()
        {
            OpenChooseLanguage();
        }

        public void OnExit()
        {
            _windowService.Close(WindowID.ChooseLanguage);
        }

        private async void OpenChooseLanguage()
        {
            _chooseLanguageUI = await _windowService.OpenAndGetComponent<ChooseLanguageUI>(WindowID.ChooseLanguage);

            var localizationsInfo = _localisationService.GetLocalizationsInfo();

            foreach (var localizationInfo in localizationsInfo)
            {
                _chooseLanguageUI.ScrollViewLanguages.AddLanguageInScrollView(
                    localizationInfo.Language,
                    localizationInfo.FlagImage);
            }

            _chooseLanguageUI.ScrollViewLanguages.OnSelectLanguage += SelectLanguage;
        }

        private void SelectLanguage(string language)
        {
            _localisationService.Load(language);

            _chooseLanguageUI.ScrollViewLanguages.OnSelectLanguage -= SelectLanguage;
            _chooseLanguageUI.SetActivePanel(false);

            Initialization(language);
        }

        private void Initialization(string language)
        {
            Initializer.StateMachine.SwitchState<InitializationState, string>(language);
        }
    }
}