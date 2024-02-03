#region

using System.Threading.Tasks;

#endregion

namespace Infrastructure.Services.UIFactory
{
	public interface IUIFactoryService : IUIFactoryInfoService
	{
		Task CreatedMainMenuScreen();
		Task CreatedDialogueScreen();
		Task CreatedBackgroundScreen();
		Task CreatedChooseLanguageScreen();
		Task CreatedConfirmationScreen();
		Task CreatedSaveLoadScreen();
		Task CreatedLastWordsScreen();

		void DestroyMainMenuScreen();
		void DestroyDialogueScreen();
		void DestroyBackgroundScreen();
		void DestroyChooseLanguageScreen();
		void DestroyConfirmationScreen();
		void DestroySaveLoadScreen();
		void DestroyLastWordsScreen();
	}
}