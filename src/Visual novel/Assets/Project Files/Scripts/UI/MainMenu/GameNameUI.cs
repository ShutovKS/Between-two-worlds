#region

using TMPro;
using UnityEngine;

#endregion

namespace UI.MainMenu
{
    public class GameNameUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _gameNameText;

        public void SetGameName(string text)
        {
            _gameNameText.text = text;
        }
    }
}