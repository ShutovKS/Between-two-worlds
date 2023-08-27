using System;

namespace Data.Dynamic
{
    [Serializable]
    public class DialoguesData
    {
        public DialoguesData()
        {
            idLastDialogue = "Start";
        }
        
        public string idLastDialogue;
    }
}