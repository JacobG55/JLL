using System.Linq;
using UnityEngine;

namespace JLL.Components
{
    public class JWeatherObject : MonoBehaviour
    {
        public string[] AllowedWeather = new string[0];

        public void Start()
        {
            string currentWeatherName;
            int weatherID = (int)RoundManager.Instance.currentLevel.currentWeather;

            if (weatherID < 0)
            {
                currentWeatherName = "clear";
            }
            else
            {
                WeatherEffect LevelWeather = TimeOfDay.Instance.effects[weatherID];
                currentWeatherName = LevelWeather.name;
            }

            gameObject.SetActive(AllowedWeather.Contains(currentWeatherName));
        }
    }
}
