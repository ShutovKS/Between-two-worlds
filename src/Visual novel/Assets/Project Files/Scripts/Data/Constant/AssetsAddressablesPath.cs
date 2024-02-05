namespace Data.Constant
{
	public static class AssetsAddressablesPath
	{
		public const string DIALOGUE_SCREEN = "Screens/Dialogue";
		public const string MAIN_MENU_SCREEN = "Screens/MainMenu";
		public const string BACKGROUND_SCREEN = "Screens/Background";
		public const string CHOOSE_LANGUAGE_SCREEN = "Screens/ChooseLanguage";
		public const string CONFIRMATION_SCREEN = "Screens/Confirmation";
		public const string SAVE_LOAD_SCREEN = "Screens/SaveLoad";
		public const string LAST_WORDS_SCREEN = "Screens/LastWords";

#if YG_SERVICES
		public const string YANDEX_GAME_PREFAB = "Prefabs/YandexGame";
#endif
	}
}