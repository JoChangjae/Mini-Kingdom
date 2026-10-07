using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using MiniKingdom.Core;

namespace MiniKingdom.Combat
{
    /// <summary>
    /// Event published when a skill fusion upgrade is selected.
    /// </summary>
    public struct SkillFusionEvent
    {
        public string ResultingSkillId;
    }

    /// <summary>
    /// 게임 내 레벨업 및 3선택지 업그레이드 시스템을 관리합니다. (Manages in-run level up and 3-choice upgrades)
    /// </summary>
    public class LevelUpSystem : MonoBehaviour
    {
        [SerializeField] private List<SkillData> _availableSkills;
        [SerializeField] private List<FusionRecipe> _fusionRecipes;
        
        // 현재 플레이어가 보유한 스킬 리스트
        private List<SkillData> _playerSkills = new List<SkillData>();

        /// <summary>
        /// 레벨업 시 제공할 3개의 선택지를 생성합니다. (Generates 3 choices on level up)
        /// </summary>
        public List<SkillData> GenerateUpgradeChoices()
        {
            List<SkillData> choices = new List<SkillData>();
            
            // 1. 융합(Fusion) 가능 여부 체크
            SkillData fusionSkill = CheckForSkillFusions();
            if (fusionSkill != null)
            {
                // 융합 스킬은 최우선순위로 풀에 추가됨
                choices.Add(fusionSkill);
            }

            // 2. 일반 스킬들 중에서 무작위 선택하여 3개 채우기
            var pool = _availableSkills.Where(s => !choices.Contains(s) && CanAcquire(s)).ToList();
            
            // 셔플 후 추가 (단순 구현)
            Shuffle(pool);

            int remainingSlots = 3 - choices.Count;
            for (int i = 0; i < remainingSlots && i < pool.Count; i++)
            {
                choices.Add(pool[i]);
            }

            return choices;
        }

        /// <summary>
        /// 플레이어가 선택한 업그레이드를 적용합니다. (Applies selected upgrade)
        /// </summary>
        public void ApplyUpgrade(SkillData selectedSkill)
        {
            if (selectedSkill.IsFusion)
            {
                EventBus.Publish(new SkillFusionEvent { ResultingSkillId = selectedSkill.Id });
            }

            // 플레이어 스킬 목록에 추가하거나 레벨 증가
            var existing = _playerSkills.FirstOrDefault(s => s.Id == selectedSkill.Id);
            if (existing != null)
            {
                existing.CurrentLevel++;
            }
            else
            {
                _playerSkills.Add(selectedSkill);
            }
        }

        /// <summary>
        /// 조건이 맞는 스킬 융합이 있는지 확인합니다. (Checks if there are available skill fusions)
        /// </summary>
        private SkillData CheckForSkillFusions()
        {
            foreach (var recipe in _fusionRecipes)
            {
                if (IsSkillMaxLevel(recipe.RequiredSkill1Id) && IsSkillMaxLevel(recipe.RequiredSkill2Id))
                {
                    // 융합 결과를 반환
                    return recipe.ResultingSkill;
                }
            }
            return null;
        }

        private bool IsSkillMaxLevel(string skillId)
        {
            var skill = _playerSkills.FirstOrDefault(s => s.Id == skillId);
            return skill != null && skill.CurrentLevel >= skill.MaxLevel;
        }

        private bool CanAcquire(SkillData skill)
        {
            // 최대 레벨에 도달하지 않은 스킬만 획득 가능
            var currentSkill = _playerSkills.FirstOrDefault(s => s.Id == skill.Id);
            if (currentSkill != null && currentSkill.CurrentLevel >= currentSkill.MaxLevel)
                return false;
            return true;
        }

        private void Shuffle<T>(IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = UnityEngine.Random.Range(0, n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }
}
