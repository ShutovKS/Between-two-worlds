using System;

namespace Data.Dynamic
{
    [Serializable]
    public class DynamicData
    {
        public DynamicData()
        {
            dialogues = new DialoguesData();
        }
        
        public DialoguesData dialogues;
    }
}