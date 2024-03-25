using System;
using System.Collections.Generic;
using Features.Services.WindowsService;
using Infrastructure.Services.Sounds;
using UnityEngine;

namespace Features.UI.Scripts.Base
{
    public class BaseScreen : MonoBehaviour
    {
        public void Construct(IWindowService windowService, ISoundService soundService)
        {
            _windowService = windowService;
            _soundService = soundService;
        }

        [SerializeField] protected List<StateWindowButton> _buttons;
        protected IWindowService _windowService;
        protected ISoundService _soundService;

        protected virtual void Awake()
        {
            foreach (var stateWindowButton in _buttons)
            {
                stateWindowButton.Button.onClick.AddListener(() => OnButtonClicked(stateWindowButton));
            }
        }

        protected virtual void OnButtonClicked(StateWindowButton stateWindowButton)
        {
            switch (stateWindowButton.ButtonState)
            {
                case ButtonState.Open:
                    _windowService.Open(stateWindowButton.WindowID);
                    break;
                case ButtonState.Close:
                    _windowService.Close(stateWindowButton.WindowID);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        protected void OnDestroy()
        {
            foreach (var stateWindowButton in _buttons)
            {
                stateWindowButton.Button.onClick.RemoveListener(() => OnButtonClicked(stateWindowButton));
            }
        }
    }
}