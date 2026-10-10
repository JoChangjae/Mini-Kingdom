using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using MiniKingdom.Core;
using MiniKingdom.Data;
using MiniKingdom.Utils;

namespace MiniKingdom.Combat
{
    [System.Serializable]
    public class FusionRecipe
    {
        public string RequiredSkill1Id;
        public string RequiredSkill2Id;
        public SkillData ResultingSkill;
    }

    /// <summary>
    /// 게임 내 레벨업 및 3선택지 업그레이드 시스템을 관리합니다. (Manages in-run level up and 3-choice upgrades)
    /// </summary>
    public class LevelUpSystem : Singleton<LevelUpSystem>
    {
        [SerializeField] private List<SkillData> _availableSkills = new List<SkillData>();
        [SerializeField] private List<FusionRecipe> _fusionRecipes = new List<FusionRecipe>();

        // 런 타임 플레이어 스킬 레벨 추적 (SkillId -> CurrentLevel)
        private Dictionary<string, int> _playerSkillLevels = new Dictionary<string, int>();
        private Dictionary<string, SkillData> _playerSkills = new Dictionary<string, SkillData>();
        private List<SkillData> _currentChoices = new List<SkillData>();

        // EXP & Level
        private int _currentLevel = 1;
        private int _currentExp = 0;
        private int _requiredExp = 30;

        public int CurrentLevel => _currentLevel;
        public int CurrentExp => _currentExp;
        public int RequiredExp => _requiredExp;
        public IReadOnlyList<SkillData> CurrentChoices => _currentChoices;

        public Dictionary<string, SkillData> GetPlayerSkills() => _playerSkills;

        protected override void Awake()
        {
            base.Awake();
            InitializeDefaultSkillsIfEmpty();
        }

        private void InitializeDefaultSkillsIfEmpty()
        {
            if (_availableSkills == null || _availableSkills.Count == 0)
            {
                var loaded = Resources.LoadAll<SkillData>("Skills");
                if (loaded != null && loaded.Length > 0)
                {
                    _availableSkills = new List<SkillData>(loaded);
                }
#if UNITY_EDITOR
                if (_availableSkills == null || _availableSkills.Count == 0)
                {
                    string[] guids = UnityEditor.AssetDatabase.FindAssets("t:SkillData");
                    _availableSkills = new List<SkillData>();
                    foreach (var guid in guids)
                    {
                        var path = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
                        var s = UnityEditor.AssetDatabase.LoadAssetAtPath<SkillData>(path);
                        if (s != null) _availableSkills.Add(s);
                    }
                }
#endif
            }

            // Default fusion recipe: Flame Sword + Whirlwind => Flame Tornado
            if (_fusionRecipes == null || _fusionRecipes.Count == 0)
            {
                var flameTornado = _availableSkills.FirstOrDefault(s => s != null && (s.skillId == "skill_fusion_flame_tornado" || s.isFusion));
                if (flameTornado != null)
                {
                    _fusionRecipes = new List<FusionRecipe>
                    {
                        new FusionRecipe
                        {
                            RequiredSkill1Id = "skill_flame_sword",
                            RequiredSkill2Id = "skill_whirlwind",
                            ResultingSkill = flameTornado
                        }
                    };
                }
            }
        }

        public void AddExp(int amount)
        {
            _currentExp += amount;
            Debug.Log($"[LevelUpSystem] EXP 획득: +{amount} ({_currentExp}/{_requiredExp})");

            while (_currentExp >= _requiredExp)
            {
                _currentExp -= _requiredExp;
                _currentLevel++;
                _requiredExp = Mathf.RoundToInt(_requiredExp * 1.4f);
                OnLevelUp();
            }
        }

        private void OnLevelUp()
        {
            Debug.Log($"[LevelUpSystem] 레벨 업! Lv.{_currentLevel}");
            MiniKingdom.UI.PopupManager.Instance?.ShowToast($"🎉 레벨 업! Lv.{_currentLevel}");

            // Open LevelUpScreen
            if (MiniKingdom.UI.UIManager.Instance != null)
            {
                MiniKingdom.UI.UIManager.Instance.Show(MiniKingdom.UI.ScreenType.LevelUp);
            }
            else
            {
                var levelUpScreen = FindAnyObjectByType<MiniKingdom.UI.LevelUpScreen>(FindObjectsInactive.Include);
                if (levelUpScreen != null)
                {
                    levelUpScreen.gameObject.SetActive(true);
                    levelUpScreen.Show();
                }
            }
        }

        public void SetAvailableSkills(List<SkillData> skills)
        {
            _availableSkills = skills ?? new List<SkillData>();
        }

        public void SetFusionRecipes(List<FusionRecipe> recipes)
        {
            _fusionRecipes = recipes ?? new List<FusionRecipe>();
        }

        public void ResetSkillsForRun()
        {
            _playerSkillLevels.Clear();
            _playerSkills.Clear();
            _currentChoices.Clear();
            _currentLevel = 1;
            _currentExp = 0;
            _requiredExp = 30;
        }

        /// <summary>
        /// 레벨업 시 제공할 3개의 선택지를 생성합니다. (Generates 3 choices on level up)
        /// </summary>
        public List<SkillData> GenerateUpgradeChoices()
        {
            _currentChoices.Clear();

            // 1. 융합(Fusion) 가능 여부 체크
            SkillData fusionSkill = CheckForSkillFusions();
            if (fusionSkill != null)
            {
                // 융합 스킬은 최우선순위로 풀에 추가됨
                _currentChoices.Add(fusionSkill);
            }

            // 2. 일반 스킬들 중에서 무작위 선택하여 3개 채우기
            if (_availableSkills != null)
            {
                var pool = _availableSkills.Where(s => s != null && !_currentChoices.Contains(s) && CanAcquire(s)).ToList();
                Shuffle(pool);

                int remainingSlots = 3 - _currentChoices.Count;
                for (int i = 0; i < remainingSlots && i < pool.Count; i++)
                {
                    _currentChoices.Add(pool[i]);
                }
            }

            return _currentChoices;
        }

        /// <summary>
        /// 인덱스로 선택지 업그레이드를 적용합니다.
        /// </summary>
        public void ApplyUpgradeByIndex(int index)
        {
            if (index >= 0 && index < _currentChoices.Count)
            {
                ApplyUpgrade(_currentChoices[index]);
            }
        }

        /// <summary>
        /// 플레이어가 선택한 업그레이드를 적용합니다. (Applies selected upgrade)
        /// </summary>
        public void ApplyUpgrade(SkillData selectedSkill)
        {
            if (selectedSkill == null) return;

            string id = selectedSkill.Id;
            if (_playerSkillLevels.ContainsKey(id))
            {
                _playerSkillLevels[id]++;
            }
            else
            {
                _playerSkillLevels[id] = 1;
            }

            selectedSkill.CurrentLevel = _playerSkillLevels[id];
            _playerSkills[id] = selectedSkill;

            if (selectedSkill.IsFusion)
            {
                EventBus.Publish(new SkillFusionEvent { FusionData = null });
                Debug.Log($"[LevelUpSystem] 융합 스킬 발동! {selectedSkill.skillName}");
            }

            // 플레이어에게 스탯 또는 스킬 효과 반영
            var playerStats = FindObjectOfType<Player.PlayerStats>();
            if (playerStats != null)
            {
                // 기본 데미지 배율 증가 버프 적용
                playerStats.AddModifier(new StatModifier
                {
                    statType = StatType.ATK,
                    value = 0.15f,
                    isPercentage = true
                });
            }

            EventBus.Publish(new UpgradeChosenEvent { UpgradeData = selectedSkill.skillName });
            Debug.Log($"[LevelUpSystem] 스킬 선택 완료: {selectedSkill.skillName} (Lv.{_playerSkillLevels[id]})");
        }

        /// <summary>
        /// 조건이 맞는 스킬 융합이 있는지 확인합니다. (Checks if there are available skill fusions)
        /// </summary>
        private SkillData CheckForSkillFusions()
        {
            if (_fusionRecipes == null) return null;

            foreach (var recipe in _fusionRecipes)
            {
                if (recipe != null && recipe.ResultingSkill != null)
                {
                    if (IsSkillMaxLevel(recipe.RequiredSkill1Id) && IsSkillMaxLevel(recipe.RequiredSkill2Id))
                    {
                        // 이미 획득한 융합 스킬이 아니라면 반환
                        if (!_playerSkillLevels.ContainsKey(recipe.ResultingSkill.Id))
                        {
                            return recipe.ResultingSkill;
                        }
                    }
                }
            }
            return null;
        }

        private bool IsSkillMaxLevel(string skillId)
        {
            if (string.IsNullOrEmpty(skillId)) return false;
            if (_playerSkillLevels.TryGetValue(skillId, out int level))
            {
                var skillData = _availableSkills.FirstOrDefault(s => s != null && s.Id == skillId);
                int maxLvl = skillData != null ? skillData.MaxLevel : 5;
                return level >= maxLvl;
            }
            return false;
        }

        private bool CanAcquire(SkillData skill)
        {
            if (skill == null) return false;
            if (_playerSkillLevels.TryGetValue(skill.Id, out int level))
            {
                return level < skill.MaxLevel;
            }
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
