#region

using System.Threading.Tasks;

#endregion

namespace Infrastructure.Services.UIFactory
{
	public interface IUIFactoryService : IUIFactoryInfoService
	{
		Task CreatedMainMenuScreen();
		Task CreatedSettingsScreen();
		Task CreatedDialogueScreen();
		Task CreatedBackgroundScreen();
		Task CreatedChooseLanguageScreen();
		Task CreatedConfirmationScreen();
		Task CreatedSaveLoadScreen();

		void DestroyMainMenuScreen();
		void DestroyDialogueScreen();
		void DestroySettingsScreen();
		void DestroyBackgroundScreen();
		void DestroyChooseLanguageScreen();
		void DestroyConfirmationScreen();
		void DestroySaveLoadScreen();
	}
}