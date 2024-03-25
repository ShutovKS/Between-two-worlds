using System;
using UnityEngine;
using UnityEngine.UI;

namespace Features.Services.WindowsService
{
    [Serializable]
    public class StateWindowButton
    {
        [field: SerializeField] public ButtonState ButtonState { get; private set; }
        [field: SerializeField] public Button Button { get; private set; }
        [field: SerializeField] public WindowID WindowID { get; private set; }
    }
}