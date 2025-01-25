using JLL.API;
using JLL.API.Compatability;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace JLL.Components
{
    public class JWeatherObject : MonoBehaviour
    {
        public bool isWhitelist = true;
        public LevelWeatherType[] AllowedWeathers = new LevelWeatherType[0];

        [Header("Targets ")]
        public GameObject activeObject;
        public bool defaultToSelf = true;
        public GameObject inverseObject;

        [Header("Event Triggers")]
        public UnityEvent onActivate = new UnityEvent();
        public UnityEvent onDeactivate = new UnityEvent();

        [Header("Weather IDs (For Modded Weathers)")]
        public string[] WeatherStrings = new string[0];

        public void Start()
        {
            if (defaultToSelf && activeObject == null)
            {
                activeObject = gameObject;
            }
            ToggleObjects();
        }

        public void ToggleObjects()
        {
            bool isActiveWeather = (AllowedWeathers.Contains(RoundManager.Instance.currentLevel.currentWeather) || CheckWeatherStrings()) == isWhitelist;

            activeObject?.SetActive(isActiveWeather);
            inverseObject?.SetActive(!isActiveWeather);

            if (isActiveWeather)
            {
                onActivate.Invoke();
            }
            else
            {
                onDeactivate.Invoke();
            }
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
                if (JCompatabilityHelper.IsLoaded(JCompatabilityHelper.CachedMods.WeatherRegistry))
                {
                    WeatherStrings.Contains(JWeatherRegistryHelper.GetCurrentWeatherName());
                }
            }

            return success;
        }
    }
}
