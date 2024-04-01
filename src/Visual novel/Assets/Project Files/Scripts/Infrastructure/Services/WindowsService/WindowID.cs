namespace Infrastructure.Services.WindowsService
{
    public enum WindowID
    {
        None = -1,
        Unknown = 0,

        Background = 1,
        ChooseLanguage = 2,
        MainMenu = 3,
        Dialogue = 4,
        Confirmation = 5,
        LastWords = 6,
        SaveLoad = 7,

        ImageCaptureForSave = 99,
    }
}