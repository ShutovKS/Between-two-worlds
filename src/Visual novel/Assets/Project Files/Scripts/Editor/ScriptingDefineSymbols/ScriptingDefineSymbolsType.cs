namespace Editor.ScriptingDefineSymbols
{
    public enum ScriptingDefineSymbolsType
    {
        YandexGameSDK, 
    }

    public static class ScriptingDefineSymbols
    {
        public static string GetDefineSymbol(ScriptingDefineSymbolsType type)
        {
            return type switch
            {
                ScriptingDefineSymbolsType.YandexGameSDK => "YG_SERVICES",
                _ => string.Empty
            };
        }
    }
}