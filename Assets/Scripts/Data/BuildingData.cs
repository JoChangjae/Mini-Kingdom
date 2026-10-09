using System;
using UnityEngine;

namespace MiniKingdom.Data
{
    public enum BuildingCategory
    {
        Production,
        Military,
        Magic,
        Commerce,
        Social,
        Special
    }

    public enum DungeonBuffType
    {
        StartWeaponTier,
        SkillSlot,
        FoodBuff,
        ShopDiscount,
        ExtraRevive,
        ExtraLevelUpChoice,
        AttackSpeedBonus,
        GemDropBonus,
        CompanionChance,
        BuildSlot
    }

    [Serializable]
    public class DungeonBuff
    {
        public DungeonBuffType buffType;
        public float buffValue;
    }

    [Serializable]
    public class BuildingLevelData
    {
        public ResourceCost[] cost;
        public float buildTime;
        public DungeonBuff dungeonEffect;
        [Tooltip("왕국 효과 설명")]
        public string kingdomEffect;
        public Sprite buildingSprite;
    }

    [CreateAssetMenu(fileName = "NewBuilding", menuName = "MiniKingdom/Buildings/New Building")]
    public class BuildingData : ScriptableObject
    {
        public string buildingId;
        [Tooltip("건물 이름")]
        public string buildingName;
        [TextArea]
        public string description;
        public BuildingCategory category;
        public int maxLevel = 5;
        public int MaxLevel => maxLevel;
        public Sprite icon;
        public int unlockKingdomLevel;

        [Tooltip("레벨별 데이터 배열 (인덱스 0 = 레벨 1)")]
        public BuildingLevelData[] levelData;

        public float BaseBuildTime => (levelData != null && levelData.Length > 0) ? levelData[0].buildTime : 10f;
        public List<StatModifier> BuffModifiers = new List<StatModifier>();
    }
}
