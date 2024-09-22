using JLL.API;
using UnityEngine;

namespace JLL.Components
{
    public class JMessageLogger : MonoBehaviour
    {
        [Tooltip("Used as the header of HUD Tips/Warnings & the name provided in logs after the location name")]
        public string logName = "";

        public void LogMessage(string message)
        {
            string location = "";
            if (StartOfRound.Instance.shipHasLanded)
            {
                location = $"{RoundManager.Instance.currentLevel.PlanetName}";
            }
            JLogHelper.LogInfo($"({location})<{logName}>: {message}", JLogLevel.User);
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
