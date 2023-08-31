using UnityEngine;

namespace UI.SaveLoad
{
    public class SaveLoadUI : MonoBehaviour
    {
        [SerializeField] private GameObject _saveLoadScreenGameObject;
        [field: SerializeField] public ButtonsUI ButtonsUI { get; private set; }
        [field: SerializeField] public SaveDataUI[] SaveDataUIs { get; private set; }

        public void SetActivePanel(bool isActive) => _saveLoadScreenGameObject.SetActive(isActive);
    }
}