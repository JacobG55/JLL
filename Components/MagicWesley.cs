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
            if (favoriteQuote != "") JLL.Instance.mls.LogInfo($"Webley: {favoriteQuote}");
            if (Random.Range(0,10) < 1.5)
            {
                JLL.Instance.mls.LogInfo("Webley: This is why I haven't updated my moons...");
            }
        }

        public void UpdateYourMoons()
        {
            JLL.Instance.mls.LogInfo("Webley: soon tm");
        }
    }
}
