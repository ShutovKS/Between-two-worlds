using System;

namespace Infrastructure.ScenesManagers.Core
{
    public class ActionTriggerManager
    {
        public void HandleActionTrigger(string actionTrigger)
        {
            switch (actionTrigger)
            {
                case "end1":
                    ActionEnd1();
                    break;
                case "end2":
                    ActionEnd2();
                    break;
                default: throw new ArgumentOutOfRangeException(nameof(actionTrigger), actionTrigger, null);
            }
        }

        private void ActionEnd1()
        {}

        private void ActionEnd2()
        {}
    }
}
