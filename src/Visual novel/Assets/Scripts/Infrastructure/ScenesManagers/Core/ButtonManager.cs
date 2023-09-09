using UI.Dialogue;
using UnityEngine.Events;

namespace Infrastructure.ScenesManagers.Core
{
    public class ButtonManager
    {
        public ButtonManager(ButtonsUI buttons)
        {
            _buttons = buttons;
        }

        private readonly ButtonsUI _buttons;

        public void RegisterOnClickBack(UnityAction onClickBack) =>
            _buttons.RegisterBackButtonCallback(onClickBack);

        public void RegisterOnClickSave(UnityAction onClickSave) =>
            _buttons.RegisterSaveButtonCallback(onClickSave);

        public void RegisterOnClickLoad(UnityAction onClickLoad) =>
            _buttons.RegisterLoadButtonCallback(onClickLoad);

        public void RegisterOnClickSettings(UnityAction onClickSettings) =>
            _buttons.RegisterSettingsButtonCallback(onClickSettings);

        public void RegisterOnClickHistory(UnityAction onClickHistory) =>
            _buttons.RegisterHistoryButtonCallback(onClickHistory);

        public void RegisterOnClickSpeedUp(UnityAction onClickSpeedUp) =>
            _buttons.RegisterSpeedUpButtonCallback(onClickSpeedUp);

        public void RegisterOnClickAuto(UnityAction onClickAuto) =>
            _buttons.RegisterAutoButtonCallback(onClickAuto);

        public void RegisterOnClickFurther(UnityAction onClickFurther) =>
            _buttons.RegisterFurtherButtonCallback(onClickFurther);
    }
}