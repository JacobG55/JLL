using LethalLevelLoader;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Video;

namespace WesleyMoonScripts.Components
{
    public class LevelCassetteItem : MonoBehaviour
    {
        [Header("Cassette Loader")]
        public bool DestroyAfterUse = false;
        public string LoaderName = "Galetry";
        public VideoClip Video;
        public AudioSource AudioLog;

        [Header("Cassette Data")]
        public string LevelSceneName = "";
        public bool unlockLevel = true;

        public UnityEvent OnCollect = new UnityEvent();

        public bool GetExtendedLevel(out ExtendedLevel extendedLevel)
        {
            extendedLevel = PatchedContent.ExtendedLevels.FirstOrDefault((x) => x.SelectableLevel.sceneName == LevelSceneName);
            if (extendedLevel == null)
            {
                extendedLevel = PatchedContent.ExtendedLevels[0];
                return false;
            }
            return true;
        }
    }
}
