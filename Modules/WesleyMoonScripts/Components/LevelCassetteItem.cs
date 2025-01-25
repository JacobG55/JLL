using LethalLevelLoader;
using System;
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
            for (int i = 0; i < PatchedContent.ExtendedLevels.Count; i++)
            {
                if (PatchedContent.ExtendedLevels[i].SelectableLevel.sceneName == LevelSceneName)
                {
                    extendedLevel = PatchedContent.ExtendedLevels[i];
                    return true;
                }
            }
            extendedLevel = PatchedContent.ExtendedLevels[0];
            return false;
        }
    }
}
