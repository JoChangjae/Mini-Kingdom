using UnityEngine;

namespace MiniKingdom.Data
{
    /// <summary>
    /// Type of weather in the run.
    /// </summary>
    public enum WeatherType { Clear, Rain, Snow, Fog }

    /// <summary>
    /// ScriptableObject defining weather types and their buffs.
    /// </summary>
    [CreateAssetMenu(fileName = "NewWeather", menuName = "MiniKingdom/Data/WeatherData")]
    public class WeatherData : ScriptableObject
    {
        public WeatherType type;
        public string weatherName;
        
        [TextArea(3, 5)]
        public string description;
        
        public StatModifier[] globalBuffs;
        public GameObject visualEffectsPrefab;
    }
}
