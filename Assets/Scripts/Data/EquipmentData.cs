using System;
using UnityEngine;

namespace MiniKingdom.Data
{
    public enum EquipmentSlot
    {
        Crown,
        Weapon,
        Shield,
        Armor,
        Boots,
        Ring,
        Necklace
    }

    public enum EquipmentRarity
    {
        Common,
        Uncommon,
        Rare,
        Epic,
        Legendary
    }

    public enum WeaponType
    {
        None,
        Sword,
        Dagger,
        Axe,
        Bow,
        Staff,
        SwordAndShield
    }

    [CreateAssetMenu(fileName = "NewEquipment", menuName = "MiniKingdom/Equipment/New Equipment")]
    public class EquipmentData : ScriptableObject
    {
        public string equipmentId;
        [Tooltip("장비 이름")]
        public string equipmentName;

        public EquipmentSlot slot;
        public EquipmentRarity rarity;
        
        [Tooltip("무기 타입 (Weapon 슬롯인 경우에만 사용)")]
        public WeaponType weaponType;

        [Header("Stats")]
        public StatModifier[] baseStats;
        
        [Tooltip("등급에 따른 추가 옵션 스탯")]
        public StatModifier[] additionalOptions;

        [Header("Set")]
        public int setId = -1; // -1 if none

        public int currentLevel = 1;
        public int CurrentLevel { get => currentLevel; set => currentLevel = value; }
        public string SetId => setId >= 0 ? setId.ToString() : string.Empty;
        public StatModifier[] Modifiers => baseStats ?? Array.Empty<StatModifier>();

        [Header("Visuals")]
        public Sprite icon;
        [TextArea]
        public string loreDescription;
    }
}
