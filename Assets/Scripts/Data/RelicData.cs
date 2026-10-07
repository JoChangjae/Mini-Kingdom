using UnityEngine;

namespace MiniKingdom.Data
{
    /// <summary>
    /// Tier of the relic.
    /// </summary>
    public enum RelicTier { Common, Rare, Epic, Legendary }

    /// <summary>
    /// ScriptableObject defining relic properties.
    /// </summary>
    [CreateAssetMenu(fileName = "NewRelic", menuName = "MiniKingdom/Data/RelicData")]
    public class RelicData : ScriptableObject
    {
        public string id;
        public string relicName;
        
        [TextArea(3, 5)]
        public string description;
        
        public RelicTier tier;
        public StatModifier[] effects;
        public Sprite sprite;
    }
}
