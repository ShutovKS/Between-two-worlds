using System.Threading.Tasks;
using Infrastructure.PSM.Core;
using Infrastructure.Services.Authenticate;
using Infrastructure.Services.Progress;
using Infrastructure.Services.ScreenshotsOfSaves;
using UnityEngine;
using YG;

namespace Infrastructure.PSM.States
{
    public class InitializationState : IState<Bootstrap>, IEnterable
    {
        public InitializationState(Bootstrap initializer, IProgressService progressService,
            IScreenshotsOfSavesService screenshotsOfSavesService, IAuthenticateService authenticateService)
        {
            _progressService = progressService;
            _screenshotsOfSavesService = screenshotsOfSavesService;
            _authenticateService = authenticateService;
            Initializer = initializer;
        }

        public Bootstrap Initializer { get; }
        private readonly IProgressService _progressService;
        private readonly IScreenshotsOfSavesService _screenshotsOfSavesService;
        private readonly IAuthenticateService _authenticateService;

        public async void OnEnter()
        {
            await Authenticated();
            await DataInitialization();

            ContinueWork();
        }

        private async Task Authenticated()
        {
            await _authenticateService.Login();
            YandexGame.GameReadyAPI();
        }

        private void ContinueWork()
        {
            Initializer.StateMachine.SwitchState<MenuState>();
        }

        private async Task DataInitialization()
        {
            var data = _progressService.GetProgress();

            foreach (var dialoguesData in data.dialogues)
            {
                if (!dialoguesData.isDataExist || dialoguesData.Background != null)
                {
                    continue;
                }

                var texture2D = await _screenshotsOfSavesService.GetImage(dialoguesData.idLastDialogue);

                dialoguesData.Background = texture2D;
            }

            _progressService.SetProgress(data);
        }
    }
}