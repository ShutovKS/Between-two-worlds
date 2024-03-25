using Infrastructure.PSM.Core;
using Infrastructure.Services.Localisation;
using Infrastructure.Services.WindowsService;
using UI.ChooseLanguage;

namespace Infrastructure.PSM.States
{
    public class LanguageSelectionState : IState<Bootstrap>, IEnterable, IExitable
    {
        public LanguageSelectionState(Bootstrap initializer, IWindowService windowService,
            ILocalisationService localisationService)
        {
            _windowService = windowService;
            _localisationService = localisationService;
            Initializer = initializer;
        }

        public Bootstrap Initializer { get; }
        private readonly IWindowService _windowService;
        private readonly ILocalisationService _localisationService;
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

            Initialization();
        }

        private void Initialization()
        {
            // Initializer.StateMachine.SwitchState<>;
        }
    }
}