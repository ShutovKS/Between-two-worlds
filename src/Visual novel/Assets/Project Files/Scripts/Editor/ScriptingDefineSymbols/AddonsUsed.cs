namespace Editor.ScriptingDefineSymbols
{
    public static class AddonsUsed
    {
        public static string GetAddonsUsed(AddonsUsedType type)
        {
            return type switch
            {
                AddonsUsedType.None => string.Empty,
                AddonsUsedType.YandexGameSDK => "YG_PLUGIN_YANDEX_GAME;YG_SERVICES",
                _ => string.Empty
            };
        }
        
        public static string GetAddonsUsedName(AddonsUsedType type)
        {
            return type switch
            {
                AddonsUsedType.None => string.Empty,
                AddonsUsedType.YandexGameSDK => "Yandex Game",
                _ => string.Empty
            };
        }

        #region Add-ons collection

#if YG_SERVICES
#endif
        
        #endregion
    }
}