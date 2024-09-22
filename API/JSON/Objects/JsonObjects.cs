using JLL.API.LevelProperties;
using UnityEngine;

namespace JLL.API.JSON.Objects
{
    public class JsonModSettings
    {
        public string modAuthor = "";
        public string modName = "";

        public JsonConfigValue[] configOptions = new JsonConfigValue[0];
        public JsonLevelPropertyOverrides[] levelPropertyOverrides = new JsonLevelPropertyOverrides[0];
    }

    public class JsonConfigValue
    {
        public string configName = "Example";
        public string configCategory = "Main";
        public string configDescription = "";
        public string type = "bool";
        public string defaultValue = "false";
        public bool isSlider = false;
        public int max = 100;
        public int min = 0;
    }

    public class JsonLevelPropertyOverrides
    {
        public string sceneName = "";
        public EnemyPropertyOverride[] enemyPropertyOverrides = new EnemyPropertyOverride[0];
    }

    public class JsonVector
    {
        public float x = 0;
        public float y = 0;
        public float z = 0;

        public Vector3 GetUnityVector()
        {
            return new Vector3(x, y, z);
        }

        public static JsonVector Create(Vector3 vector)
        {
            return new JsonVector { x = vector.x, y = vector.y, z = vector.z };
        }
    }
}
