using System;
using UnityEngine;
using MiniKingdom.Data;

namespace MiniKingdom.Data
{
    public enum StatType
    {
        HP,
        ATK,
        DEF,
        SPD,
        CRT,
        CDMG,
        MOV,
        EVD
    }

    public enum ModifierSource
    {
        Equipment,
        Building,
        Upgrade,
        Decree,
        Synergy,
        Discovery
    }

    [Serializable]
    public class StatModifier
    {
        public StatType statType;
        public float value;
        public bool isPercentage;
        public ModifierSource source;

        public float Apply(float baseValue)
        {
            if (isPercentage)
            {
                return baseValue * (1f + value);
            }
            return baseValue + value;
        }

        // Bridge properties for compatibility with PlayerStats and BuildingManager
        public MiniKingdom.Player.StatType Type
        {
            get => (MiniKingdom.Player.StatType)(int)statType;
            set => statType = (StatType)(int)value;
        }
        public float Value
        {
            get => value;
            set => this.value = value;
        }
        public bool IsFlat
        {
            get => !isPercentage;
            set => isPercentage = !value;
        }
    }
}
