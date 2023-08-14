using UnityEngine;

namespace UI.Dialogue
{
    public class DialogueUI : MonoBehaviour
    {
        [SerializeField] private GameObject _dialogueScreenGameObject;

        [field: SerializeField] public AnswerOptionsUI Answers { get; private set; }
        [field: SerializeField] public PersonAvatarUI Person { get; private set; }
        [field: SerializeField] public DialogueTextUI DialogueText { get; private set; }

        public void SetActivePanel(bool value) => _dialogueScreenGameObject.SetActive(value);
    }
}