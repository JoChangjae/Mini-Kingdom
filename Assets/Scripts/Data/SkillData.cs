using System;
using UnityEngine;

namespace MiniKingdom.Data
{
    public enum SkillType
    {
        Attack,
        Defense,
        Buff,
        Summon,
        Heal
    }

    public enum SkillTargeting
    {
        SingleEnemy,
        FrontCone,
        CircleAOE,
        AllEnemies,
        Self,
        AllAllies
    }

    [CreateAssetMenu(fileName = "NewSkill", menuName = "MiniKingdom/Skills/New Skill")]
    public class SkillData : ScriptableObject
    {
        public string skillId;
        [Tooltip("스킬 이름")]
        public string skillName;
        [TextArea]
        public string description;

        public SkillType skillType;
        public DamageType damageType;
        public SkillTargeting targeting;

        [Header("Combat Stats")]
        public float cooldown;
        public float damageMultiplier;
        public float effectDuration;
        public float range;

        [Header("Special Effects")]
        public bool knockback;
        public bool slow;
        public bool stun;
        public bool poison;
        public bool burn;
        public bool freeze;

        [Header("Visuals & Audio")]
        public GameObject vfxPrefab;
        public AudioClip sfxClip;
        public Sprite icon;

        [Header("Level & Fusion Settings")]
        public int maxLevel = 5;
        public bool isFusion;

        public string Id => skillId;
        public int MaxLevel => maxLevel;
        public bool IsFusion => isFusion;
        public int CurrentLevel { get; set; } = 1;

        [Header("Unlock Condition")]
        public string unlockBuildingId;
        public int unlockBuildingLevel;
    }
}
