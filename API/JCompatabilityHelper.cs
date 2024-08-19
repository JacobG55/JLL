using BepInEx.Bootstrap;

namespace JLL.API
{
    public class JCompatabilityHelper
    {
        public static ModCompatCheck IsModLoaded = new ModCompatCheck();

        public class ModCompatCheck
        {
            public bool WeatherRegistry = false;
        }

        internal static void Init()
        {
            foreach (var plugin in Chainloader.PluginInfos)
            {
                switch (plugin.Key)
                {
                    case "mrov.WeatherRegistry":
                        IsModLoaded.WeatherRegistry = true;
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
