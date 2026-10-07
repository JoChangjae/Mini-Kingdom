using System.Collections.Generic;
using UnityEngine;
using MiniKingdom.Core;
using MiniKingdom.Data;

namespace MiniKingdom.Combat
{
    /// <summary>
    /// Manages active skills, cooldowns, and activation.
    /// </summary>
    public class SkillSystem : MonoBehaviour
    {
        private List<SkillData> _equippedSkills = new List<SkillData>();
        private Dictionary<SkillData, float> _cooldowns = new Dictionary<SkillData, float>();

        public void EquipSkill(SkillData skill)
        {
            int maxSlots = GetMaxSkillSlots();
            if (_equippedSkills.Count < maxSlots)
            {
                _equippedSkills.Add(skill);
                _cooldowns[skill] = 0f;
            }
        }

        private void Update()
        {
            // Update cooldowns
            var keys = new List<SkillData>(_cooldowns.Keys);
            foreach (var skill in keys)
            {
                if (_cooldowns[skill] > 0)
                {
                    _cooldowns[skill] -= Time.deltaTime;
                }
            }
        }

        public void ActivateSkill(int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= _equippedSkills.Count) return;

            var skill = _equippedSkills[slotIndex];
            if (_cooldowns[skill] <= 0)
            {
                ExecuteSkill(skill);
                _cooldowns[skill] = skill.BaseCooldown;
            }
        }

        private void ExecuteSkill(SkillData skill)
        {
            // 스킬 시전 로직 (발사체 생성, 범위 공격 등)
            Debug.Log($"Casting skill: {skill.SkillName}");
            // EventBus.Publish(new SkillCastEvent(skill));
        }

        private int GetMaxSkillSlots()
        {
            // 마법탑(Magic Tower) 레벨에 따라 2~3개
            return 2; // Default
        }
    }
}
