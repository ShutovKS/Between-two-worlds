using UnityEngine;

namespace Infrastructure.Services.UIFactory
{
    public interface IUIFactoryInfoService
    {
        GameObject DialogueScreen { get; }
        GameObject MainMenuScreen { get; }
        GameObject SettingsScreen { get; }
        GameObject BackgroundScreen { get; }
    }
}