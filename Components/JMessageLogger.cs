using JLL.API;
using UnityEngine;

namespace JLL.Components
{
    public class JMessageLogger : MonoBehaviour
    {
        public string logName = "";

        public void LogMessage(string message)
        {
            string location = "";
            if (StartOfRound.Instance.shipHasLanded)
            {
                location = $"{RoundManager.Instance.currentLevel.PlanetName}";
            }
            JLL.Instance.mls.LogMessage($"({location})<{logName}>: {message}");
        }

        public void SendTip(string message)
        {
            JHudHelper.QueueDisplayTip(logName, message);
        }

        public void SendWarningTip(string message)
        {
            JHudHelper.QueueDisplayTip(logName, message, isWarning: true);
        }

        public void SendChatMessage(string message)
        {
            HUDManager.Instance.AddTextToChatOnServer(message);
        }
    }
}
