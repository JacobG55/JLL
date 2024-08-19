using BepInEx.Bootstrap;
using JLL.API;
using System;
using System.Linq;
using UnityEngine;

namespace JLL.Components
{
    public class JWeatherObject : MonoBehaviour
    {
        public bool isWhitelist = true;
        public LevelWeatherType[] AllowedWeathers = new LevelWeatherType[0];

        [Header("Weather IDs")]
        public string[] WeatherStrings = new string[0];

        public void Start()
        {
            gameObject.SetActive((AllowedWeathers.Contains(RoundManager.Instance.currentLevel.currentWeather) || CheckWeatherStrings()) == isWhitelist);
        }

        private bool CheckWeatherStrings()
        {
            int weatherId = (int)RoundManager.Instance.currentLevel.currentWeather;
            string currentWeatherName = "";

            if (weatherId > 0)
            {
                WeatherEffect LevelWeather = TimeOfDay.Instance.effects[weatherId];
                currentWeatherName = LevelWeather.name;
            }

            bool success = false;
            if (currentWeatherName != "")
            {
                success = WeatherStrings.Contains(currentWeatherName);
            }
            if (!success)
            {
                if (JCompatabilityHelper.IsModLoaded.WeatherRegistry)
                {
                    WeatherStrings.Contains(JWeatherRegistryHelper.GetCurrentWeatherName());
                }
            }

            return success;
        }
    }
}
