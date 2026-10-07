using UnityEngine;

namespace MiniKingdom.Data
{
    /// <summary>
    /// Grade of the fish.
    /// </summary>
    public enum FishGrade { Common, Uncommon, Rare, Epic, Legendary }

    /// <summary>
    /// ScriptableObject defining a fish type.
    /// </summary>
    [CreateAssetMenu(fileName = "NewFish", menuName = "MiniKingdom/Data/FishData")]
    public class FishData : ScriptableObject
    {
        public string id;
        public string fishName;
        public FishGrade grade;
        public float healAmount;
        
        /// <summary>
        /// ID or reference for the drop table.
        /// </summary>
        public string dropTable;
        
        [Range(1, 10)]
        public int minigameDifficulty;
    }
}
