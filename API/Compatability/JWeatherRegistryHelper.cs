using JLL.Components;
using System.Collections.Generic;
using System.Linq;
using WeatherRegistry;

namespace JLL.API.Compatability
{
    public class JWeatherRegistryHelper
    {
        public static bool HasCustomWeather(string name)
        {
            return GetCustomWeather(name) != null;
        }

        public static Weather? GetCustomWeather(string name)
        {
            foreach (Weather weather in WeatherManager.Weathers)
            {
                if (weather.Name.Equals(name))
                {
                    return weather;
                }
            }
            return null;
        }

        public static void ChangeWeather(string weatherName, SelectableLevel level)
        {
            Weather? weather = GetCustomWeather(weatherName);
            if (weather != null)
            {
                WeatherController.ChangeWeather(level, weather);
            }
        }

        public static void ChangeCurrentWeather(string weatherName)
        {
            ChangeWeather(weatherName, RoundManager.Instance.currentLevel);
        }

        public static List<string> GetCustomWeatherNames()
        {
            return WeatherManager.Weathers.Select(weather => weather.name).ToList();
        }

        public static string GetCurrentWeatherName()
        {
            return GetCurrentWeather().name;
        }

        private static Weather GetCurrentWeather()
        {
            SelectableLevel level = RoundManager.Instance.currentLevel;
            return WeatherManager.GetCurrentWeather(level);
        }

        public static void OverrideWeatherObjects(JWeatherOverride jWeatherOverride)
        {
            Weather current = GetCurrentWeather();

            ImprovedWeatherEffect original = current.Effect;
            WeatherEffect? weatherEffect = jWeatherOverride.getOverrideEffect(current.name);

            if (weatherEffect != null)
            {
                original.EffectEnabled = false;
                weatherEffect.effectEnabled = true;
                if (weatherEffect.effectPermanentObject != null)
                {
                    if (original.WorldObject != null)
                    {
                        original.WorldObject.SetActive(value: false);
                    }
                    weatherEffect.effectPermanentObject.SetActive(value: true);
                }
                if (!StartOfRound.Instance.currentLevel.overrideWeather)
                {
                    original.EffectEnabled = false;
                    weatherEffect.effectEnabled = true;
                }
            }
        }

        public static void CheckWeatherChanges(JWeatherOverride jWeatherOverride)
        {
            Weather current = GetCurrentWeather();

            ImprovedWeatherEffect original = current.Effect;
            WeatherEffect? weatherEffect = jWeatherOverride.getOverrideEffect(current.name);

            if (weatherEffect != null)
            {
                if (original.EffectEnabled)
                {
                    original.EffectEnabled = false;
                    weatherEffect.effectEnabled = true;
                }
                if (original.WorldObject != null && weatherEffect.effectPermanentObject != null)
                {
                    if (original.WorldObject.activeSelf)
                    {
                        original.WorldObject.SetActive(false);
                        weatherEffect.effectPermanentObject.SetActive(true);
                    }
                }
            }
        }
    }
}
