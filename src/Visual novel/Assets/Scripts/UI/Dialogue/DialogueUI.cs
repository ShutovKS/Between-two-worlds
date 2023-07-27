using UnityEngine;

namespace UI.Dialogue
{
    public class DialogueUI : MonoBehaviour
    {
        [SerializeField] private GameObject _dialogueScreenGameObject;

        [SerializeField] private AnswerOptionsUI _answers;
        [SerializeField] private BackgroundUI _background;
        [SerializeField] private PersonAvatarUI _person;
        [SerializeField] private DialogueTextUI _dialogueText;

        public void SetActivePanel(bool value) => _dialogueScreenGameObject.SetActive(value);
    }
}