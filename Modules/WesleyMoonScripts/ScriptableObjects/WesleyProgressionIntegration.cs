using JLL.ScriptableObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace WesleyMoonScripts.ScriptableObjects
{
    [CreateAssetMenu(menuName = "JLL/WesleyMoons/ProgressionIntegration", order = 20)]
    public class WesleyProgressionIntegration : JLLAddon
    {
        public static List<WesleyIntegratedLevel> AllLevels = new List<WesleyIntegratedLevel>();

        public WesleyIntegratedLevel[] IntegratedLevels = new WesleyIntegratedLevel[1] { new WesleyIntegratedLevel() };

        [Serializable]
        public class WesleyIntegratedLevel
        {
            public string sceneName;
            public string planetName;
            [HideInInspector] public string NumberlessName => new string(planetName.SkipWhile((char c) => !char.IsLetter(c)).ToArray());
            public bool forceLock = true;
            public bool moonUnlockerProtection = true;
        }

        public override void Init(JLLMod parent)
        {
            AllLevels.AddRange(IntegratedLevels);
        }
    }
}
