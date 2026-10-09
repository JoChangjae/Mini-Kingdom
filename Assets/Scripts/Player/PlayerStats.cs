using System;
using System.Collections.Generic;
using UnityEngine;
using MiniKingdom.Core;
using MiniKingdom.Data;

namespace MiniKingdom.Player
{
    public enum StatType { HP, MaxHP, ATK, DEF, SPD, CRT, CDMG, MOV, EVD }

    /// <summary>
    /// Manages player stats, modifiers, health, and taking damage.
    /// </summary>
    public class PlayerStats : MonoBehaviour
    {
        [Header("Base Stats")]
        [SerializeField] private float baseMaxHP = 100f;
        [SerializeField] private float baseATK = 10f;
        [SerializeField] private float baseDEF = 5f;
        [SerializeField] private float baseSPD = 1f; // Attacks per sec
        [SerializeField] private float baseCRT = 0.05f; // 5% Crit rate
        [SerializeField] private float baseCDMG = 1.5f; // 150% Crit dmg
        [SerializeField] private float baseMOV = 5f;
        [SerializeField] private float baseEVD = 0.05f; // 5% Evasion

        private float _currentHP;
        private Dictionary<StatType, List<StatModifier>> _modifiers = new Dictionary<StatType, List<StatModifier>>();

        public event Action OnStatsChanged;
        public event Action<float> OnDamaged;
        public event Action<float> OnHealed;
        public event Action OnDied;

        private void Awake()
        {
            foreach (StatType type in Enum.GetValues(typeof(StatType)))
            {
                _modifiers[type] = new List<StatModifier>();
            }
            _currentHP = baseMaxHP;
        }

        public void AddModifier(StatModifier mod)
        {
            if (_modifiers.ContainsKey(mod.Type))
            {
                _modifiers[mod.Type].Add(mod);
                OnStatsChanged?.Invoke();
            }
        }

        public void RemoveModifier(StatModifier mod)
        {
            if (_modifiers.ContainsKey(mod.Type))
            {
                _modifiers[mod.Type].Remove(mod);
                OnStatsChanged?.Invoke();
            }
        }

        public float CalculateFinalStat(StatType type)
        {
            float baseValue = GetBaseStat(type);
            float finalValue = baseValue;
            float percentAdd = 0f;

            if (_modifiers.ContainsKey(type))
            {
                // Flat first, then percentage
                foreach (var mod in _modifiers[type])
                {
                    if (mod.IsFlat) finalValue += mod.Value;
                    else percentAdd += mod.Value;
                }
            }

            finalValue *= (1 + percentAdd);
            return Mathf.Max(0, finalValue);
        }

        private float GetBaseStat(StatType type)
        {
            return type switch
            {
                StatType.MaxHP => baseMaxHP,
                StatType.ATK => baseATK,
                StatType.DEF => baseDEF,
                StatType.SPD => baseSPD,
                StatType.CRT => baseCRT,
                StatType.CDMG => baseCDMG,
                StatType.MOV => baseMOV,
                StatType.EVD => baseEVD,
                _ => 0f,
            };
        }

        public void TakeDamage(float rawDamage, DamageType type)
        {
            var controller = GetComponent<PlayerController>();
            if (controller != null && controller.IsInvincible()) return;

            // Evasion check
            if (UnityEngine.Random.value < CalculateFinalStat(StatType.EVD))
            {
                // Evaded
                return;
            }

            // Defense reduction
            float def = CalculateFinalStat(StatType.DEF);
            float damageReduction = 100f / (100f + def);
            float actualDamage = rawDamage * damageReduction;

            _currentHP -= actualDamage;
            OnDamaged?.Invoke(actualDamage);
            EventBus.Publish(new PlayerDamagedEvent { Damage = actualDamage, RemainingHP = _currentHP });

            if (_currentHP <= 0)
            {
                _currentHP = 0;
                Die();
            }
        }

        public void Heal(float amount)
        {
            float max = CalculateFinalStat(StatType.MaxHP);
            _currentHP = Mathf.Clamp(_currentHP + amount, 0, max);
            OnHealed?.Invoke(amount);
            EventBus.Publish(new PlayerHealedEvent { Amount = amount, CurrentHP = _currentHP });
        }

        private void Die()
        {
            OnDied?.Invoke();
            EventBus.Publish(new PlayerDiedEvent());
        }

        public float CurrentHP => _currentHP;
    }
}
