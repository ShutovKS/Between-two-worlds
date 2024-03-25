using System.Threading.Tasks;
using Infrastructure.PSM.Core;
using Infrastructure.Services.Progress;
using Infrastructure.Services.ScreenshotsOfSaves;

namespace Infrastructure.PSM.States
{
    public class InitializationState : IState<Bootstrap>, IEnterable
    {
        public InitializationState(Bootstrap initializer, IProgressService progressService,
            IScreenshotsOfSavesService screenshotsOfSavesService)
        {
            _progressService = progressService;
            _screenshotsOfSavesService = screenshotsOfSavesService;
            Initializer = initializer;
        }

        public Bootstrap Initializer { get; }
        private readonly IProgressService _progressService;
        private readonly IScreenshotsOfSavesService _screenshotsOfSavesService;

        public async void OnEnter()
        {
            await DataInitialization();

            ContinueWork();
        }

        private void ContinueWork()
        {
            // Initializer.StateMachine.SwitchState<>();
        }

        private async Task DataInitialization()
        {
            var data = _progressService.GetProgress();

            foreach (var dialoguesData in data.dialogues)
            {
                if (!dialoguesData.isDataExist)
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