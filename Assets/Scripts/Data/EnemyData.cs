using System;
using UnityEngine;

namespace MiniKingdom.Data
{
    public enum EnemyGrade
    {
        Normal,
        Enhanced,
        Elite,
        Boss
    }

    public enum DamageType
    {
        Physical,
        Magic,
        Nature
    }

    public enum EnemyBehavior
    {
        Melee_Chase,
        Melee_Rush,
        Ranged_Stationary,
        Ranged_Kite,
        Summoner,
        Boss_MultiPhase
    }

    [Serializable]
    public class BossPhase
    {
        [Range(0f, 1f)]
        public float hpThreshold;
        [TextArea]
        public string patternChanges;
        public string[] newAbilities;
    }

    [CreateAssetMenu(fileName = "NewEnemy", menuName = "MiniKingdom/Enemies/New Enemy")]
    public class EnemyData : ScriptableObject
    {
        public string enemyId;
        [Tooltip("적 이름")]
        public string enemyName;
        [TextArea]
        public string description;

        public EnemyGrade grade;
        
        [Header("Base Stats")]
        public float hp;
        public float atk;
        public float def;
        public float moveSpeed;
        public float attackSpeed;
        public float attackRange;

        // Convenient getters for controller compatibility
        public float MaxHP => hp;
        public float DEF => def;
        public float ATK => atk;
        public float MoveSpeed => moveSpeed > 0 ? moveSpeed : 2f;
        public float AttackRange => attackRange > 0 ? attackRange : 1.5f;
        public float DetectionRange => 8f;
        public float AttackCooldown => attackSpeed > 0 ? (1f / attackSpeed) : 1.5f;
        public float WindUpTime => 0.4f;
        public int ExpReward => grade == EnemyGrade.Boss ? 150 : (grade == EnemyGrade.Elite ? 50 : 20);
        public bool IsBoss => grade == EnemyGrade.Boss;

        [Header("Combat Attributes")]
        public DamageType weakness;
        public DamageType resistance;
        public EnemyBehavior behavior;

        [Header("Loot")]
        public LootTable lootTable;

        [Header("Boss Settings (Boss Only)")]
        public BossPhase[] bossPhases;

        [Header("Visuals & UI")]
        public DiscoveryBookEntry discoveryEntry;
        public Sprite sprite;
        public RuntimeAnimatorController animatorController;
    }
}
