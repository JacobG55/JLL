using JLL.API;
using UnityEngine;

namespace JLL.Components
{
    public class MagicWesley : MonoBehaviour
    {
        public string favoriteQuote = "";
        public bool inRubberRoom = false;
        public bool withRats = true;

        public void Start ()
        {
            if (JLL.purgeWesley.Value)
            {
                Kill();
                return;
            }

            if (!inRubberRoom && favoriteQuote != "") WesleyLog(favoriteQuote);
            if (withRats && Random.Range(0f,10f) < 1.5f)
            {
                WesleyLog("This is why I haven't updated my moons...");
            }
        }

        public void UpdateYourMoons()
        {
            WesleyLog("soon tm");
        }

        public void PleaseSay(string txt)
        {
            if (Random.Range(0f, 10f) < 1f)
            {
                WesleyLog("No");
            }
            else
            {
                WesleyLog(txt);
            }
        }

        public void Kill()
        {
            JLogHelper.LogInfo("Removing Wesley.", JLogLevel.User);
            WesleyLog("NOOOO-");
            Destroy(this);
        }

        private static void WesleyLog(string msg)
        {
            if (JLogHelper.AcceptableLogLevel(JLogLevel.User))
            {
                JLL.Instance.wesley.LogInfo(msg);
            }
        } 
    }
}
