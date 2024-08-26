using System.Collections.Generic;

namespace JLL.API
{
    public class JHudHelper
    {
        internal static bool isTipActive = false;
        private static List<DisplayTip> displayTipQueue = new List<DisplayTip>();

        public static void QueueDisplayTip(string headerText, string bodyText, bool isWarning = false, bool useSave = false, string prefsKey = "LC_Tip1")
        {
            displayTipQueue.Add(new DisplayTip(headerText, bodyText, isWarning, useSave, prefsKey));

            DisplayNextTip();
        }

        public static void ClearDisplayTipQueue()
        {
            displayTipQueue = new List<DisplayTip>();
        }

        internal static bool DisplayNextTip()
        {
            if (isTipActive)
            {
                return false;
            }

            if (displayTipQueue.Count > 0)
            {
                DisplayTip tip = displayTipQueue[0];
                displayTipQueue.RemoveAt(0);

                HUDManager.Instance.DisplayTip(tip.headerText, tip.bodyText, tip.isWarning, tip.useSave, tip.prefsKey);

                return true;
            }

            return false;
        }

        private class DisplayTip
        {
            public string headerText;
            public string bodyText;
            public bool isWarning;
            public bool useSave;
            public string prefsKey;
            public DisplayTip(string headerText, string bodyText, bool isWarning, bool useSave, string prefsKey)
            {
                this.headerText = headerText;
                this.bodyText = bodyText;
                this.isWarning = isWarning;
                this.useSave = useSave;
                this.prefsKey = prefsKey;
            }
        }
    }
}
