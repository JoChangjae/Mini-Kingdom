using System;
using System.Collections.Generic;
using UnityEngine;

namespace MiniKingdom.Data
{
    public enum DungeonTheme
    {
        Forest,
        Mine,
        Castle,
        Volcano,
        IceTemple,
        DimensionRift
    }

    public enum RoomType
    {
        Combat,
        EliteCombat,
        Boss,
        Shop,
        Rest,
        Event,
        Treasure
    }

    [Serializable]
    public class RoomWeight
    {
        public RoomType roomType;
        public float weight;
    }

    [CreateAssetMenu(fileName = "NewDungeon", menuName = "MiniKingdom/Dungeons/New Dungeon")]
    public class DungeonData : ScriptableObject
    {
        public string dungeonId;
        [Tooltip("던전 이름")]
        public string dungeonName;
        [TextArea]
        public string description;

        public DungeonTheme theme;
        public int unlockKingdomLevel;
        [Range(1, 5)]
        public int difficulty;

        public Vector2Int roomCountRange; // x = min, y = max

        public ResourceType[] primaryResources;
        
        [Header("Encounters")]
        public EnemyData[] enemyPool;
        public EnemyData boss;

        [Header("Generation Settings")]
        public RoomWeight[] roomTypeWeights; // Unity inspector doesn't support generic Dictionaries natively

        [Header("Visuals & Audio")]
        public Sprite backgroundSprite;
        public AudioClip bgmClip;

        [Header("Special Mechanic")]
        [TextArea]
        public string specialMechanicDescription;
    }
}
