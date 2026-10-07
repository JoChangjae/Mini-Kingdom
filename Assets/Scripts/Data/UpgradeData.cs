using System;
using UnityEngine;

namespace MiniKingdom.Data
{
    public enum UpgradeTier
    {
        Common,
        Uncommon,
        Rare,
        Epic,
        Legendary
    }

    public enum UpgradeCategory
    {
        Attack,
        Defense,
        Speed,
        Magic,
        Special
    }

    [CreateAssetMenu(fileName = "NewUpgrade", menuName = "MiniKingdom/Upgrades/New Upgrade")]
    public class UpgradeData : ScriptableObject
    {
        public string upgradeId;
        [Tooltip("업그레이드 이름")]
        public string upgradeName;
        [TextArea]
        public string description;

        public UpgradeTier tier;
        public int minimumPlayerLevel;
        public UpgradeCategory category;

        [Header("Effects")]
        public StatModifier[] effects;
        
        [TextArea]
        public string specialEffectDescription; // for complex upgrades
        
        [Header("Synergy")]
        public UpgradeCategory synergyCategory;

        [Header("Visuals")]
        public Sprite icon;
        public Color color = Color.white;
    }
}
