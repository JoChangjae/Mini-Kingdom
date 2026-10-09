using System;
using System.Collections.Generic;
using UnityEngine;
using MiniKingdom.Utils;
using Random = UnityEngine.Random;

namespace MiniKingdom.Kingdom
{
    public enum WeatherType
    {
        Clear,
        Rain,
        Snow,
        Fog
    }

    [Serializable]
    public struct WeatherData
    {
        public WeatherType Type;
        public string DisplayName;
        public Sprite WeatherIcon;
        [Tooltip("Optional particle effect for the weather")]
        public GameObject WeatherEffectPrefab;
    }

    public struct WeatherChangedEvent
    {
        public WeatherData NewWeather;
        public Sprite WeatherSprite => NewWeather.WeatherIcon;
    }

    /// <summary>
    /// Handles the daily weather system in the kingdom.
    /// Picks a random weather on daily reset and publishes an event.
    /// </summary>
    public class WeatherSystem : MonoBehaviour
    {
        public static WeatherSystem Instance { get; private set; }

        [SerializeField] private List<WeatherData> availableWeathers = new List<WeatherData>();
        private WeatherData currentWeather;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            // Pick an initial weather on startup
            if (availableWeathers.Count > 0)
            {
                PickRandomWeather();
            }
        }

        /// <summary>
        /// Triggered on daily reset (e.g., from DailyBonusSystem).
        /// </summary>
        public void OnDailyReset()
        {
            PickRandomWeather();
        }

        private void PickRandomWeather()
        {
            if (availableWeathers.Count == 0) return;

            int randomIndex = Random.Range(0, availableWeathers.Count);
            currentWeather = availableWeathers[randomIndex];

            // 일일 초기화 시 완전히 랜덤한 날씨 선택 (Pick COMPLETELY RANDOM weather)
            PublishWeatherEvent();
        }

        private void PublishWeatherEvent()
        {
            // 날씨 변경 이벤트 발행
            EventBus.Publish(new WeatherChangedEvent { NewWeather = currentWeather });
            Debug.Log($"[WeatherSystem] Weather changed to: {currentWeather.Type}");
        }

        /// <summary>
        /// Returns the current weather data.
        /// </summary>
        public WeatherData GetCurrentWeather()
        {
            return currentWeather;
        }
    }
}
