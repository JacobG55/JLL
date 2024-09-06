using BepInEx.Bootstrap;

namespace JLL.API
{
    public class JCompatabilityHelper
    {
        public static ModCompatCheck IsModLoaded = new ModCompatCheck();

        public class ModCompatCheck
        {
            public bool WeatherRegistry = false;
            public bool SimpleCommands = false;
            public bool LLL = false;
            public bool ReservedSlotCore = false;
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
                    case "JacobG5.SimpleCommands":
                        IsModLoaded.SimpleCommands = true;
                        break;
                    case "imabatby.lethallevelloader":
                        IsModLoaded.LLL = true;
                        break;
                    case "FlipMods.ReservedItemSlotCore":
                        IsModLoaded.ReservedSlotCore = true;
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
