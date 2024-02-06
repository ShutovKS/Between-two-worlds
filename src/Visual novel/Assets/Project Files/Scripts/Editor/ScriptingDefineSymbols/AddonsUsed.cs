namespace Editor.ScriptingDefineSymbols
{
    public static class AddonsUsed
    {
        public static string GetAddonsUsed(AddonsUsedType type)
        {
            return type switch
            {
                AddonsUsedType.None => string.Empty,
                AddonsUsedType.YandexGameSDK => "YG_SERVICES",
                _ => string.Empty
            };
        }

        #region Add-ons collection

#if YG_SERVICES
#endif
        
        #endregion
    }
}