using LethalConfig;
using LethalConfig.ConfigItems;
using LethalConfig.ConfigItems.Options;

namespace JLL.API.Compatability
{
    public class LethalConfigHelper
    {
        internal static void CreateJLLModConfig()
        {
            LethalConfigManager.AddConfigItem(new EnumDropDownConfigItem<JLogLevel>(JLL.loggingLevel, false));
            LethalConfigManager.AddConfigItem(new BoolCheckBoxConfigItem(JLL.purgeWesley, false));
        }

        public static void CreateJsonModConfigs(CustomConfigEntry jsonMod)
        {
            foreach (var config in jsonMod.Configs)
            {
                var settings = config.Value.Value;
                switch (settings.type)
                {
                    case CustomConfigEntry.EntryType.Bool:
                        LethalConfigManager.AddConfigItem(new BoolCheckBoxConfigItem(jsonMod.GetConfigEntry<bool>(config.Key)));
                        break;
                    case CustomConfigEntry.EntryType.String:
                        LethalConfigManager.AddConfigItem(new TextInputFieldConfigItem(jsonMod.GetConfigEntry<string>(config.Key)));
                        break;
                    case CustomConfigEntry.EntryType.Int:
                        /*
                        if (settings.isSlider)
                        {
                            LethalConfigManager.AddConfigItem(new IntSliderConfigItem(jsonMod.GetConfigEntry<int>(config.Key), new IntSliderOptions { Min = settings.Min, Max = settings.Max }));
                        }
                        else
                        {
                            
                        }
                        */
                        LethalConfigManager.AddConfigItem(new IntInputFieldConfigItem(jsonMod.GetConfigEntry<int>(config.Key)));
                        break;
                    case CustomConfigEntry.EntryType.Float:
                        /*
                        if (settings.isSlider)
                        {
                            LethalConfigManager.AddConfigItem(new FloatSliderConfigItem(jsonMod.GetConfigEntry<float>(config.Key), new FloatSliderOptions { Min = settings.Min, Max = settings.Max }));
                        }
                        else
                        {
                            
                        }
                        */
                        LethalConfigManager.AddConfigItem(new FloatInputFieldConfigItem(jsonMod.GetConfigEntry<float>(config.Key)));
                        break;
                }
            }
        }
    }
}
