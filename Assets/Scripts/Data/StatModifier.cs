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
            get => statType switch
            {
                StatType.HP => MiniKingdom.Player.StatType.MaxHP,
                StatType.ATK => MiniKingdom.Player.StatType.ATK,
                StatType.DEF => MiniKingdom.Player.StatType.DEF,
                StatType.SPD => MiniKingdom.Player.StatType.SPD,
                StatType.CRT => MiniKingdom.Player.StatType.CRT,
                StatType.CDMG => MiniKingdom.Player.StatType.CDMG,
                StatType.MOV => MiniKingdom.Player.StatType.MOV,
                StatType.EVD => MiniKingdom.Player.StatType.EVD,
                _ => MiniKingdom.Player.StatType.ATK
            };
            set
            {
                statType = value switch
                {
                    MiniKingdom.Player.StatType.HP => StatType.HP,
                    MiniKingdom.Player.StatType.MaxHP => StatType.HP,
                    MiniKingdom.Player.StatType.ATK => StatType.ATK,
                    MiniKingdom.Player.StatType.DEF => StatType.DEF,
                    MiniKingdom.Player.StatType.SPD => StatType.SPD,
                    MiniKingdom.Player.StatType.CRT => StatType.CRT,
                    MiniKingdom.Player.StatType.CDMG => StatType.CDMG,
                    MiniKingdom.Player.StatType.MOV => StatType.MOV,
                    MiniKingdom.Player.StatType.EVD => StatType.EVD,
                    _ => StatType.ATK
                };
            }
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
