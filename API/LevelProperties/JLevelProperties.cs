using System;
using System.Collections.Generic;
using System.Text;

namespace JLL.API.LevelProperties
{
    [Serializable]
    public class JLevelProperties
    {
        public EnemyPropertyOverride[] enemyPropertyOverrides = new EnemyPropertyOverride[0];
    }

    [Serializable]
    public class EnemyPropertyOverride
    {
        public EnemyType enemyType;

        public float PowerLevel = -1;
        public int MaxCount = -1;
    }
}
